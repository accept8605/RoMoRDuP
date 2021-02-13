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

using System.ComponentModel;

namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCSelectSource.xaml
    /// </summary>
    public partial class UCSelectSource : UserControl
    {

        Tasks.Tasks tasks;

        System.Windows.Forms.FolderBrowserDialog objDialog;

        public UserInterface.UserInterfaceBase userOptionsBase { get; set; }

        public UCSelectSource(Tasks.Tasks tasks, UserInterface.UserInterfaceBase userOptionsBase)
        {
            objDialog = new System.Windows.Forms.FolderBrowserDialog();

            this.tasks = tasks;

            this.userOptionsBase = userOptionsBase;
        
            //Playlist_Hide_all();

            // Probleme mit aktualisierung, daher window immer neu
            //window_FileFilter = new WFileFilter(userOptionsBase.fileFilterOptions);
            //window_PlaylistFileFilter = new WFileFilter(userOptionsBase.PlaylistFileFilterOptions);


            InitializeComponent();
        }



        /*
        private void Checked_Playlist_UseSameFolderPaths(object sender, RoutedEventArgs e)
        {
            Playlist_Hide_all();
        }

        private void UnChecked_Playlist_UseSameFolderPaths(object sender, RoutedEventArgs e)
        {
            userOptionsBase.Button_AddPlaylistPath_Visibility = true;
        }
        */



        private void Click_Playlist_Button_AddPlaylistPath(object sender, RoutedEventArgs e)
        {
            Playlist_Add_Show();
        }

        private void Click_Button_RemovePlaylistPath1(object sender, RoutedEventArgs e)
        {
            userOptionsBase.bPlaylistPath1_Visible = false;
        }

        private void Click_Button_RemovePlaylistPath2(object sender, RoutedEventArgs e)
        {
            userOptionsBase.bPlaylistPath2_Visible = false;
        }

        private void Click_Button_RemovePlaylistPath3(object sender, RoutedEventArgs e)
        {
            userOptionsBase.bPlaylistPath3_Visible = false;
        }

        private void Click_Button_RemovePlaylistPath4(object sender, RoutedEventArgs e)
        {
            userOptionsBase.bPlaylistPath4_Visible = false;
        }

        private void Click_Button_RemovePlaylistPath5(object sender, RoutedEventArgs e)
        {
            userOptionsBase.bPlaylistPath5_Visible = false;
        }

        /*
        private void Playlist_Hide_all()
        {
            userOptionsBase.bPlaylistPath1_Visible = false;
            userOptionsBase.bPlaylistPath2_Visible = false;
            userOptionsBase.bPlaylistPath3_Visible = false;
            userOptionsBase.bPlaylistPath4_Visible = false;
            userOptionsBase.bPlaylistPath5_Visible = false;
            userOptionsBase.Button_AddPlaylistPath_Visibility = false;
        }
         */

        private void Playlist_Add_Show()
        {
            if (!userOptionsBase.bPlaylistPath1_Visible)
            {
                userOptionsBase.bPlaylistPath1_Visible = true;
                return;
            }

            if (!userOptionsBase.bPlaylistPath2_Visible)
            {
                userOptionsBase.bPlaylistPath2_Visible = true;
                return;
            }

            if (!userOptionsBase.bPlaylistPath3_Visible)
            {
                userOptionsBase.bPlaylistPath3_Visible = true;
                return;
            }

            if (!userOptionsBase.bPlaylistPath4_Visible)
            {
                userOptionsBase.bPlaylistPath4_Visible = true;
                return;
            }

            if (!userOptionsBase.bPlaylistPath5_Visible)
            {
                userOptionsBase.bPlaylistPath5_Visible = true;
                return;
            }
        }





        private void Click_Button_SelectSource(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;       // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strSelectSourcePath = objDialog.SelectedPath; //MessageBox.Show("Neuer Pfad : " + objDialog.SelectedPath);
            /*
            else
                MessageBox.Show("Abbruch gewählt!");
            */
        }

        private void Click_Button_SelectTarget(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;         // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strSelectTargetPath = objDialog.SelectedPath; //MessageBox.Show("Neuer Pfad : " + objDialog.SelectedPath);
            /*
            else
                MessageBox.Show("Abbruch gewählt!");
            */
        }



        private void Click_Button_AddPlaylistPath1(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;        // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strPlaylistPath1 = objDialog.SelectedPath; 
        }

        private void Click_Button_AddPlaylistPath2(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;        // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strPlaylistPath2 = objDialog.SelectedPath; 
        }

        private void Click_Button_AddPlaylistPath3(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;        // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strPlaylistPath3 = objDialog.SelectedPath; 
        }

        private void Click_Button_AddPlaylistPath4(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;        // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strPlaylistPath4 = objDialog.SelectedPath; 
        }

        private void Click_Button_AddPlaylistPath5(object sender, RoutedEventArgs e)
        {
            objDialog.Description = "Beschreibung";
            objDialog.RootFolder = Environment.SpecialFolder.MyComputer;        // Vorgabe Pfad (und danach der gewählte Pfad)
            System.Windows.Forms.DialogResult objResult = objDialog.ShowDialog();
            if (objResult == System.Windows.Forms.DialogResult.OK)
                userOptionsBase.strPlaylistPath5 = objDialog.SelectedPath; 
        }



        private void Click_Button_FileFilters(object sender, RoutedEventArgs e)
        {
            WFileFilter window_FileFilter = new WFileFilter(userOptionsBase.fileFilterOptions);
            window_FileFilter.Owner = userOptionsBase.mainWindow;
            window_FileFilter.ShowDialog();
        }

        private void Click_Button_PlaylistFileFilters(object sender, RoutedEventArgs e)
        {
            WFileFilter window_PlaylistFileFilter = new WFileFilter(userOptionsBase.PlaylistFileFilterOptions);
            window_PlaylistFileFilter.Owner = userOptionsBase.mainWindow;
            window_PlaylistFileFilter.ShowDialog();
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
