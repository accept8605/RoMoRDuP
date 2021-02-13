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

    /// <summary>
    /// Interaktionslogik für WMBExecuteTasks.xaml
    /// </summary>
    public partial class WMBExecuteTasks : Window
    {
        public UserInterface.MBUI mbUI { get; set; }

        public WMBExecuteTasks(UserInterface.MBUI mbUI)
        {
            this.mbUI = mbUI;

            InitializeComponent();
        }

        private void Click_Button_Yes(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Click_Button_No(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = true;
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            ((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = false;
        }
    }

}
