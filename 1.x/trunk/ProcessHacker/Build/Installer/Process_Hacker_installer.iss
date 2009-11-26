;* Process Hacker - Installer script
;*
;* Copyright (C) 2009 XhmikosR
;*
;* This file is part of Process Hacker.
;*
;* Process Hacker is free software; you can redistribute it and/or modify
;* it under the terms of the GNU General Public License as published by
;* the Free Software Foundation, either version 3 of the License, or
;* (at your option) any later version.
;*
;* Process Hacker is distributed in the hope that it will be useful,
;* but WITHOUT ANY WARRANTY; without even the implied warranty of
;* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;* GNU General Public License for more details.
;*
;* You should have received a copy of the GNU General Public License
;* along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.


; Inno Setup v5.3.6
;
; Requirements:
; *Inno Setup QuickStart Pack:
;   http://www.jrsoftware.org/isdl.php#qsp


#define installer_build_number "40"

#define VerMajor
#define VerMinor
#define VerRevision
#define VerBuild

#expr ParseVersion("..\..\bin\Release\ProcessHacker.exe", VerMajor, VerMinor, VerRevision, VerBuild)
#define app_version str(VerMajor) + "." + str(VerMinor) + "." + str(VerRevision) + "." + str(VerBuild)
#define simple_app_version str(VerMajor) + "." + str(VerMinor)
#define installer_build_date GetDateTimeString('dd/mm/yyyy', '.', '')


[Setup]
AppID=Process_Hacker
AppCopyright=Copyright © 2008-2009, Process Hacker Team. Licensed under the GNU GPL, v3.
AppContact=http://sourceforge.net/tracker/?group_id=242527
AppName=Process Hacker
AppVerName=Process Hacker {#= simple_app_version}
AppVersion={#= simple_app_version}
AppPublisher=wj32
AppPublisherURL=http://processhacker.sourceforge.net/
AppSupportURL=http://sourceforge.net/tracker/?group_id=242527
AppUpdatesURL=http://processhacker.sourceforge.net/
UninstallDisplayName=Process Hacker {#= simple_app_version}
DefaultDirName={pf}\Process Hacker
DefaultGroupName=Process Hacker
VersionInfoCompany=wj32
VersionInfoCopyright=Licensed under the GNU GPL, v3.
VersionInfoDescription=Process Hacker {#= simple_app_version} Setup
VersionInfoTextVersion={#= app_version}
VersionInfoVersion={#= app_version}
VersionInfoProductName=Process Hacker
VersionInfoProductVersion={#= app_version}
VersionInfoProductTextVersion={#= app_version}
MinVersion=0,5.0.2195
AppReadmeFile={app}\README.txt
LicenseFile=..\..\..\LICENSE.txt
InfoAfterFile=..\..\..\CHANGELOG.txt
InfoBeforeFile=..\..\..\README.txt
SetupIconFile=Icons\ProcessHacker.ico
UninstallDisplayIcon={app}\ProcessHacker.exe
WizardImageFile=Icons\ProcessHackerLarge.bmp
WizardSmallImageFile=Icons\ProcessHackerSmall.bmp
OutputDir=.
OutputBaseFilename=processhacker-{#= simple_app_version}-setup
AllowNoIcons=yes
Compression=lzma/ultra64
SolidCompression=yes
InternalCompressLevel=ultra64
EnableDirDoesntExistWarning=no
DirExistsWarning=no
ShowTasksTreeLines=yes
AlwaysShowDirOnReadyPage=yes
AlwaysShowGroupOnReadyPage=yes
WizardImageStretch=no
PrivilegesRequired=admin
ShowLanguageDialog=auto
DisableDirPage=auto
DisableProgramGroupPage=auto
LanguageDetectionMethod=uilanguage
AppMutex=Global\ProcessHackerMutex
ArchitecturesInstallIn64BitMode=x64


[Languages]
; Installer's languages
Name: en; MessagesFile: compiler:Default.isl
Name: de; MessagesFile: compiler:Languages\German.isl
Name: gr; MessagesFile: Languages\Greek.isl


; Include the installer's custom messages and services stuff
#include "Custom_Messages.iss"
#include "Services.iss"


[Messages]
BeveledLabel=Process Hacker v{#= simple_app_version} by wj32                                                                      Setup v{#= installer_build_number} built on {#= installer_build_date}


[Files]
Source: ..\..\bin\Release\base.txt; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\CHANGELOG.txt; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\Help.htm; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\LICENSE.txt; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\kprocesshacker.sys; DestDir: {app}; Flags: ignoreversion; Check: NOT Is64BitInstallMode(); MinVersion: 0,5.01.2600
Source: ..\..\bin\Release\NProcessHacker.dll; DestDir: {app}; Flags: ignoreversion; Check: NOT Is64BitInstallMode()
Source: ..\..\bin\Release\NProcessHacker64.dll; DestName: NProcessHacker.dll; DestDir: {app}; Flags: ignoreversion; Check: Is64BitInstallMode()
Source: ..\..\bin\Release\ProcessHacker.exe; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\README.txt; DestDir: {app}; Flags: ignoreversion
Source: ..\..\bin\Release\structs.txt; DestDir: {app}; Flags: ignoreversion
Source: Icons\uninstall.ico; DestDir: {app}; Flags: ignoreversion


[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}
Name: desktopicon\user; Description: {cm:tsk_CurrentUser}; GroupDescription: {cm:AdditionalIcons}; Flags: exclusive
Name: desktopicon\common; Description: {cm:tsk_AllUsers}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked exclusive
Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; OnlyBelowVersion: 0,6.01; Flags: unchecked

Name: startup_task; Description: {cm:tsk_StartupDescr}; GroupDescription: {cm:tsk_Startup}; Check: StartupCheck(); Flags: unchecked checkablealone
Name: startup_task\minimized; Description: {cm:tsk_StartupDescrMin}; GroupDescription: {cm:tsk_Startup}; Check: StartupCheck(); Flags: unchecked
Name: remove_startup_task; Description: {cm:tsk_RemoveStartup}; GroupDescription: {cm:tsk_Startup}; Check: NOT StartupCheck(); Flags: unchecked

Name: create_KPH_service; Description: {cm:tsk_CreateKPHService}; GroupDescription: {cm:tsk_Other}; Check: NOT KPHServiceCheck() AND NOT Is64BitInstallMode(); Flags: unchecked dontinheritcheck
Name: delete_KPH_service; Description: {cm:tsk_DeleteKPHService}; GroupDescription: {cm:tsk_Other}; Check: KPHServiceCheck() AND NOT Is64BitInstallMode(); Flags: unchecked dontinheritcheck

Name: reset_settings; Description: {cm:tsk_ResetSettings}; GroupDescription: {cm:tsk_Other}; Check: SettingsExistCheck(); Flags: unchecked checkablealone

Name: set_default_taskmgr; Description: {cm:tsk_SetDefaultTaskmgr}; GroupDescription: {cm:tsk_Other}; Check: PHDefaulTaskmgrCheck(); Flags: unchecked dontinheritcheck
Name: restore_taskmgr; Description: {cm:tsk_RestoreTaskmgr}; GroupDescription: {cm:tsk_Other}; Check: NOT PHDefaulTaskmgrCheck(); Flags: unchecked dontinheritcheck


[Icons]
Name: {group}\Process Hacker; Filename: {app}\ProcessHacker.exe; Comment: Process Hacker {#= simple_app_version}; WorkingDir: {app}; IconFilename: {app}\ProcessHacker.exe; IconIndex: 0
Name: {group}\{cm:sm_Help}\{cm:sm_Changelog}; Filename: {app}\CHANGELOG.txt; Comment: {cm:sm_com_Changelog}; WorkingDir: {app}
Name: {group}\{cm:sm_Help}\{cm:sm_HelpFile}; Filename: {app}\Help.htm; Comment: {cm:sm_HelpFile}; WorkingDir: {app}
Name: {group}\{cm:sm_Help}\{cm:sm_ReadmeFile}; Filename: {app}\README.txt; Comment: {cm:sm_com_ReadmeFile}; WorkingDir: {app}
Name: {group}\{cm:sm_Help}\{cm:ProgramOnTheWeb,Process Hacker}; Filename: http://processhacker.sourceforge.net/; Comment: {cm:ProgramOnTheWeb,Process Hacker}
Name: {group}\{cm:UninstallProgram,Process Hacker}; Filename: {uninstallexe}; IconFilename: {app}\uninstall.ico; Comment: {cm:UninstallProgram,Process Hacker}; WorkingDir: {app}

Name: {commondesktop}\Process Hacker; Filename: {app}\ProcessHacker.exe; Tasks: desktopicon\common; Comment: Process Hacker {#= simple_app_version}; WorkingDir: {app}; IconFilename: {app}\ProcessHacker.exe; IconIndex: 0
Name: {userdesktop}\Process Hacker; Filename: {app}\ProcessHacker.exe; Tasks: desktopicon\user; Comment: Process Hacker {#= simple_app_version}; WorkingDir: {app}; IconFilename: {app}\ProcessHacker.exe; IconIndex: 0
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\Process Hacker; Filename: {app}\ProcessHacker.exe; Tasks: quicklaunchicon; Comment: Process Hacker {#= simple_app_version}; WorkingDir: {app}; IconFilename: {app}\ProcessHacker.exe; IconIndex: 0


[InstallDelete]
; Remove files from the install folder which are not needed anymore
Type: files; Name: {app}\ProcessHacker.exe.config
Type: files; Name: {app}\HACKING.txt
Type: files; Name: {app}\psvince.dll
Type: files; Name: {app}\Homepage.url
Type: files; Name: {app}\kprocesshacker.sys; Check: Is64BitInstallMode()

Type: files; Name: {userdesktop}\Process Hacker.lnk; Tasks: NOT desktopicon\user
Type: files; Name: {commondesktop}\Process Hacker.lnk; Tasks: NOT desktopicon\common

; Remove other languages' shortcuts in Start Menu
Type: files; Name: {group}\Process Hacker's Readme file.lnk
Type: files; Name: {group}\Process Hacker on the Web.url
Type: files; Name: {group}\Uninstall Process Hacker.lnk
Type: files; Name: {group}\Help and Support\Process Hacker on the Web.url
Type: files; Name: {group}\Help and Support\Change Log.lnk
Type: files; Name: {group}\Help and Support\Changelog.lnk
Type: files; Name: {group}\Help and Support\Process Hacker's Help.lnk
Type: files; Name: {group}\Help and Support\ReadMe File.lnk
Type: files; Name: {group}\Help and Support\ReadMe.lnk
Type: dirifempty; Name: {group}\Help and Support

Type: files; Name: {group}\Αρχείο βοήθειας του Process Hacker.lnk
Type: files; Name: {group}\Το Process Hacker στο Internet.url
Type: files; Name: {group}\Απεγκατάσταση του Process Hacker.lnk
Type: files; Name: {group}\Βοήθεια και Υποστήριξη\Το Process Hacker στο Internet.url
Type: files; Name: {group}\Βοήθεια και Υποστήριξη\Ιστορικό Εκδόσεων.lnk
Type: files; Name: {group}\Βοήθεια και Υποστήριξη\Αρχείο βοήθειας του Process Hacker.lnk
Type: files; Name: {group}\Βοήθεια και Υποστήριξη\Αρχείο ReadMe.lnk
Type: dirifempty; Name: {group}\Βοήθεια και Υποστήριξη

; Always remove older settings folder
Type: filesandordirs; Name: {localappdata}\wj32

Type: files; Name: {userappdata}\Process Hacker\settings.xml; Tasks: reset_settings
Type: dirifempty; Name: {userappdata}\Process Hacker; Tasks: reset_settings


[Registry]
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe; Flags: uninsdeletekeyifempty dontcreatekey
Root: HKCU; SubKey: Software\Microsoft\Windows\CurrentVersion\Run; ValueType: string; ValueName: Process Hacker; ValueData: """{app}\ProcessHacker.exe"""; Tasks: startup_task; Flags: uninsdeletevalue
Root: HKCU; SubKey: Software\Microsoft\Windows\CurrentVersion\Run; ValueType: string; ValueName: Process Hacker; ValueData: """{app}\ProcessHacker.exe"" -m"; Tasks: startup_task\minimized; Flags: uninsdeletevalue
Root: HKCU; SubKey: Software\Microsoft\Windows\CurrentVersion\Run; ValueName: Process Hacker; Tasks: remove_startup_task; Flags: deletevalue uninsdeletevalue
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe; ValueType: string; ValueName: Debugger; ValueData: """{app}\ProcessHacker.exe"""; Tasks: set_default_taskmgr
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe; ValueType: string; ValueName: Debugger; ValueData: """{app}\ProcessHacker.exe"""; Flags: uninsdeletevalue; Check: NOT PHDefaulTaskmgrCheck()
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe; ValueName: Debugger; Tasks: restore_taskmgr reset_settings; Flags: deletevalue uninsdeletevalue; Check: NOT PHDefaulTaskmgrCheck()

; Windows Error Reporting keys
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps; ValueType: none; Flags: uninsdeletekeyifempty createvalueifdoesntexist; MinVersion: 0,6.0.6001
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\ProcessHacker.exe; ValueType: none; Flags: uninsdeletekey; MinVersion: 0,6.0.6001
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\ProcessHacker.exe; ValueType: dword; ValueName: DumpCount; ValueData: 5; Flags: uninsdeletevalue; MinVersion: 0,6.0.6001
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\ProcessHacker.exe; ValueType: expandsz; ValueName: DumpFolder; ValueData: {sd}\ProgramData\wj32\mdmp; Flags: uninsdeletevalue; MinVersion: 0,6.0.6001
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\ProcessHacker.exe; ValueType: dword; ValueName: DumpType; ValueData: 1; Flags: uninsdeletevalue; MinVersion: 0,6.0.6001


[Run]
Filename: {win}\Microsoft.NET\Framework\v2.0.50727\ngen.exe; Parameters: "install ""{app}\ProcessHacker.exe"""; StatusMsg: {cm:msg_OptimizingPerformance}; Flags: runhidden runascurrentuser skipifdoesntexist

Filename: {app}\ProcessHacker.exe; Description: {cm:LaunchProgram,Process Hacker}; Flags: nowait postinstall skipifsilent runascurrentuser
Filename: http://processhacker.sourceforge.net/; Description: {cm:run_VisitWebsite}; Flags: nowait postinstall skipifsilent shellexec runascurrentuser unchecked


[UninstallDelete]
Name: {app}\Homepage.url; Type: files
Name: {sd}\ProgramData\wj32\*.dmp; Type: files; MinVersion: 0,6.0.6001
Name: {sd}\ProgramData\wj32\mdmp\*.dmp; Type: files; MinVersion: 0,6.0.6001
Name: {sd}\ProgramData\wj32\mdmp; Type: dirifempty; MinVersion: 0,6.0.6001
Name: {sd}\ProgramData\wj32; Type: dirifempty; MinVersion: 0,6.0.6001


[Code]
// Global variables and constants
var
  SetResetTask: Boolean;

const installer_mutex_name = 'process_hacker_setup_mutex';


// Check if Process Hacker is configured to run on startup in order to control
// startup choice from within the installer
function StartupCheck(): Boolean;
begin
  Result := True;
  if RegValueExists(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Run', 'Process Hacker') then
  Result := False;
end;


// Check if Process Hacker's settings exist
function SettingsExistCheck(): Boolean;
begin
  Result := False;
  if FileExists(ExpandConstant('{userappdata}\Process Hacker\settings.xml')) then
  Result := True;
end;


// Check if Process Hacker is set as the default Task Manager for Windows
function PHDefaulTaskmgrCheck(): Boolean;
var
  svalue: String;
begin
  Result := True;
  if RegQueryStringValue(HKLM,
  'SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe', 'Debugger', svalue) then begin
    if svalue = (ExpandConstant('"{app}\ProcessHacker.exe"')) then
    Result := False;
  end;
end;


// Check if KProcessHacker is installed as a service
function KPHServiceCheck(): Boolean;
var
  dvalue: DWORD;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SYSTEM\CurrentControlSet\Services\KProcessHacker', 'Start', dvalue) then begin
    if dvalue = 1 then
    Result := True;
  end;
end;


procedure CleanUpFiles();
begin
  DeleteFile(ExpandConstant('{userappdata}\Process Hacker\settings.xml'));
  RemoveDir(ExpandConstant('{userappdata}\Process Hacker\'));
  DeleteFile(ExpandConstant('{app}\Process Hacker.txt'));
  DeleteFile(ExpandConstant('{app}\Process Hacker.log'));
  DeleteFile(ExpandConstant('{app}\Process Hacker.csv'));
  DeleteFile(ExpandConstant('{app}\Process Hacker Log.txt'));
  DeleteFile(ExpandConstant('{app}\CSR Processes.txt'));
  DeleteFile(ExpandConstant('{userdocs}\Process Hacker Log.txt'));
  DeleteFile(ExpandConstant('{userdocs}\CSR Processes.txt'));
  DeleteFile(ExpandConstant('{app}\scratchpad.txt'));
end;


// Bypass Inno Setup UsePreviousTasks directive only for the "reset_settings" task
procedure UncheckTask();
var
  i: Integer;
begin
  i := WizardForm.TasksList.Items.IndexOf(ExpandConstant('{cm:tsk_ResetSettings}'));

  if(i <> -1) then begin
    WizardForm.TasksList.Checked[i] := False;
  end;
end;


procedure URLLabelOnClick(Sender: TObject);
var
  ErrorCode: Integer;
begin
  ShellExec('open', 'http://processhacker.sourceforge.net/', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;


procedure CreateURLLabel(ParentForm: TSetupForm; CancelButton: TNewButton);
var
  URLLabel: TNewStaticText;
begin
  URLLabel := TNewStaticText.Create(ParentForm);
  URLLabel.Caption := 'Homepage';
  URLLabel.Cursor := crHand;
  URLLabel.OnClick := @URLLabelOnClick;
  URLLabel.Parent := ParentForm;
  { Alter Font *after* setting Parent so the correct defaults are inherited first }
  URLLabel.Font.Style := URLLabel.Font.Style + [fsUnderline];
  URLLabel.Font.Color := clBlue;
  URLLabel.Top := CancelButton.Top + CancelButton.Height - URLLabel.Height - 2;
  URLLabel.Left := ParentForm.ClientWidth - CancelButton.Left - CancelButton.Width;
end;


procedure InitializeWizard();
begin
  CreateURLLabel(WizardForm, WizardForm.CancelButton);
end;


procedure InitializeUninstallProgressForm();
begin
  CreateURLLabel(UninstallProgressForm, UninstallProgressForm.CancelButton);
end;


procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of ssInstall:
  begin
    if IsServiceRunning('KProcessHacker') then begin
      StopService('KProcessHacker');
    end;
    if IsTaskSelected('delete_KPH_service') then begin
      RemoveService('KProcessHacker');
    end;
  end;
  ssPostInstall:
  begin
    if (KPHServiceCheck AND NOT IsTaskSelected('delete_KPH_service') OR (IsTaskSelected('create_KPH_service'))) then begin
      StopService('KProcessHacker');
      RemoveService('KProcessHacker');
      InstallService(ExpandConstant('{app}\kprocesshacker.sys'),'KProcessHacker','KProcessHacker','KProcessHacker driver',SERVICE_KERNEL_DRIVER,SERVICE_SYSTEM_START);
      StartService('KProcessHacker');
    end;
  end;
 end;
end;


procedure CurPageChanged(CurPageID: Integer);
begin
  if not SetResetTask and (CurPageID = wpSelectTasks) then begin
    UncheckTask();
    SetResetTask := True;
  end;
end;


Procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  // When uninstalling ask user to delete Process Hacker's logs and settings
  // based on whether these files exist only
  if CurUninstallStep = usUninstall then begin
    StopService('KProcessHacker');
    RemoveService('KProcessHacker');
  if FileExists(ExpandConstant('{userappdata}\Process Hacker\settings.xml'))
  OR fileExists(ExpandConstant('{app}\Process Hacker Log.txt'))
  OR fileExists(ExpandConstant('{userdocs}\Process Hacker.txt'))
  OR fileExists(ExpandConstant('{userdocs}\Process Hacker.log'))
  OR fileExists(ExpandConstant('{userdocs}\Process Hacker.csv'))
  OR fileExists(ExpandConstant('{userdocs}\Process Hacker Log.txt'))
  OR fileExists(ExpandConstant('{userdocs}\CSR Processes.txt'))
  OR fileExists(ExpandConstant('{app}\scratchpad.txt'))then begin
    if MsgBox(ExpandConstant('{cm:msg_DeleteLogSettings}'),
     mbConfirmation, MB_YESNO or MB_DEFBUTTON2) = IDYES then begin
       CleanUpFiles;
     end;
      //Always delete older settings folder
      DelTree(ExpandConstant('{localappdata}\wj32\'), True, True, True);
      RemoveDir(ExpandConstant('{app}'));
    end;
  end;
end;


function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
begin
  // Create a mutex for the installer and if it's already running then expose a message and stop installation
  if CheckForMutexes(installer_mutex_name) then begin
    if not WizardSilent() then
      MsgBox(ExpandConstant('{cm:msg_SetupIsRunningWarning}'), mbCriticalError, MB_OK);
    exit;
  end;
  CreateMutex(installer_mutex_name);

  // Check if .NET Framework 2.0 is installed and if not offer to download it
  try
    ExpandConstant('{dotnet20}');
    Result := True;
  except
    begin
      if not WizardSilent() then
        if MsgBox(ExpandConstant('{cm:msg_AskToDownNET}'), mbCriticalError, MB_YESNO or MB_DEFBUTTON1) = IDYES then begin
          Result := False;
          ShellExec('open', 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe',
          '','',SW_SHOWNORMAL,ewNoWait,ErrorCode);
        end else begin
          Result := False;
        end;
    end;
  end;
end;


function InitializeUninstall(): Boolean;
begin
  Result := True;
  if CheckForMutexes(installer_mutex_name) then begin
    if not WizardSilent() then
      MsgBox(ExpandConstant('{cm:msg_SetupIsRunningWarning}'), mbCriticalError, MB_OK);
      exit;
   end;
   CreateMutex(installer_mutex_name);
end;
