@echo off
set outd=%~p1

ilmerge /t:winexe /out:"%outd%\ProcessHacker_out.exe" "%outd%\ProcessHacker.exe" "%outd%\Aga.Controls.dll"
del "%outd%\ProcessHacker.exe"
del "%outd%\Aga.Controls.dll"
del "%outd%\*.pdb"
rename "%outd%\ProcessHacker_out.exe" ProcessHacker.exe