using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.ComponentModel;


namespace RoMoRDuP
{
 
    public partial class WLicense : Window
    {

        public WLicense()
        {
            InitializeComponent();

            Closing += this.OnWindowClosing;
        }

        private void Click_Button_Exit(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            this.Close();
        }

        private void Click_Button_Accept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            this.Close();
        }


        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Handle closing logic, set e.Cancel as needed


        }
    }
}
