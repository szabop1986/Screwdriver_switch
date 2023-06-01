using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Screwdriver_switch
{
    public static class Create_MES_Log
    {
        public static void Create_MES_LOG(string serial)
        {
            String current_time = DateTime.Now.ToString("M\\/dd\\/yyyy HH:mm:ss");
            String current_time_1 = DateTime.Now.ToString("yyyyMdd_HHmmss");
            string filename = "P_S" + serial + "_" + current_time_1;

            string Dummy_directory = @"C:\Users\szabop1\Desktop\Dummy logs\" + filename + ".TAR";
            string MES_Dir = @"\\HUTISM0PARSERV1\ERICSSON_PREVAS_TESTERS\" + filename + ".TAR";

            StreamWriter create_txt = new StreamWriter(MES_Dir);
            create_txt.WriteLine("S" + serial);
            create_txt.WriteLine("CERICSSON_QM");
            create_txt.WriteLine("P" + "TNBSC_QC");  // Tester name
            create_txt.WriteLine("N" + "ERICSSON_TNBSC_QC");  // Test process
            create_txt.WriteLine("TP");
            create_txt.WriteLine("0");
            create_txt.WriteLine("[" + current_time);
            create_txt.WriteLine("]" + current_time);
            create_txt.WriteLine("O" + Environment.UserName);
            create_txt.Close();
        }
    }
    }
