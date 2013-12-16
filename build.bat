@echo off
goto DefineLogFiles

:DefineLogFiles
	set TemporaryLog=Build.log
	set SuccessLog=Success.%TemporaryLog%
	set ErrorLog=Error.%TemporaryLog%
	goto RemoveLogs
	
:RemoveLogs
	if exist %TemporaryLog% del %TemporaryLog%
	if exist %SuccessLog% del %SuccessLog%
	if exist %ErrorLog% del %ErrorLog%
	goto FindMSBuild
	
:FindMSBuild
	echo searching for .net framework
	set Framework=%WINDIR%\Microsoft.NET\Framework
	rem set Framework64=%WINDIR%\Microsoft.NET\Framework64
	rem f exist "%Framework64%" set Framework=%Framework64%
	for /f %%f in ('dir /b /a:D "%Framework%" ^| findstr v[0-9]') do set DotNetFramework=%Framework%\%%f
	if defined DotNetFramework goto RunBuildScript
	goto DotNetNotFound
	
:DotNetNotFound
	echo Could not locate .Net Framework.  Please verify .Net Framework is installed.>%TemporaryLog%
	echo .>%TemporaryLog%
	set ERRORLEVEL=-1
	goto BuildError
	
:RunBuildScript
	@echo %DotNetFramework%\MSBuild.exe build.proj /fl /flp:LogFile=%TemporaryLog%
	%DotNetFramework%\MSBuild.exe build.proj /fl /flp:LogFile=%TemporaryLog%
	IF %ERRORLEVEL% NEQ 0 goto BuildError
	goto BuildSuccess

:BuildSuccess
	set MSBuildLog=%SuccessLog%
	goto Report

:BuildError
	set MSBuildLog=%ErrorLog%
	goto Report

:Report
	rename %TemporaryLog% %MSBuildLog%
	echo.
	echo --- Build Log saved as %MSBuildLog% ---
	goto End

:End
	set TemporaryLog=
	set SuccessLog=
	set ErrorLog=
	set MSBuildLog=
	set Framework=
	rem set Framework64=
	set DotNetFramework=
