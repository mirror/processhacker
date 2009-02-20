@echo off
set outd=%~p1

copy "%outd%\..\..\..\*.txt" "%outd%\"
copy "%outd%\..\..\..\KProcessHacker\i386\kprocesshacker.sys" "%outd%\"

@rem check if ILMerge is present
ilmerge
if %errorlevel%==9009 goto end

rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
del "%outd%\ProcessHacker_in.exe"
del "%outd%\Aga.Controls.dll"
del "%outd%\ProcessHacker.exe.config"

@rem check Inno Setup
iscc
if %errorlevel%==9009 goto end

iscc /q /o"%outd%\..\..\bin\Release" /fSetup "%outd%\..\..\Build\Installer\Process_Hacker_installer.iss"

:end
del "%outd%\*.pdb"