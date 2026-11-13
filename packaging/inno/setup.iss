; =====================================================================
; MDReader — Inno Setup Script
; Generates standard Windows installer: MDReader-Setup-x64.exe
; =====================================================================

#define MyAppName "MDReader"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "birotech"
#define MyAppURL "https://github.com/ABDULLAHALI4KSA/MDReader"
#define MyAppSupportEmail "Abdullah444ali@outlook.com"
#define MyAppExeName "MDReader.App.exe"
#define MyAppId "{{B9A5F148-36D2-475C-9A8E-2B60AC00D413}"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL=mailto:{#MyAppSupportEmail}
AppUpdatesURL={#MyAppURL}
AppCopyright=Copyright (C) 2026 birotech. All rights reserved.
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=..\..\dist\installer
OutputBaseFilename=MDReader-Setup-x64
SetupIconFile=..\..\src\MDReader.App\Assets\AppIcon.ico
UninstallDisplayIcon={app}\Assets\AppIcon.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ChangesAssociations=yes
DisableProgramGroupPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "fileassoc"; Description: "Associate Markdown files (.md, .markdown) with MDReader"; GroupDescription: "File Associations:"; Flags: checkedonce

[Files]
Source: "..\..\dist\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\AppIcon.ico"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\AppIcon.ico"; Tasks: desktopicon

[Registry]
; File association for .md
Root: HKA; Subkey: "Software\Classes\.md"; ValueType: string; ValueName: ""; ValueData: "MDReader.Doc"; Flags: uninsdeletevalue; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\.markdown"; ValueType: string; ValueName: ""; ValueData: "MDReader.Doc"; Flags: uninsdeletevalue; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\MDReader.Doc"; ValueType: string; ValueName: ""; ValueData: "Markdown Document"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\MDReader.Doc\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\Assets\AppIcon.ico,0"; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\MDReader.Doc\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Tasks: fileassoc

; Context Menu entry: "Open with MDReader" for any file
Root: HKA; Subkey: "Software\Classes\*\shell\OpenWithMDReader"; ValueType: string; ValueName: ""; ValueData: "Open with MDReader"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\*\shell\OpenWithMDReader"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\Assets\AppIcon.ico"""; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\*\shell\OpenWithMDReader\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Tasks: fileassoc

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
