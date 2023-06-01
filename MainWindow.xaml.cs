using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Configuration;
using System.IO;


namespace Screwdriver_switch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   

    public partial class MainWindow : Window
    {
       
        public const string enable_switch = "";
        public const string disable_switch = "";
        public List<Screwing_dictionary.One_Screwing_parameter> Jig_list;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        static int remaining_time;
        public string scanned_jig;
        public static int socket_to_toggle;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {          
            remaining_time = remaining_time - 1;
            Remaining_time_label.Content = remaining_time +" [sec]";

            if(remaining_time <= 10)
            {
                Remaining_time_label.Foreground = Brushes.Red;
            }
            else
            {
                Remaining_time_label.Foreground = Brushes.Green;
            }

            if(remaining_time == 0)
            {
                dispatcherTimer.Stop();             
                Switch_toggling.Toggle_off(0);
                Already_running = false;
                dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);

                Output_of_check_label.Content = "---";
                Alert_rectangle.Fill = new SolidColorBrush(Color.FromRgb(244, 244, 245));

                Serial_textbox.Focus();
                Serial_textbox.SelectAll();

            }
        }

        public static List<Screwing_dictionary.One_Screwing_parameter> Load_List()
        {
            string location = ConfigurationManager.AppSettings.Get("Jig_list_location");
            
            XmlReader xmlread = XmlReader.Create(location);
            List<Screwing_dictionary.One_Screwing_parameter> List_of_jigs = new List<Screwing_dictionary.One_Screwing_parameter>();

            while (xmlread.Read())
            {
                if((xmlread.NodeType == XmlNodeType.Element) && (xmlread.Name == "jig")){
                    if (xmlread.HasAttributes)
                    {                      
                        Screwing_dictionary.One_Screwing_parameter param1 = new Screwing_dictionary.One_Screwing_parameter(xmlread.GetAttribute("Name"), xmlread.GetAttribute("Assembly"), xmlread.GetAttribute("ID"),int.Parse(xmlread.GetAttribute("Screwing_time")));
                        List_of_jigs.Add(param1);
                    }
                }

                if ((xmlread.NodeType == XmlNodeType.Element) && (xmlread.Name == "Socket_to_toggle"))
                {
                    if (xmlread.HasAttributes)
                    {
                        socket_to_toggle =Convert.ToInt16(xmlread.GetAttribute("Number"));                        
                    }
                }
            }
            return List_of_jigs;            
        }


        //public string Get_type(string serial)
        //{
        //    string type = "Nincs adat";
        //    try
        //    {
        //        List<MyMESServices.OneMESHistoryRow> bh = MyMESServices.LongBoardHistory.Get(serial);
          
        //        for (int i = 0; i <= bh.Count - 1; i++)
        //        {
        //            if (bh[i].Test_Process == "BIRTH / BIRTH")
        //            {
        //                type = bh[i].Assembly;
        //                break;
        //            }
        //        }
        //        return type;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return type;
        //    }

        //}


        public static void Get_type(string serial, out string type, out string Error_message, out string PV_correct_location_result)
        {          
            string Number = "No number";
            string revision = "No revision";
            string Step_instance = "TNBSC_QC";
            string Equipment_name = "ERICSSON_TNBSC_QC";
            string Routestep_ID_Xml = "No data";
            string Routestep_ID = "No returned data";
            string get_PV_correct_location_XML = "No data";

            PV_correct_location_result = "No data";
            type = "Nincs adat";
            Error_message = "No error";


            try
            {
                List<MyMESServices.OneMESHistoryRow> bh = MyMESServices.LongBoardHistory.Get(serial);

                for (int i = 0; i <= bh.Count - 1; i++)
                {
                    if (bh[i].Test_Process == "BIRTH / BIRTH")
                    {
                        type = bh[i].Assembly;                      
                        Number = bh[i].Number;
                        revision = bh[i].Revision;
                        break;
                    }
                }

                MyMESServices.ServiceReference1.TestServicesSoapClient get_routestep = new MyMESServices.ServiceReference1.TestServicesSoapClient();
                Routestep_ID_Xml = get_routestep.getRouteStep_ID("TISSQLV10A", "JEMS", Step_instance, Equipment_name, Number, revision);

                XmlReader xmlread = XmlReader.Create(new StringReader(Routestep_ID_Xml));

                while (xmlread.Read())
                {
                    if ((xmlread.NodeType == XmlNodeType.Element) && (xmlread.Name == "RouteStep_ID"))
                    {
                        Routestep_ID = xmlread.ReadElementContentAsString();
                        break;
                    }
                }

                MyMESServices.ServiceReference1.TestServicesSoapClient get_PV_correct_location = new MyMESServices.ServiceReference1.TestServicesSoapClient();
                get_PV_correct_location_XML = get_PV_correct_location.PVCorrectLocation("TISSQLV10A", "JEMS", "ERICSSON_QM", serial, Routestep_ID, "1");

                XmlReader xmlread_1 = XmlReader.Create(new StringReader(get_PV_correct_location_XML));

                while (xmlread_1.Read())
                {
                    if ((xmlread_1.NodeType == XmlNodeType.Element) && (xmlread_1.Name == "Result"))
                    {
                        PV_correct_location_result = xmlread_1.ReadElementContentAsString();                                               
                    }

                    if ((xmlread_1.NodeType == XmlNodeType.Element) && (xmlread_1.Name == "Error"))
                    {
                        Error_message = xmlread_1.ReadElementContentAsString();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public MainWindow()
        {
            try
            { 
            Jig_list = Load_List();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült betölteni a jig -ek könyvtárát!" + ex.Message);
                Environment.Exit(1);
            }
      
            InitializeComponent();
            Serial_textbox.Focus();
            Serial_textbox.SelectAll();          
        }

       
        private void Change_screwing_jig_Click(object sender, RoutedEventArgs e)
        {
            Change_screwing_jig jig1 = new Screwdriver_switch.Change_screwing_jig();
            jig1.Show();
        }


        bool Already_running=false;

        private void Serial_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return){
                try
                {
                    string Type_of_scanned_product;
                    string Error_message= "No_error";
                    string PV_check_result;

                    Get_type(Convert.ToString(Serial_textbox.Text), out Type_of_scanned_product, out Error_message, out PV_check_result);

                    Scanned_product_type_label.Content = Type_of_scanned_product;

                    string Assembly;
                    string JIG_ID;
                    bool parameters_found = false;
                    bool jig_is_matching = false;
                    bool no_jig_exists = false;

                    foreach(Screwing_dictionary.One_Screwing_parameter p in Jig_list)
                    {
                        if (p.Product_name == Type_of_scanned_product)
                        {
                            remaining_time = p.Screwing_time;
                            Assembly = p.Assembly;
                            JIG_ID = p.Jig_ID;

                            if(JIG_ID == scanned_jig)
                            {
                                jig_is_matching = true;
                            }

                            if(JIG_ID == "_X_")
                            {
                                no_jig_exists = true;
                            }

                            parameters_found = true;
                            break;
                        }

                       
                    }
                    
                    if ((parameters_found == true && jig_is_matching == true) || (no_jig_exists == true && parameters_found == true)) {
                  
                    Output_of_check_label.Content = "OK";

                        if(no_jig_exists == true)
                        {
                            Currently_scanned_jig.Content = "Ehhez a termékhez nem tartozik jig.";
                        }
                        
                    Alert_rectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));                    
                    Switch_toggling.Toggle_On(socket_to_toggle);

                    if(MES_log_checkbox.IsChecked == true && PV_check_result =="PASS"){
                   Create_MES_Log.Create_MES_LOG(Convert.ToString(Serial_textbox.Text));
                        }

                    if(PV_check_result != "PASS" && MES_log_checkbox.IsChecked == true)
                        {
                            Alert_rectangle.Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                            MessageBox.Show(Error_message);
                        }

                      
                    if (Already_running == false)
                    {
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0,0, 1);
                    dispatcherTimer.Start();
                    }
                     Already_running = true;
                    }


                    // NOK case
                    if (jig_is_matching != true && (no_jig_exists == false))
                    {
                        Output_of_check_label.Content = "NOK";
                        Alert_rectangle.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        remaining_time = 0;
                        Remaining_time_label.Content = remaining_time;
                       
                        if (Already_running == true)
                        {
                            dispatcherTimer.Stop();
                            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
                        }

                        Already_running = false;
                        Switch_toggling.Toggle_off(socket_to_toggle);

                        MessageBox.Show("A jig és termék pár nem megfelelő!\n" + "Пара джиг і продукт неправильна!");

                       
                    }

                    Serial_textbox.Focus();
                    Serial_textbox.SelectAll();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nem sikerült a termék típusának meghatározása!" + "\n" +  ex.Message);
                }                                             
            }        
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Switch_toggling.Toggle_off(socket_to_toggle);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MES_log_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ha ezt kikapcsolod, akkor a MES -ben nem lesz BSC QC log!");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MES_history_window.MESHistory hw = new MES_history_window.MESHistory();
            hw.Show();
        }
    }
}
