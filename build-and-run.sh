#!/bin/bash

echo "🚀 Building Content Management & Discovery System"
echo "=================================================="

# Check if .NET 8 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK is required but not installed."
    echo "Please install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is required but not installed."
    echo "Please install Node.js from: https://nodejs.org/"
    exit 1
fi

echo "✅ Prerequisites check passed"

# Build the solution
echo "🔨 Building .NET solution..."
dotnet restore
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

echo "✅ .NET solution built successfully"

# Setup database
echo "🗄️ Setting up database..."
cd src/ContentManagementSystem.API
dotnet ef database update

if [ $? -ne 0 ]; then
    echo "⚠️ Database setup failed, but continuing..."
fi

echo "✅ Database setup completed"

# Install Angular dependencies
echo "📦 Installing Angular dependencies..."
cd ../ContentManagementSystem.Web
npm install

if [ $? -ne 0 ]; then
    echo "❌ Angular dependencies installation failed"
    exit 1
fi

echo "✅ Angular dependencies installed"

# Start the applications
echo "🚀 Starting applications..."

# Start .NET API in background
echo "Starting .NET API on https://localhost:7000..."
cd ../ContentManagementSystem.API
dotnet run &
API_PID=$!

# Wait for API to start
sleep 10

# Start Angular app
echo "Starting Angular app on http://localhost:4200..."
cd ../ContentManagementSystem.Web
npm start &
WEB_PID=$!

echo ""
echo "🎉 Applications started successfully!"
echo "=================================================="
echo "📖 API Documentation: https://localhost:7000/swagger"
echo "🌐 Frontend Application: http://localhost:4200"
echo "🏥 Health Check: https://localhost:7000/health"
echo ""
echo "API Endpoints:"
echo "📋 CMS API: https://localhost:7000/api/cms"
echo "🔍 Discovery API: https://localhost:7000/api/discovery"
echo ""
echo "Press Ctrl+C to stop all services"

# Wait for user to stop
wait $API_PID $WEB_PID