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

    public partial class WFileFilter : Window
    {
        public UserInterface.FileFilterOptions fileFilterOptions { get; set; }    // does not need OnPropertyChanged because always before InitializeComponent



        public WFileFilter(UserInterface.FileFilterOptions fileFilterOptions)
        {
            this.fileFilterOptions = fileFilterOptions;

            int_fileFilterOptionsBackup = fileFilterOptions.Clone();

            InitializeComponent();

            ParentPanel.DataContext = this; // sets elementname for databinding

            Closing += this.OnWindowClosing;
        }



        private UserInterface.FileFilterOptions int_fileFilterOptionsBackup;

        private void SaveFileFilterOptions()
        {
            int_fileFilterOptionsBackup = fileFilterOptions.Clone();
        }

        private void RestoreFileFilterOptions()
        {
            fileFilterOptions.Update(int_fileFilterOptionsBackup);
        }



        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Handle closing logic, set e.Cancel as needed
            CloseWindow();

            // Probleme mit aktualisierung des Textes
            //e.Cancel = true;
        }

        public void CloseWindow()
        {
            //Reset();
            this.Hide();
        }

        private void Click_Abort(object sender, RoutedEventArgs e)
        {
            RestoreFileFilterOptions();

            CloseWindow();
        }

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            CloseWindow();
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
