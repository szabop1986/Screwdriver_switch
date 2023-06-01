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
using System.Windows.Shapes;

namespace Screwdriver_switch
{
    /// <summary>
    /// Interaction logic for Change_screwing_jig.xaml
    /// </summary>
    public partial class Change_screwing_jig : Window
    {
        MainWindow m = Application.Current.MainWindow as MainWindow;
        string scanned_jig_id ="";

        public Change_screwing_jig()
        {
            InitializeComponent();
            JIG_ID_Textbox.Focus();
            JIG_ID_Textbox.SelectAll();
        }

        private void JIG_ID_Textbox_KeyDown(object sender, KeyEventArgs e)
        {
          
            if (e.Key == Key.Return)
            {
               scanned_jig_id = JIG_ID_Textbox.Password;

                bool found = false;

                         
                try
                {
                    foreach (Screwing_dictionary.One_Screwing_parameter s in m.Jig_list)
                    {                        
                        if ( Convert.ToString(s.Jig_ID)== scanned_jig_id)
                        {
                            m.scanned_jig = scanned_jig_id;
                            m.Currently_scanned_jig.Content = s.Product_name + "(" + s.Assembly + ")";
                            found = true;
                           break;
                        }                                                                          
                    }

                    if(found != true)
                    {
                        MessageBox.Show("Nem sikerült a scannelt azonosítóhoz jig -et társítani");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally {
                    m.Serial_textbox.Focus();
                    m.Serial_textbox.SelectAll();
                   this.Close();
                }
            }
        }
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            m.Change_screwing_jig.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            m.Change_screwing_jig.IsEnabled = true;
           
        }

        private void JIG_ID_Textbox_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
