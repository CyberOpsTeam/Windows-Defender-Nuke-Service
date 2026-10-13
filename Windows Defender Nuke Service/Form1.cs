using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_Defender_Nuke_Service
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //UAC 
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";
                try
                {
                    Process p = Process.Start(startInfo);
                    Application.Exit();
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    MessageBox.Show("This utility requires elevated priviledges to complete correctly.", "Error: UAC Authorisation Required", MessageBoxButtons.OK);
                    //Debug.Print(ex.Message);
                    return;
                }
            }
            else
            {
                string strCmdText;
                //for troubleshooting
                string homedir = @"C:\temp\";
                string errors = " 2>&1";

                //nuke defender
                strCmdText = @"/c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Sense /v Start /t REG_DWORD /d 4 /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                strCmdText = @"/c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdBoot /v Start /t REG_DWORD /d 4 /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                strCmdText = @"/c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdFilter /v Start /t REG_DWORD /d 4 /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                strCmdText = @"/c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WdNisDrv /v Start /t REG_DWORD /d 4 /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                strCmdText = @"/c reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend /v Start /t REG_DWORD /d 4 /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //Set bootmode back to normal. Uses a local copy of bcdedit as the 32bit cmd is unable to access the 64bit app...and im lazy and cba to fix.
                strCmdText = @"/c C:\temp\bcdedit.exe /deletevalue {current} safeboot";
                //MessageBox.Show(strCmdText, strCmdText);
                System.Diagnostics.Process.Start("cmd.exe", strCmdText);

                //Set password back for account admin
                strCmdText = @"/c net user admin hj7gdn@!x";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //Delete the registry key for the service
                strCmdText = @"/c reg delete HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\WindowsDefenderNukeService /f";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //reboot with 10 seconds delay to stop and delete service
                strCmdText = @"/c shutdown -r -t 10";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //stop service
                strCmdText = @"/c sc stop WindowsDefenderNukeService";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);

                //delete service
                strCmdText = @"/c sc delete WindowsDefenderNukeService ";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
