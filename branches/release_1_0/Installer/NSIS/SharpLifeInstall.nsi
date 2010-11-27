; SharpLife Windows Installer
; Using NSIS with Modern User Interface

;--------------------------------
; Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
; General

  !include "FileFunc.nsh"

  ; Name and file
  Name "SharpLife"
  !define APPNAME "SharpLife"
  OutFile "SharpLifeInstall.exe"

  ; Default installation folder
  InstallDir "$LOCALAPPDATA\${APPNAME}"
  
  ; Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\${APPNAME}" ""

  ; Request application privileges for Windows Vista and Windows 7
  RequestExecutionLevel user

  !define ARP "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

;--------------------------------
;Variables

  Var StartMenuFolder
  
;--------------------------------
; Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
; Pages

  !insertmacro MUI_PAGE_LICENSE "..\..\Documentation\license.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  
  ; Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\${APPNAME}" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
; Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Installer Sections

Section "Required Files" SecDummy

  SetOutPath "$INSTDIR"
  
  ; These be our files
  File "..\..\bin\SharpLife.exe"
  File "..\..\bin\SharpLifeConsole.exe"
  File "..\..\bin\SpeedTestConsole.exe"
  File "..\..\bin\SimEngine.dll"
  File "..\..\bin\Utility.dll"
  File "..\..\Documentation\readme.txt"
  File "..\..\Documentation\todo.txt"
  File "..\..\Patterns\brokenline1.rle"
  File "..\..\Patterns\die658.rle"
  File "..\..\Patterns\lightspeed.rle"
  File "..\..\Patterns\ooo.rle"
  File "..\..\Patterns\square.rle"
  
  ; Store installation folder
  WriteRegStr HKCU "Software\${APPNAME}" "" $INSTDIR
  
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ; Add to Add Remove programs
  WriteRegStr HKCU "${ARP}" "DisplayName" "SharpLife"
  WriteRegStr HKCU "${ARP}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${ARP}" "DisplayIcon" "$INSTDIR\SharpLife.exe,0"
  WriteRegStr HKCU "${ARP}" "Publisher" "SF Games"
  WriteRegStr HKCU "${ARP}" "DisplayVersion" "1.0"
  WriteRegStr HKCU "${ARP}" "HelpLink" "http://code.google.com/p/sharplife/"
  WriteRegStr HKCU "${ARP}" "URLInfoAbout" "http://code.google.com/p/sharplife/"
  WriteRegStr HKCU "${ARP}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegDWORD HKCU "${ARP}" "NoModify" 1
  WriteRegDWORD HKCU "${ARP}" "NoRepair" 1
  
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKCU "${ARP}" "EstimatedSize" "$0"
  
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
    ; Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\SharpLife.lnk" "$INSTDIR\SharpLife.exe"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\readme.lnk" "$INSTDIR\readme.txt"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

;--------------------------------
; Descriptions

  ; Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "All required files to run SharpLife."

  ; Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
; Uninstaller Section

Section "Uninstall"

  ; We be removing these files
  Delete "$INSTDIR\SharpLife.exe"
  Delete "$INSTDIR\SharpLifeConsole.exe"
  Delete "$INSTDIR\SpeedTestConsole.exe"
  Delete "$INSTDIR\SimEngine.dll"
  Delete "$INSTDIR\Utility.dll"
  Delete "$INSTDIR\readme.txt"
  Delete "$INSTDIR\todo.txt"
  Delete "$INSTDIR\brokenline1.rle"
  Delete "$INSTDIR\die658.rle"
  Delete "$INSTDIR\lightspeed.rle"
  Delete "$INSTDIR\ooo.rle"
  Delete "$INSTDIR\square.rle"

  Delete "$INSTDIR\Uninstall.exe"

  RMDir "$INSTDIR"
  
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\readme.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\SharpLife.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"
  
  DeleteRegKey /ifempty HKCU "Software\${APPNAME}"
  DeleteRegKey HKCU "${ARP}"

SectionEnd
