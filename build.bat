@echo off
echo Building ByteShield.API...
echo.

echo.
echo Building Docker image: quickquiz-app:latest
echo Build context: ..\
echo Dockerfile: Dockerfile
docker build -t quickquiz-app:latest --no-cache -f Dockerfile .
if errorlevel 1 (
    echo ERROR: Docker build failed!
    pause
    exit /b 1
)

echo.
echo Saving Docker image to tar file...
docker save quickquiz-app:latest -o quickquiz-app-latest.tar
if errorlevel 1 (
    echo ERROR: Docker save failed!
    pause
    exit /b 1
)

echo.
echo ===================================
echo Build completed successfully!
echo ===================================
echo.
pause