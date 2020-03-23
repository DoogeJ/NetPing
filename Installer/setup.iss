#define use_dotnetfx46

#define MyAppName "NetPing"
#define MyAppVersion "1.0.2.0"
#define MyAppPublisher "DoogeJ"
#define MyAppURL "https://github.com/DoogeJ/NetPing"
#define MyAppExeName "NetPing.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{8F0F9A45-E913-479D-97DE-0E8017B7B687}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=bin
LicenseFile=..\LICENSE
OutputBaseFilename=Install_{#MyAppName}_{#MyAppVersion}
SetupIconFile=..\1476172268_electronics-20.ico
Compression=lzma
SolidCompression=yes
UsePreviousGroup=no
UsePreviousAppDir=no  

; we will need administrator privileges to copy to program files and / or install prerequirements
PrivilegesRequired=admin

;Downloading and installing dependencies will only work if the memo/ready page is enabled (default behaviour)
DisableReadyPage=no
DisableReadyMemo=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\NetPing\bin\Release\NetPing.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2


; shared code for installing the products
#include "scripts\products.iss"
; helper functions
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"

#ifdef use_dotnetfx46
#include "scripts\products\dotnetfx46.iss"
#endif


[Code]
function InitializeSetup(): boolean;
begin
	// initialize windows version
	initwinversion();

#ifdef use_dotnetfx46
    dotnetfx46(50); // min allowed version is 4.5.0
#endif

	Result := true;
end;