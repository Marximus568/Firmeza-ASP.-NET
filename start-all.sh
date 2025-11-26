#!/bin/bash

# Create logs directory if it doesn't exist
LOGS_DIR="./logs"
mkdir -p "$LOGS_DIR"

# Function to kill processes on exit
cleanup() {
    echo ""
    echo "🛑 Deteniendo todos los servicios..."
    
    # Kill all background jobs started by this script
    if [ -n "$(jobs -p)" ]; then
        kill $(jobs -p) 2>/dev/null
    fi
    
    echo "✅ Servicios detenidos."
    echo "📁 Los logs están disponibles en: $LOGS_DIR/"
    exit
}

# Trap Ctrl+C to run cleanup
trap cleanup SIGINT

echo "==================================================="
echo "🚀 Iniciando Sistema Firmeza (Full Stack)"
echo "==================================================="
echo "📁 Los logs se guardarán en: $LOGS_DIR/"
echo ""

# Function to wait for a service to start
wait_for_service() {
    local url=$1
    local pid=$2
    local name=$3
    local log_file=$4
    local max_retries=60  # Wait up to 60 seconds (30 * 2s)
    local count=0

    echo "   ⏳ Esperando a que $name inicie en $url (PID: $pid)..."

    while [ $count -lt $max_retries ]; do
        if ! kill -0 $pid 2>/dev/null; then
            echo "   ❌ $name se detuvo inesperadamente."
            echo "   📄 Revisar log: $log_file"
            tail -10 "$log_file"
            return 1
        fi

        # Check if service is responding (200, 302, or even 404 means it's listening)
        if curl -s -o /dev/null -w "%{http_code}" "$url" | grep -q -E "200|302|404"; then
            echo "   ✅ $name iniciado correctamente!"
            return 0
        fi

        sleep 2
        count=$((count + 1))
        echo -ne "      Intentando conectar... ($count/$max_retries)\r"
    done

    echo ""
    echo "   ⚠️  $name no respondió después de $((max_retries * 2)) segundos."
    echo "   📄 Revisar log: $log_file"
    return 1
}

# 1. Start WebApi (Backend)
echo "🔹 Iniciando WebApi (Backend)..."
dotnet run --project Firmeza.WebApi/Firmeza.WebApi.csproj --urls "http://localhost:5000" > "$LOGS_DIR/webapi.log" 2>&1 &
API_PID=$!
wait_for_service "http://localhost:5000" $API_PID "WebApi" "$LOGS_DIR/webapi.log"

# 2. Start AdminDashboard
echo ""
echo "🔹 Iniciando AdminDashboard..."
dotnet run --project AdminDashboard.Web/AdminDashboard.Web.csproj --urls "http://localhost:5001" > "$LOGS_DIR/admin.log" 2>&1 &
ADMIN_PID=$!
wait_for_service "http://localhost:5001" $ADMIN_PID "AdminDashboard" "$LOGS_DIR/admin.log"

# 3. Start Frontend
echo ""
echo "🔹 Iniciando Frontend React..."
cd Front-end
npm run dev > "../$LOGS_DIR/frontend.log" 2>&1 &
FRONTEND_PID=$!
cd ..
echo "   ✅ Frontend React iniciando... (PID: $FRONTEND_PID)"
echo "   📄 Log: $LOGS_DIR/frontend.log"

echo ""
echo "==================================================="
echo "🎉 ¡Todo listo! El sistema está corriendo."
echo "==================================================="
echo "   - 🔌 API Backend:      http://localhost:5000"
echo "   - 🛠️ Admin Dashboard:  http://localhost:5001"
echo "   - 💻 Frontend React:   http://localhost:5173"
echo ""
echo "📝 Presiona Ctrl+C para detener todos los servicios."
echo "📁 Logs disponibles en: $LOGS_DIR/"
echo ""
echo "Si algún servicio no funciona, revisa los logs:"
echo "   - cat $LOGS_DIR/webapi.log"
echo "   - cat $LOGS_DIR/admin.log"
echo "   - cat $LOGS_DIR/frontend.log"
echo "==================================================="

# Wait for all background processes
wait
