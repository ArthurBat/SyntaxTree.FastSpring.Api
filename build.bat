@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Package restore
REM tools\nuget.exe restore src\SyntaxTree.FastSpring.Api.sln -OutputDirectory %cd%\src\packages -NonInteractive

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\SyntaxTree.FastSpring.Api.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir release
mkdir release\nuget
tools\nuget.exe pack "src\SyntaxTree.FastSpring.Api\SyntaxTree.FastSpring.Api.csproj" -symbols -o release\nuget -p Configuration=%config% %version%

REM Plain assemblies
mkdir release\assemblies
copy src\SyntaxTree.FastSpring.Api\bin\%config%\SyntaxTree.FastSpring*.dll release\assemblies
