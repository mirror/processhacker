:: Set the path of Process Hacker and optimize performance after installation
@ECHO OFF
SET "U_=HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
SET "I_=Inno Setup"
SET "A_=Process_Hacker"
FOR /f "delims=" %%a IN (
	'REG QUERY "%U_%\%A_%_is1" /v "%I_%: App Path"2^>Nul^|FIND "REG_"') DO (
	SET "ProcessHackerPath=%%a"&Call :Sub %%ProcessHackerPath:*Z=%%)

IF DEFINED ProcessHackerPath ("%WINDIR%\Microsoft.NET\Framework\v2.0.50727\ngen.exe" install "%ProcessHackerPath%\ProcessHacker.exe") ELSE (GOTO :END)

:END
GOTO :EOF

:Sub
SET ProcessHackerPath=%*