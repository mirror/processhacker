@echo off
set outd=%~p1

@ check if ILMerge is present
where ilmerge
if not %errorlevel%==0 goto end

rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
del "%outd%\ProcessHacker_in.exe"
del "%outd%\Aga.Controls.dll"

:end
del "%outd%\*.pdb"