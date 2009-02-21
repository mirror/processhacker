@ECHO OFF
SET outd=%~p1

::Copy CHANGELOG.txt, HACKING.txt, LICENSE.txt, README.txt and kprocesshacker.sys
COPY "%outd%\..\..\..\CHANGELOG.txt" "%outd%\" /V
COPY "%outd%\..\..\..\HACKING.txt" "%outd%\" /V
COPY "%outd%\..\..\..\LICENSE.txt" "%outd%\" /V
COPY "%outd%\..\..\..\README.txt" "%outd%\" /V
COPY "%outd%\..\..\..\KProcessHacker\i386\kprocesshacker.sys" "%outd%\" /V

::Clear older files
DEL "%outd%\ProcessHacker.exe.config" /Q >nul 2>&1
DEL "%outd%\processhacker-*-setup.exe" /Q >nul 2>&1
DEL "%outd%\ProcessHacker_in.exe" /Q >nul 2>&1

::Check if ILMerge is present
ilmerge
IF %errorlevel%==9009 GOTO END

rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
DEL "%outd%\ProcessHacker_in.exe" /Q >nul 2>&1
DEL "%outd%\Aga.Controls.dll" /Q >nul 2>&1

::Set the path of Inno Setup and compile setup
FOR /f "tokens=3 skip=3 delims=    " %%i in (
    'reg query "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\Inno Setup 5_is1" /v "Inno Setup: App Path"'
) DO (
    SET InnoSetupPath=%%i
)

"%InnoSetupPath%iscc.exe" /q /o"%outd%\..\..\bin\Release" "%outd%\..\..\Build\Installer\Process_Hacker_installer.iss"

:END
::Clean up the .pdb files
DEL "%outd%\*.pdb" /Q >nul 2>&1