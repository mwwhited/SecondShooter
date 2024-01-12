
SETLOCAL ENABLEEXTENSIONS

REM @ECHO OFF

REM Start -- Configuration this section Only

SET DATABASE_NAME=secondshooter
SET COLLECTION_ASSEMBLY=SecondShooter.Database
SET PERSISTENCE_ASSEMBLY=SecondShooter.Persistance

REM End -- Configuration this section Only 

SET SCRIPT_COLLECTION=..\%COLLECTION_ASSEMBLY%\Generated\%DATABASE_NAME%Context.sql
SET ENTRTY_PATH=%0
SET ENTRY_FOLDER=%~dp0

CALL :NORMALIZEPATH "%~dp0..\..\.."
SET PROJECT_ROOT=%RETVAL%

CALL :NORMALIZEPATH "%PROJECT_ROOT%\tools"
SET TOOL_PATH=%RETVAL%

SET NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED=true
SET NUGET_PLUGIN_PATHS=%TOOL_PATH%\nuget\plugins\netcore\CredentialProvider.Microsoft\CredentialProvider.Microsoft.dll

IF "%1"=="NO_BUILD" GOTO NO_BUILD

dotnet tool restore
@IF NOT %ERRORLEVEL%==0 GOTO :error
dotnet build
@IF NOT %ERRORLEVEL%==0 GOTO :error

:NO_BUILD

dotnet ef dbcontext script --output "%SCRIPT_COLLECTION%" --no-build
@IF NOT %ERRORLEVEL%==0 GOTO :error

GOTO done

:error
ECHO "Failed"

:done
ENDLOCAL

:: ========== FUNCTIONS ==========
EXIT /B

:NORMALIZEPATH
  SET RETVAL=%~f1
  EXIT /B
GOTO DONE