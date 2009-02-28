@ECHO OFF
TITLE Cleanup Release Folders
SET /p target=Hit the ENTER key to start the cleanup process!
@ECHO.

DEL "ProcessHacker\bin\Release\Aga.Controls.dll" /Q
DEL "ProcessHacker\bin\Release\Assistant.exe" /Q
DEL "ProcessHacker\bin\Release\base.txt" /Q
DEL "ProcessHacker\bin\Release\CHANGELOG.txt" /Q
DEL "ProcessHacker\bin\Release\HACKING.txt" /Q
DEL "ProcessHacker\bin\Release\Help.htm" /Q
DEL "ProcessHacker\bin\Release\kprocesshacker.sys" /Q
DEL "ProcessHacker\bin\Release\LICENSE.txt" /Q
DEL "ProcessHacker\bin\Release\ProcessHacker.exe" /Q
DEL "ProcessHacker\bin\Release\ProcessHacker.exe.config" /Q
DEL "ProcessHacker\bin\Release\ProcessHacker_in.exe" /Q
DEL "ProcessHacker\bin\Release\processhacker-*-setup.exe" /Q
DEL "ProcessHacker\bin\Release\processhacker-bin.zip" /Q
DEL "ProcessHacker\bin\Release\README.txt" /Q
DEL "ProcessHacker\bin\Release\structs.txt" /Q
DEL "ProcessHacker\bin\Release\*.pdb" /Q

DEL "Assistant\bin\Release\Assistant.exe" /Q
DEL "Assistant\bin\Release\Assistant.pdb" /Q

DEL "TreeViewAdv\bin\Release\Aga.Controls.dll" /Q
DEL "TreeViewAdv\bin\Release\Aga.Controls.pdb" /Q

RD /S /Q "TreeViewAdv\obj\Release\TempPE" /Q
DEL "TreeViewAdv\obj\Release\Aga.Controls*.*" /Q

>NUL PING -n 5 127.0.0.1&&GOTO :EOF