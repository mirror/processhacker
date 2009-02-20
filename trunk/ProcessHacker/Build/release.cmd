@ECHO OFF
SET outd=%~p1

::Copy CHANGELOG.txt, HACKING.txt, LICENSE.txt, README.txt and kprocesshacker.sys
COPY "%outd%\..\..\..\*.txt" "%outd%\"
COPY "%outd%\..\..\..\KProcessHacker\i386\kprocesshacker.sys" "%outd%\"

::Clear older files
DEL "%outd%\ProcessHacker.exe.config"
DEL "%outd%\processhacker-*-setup.exe"
DEL "%outd%\ProcessHacker_in.exe"

::Check if ILMerge is present
ilmerge
IF %errorlevel%==9009 GOTO END

rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
DEL "%outd%\ProcessHacker_in.exe"
DEL "%outd%\Aga.Controls.dll"

::Set the path of Inno Setup and compile setup
FOR /f "tokens=3 skip=3 delims=    " %%i in (
    'reg query "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\Inno Setup 5_is1" /v "Inno Setup: App Path"'
) DO (
    SET InnoSetupPath=%%i
)

"%InnoSetupPath%iscc.exe" /q /o"%outd%\..\..\bin\Release" "%outd%\..\..\Build\Installer\Process_Hacker_installer.iss"

:END
::Clean up the .pdb files
DEL "%outd%\*.pdb"