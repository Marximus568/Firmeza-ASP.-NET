#!/bin/bash

# Function to kill processes on exit
cleanup() {
    echo ""
    echo "🛑 Deteniendo todos los servicios..."
    
    # Kill all background jobs started by this script
    if [ -n "$(jobs -p)" ]; then
        kill $(jobs -p) 2>/dev/null
    fi
    
    echo "✅ Servicios detenidos."
    exit
}

# Trap Ctrl+C to run cleanup
trap cleanup SIGINT

echo "==================================================="
echo "🚀 Iniciando Sistema Firmeza (Full Stack)"
echo "==================================================="

# 1. Start WebApi (Backend)
echo "🔹 Iniciando WebApi (Backend)..."
dotnet run --project Firmeza.WebApi/Firmeza.WebApi.csproj --urls "http://localhost:5000" > /dev/null 2>&1 &
API_PID=$!
echo "   ✅ WebApi iniciada en http://localhost:5000 (PID: $API_PID)"

# 2. Start AdminDashboard
echo "🔹 Iniciando AdminDashboard..."
dotnet run --project AdminDashboard.Web/AdminDashboard.Web.csproj --urls "http://localhost:5001" > /dev/null 2>&1 &
ADMIN_PID=$!
echo "   ✅ AdminDashboard iniciado en http://localhost:5001 (PID: $ADMIN_PID)"

# 3. Start Frontend
echo "🔹 Iniciando Frontend React..."
cd Front-end
npm run dev &
FRONTEND_PID=$!
cd ..

echo "==================================================="
echo "🎉 ¡Todo listo! El sistema está corriendo."
echo "==================================================="
echo "   - 🔌 API Backend:      http://localhost:5000"
echo "   - 🛠️ Admin Dashboard:  http://localhost:5001"
echo "   - 💻 Frontend React:   http://localhost:5173 (verificar abajo)"
echo ""
echo "📝 Presiona Ctrl+C para detener todos los servicios."
echo "==================================================="

# Wait for all background processes
wait
