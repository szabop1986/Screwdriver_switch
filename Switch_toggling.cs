using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
namespace Screwdriver_switch
{
    public static class Switch_toggling
    {
        static string file_location = System.Reflection.Assembly.GetEntryAssembly().Location;
        static string Get_Folder = System.IO.Path.GetDirectoryName(file_location);
        static string CMD_file_name = "USBswitchCmd.exe";

        public static void Toggle_On(int socket_number)
        {
            //MessageBox.Show(Get_Folder);
            System.Diagnostics.Process.Start(Path.Combine(Get_Folder, CMD_file_name),  "1" +" " +"-#" + " " + socket_number);
        }

        public static void Toggle_off(int socket_number)
        {
            System.Diagnostics.Process.Start(Path.Combine(Get_Folder, CMD_file_name), "0" + " " + "-#" + " " + socket_number);
        }
    }
}
