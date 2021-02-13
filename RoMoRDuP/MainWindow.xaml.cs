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


namespace RoMoRDuP
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        UCMirrorFolders ucMirrorFolders;
        UCRemoveDuplicates ucRemoveDuplicates;
        UCOptimize ucOptimize;

        public Tasks.Tasks tasks {get;set;}

        private UCMain ucMain { get; set; }


        private bool int_bLoadProfileEnabled;
        public bool bLoadProfileEnabled
        {
            get
            {
                return int_bLoadProfileEnabled;
            }
            set
            {
                int_bLoadProfileEnabled = value;
                OnPropertyChanged("bLoadProfileEnabled");
            }
        }


        public MainWindow()
        {
            WLicense wLicense = new WLicense();
            //wLicense.Owner = this;

            if (wLicense.ShowDialog() == true)
            {

                // Lizenz zuerst zeigen aus Sicherheitsgründen

                tasks = new Tasks.Tasks(this);

                InitializeComponent();

                bLoadProfileEnabled = true;

                //ToolTipService ShowDuration changing global
                ToolTipService.ShowDurationProperty.OverrideMetadata(
                    typeof(DependencyObject), new FrameworkPropertyMetadata(120000));

                ucMirrorFolders = new UCMirrorFolders(tasks);
                ucRemoveDuplicates = new UCRemoveDuplicates(tasks);
                ucOptimize = new UCOptimize(tasks);

                //String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(MainWindow)).CodeBase);
                //appdir = appdir.Replace("file:\\", "");

                //String path = appdir + "\\" + "default" + ".RMDPProf";

                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (System.IO.Directory.Exists(path + "\\RoMoRDuP\\") == false)
                    if (System.IO.Directory.CreateDirectory(path + "\\RoMoRDuP\\") == null)
                    {
                        MessageBox.Show("Cannot create Directory " + path + "\\RoMoRDuP\\");
                        return;
                    }

                path = path + "\\RoMoRDuP\\" + "default" + ".RMDPProf";

                tasks.userOptions.profiles.LoadProfile(path);




                ucMain = new UCMain(this, tasks, ucMirrorFolders, ucRemoveDuplicates, ucOptimize);

                ParentPanel.Children.Add(ucMain);
                ParentPanel.Children.Add(ucMirrorFolders);
                ParentPanel.Children.Add(ucRemoveDuplicates);
                ParentPanel.Children.Add(ucOptimize);

                ShowMain();




                Closing += this.OnWindowClosing;


                // This line should fix window closing Issues
                this.Closed += (sender, e) => this.Dispatcher.InvokeShutdown();


            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public void ShowMain()
        {
            bLoadProfileEnabled = true;

            CollapseAll();

            ucMain.Visibility = Visibility.Visible;
        }

        public void ShowMirror()
        {
            bLoadProfileEnabled = false;

            CollapseAll();

            ucMirrorFolders.Visibility = Visibility.Visible;
        }

        public void ShowRemDup()
        {
            bLoadProfileEnabled = false;

            CollapseAll();

            ucRemoveDuplicates.Visibility = Visibility.Visible;
        }

        public void ShowOptimize()
        {
            bLoadProfileEnabled = false;

            CollapseAll();

            ucOptimize.Visibility = Visibility.Visible;
        }

        private void CollapseAll()
        {
            ucMirrorFolders.Visibility = Visibility.Collapsed;
            ucRemoveDuplicates.Visibility = Visibility.Collapsed;
            ucOptimize.Visibility = Visibility.Collapsed;
            ucMain.Visibility = Visibility.Collapsed;
        }


        private void OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //TBD
            e.CanExecute = true;
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.Filter = "RMDPPRof files (*.RMDPProf)|*.RMDPProf|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            //String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(MainWindow)).CodeBase);
            //appdir = appdir.Replace("file:\\", "");

            String appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (System.IO.Directory.Exists(appdir + "\\RoMoRDuP\\") == false)
                if (System.IO.Directory.CreateDirectory(appdir + "\\RoMoRDuP\\") == null)
                {
                    MessageBox.Show("Cannot create Directory " + appdir + "\\RoMoRDuP\\");
                    return;
                }

            openFileDialog.InitialDirectory = appdir + "\\RoMoRDuP\\";

            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {

                string path = openFileDialog.FileName;

                tasks.userOptions.profiles.LoadProfile(path);
            }
        }



        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            saveFileDialog.Filter = "RMDPProf files (*.RMDPProf)|*.RMDPProf|All files (*.*)| *.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.FileName = "cur.RMDPProf";

            //String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(MainWindow)).CodeBase);
            //appdir = appdir.Replace("file:\\", "");

            String appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if(System.IO.Directory.Exists(appdir + "\\RoMoRDuP\\")==false)
                if (System.IO.Directory.CreateDirectory(appdir + "\\RoMoRDuP\\") == null)
                {
                    MessageBox.Show("Cannot create Directory " + appdir + "\\RoMoRDuP\\");
                    return;
                }

            saveFileDialog.InitialDirectory = appdir + "\\RoMoRDuP\\";

            System.Windows.Forms.DialogResult result = saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String path = saveFileDialog.FileName;

                tasks.userOptions.profiles.SaveProfile(path);
            }
        }



        private void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application curApp = Application.Current;
            curApp.Shutdown();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Owner = this;

            aboutBox.ShowDialog();
        }

        


        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Handle closing logic, set e.Cancel as needed
            /*
            wMirrorFolders.bCancelWindowClosing = false;
            wMirrorFolders.Close();
            wRemoveDuplicates.bCancelWindowClosing = false;
            wRemoveDuplicates.Close();
            wOptimize.bCancelWindowClosing = false;
            wOptimize.Close();  
             */
        }


        private void Click_Menu_Twitter(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://twitter.com/accept_86/with_replies");
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open browser");
            }
        }

        private void Click_Menu_Blog(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.blogger.com/profile/06939414270210295989");
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open browser");
            }
        }

        private void Click_Menu_UserManual(object sender, RoutedEventArgs e)
        {
            try
            {
                String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(MainWindow)).CodeBase);
                appdir = appdir.Replace("file:\\", "");


                string[] results = System.IO.Directory.GetFiles(appdir, "RoMoRDuP - User manual*.pdf");

                if (results.Count() > 0)
                    System.Diagnostics.Process.Start(results[0]);
                else
                    MessageBox.Show("Could not open user manual!");
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open pdf");
            }
        }

        /*
        private void Click_Menu_RequirementSpec(object sender, RoutedEventArgs e)
        {
            
        }
         */

        private void Click_Menu_RequestFeature(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }




        // ______________Property Updates
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }
}
