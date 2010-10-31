; SharpLife Windows Installer
; Using NSIS with Modern User Interface

;--------------------------------
; Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
; General

  ; Name and file
  Name "SharpLife"
  OutFile "SharpLifeInstall.exe"

  ; Default installation folder
  InstallDir "$LOCALAPPDATA\SharpLife"
  
  ; Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\SharpLife" ""

  ; Request application privileges for Windows Vista and Windows 7
  RequestExecutionLevel user

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
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\SharpLife" 
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
  WriteRegStr HKCU "Software\SharpLife" "" $INSTDIR
  
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
    ; Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
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
  Delete "$INSTDIR\example2.nsi"
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
  RMDir "$SMPROGRAMS\$StartMenuFolder"
  
  DeleteRegKey /ifempty HKCU "Software\SharpLife"

SectionEnd
