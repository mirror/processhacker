@ECHO OFF
SETLOCAL
SET outd=%~p1

::Copy CHANGELOG.txt, LICENSE.txt, README.txt and kprocesshacker.sys
COPY "%outd%\..\..\..\CHANGELOG.txt" "%outd%\" /V >nul 2>&1
COPY "%outd%\..\..\..\LICENSE.txt" "%outd%\" /V >nul 2>&1
COPY "%outd%\..\..\..\README.txt" "%outd%\" /V >nul 2>&1
COPY "%outd%\..\..\..\KProcessHacker\i386\kprocesshacker.sys" "%outd%\" /V >nul 2>&1

::Clear older files
DEL "%outd%\ProcessHacker.exe.config" /Q >nul 2>&1
DEL "%outd%\processhacker-*-setup.exe" /Q >nul 2>&1
DEL "%outd%\ProcessHacker_in.exe" /Q >nul 2>&1
DEL "%outd%\processhacker-bin.zip" /Q >nul 2>&1

::Check if ILMerge is present in the default installation location
IF EXIST "%PROGRAMFILES%\Microsoft\ILMerge\ILMerge.exe" (
	SET ILMergePath=%PROGRAMFILES%\Microsoft\ILMerge\ILMerge.exe&&GOTO :ILMerge
) ELSE (
	GOTO :SearchILMergeInPATH
)

:SearchILMergeInPATH
::Check if ILMerge is present in PATH
SET "IL_=ILMerge.exe"
SET "MSGIL_=ILMerge IS NOT INSTALLED"

FOR %%# IN (%IL_%) DO IF %%~$PATH:#' EQU ' (
	ECHO:%MSGIL_%&&GOTO :END
) ELSE (
	GOTO :ILMergeInPATH
)

:ILMergeInPath
REN "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
DEL "%outd%\ProcessHacker_in.exe" /Q >nul 2>&1
DEL "%outd%\Aga.Controls.dll" /Q >nul 2>&1
GOTO :Installer

:ILMerge
rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
"%ILMergePath%" /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
DEL "%outd%\ProcessHacker_in.exe" /Q >nul 2>&1
DEL "%outd%\Aga.Controls.dll" /Q >nul 2>&1
GOTO :Installer

:Installer
::Set the path of Inno Setup and compile setup
SET "U_=HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
SET "I_=Inno Setup"

SET "A_=%I_% 5"
SET "M_=Inno Setup IS NOT INSTALLED"

FOR /f "delims=" %%a IN (
'REG QUERY "%U_%\%A_%_is1" /v "%I_%: App Path"2^>Nul^|FIND "REG_"') DO (
SET "InnoSetupPath=%%a
SET "InnoSetupPath=%%a"&Call :Sub %%InnoSetupPath:*Z=%%)

IF NOT DEFINED InnoSetupPath ECHO:%M_%&&GOTO :END

"%InnoSetupPath%\iscc.exe" /Q /O"%outd%\..\..\bin\Release" "%outd%\..\..\Build\Installer\Process_Hacker_installer.iss"
ECHO Instaler compiled successfully&&GOTO :CLEANUP

:CLEANUP
::Clean up the .pdb files
DEL "%outd%\*.pdb" /Q >nul 2>&1
GOTO :ZIP

:ZIP
"%outd%\..\..\Build\7za\7za.exe" a -tzip "processhacker-bin.zip" "*" -x!*setup.exe -mx=9 >nul
ECHO ZIP created successfully

:END

:Sub
SET InnoSetupPath=%*