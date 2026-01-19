@echo off
setlocal

REM FanQuest Backend Setup Script
REM Run from the folder where you want the solution created.

echo =============================
echo Creating solution...
echo =============================
dotnet new sln -n FanQuest
if errorlevel 1 goto :error

echo =============================
echo Creating projects...
echo =============================
dotnet new webapi -n FanQuest.API -o src\FanQuest.API
if errorlevel 1 goto :error

dotnet new classlib -n FanQuest.Application -o src\FanQuest.Application
if errorlevel 1 goto :error

dotnet new classlib -n FanQuest.Domain -o src\FanQuest.Domain
if errorlevel 1 goto :error

dotnet new classlib -n FanQuest.Infrastructure -o src\FanQuest.Infrastructure
if errorlevel 1 goto :error

dotnet new xunit -n FanQuest.UnitTests -o tests\FanQuest.UnitTests
if errorlevel 1 goto :error

echo =============================
echo Adding projects to solution...
echo =============================
dotnet sln add src\FanQuest.API\FanQuest.API.csproj
dotnet sln add src\FanQuest.Application\FanQuest.Application.csproj
dotnet sln add src\FanQuest.Domain\FanQuest.Domain.csproj
dotnet sln add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj
dotnet sln add tests\FanQuest.UnitTests\FanQuest.UnitTests.csproj

echo =============================
echo Adding project references...
echo =============================
dotnet add src\FanQuest.API\FanQuest.API.csproj reference src\FanQuest.Application\FanQuest.Application.csproj
dotnet add src\FanQuest.API\FanQuest.API.csproj reference src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj

dotnet add src\FanQuest.Application\FanQuest.Application.csproj reference src\FanQuest.Domain\FanQuest.Domain.csproj

dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj reference src\FanQuest.Domain\FanQuest.Domain.csproj
dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj reference src\FanQuest.Application\FanQuest.Application.csproj

dotnet add tests\FanQuest.UnitTests\FanQuest.UnitTests.csproj reference src\FanQuest.Domain\FanQuest.Domain.csproj
dotnet add tests\FanQuest.UnitTests\FanQuest.UnitTests.csproj reference src\FanQuest.Application\FanQuest.Application.csproj

echo =============================
echo Installing NuGet packages...
echo =============================

REM API packages
dotnet add src\FanQuest.API\FanQuest.API.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src\FanQuest.API\FanQuest.API.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src\FanQuest.API\FanQuest.API.csproj package Microsoft.AspNetCore.SignalR
dotnet add src\FanQuest.API\FanQuest.API.csproj package StackExchange.Redis
dotnet add src\FanQuest.API\FanQuest.API.csproj package Serilog.AspNetCore
dotnet add src\FanQuest.API\FanQuest.API.csproj package Swashbuckle.AspNetCore

REM Infrastructure packages
dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj package StackExchange.Redis
dotnet add src\FanQuest.Infrastructure\FanQuest.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools

REM Application packages
dotnet add src\FanQuest.Application\FanQuest.Application.csproj package FluentValidation

REM Test packages
dotnet add tests\FanQuest.UnitTests\FanQuest.UnitTests.csproj package Moq
dotnet add tests\FanQuest.UnitTests\FanQuest.UnitTests.csproj package FluentAssertions

echo =============================
echo Setup complete!
echo Build solution using:
echo dotnet build
echo =============================
goto :done

:error
echo.
echo ❌ Setup failed. Check the error above.
exit /b 1

:done
endlocal
