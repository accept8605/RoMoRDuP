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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoMoRDuP
{
    /// <summary>
    /// Interaktionslogik für UCMain.xaml
    /// </summary>
    public partial class UCMain : UserControl
    {

        UCMirrorFolders ucMirrorFolders;
        UCRemoveDuplicates ucRemoveDuplicates;
        UCOptimize ucOptimize;
        MainWindow mainWindow { get; set; }
        public Tasks.Tasks tasks { get; set; }

        public UCMain(MainWindow mainWindow,Tasks.Tasks tasks, UCMirrorFolders ucMirrorFolders,UCRemoveDuplicates ucRemoveDuplicates,UCOptimize ucOptimize)
        {
            this.ucMirrorFolders = ucMirrorFolders;
            this.ucRemoveDuplicates = ucRemoveDuplicates;
            this.ucOptimize = ucOptimize;
            this.mainWindow = mainWindow;
            this.tasks = tasks;

            InitializeComponent();
        }


        private void MirrorFolders_Click(object sender, RoutedEventArgs e)
        {
            ucMirrorFolders.Reset();
            
            //wMirrorFolders.ShowDialog();

            mainWindow.ShowMirror();
        }


        private void Remove_Duplicates_Click(object sender, RoutedEventArgs e)
        {
            ucRemoveDuplicates.Reset();

            //wRemoveDuplicates.ShowDialog();

            mainWindow.ShowRemDup();
        }

        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not functional yet!");
            //wOptimize.ShowDialog();

            //mainWindow.ShowOptimize(); // TBD
        }

        private void RepairPlaylistPaths_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }



        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.unicef.org/");
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
