; Hammertime NSIS Installer
; ---------------------

; Installer Info
Name "Hammertime"
OutFile "../Hammertime.Editor.{version}.{arch}.exe"
InstallDir "$PROGRAMFILES\HammertimeEditor"
InstallDirRegKey HKLM "Software\Hammertime\Editor" "InstallDir"
RequestExecutionLevel admin

; Version Info
VIProductVersion "{version}"
VIAddVersionKey "FileVersion" "{version}"
VIAddVersionKey "ProductName" "Hammertime Editor"
VIAddVersionKey "FileDescription" "Installer for Hammertime Editor"
VIAddVersionKey "LegalCopyright" "https://github.com/Duude92/hammertime 2024"

; Ensure Admin Rights
!include LogicLib.nsh

Function .onInit
    UserInfo::GetAccountType
    pop $0
    ${If} $0 != "admin" ;Require admin rights on NT4+
        MessageBox mb_iconstop "Administrator rights required!" /SD IDOK
        SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        Quit
    ${EndIf}
FunctionEnd

; Installer Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

; Installer Sections

Section "Clean Install"
    IfSilent 0 +2
        Goto end ; Silent update: Don't use clean install
        
    Delete "$INSTDIR\*.dll"

    end:
SectionEnd

Section "Hammertime Editor"
    IfSilent 0 +2 ; Silent mode: Hammertime has executed the installer for an update
        Sleep 2000 ; Make sure the program has shut down...
    
    SectionIn RO
    SetOutPath $INSTDIR

    ; Purge junk from old installs
    ; Delete "$INSTDIR\*.dll"
    Delete "$INSTDIR\*.pdb"
    Delete "$INSTDIR\Hammertime.Editor.Elevate.exe"
    Delete "$INSTDIR\Hammertime.Editor.Updater.exe"
    Delete "$INSTDIR\UpdateSources.txt"

    File /r "../bin\*"
    
    WriteRegStr HKLM "Software\Hammertime\Editor" "InstallDir" "$INSTDIR"
    WriteRegStr HKLM "Software\Hammertime\Editor" "Version" "{version}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HammertimeEditor" "DisplayName" "Hammertime Editor"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HammertimeEditor" "UninstallString" '"$INSTDIR\Uninstall.exe"'
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HammertimeEditor" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HammertimeEditor" "NoRepair" 1
    WriteUninstaller "Uninstall.exe"
SectionEnd

Section "Start Menu Shortcuts"
    IfSilent 0 +2
        Goto end ; Silent update: Don't redo shortcuts
        
    SetShellVarContext all
    CreateDirectory "$SMPROGRAMS\Hammertime Editor"
    CreateShortCut "$SMPROGRAMS\Hammertime Editor\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
    CreateShortCut "$SMPROGRAMS\Hammertime Editor\Hammertime Editor.lnk" "$INSTDIR\Hammertime.Editor.exe" "" "$INSTDIR\Hammertime.Editor.exe" 0

    end:
SectionEnd

Section "Desktop Shortcut"
    IfSilent 0 +2
        Goto end ; Silent update: Don't redo shortcuts
    
    SetShellVarContext all
    CreateShortCut "$DESKTOP\Hammertime Editor.lnk" "$INSTDIR\Hammertime.Editor.exe" "" "$INSTDIR\Hammertime.Editor.exe" 0
    
    end:
SectionEnd

Section "Run Hammertime After Installation"
    SetAutoClose true
    Exec "$INSTDIR\Hammertime.Editor.exe"
SectionEnd

; Uninstall

Section "Uninstall"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HammertimeEditor"
  DeleteRegKey HKLM "Software\Hammertime\Editor"

  SetShellVarContext all
  Delete "$SMPROGRAMS\Hammertime Editor\*.*"
  Delete "$DESKTOP\Hammertime Editor.lnk"

  RMDir /r "$SMPROGRAMS\Hammertime Editor"
  RMDir /r "$INSTDIR"

SectionEnd