@echo off
REM ============================================================
REM  Chay demo website Dang Phat Flex
REM  Nhap chuot 2 lan vao file nay de khoi dong website.
REM  Sau khi thay dong "Now listening on...", mo trinh duyet:
REM      http://localhost:5200
REM  Nhan Ctrl + C trong cua so nay de dung website.
REM ============================================================

REM Cho phep chay tren .NET runtime moi hon (may nay dang co .NET 11)
set DOTNET_ROLL_FORWARD=LatestMajor
set DOTNET_ROLL_FORWARD_TO_PRERELEASE=1
set ASPNETCORE_ENVIRONMENT=Development

cd /d "%~dp0src\DangPhatFlex.Web"

echo.
echo   Dang khoi dong website Dang Phat Flex...
echo   Trang chu:  http://localhost:5200
echo   Quan tri:   http://localhost:5200/Admin/Dashboard
echo               (admin@dangphatflex.vn / ChangeMe123!)
echo.

dotnet run --urls "http://localhost:5200"
pause
