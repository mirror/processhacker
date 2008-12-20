@echo off
set outd=%~p1

rename "%outd%\ProcessHacker.exe" ProcessHacker_in.exe
ilmerge /t:winexe /out:"%outd%\ProcessHacker.exe" "%outd%\ProcessHacker_in.exe" "%outd%\Aga.Controls.dll"
del "%outd%\ProcessHacker_in.exe"
del "%outd%\Aga.Controls.dll"
del "%outd%\*.pdb"