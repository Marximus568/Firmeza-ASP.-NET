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

# 1. Start WebApi (Backend)
echo "🔹 Iniciando WebApi (Backend)..."
dotnet run --project Firmeza.WebApi/Firmeza.WebApi.csproj --urls "http://localhost:5000" > "$LOGS_DIR/webapi.log" 2>&1 &
API_PID=$!
echo "   ⏳ Esperando inicio del WebApi..."
sleep 3

# Check if WebApi is running
if kill -0 $API_PID 2>/dev/null; then
    # Check if it's actually listening
    if curl -s http://localhost:5000 > /dev/null 2>&1 || curl -s -o /dev/null -w "%{http_code}" http://localhost:5000 | grep -q "404"; then
        echo "   ✅ WebApi iniciada en http://localhost:5000 (PID: $API_PID)"
        echo "   📄 Log: $LOGS_DIR/webapi.log"
    else
        echo "   ⚠️  WebApi iniciada pero no responde en http://localhost:5000"
        echo "   📄 Revisar log: $LOGS_DIR/webapi.log"
    fi
else
    echo "   ❌ Error al iniciar WebApi"
    echo "   📄 Revisar log: $LOGS_DIR/webapi.log"
    tail -20 "$LOGS_DIR/webapi.log"
    cleanup
fi

# 2. Start AdminDashboard
echo ""
echo "🔹 Iniciando AdminDashboard..."
dotnet run --project AdminDashboard.Web/AdminDashboard.Web.csproj --urls "http://localhost:5001" > "$LOGS_DIR/admin.log" 2>&1 &
ADMIN_PID=$!
echo "   ⏳ Esperando inicio del AdminDashboard..."
sleep 3

# Check if AdminDashboard is running
if kill -0 $ADMIN_PID 2>/dev/null; then
    if curl -s http://localhost:5001 > /dev/null 2>&1 || curl -s -o /dev/null -w "%{http_code}" http://localhost:5001 | grep -q -E "200|302|404"; then
        echo "   ✅ AdminDashboard iniciado en http://localhost:5001 (PID: $ADMIN_PID)"
        echo "   📄 Log: $LOGS_DIR/admin.log"
    else
        echo "   ⚠️  AdminDashboard iniciado pero no responde en http://localhost:5001"
        echo "   📄 Revisar log: $LOGS_DIR/admin.log"
    fi
else
    echo "   ❌ Error al iniciar AdminDashboard"
    echo "   📄 Revisar log: $LOGS_DIR/admin.log"
    tail -20 "$LOGS_DIR/admin.log"
fi

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
echo "   - 💻 Frontend React:   http://localhost:5173 (espera ~10s)"
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
