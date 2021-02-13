using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Text;
using System.Windows;

using System.ComponentModel;
using System.Collections.Generic;


namespace RoMoRDuP.UserInterface
{

    public class MBUI : INotifyPropertyChanged  // MessageBoxUI
    {
        private bool int_GetAdministratorRights;
        public bool GetAdministratorRights
        {
            get
            {
                return int_GetAdministratorRights;
            }

            set
            {
                int_GetAdministratorRights = value;
                OnPropertyChanged("GetAdministratorRights");
            }
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



    public class UserInterface
    {
        public UserOptionsMirror userOptionsMirror { get; set; }
        public UserOptionsRemoveDuplicates userOptionsRemoveDuplicates { get; set; }
        public Optimize.UserOptionsOptimize userOptionsOptimize { get; set; }

        public Profiles.Profiles profiles { get; set; }

        public UserInterface(Tasks.FileLists fileLists, RoMoRDuP.MainWindow mainWindow)
        {
            userOptionsMirror = new UserOptionsMirror(fileLists, UserOptionsType.Mirror, mainWindow);
            userOptionsRemoveDuplicates = new UserOptionsRemoveDuplicates(fileLists, UserOptionsType.RemDup, mainWindow);
            userOptionsOptimize = new Optimize.UserOptionsOptimize(fileLists, UserOptionsType.Optimize, mainWindow);

            profiles = new Profiles.Profiles(this);
        }

    }


    public enum enOptionsMirrorEasy { SyncOneWayLeave, SyncBothWays, SyncOneWayRemove }

    public enum enOptionsMirrorDuplicates { Skip, RenameSyncDir, CopyAnyway }
    public enum enOptionsMirrorUpdated { Skip=0, MostRecentDate=1, SyncDir=2 /*, Target=3, RenameAddNumber=4, RenameAddDate=5*/ }
    public enum enOptionsMirrorMoveTo { SyncDir, NoMoving }
    public enum enOptionsMirrorMovedRenaming { SyncDir, NoRenaming }

    public class UserOptionsMirror : UserInterfaceBase
    {
        public UserOptionsMirror(Tasks.FileLists fileLists, UserOptionsType userOptionsType, RoMoRDuP.MainWindow mainWindow)
            : base(fileLists, userOptionsType, mainWindow)
        {
            bSelectTarget_Visible = true;

            bProcessHashCodes_Visible = true;

            OptionsMirrorEasy = enOptionsMirrorEasy.SyncOneWayLeave;
            
            fileFilterOptions.IncludeOnly = "";
            fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";
            fileFilterOptions.bIncludeOnly = false; // should be set last


            PlaylistFileFilterOptions.bIncludeOnly = true; // should be set last only when IncludeOnly should be overwritten
            PlaylistFileFilterOptions.IncludeOnly = "*.m3u, *.wpl, *.kpl";
            PlaylistFileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";
        }

        // Selected Modes

        public UserControls.enSelectedMode ModeSelectSource { get; set; }

        public UserControls.enSelectedMode ModeUserOptions { get; set; }

        // UserOptions easy

        private enOptionsMirrorEasy int_OptionsMirrorEasy;
        public enOptionsMirrorEasy OptionsMirrorEasy
        {
            get
            {
                return int_OptionsMirrorEasy;
            }

            set
            {
                int_OptionsMirrorEasy = value;
                OnPropertyChanged("OptionsMirrorEasy");
                OnPropertyChanged("OptionsMirrorEasySyncOneWayKeep");
                OnPropertyChanged("OptionsMirrorEasySyncOneWayRemove");
                OnPropertyChanged("OptionsMirrorEasySyncBothWays");
            }

        }



        public bool OptionsMirrorEasySyncOneWayKeep
        {
            get
            {
                if(OptionsMirrorEasy == enOptionsMirrorEasy.SyncOneWayLeave)
                    return true;

                return false;
            }
        }

        public bool OptionsMirrorEasySyncOneWayRemove
        {
            get
            {
                if (OptionsMirrorEasy == enOptionsMirrorEasy.SyncOneWayRemove)
                    return true;

                return false;
            }
        }

        public bool OptionsMirrorEasySyncBothWays
        {
            get
            {
                if (OptionsMirrorEasy == enOptionsMirrorEasy.SyncBothWays)
                    return true;

                return false;
            }
        }



        public void ApplyMirrorUserOptionsEasy()
        {
            switch (OptionsMirrorEasy)
            {
                case enOptionsMirrorEasy.SyncOneWayLeave:
                    OptionsMirrorDuplicates = enOptionsMirrorDuplicates.RenameSyncDir;
                    OptionsMirrorUpdated = enOptionsMirrorUpdated.MostRecentDate;
                    OptionsMirrorMoveTo = enOptionsMirrorMoveTo.SyncDir;
                    OptionsMirrorMovedRenaming = enOptionsMirrorMovedRenaming.SyncDir;
                    OptionsMirrorCopySourceTarget = true;
                    OptionsMirrorCopyTargetSource = false;
                    OptionsMirrorDeleteSourceTarget = false;
                    OptionsMirrorDeleteTargetSource = false;
                    OptionsMirrorSyncAttributes = true;
                    break;

                case enOptionsMirrorEasy.SyncBothWays:
                    OptionsMirrorDuplicates = enOptionsMirrorDuplicates.RenameSyncDir;
                    OptionsMirrorUpdated = enOptionsMirrorUpdated.MostRecentDate;
                    OptionsMirrorMoveTo = enOptionsMirrorMoveTo.SyncDir;
                    OptionsMirrorMovedRenaming = enOptionsMirrorMovedRenaming.SyncDir;
                    OptionsMirrorCopySourceTarget = true;
                    OptionsMirrorCopyTargetSource = true;
                    OptionsMirrorDeleteSourceTarget = false;
                    OptionsMirrorDeleteTargetSource = false;
                    OptionsMirrorSyncAttributes = true;
                    break;

                case enOptionsMirrorEasy.SyncOneWayRemove:
                    OptionsMirrorDuplicates = enOptionsMirrorDuplicates.RenameSyncDir;
                    OptionsMirrorUpdated = enOptionsMirrorUpdated.MostRecentDate;
                    OptionsMirrorMoveTo = enOptionsMirrorMoveTo.SyncDir;
                    OptionsMirrorMovedRenaming = enOptionsMirrorMovedRenaming.SyncDir;
                    OptionsMirrorCopySourceTarget = true;
                    OptionsMirrorCopyTargetSource = false;
                    OptionsMirrorDeleteSourceTarget = true;
                    OptionsMirrorDeleteTargetSource = false;
                    OptionsMirrorSyncAttributes = true;
                    break;
            }
        }


        public void ApplyMirrorSelectSourceEasy()
        {
            fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";

            ApplySelectSourceEasyBase();
        }


        // User Options Expert



        private enOptionsMirrorDuplicates int_OptionsMirrorDuplicates;
        public enOptionsMirrorDuplicates OptionsMirrorDuplicates
        {
            get
            {
                return int_OptionsMirrorDuplicates;
            }

            set
            {
                int_OptionsMirrorDuplicates = value;
                OnPropertyChanged("OptionsMirrorDuplicates");
            }

        }

        private enOptionsMirrorUpdated int_OptionsMirrorUpdated;
        public enOptionsMirrorUpdated OptionsMirrorUpdated
        {
            get
            {
                return int_OptionsMirrorUpdated;
            }

            set
            {
                int_OptionsMirrorUpdated = value;
                OnPropertyChanged("OptionsMirrorUpdated");
            }

        }

        private enOptionsMirrorMoveTo int_OptionsMirrorMoveTo;
        public enOptionsMirrorMoveTo OptionsMirrorMoveTo
        {
            get
            {
                return int_OptionsMirrorMoveTo;
            }

            set
            {
                int_OptionsMirrorMoveTo = value;
                OnPropertyChanged("OptionsMirrorMoveTo");
            }

        }

        private enOptionsMirrorMovedRenaming int_OptionsMirrorMovedRenaming;
        public enOptionsMirrorMovedRenaming OptionsMirrorMovedRenaming
        {
            get
            {
                return int_OptionsMirrorMovedRenaming;
            }

            set
            {
                int_OptionsMirrorMovedRenaming = value;
                OnPropertyChanged("OptionsMirrorMovedRenaming");
            }

        }


        private bool int_OptionsMirrorCopySourceTarget;
        public bool OptionsMirrorCopySourceTarget
        {
            get
            {
                return int_OptionsMirrorCopySourceTarget;
            }

            set
            {
                int_OptionsMirrorCopySourceTarget = value;
                OnPropertyChanged("OptionsMirrorCopySourceTarget");
                OnPropertyChanged("OptionsMirrorDeleteTargetSourceIsEnabled");
            }

        }

        public bool OptionsMirrorCopySourceTargetIsEnabled
        {
            get
            {
                return !OptionsMirrorDeleteTargetSource;
            }

        }

        private bool int_OptionsMirrorCopyTargetSource;
        public bool OptionsMirrorCopyTargetSource
        {
            get
            {
                return int_OptionsMirrorCopyTargetSource;
            }

            set
            {
                int_OptionsMirrorCopyTargetSource = value;
                OnPropertyChanged("OptionsMirrorCopyTargetSource");
                OnPropertyChanged("OptionsMirrorDeleteSourceTargetIsEnabled");
            }

        }




        private bool int_OptionsMirrorSyncAttributes;
        public bool OptionsMirrorSyncAttributes
        {
            get
            {
                return int_OptionsMirrorSyncAttributes;
            }

            set
            {
                int_OptionsMirrorSyncAttributes = value;
                OnPropertyChanged("OptionsMirrorSyncAttributes");
            }

        }


        public bool OptionsMirrorCopyTargetSourceIsEnabled
        {
            get
            {
                return !OptionsMirrorDeleteSourceTarget;
            }

        }

        private bool int_OptionsMirrorDeleteSourceTarget;
        public bool OptionsMirrorDeleteSourceTarget
        {
            get
            {
                return int_OptionsMirrorDeleteSourceTarget;
            }

            set
            {
                int_OptionsMirrorDeleteSourceTarget = value;
                OnPropertyChanged("OptionsMirrorDeleteSourceTarget");
                OnPropertyChanged("OptionsMirrorCopyTargetSourceIsEnabled");
            }

        }

        public bool OptionsMirrorDeleteSourceTargetIsEnabled
        {
            get
            {
                return !OptionsMirrorCopyTargetSource;
            }

        }


        private bool int_OptionsMirrorDeleteTargetSource;
        public bool OptionsMirrorDeleteTargetSource
        {
            get
            {
                return int_OptionsMirrorDeleteTargetSource;
            }

            set
            {
                int_OptionsMirrorDeleteTargetSource = value;
                OnPropertyChanged("OptionsMirrorDeleteTargetSource");
                OnPropertyChanged("OptionsMirrorCopySourceTargetIsEnabled");
            }

        }


        public bool OptionsMirrorDeleteTargetSourceIsEnabled
        {
            get
            {
                return !OptionsMirrorCopySourceTarget;
            }

        }

    }




    public class UserOptionsRemoveDuplicates : UserInterfaceBase
    {
        public UserOptionsRemoveDuplicates(Tasks.FileLists fileLists, UserOptionsType userOptionsType, RoMoRDuP.MainWindow mainWindow)
            : base(fileLists, userOptionsType, mainWindow)
        {
            bSelectTarget_Visible = false;

            bProcessHashCodes_Visible = true;


            
            fileFilterOptions.IncludeOnly = "";
            fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*, *.lnk, *.jpg, *.jpeg, *.png, *.bmp, *.gif, *.ico";
            fileFilterOptions.bIncludeOnly = false; // should be set last


            PlaylistFileFilterOptions.bIncludeOnly = true; // should be set last only when overwritten
            PlaylistFileFilterOptions.IncludeOnly = "*.m3u, *.wpl, *.kpl";
            PlaylistFileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";
            
        }


        // Selected Modes

        public UserControls.enSelectedMode ModeSelectSource { get; set; }


        public void ApplyRDSelectSourceEasy()
        {
            fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*, *.lnk, *.jpg, *.jpeg, *.png, *.bmp, *.gif, *.ico";

            ApplySelectSourceEasyBase();
        }


    }







    public delegate void degScrollToEndTaskExecute();

    public enum DataSizeUnit { kByte, mByte, gByte }
    public enum UserOptionsType { Mirror, RemDup, Optimize }

    public class UserInterfaceBase : INotifyPropertyChanged
    {
        public RoMoRDuP.MainWindow mainWindow { get; set; }

        public UserInterfaceBase(Tasks.FileLists fileLists, UserOptionsType userOptionsType, RoMoRDuP.MainWindow mainWindow)
        {
            strSelectSourcePath = "SourcePath";
            strSelectTargetPath = "TargetPath";

            this.fileLists = fileLists;

            this.userOptionsType = userOptionsType;

            bTaskExecuteAutoscroll = true;

            this.mainWindow = mainWindow;

            ProcessHash = "Data Amount";

            int_fileFilterOptions = new FileFilterOptions();
            int_PlaylistFileFilterOptions = new FileFilterOptions();

            SelectedSizeProcessHashItems = new List<System.Windows.Controls.ComboBoxItem>();
            System.Windows.Controls.ComboBoxItem item = new System.Windows.Controls.ComboBoxItem();
            item.Content = "kByte";
            SelectedSizeProcessHashItems.Add(item);

            System.Windows.Controls.ComboBoxItem item2 = new System.Windows.Controls.ComboBoxItem();
            item2.Content = "mByte";
            SelectedSizeProcessHashItems.Add(item2);

            /*
            System.Windows.Controls.ComboBoxItem item3 = new System.Windows.Controls.ComboBoxItem();
            item3.Content = "gByte";
            SelectedSizeProcessHashItems.Add(item3);
             */

            messageBoxUI = new MBUI();

            bUseSameFolderPaths = true;
            bUpdatePlaylists = true;

            visTarget = Visibility.Collapsed;
            visSource = Visibility.Visible;
        }

        private Visibility int_visTarget;
        public Visibility visTarget
        {
            get
            {
                return int_visTarget;
            }
            set
            {
                int_visTarget = value;
                OnPropertyChanged("visTarget");
            }
        }

        private Visibility int_visSource;
        public Visibility visSource
        {
            get
            {
                return int_visSource;
            }
            set
            {
                int_visSource = value;
                OnPropertyChanged("visSource");
            }
        }


        public void UpdateTaskViewSpaceData(UserControls.TaskViewParentType TaskViewType)
        {
            // Availiable Space
            ulong ulFreeDiskSpace = 0;

            string sourcePath = "";
            if (TaskViewType == UserControls.TaskViewParentType.Source)
                sourcePath = strSelectSourcePath;
            else
                sourcePath = strSelectTargetPath;

            TaskPlanner.UsefulMethods.DriveFreeBytes(sourcePath, out ulFreeDiskSpace);

            AvailiableSpace = TaskPlanner.UsefulMethods.ConvertFileSizeToString((double)ulFreeDiskSpace);



            // Required Space

            double doubReqSpace = 0;

            // TBD: Fall wenn Source und Target auf dem gleichen Laufwerk zusätzlich beachten!

            List<TaskPlanner.FileListEntry> fileList = null;
            List<TaskPlanner.FileListEntry> fileListSecond = null;
            bool fSourceTargetSameDrive = System.IO.Path.GetPathRoot(strSelectSourcePath) == System.IO.Path.GetPathRoot(strSelectTargetPath);

            if (TaskViewType == UserControls.TaskViewParentType.Source)
            {
                fileList = fileLists.SourceFileListAfter;
                if(fSourceTargetSameDrive)
                    fileListSecond = fileLists.TargetFileListAfter;
            }
            else
            {
                fileList = fileLists.TargetFileListAfter;
                if (fSourceTargetSameDrive)
                    fileListSecond = fileLists.SourceFileListAfter;
            }

            if(fileList != null)
                foreach (TaskPlanner.FileListEntry entry in fileList)
                {
                doubReqSpace += entry.targetNode.RequiredSpaceThisNode();
                }

            if(fileListSecond!=null)
                foreach (TaskPlanner.FileListEntry entry in fileListSecond)
                {
                    doubReqSpace += entry.targetNode.RequiredSpaceThisNode();
                }


            RequiredSpace = TaskPlanner.UsefulMethods.ConvertFileSizeToString(doubReqSpace);

        }


        public Tasks.FileLists fileLists { get; set; }

        public UserOptionsType userOptionsType { get; set; }

        public MBUI messageBoxUI { get; set; }


        // ______________Propertys für Anzeige des User Interface


        public void ApplySelectSourceEasyBase()
        {
            fileFilterOptions.bIncludeOnly = false;
            //fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*"; //specific

            dsuSelectedSizeProcessHash = DataSizeUnit.kByte; // KByte selektieren
            ProcessHash = "50"; // 10 was not enough for grave digger mp3s

            PlaylistFileFilterOptions.bIncludeOnly = true;
            PlaylistFileFilterOptions.IncludeOnly = "*.m3u, *.wpl, *.kpl";
            PlaylistFileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";

            bUpdatePlaylists = true;
            bUseSameFolderPaths = true;

        }

        private bool int_bTabcontrolModeSelectEnabled;
        public bool bTabcontrolModeSelectEnabled
        {
            get
            {
                return int_bTabcontrolModeSelectEnabled;
            }
            set
            {
                int_bTabcontrolModeSelectEnabled = value;
                OnPropertyChanged("bTabcontrolModeSelectEnabled");
            }
        }


        private bool int_bExpertOptions_Visible;
        public bool bExpertOptions_Visible
        {
            get
            {
                return int_bExpertOptions_Visible;
            }

            set
            {
                int_bExpertOptions_Visible = value;
                OnPropertyChanged("bExpertOptions_Visible");
                OnPropertyChanged("bSelectSourceVisibleResult");
                OnPropertyChanged("Button_AddPlaylistPath_VisibilityResult");
            }

        }


        public bool bSelectSourceVisibleResult
        {
            get
            {
                return bExpertOptions_Visible && bProcessHashCodes_Visible;
            }

        }

        public bool Button_AddPlaylistPath_VisibilityResult
        {
            get
            {
                return Button_AddPlaylistPath_Visibility && bExpertOptions_Visible;
            }

        }





        private string int_AvailiableSpace;
        public string AvailiableSpace
        {
            get
            {
                return int_AvailiableSpace;
            }
            set
            {
                int_AvailiableSpace = value;
                OnPropertyChanged("AvailiableSpace");
            }
        }

        private string int_RequiredSpace;
        public string RequiredSpace
        {
            get
            {
                return int_RequiredSpace;
            }
            set
            {
                int_RequiredSpace = value;
                OnPropertyChanged("RequiredSpace");
            }
        }


        // Finished Button Visibility
        private Visibility int_FinishedButtonVisibility;
        public Visibility FinishedButtonVisibility
        {
            get
            {
                return int_FinishedButtonVisibility;
            }
            set
            {
                int_FinishedButtonVisibility = value;
                OnPropertyChanged("FinishedButtonVisibility");
                OnPropertyChanged("CancelButtonEnabled");
            }
        }

        //Buttons Enabled

        private bool int_BackButtonEnabled;
        public bool BackButtonEnabled
        {
            get
            {
                return int_BackButtonEnabled;
            }
            set
            {
                int_BackButtonEnabled = value;
                OnPropertyChanged("BackButtonEnabled");
            }
        }

        private bool int_NextButtonEnabled;
        public bool NextButtonEnabled
        {
            get
            {
                return int_NextButtonEnabled;
            }
            set
            {
                int_NextButtonEnabled = value;
                OnPropertyChanged("NextButtonEnabled");
                OnPropertyChanged("CancelButtonEnabled");
            }
        }

        public bool CancelButtonEnabled
        {
            get
            {
                if ((NextButtonEnabled == false) && (FinishedButtonVisibility== Visibility.Visible))
                    return false;

                return true;
            }
        }




        // ScrollBarVisibility
        private System.Windows.Controls.ScrollBarVisibility int_ParentScrollBarVisibility;
        public System.Windows.Controls.ScrollBarVisibility ParentScrollBarVisibility
        {
            get
            {
                return int_ParentScrollBarVisibility;
            }

            set
            {
                int_ParentScrollBarVisibility = value;
                OnPropertyChanged("ParentScrollBarVisibility");
            }

        }


        // Generate Hashes Continue
        private bool int_bContinue_Visible;
        public bool bContinue_Visible // write in CurrentProgress Property
        {
            get
            {
                return int_bContinue_Visible;
            }

            set
            {
                int_bContinue_Visible = value;
                OnPropertyChanged("bContinue_Visible");
            }

        }


        // Process size
        private string int_ProcessTargetSize;
        public string ProcessTargetSize
        {
            get
            {
                return int_ProcessTargetSize;
            }

            set
            {
                int_ProcessTargetSize = value;
                OnPropertyChanged("ProcessTargetSize");
                OnPropertyChanged("CurrentProgress");
                OnPropertyChanged("ProcessTargetSizeReadable");
            }

        }


        private string int_ProcessCurrentSize;
        public string ProcessCurrentSize
        {
            get
            {
                return int_ProcessCurrentSize;
            }

            set
            {
                int_ProcessCurrentSize = value;

                OnPropertyChanged("ProcessCurrentSize");
                OnPropertyChanged("CurrentProgress");
                OnPropertyChanged("ProcessCurrentSizeReadable");
            }

        }



        public string ProcessTargetSizeReadable
        {
            get
            {
                return TaskPlanner.UsefulMethods.ConvertFileSizeToString(Convert.ToDouble(ProcessTargetSize));
            }

            set
            {
            }

        }


        public string ProcessCurrentSizeReadable
        {
            get
            {
                return TaskPlanner.UsefulMethods.ConvertFileSizeToString(Convert.ToDouble(ProcessCurrentSize));
            }

            set
            {
            }

        }



        public int CurrentProgress
        {
            get
            {
                int ret = 0;

                if (Convert.ToDouble(ProcessTargetSize) > 0)
                {
                    ret = (int)(Convert.ToDouble(ProcessCurrentSize) * 100 / Convert.ToDouble(ProcessTargetSize));
                    if (ret > 100)
                        ret = 100;
                    if (ret < 0)
                        ret = 0;
                }
                else
                    ret = 100;


                if (ret < 100)
                    bContinue_Visible = false;
                else
                    bContinue_Visible = true;

                return ret;
            }
            set
            {
                OnPropertyChanged("CurrentProgress");
            }
        }




        private string int_strTaskExecution;
        public string strTaskExecution
        {
            get
            {
                return int_strTaskExecution;
            }

            set
            {
                int_strTaskExecution = value;
                OnPropertyChanged("strTaskExecution");

                if (bTaskExecuteAutoscroll)
                {
                    // Autoscroll
                    if (ScrollToEndTaskExecute != null)
                    {
                        if (mainWindow != null)
                        {
                            mainWindow.Dispatcher.Invoke(
                               (Action)(() =>
                               {
                                   ScrollToEndTaskExecute();
                               }
                                ), null);
                        }
                    }
                }
            }

        }



        public int CurrentProgressTaskExecution
        {
            get
            {
                int ret = 0;

                if (Convert.ToDouble(ProcessTargetSizeTaskExecution) > 0)
                {
                    ret = (int)(Convert.ToDouble(ProcessCurrentSizeTaskExecution) * 100 / Convert.ToDouble(ProcessTargetSizeTaskExecution));
                    if (ret > 100)
                        ret = 100;
                    if (ret < 0)
                        ret = 0;
                }
                else
                    ret = 100;


                if (ret == 100)
                    FinishedButtonVisibility = Visibility.Visible;
                else
                    FinishedButtonVisibility = Visibility.Hidden;


                return ret;
            }
            set
            {
                OnPropertyChanged("CurrentProgressTaskExecution");
            }
        }

        private string int_ProcessTargetSizeTaskExecution;
        public string ProcessTargetSizeTaskExecution
        {
            get
            {
                return int_ProcessTargetSizeTaskExecution;
            }

            set
            {
                int_ProcessTargetSizeTaskExecution = value;
                OnPropertyChanged("ProcessTargetSizeTaskExecution");
                OnPropertyChanged("CurrentProgressTaskExecution");
            }

        }


        private string int_ProcessCurrentSizeTaskExecution;
        public string ProcessCurrentSizeTaskExecution
        {
            get
            {
                return int_ProcessCurrentSizeTaskExecution;
            }

            set
            {
                int_ProcessCurrentSizeTaskExecution = value;
                OnPropertyChanged("ProcessCurrentSizeTaskExecution");
                OnPropertyChanged("CurrentProgressTaskExecution");
            }

        }



        private bool int_bTaskExecuteAutoscroll;
        public bool bTaskExecuteAutoscroll
        {
            get
            {
                return int_bTaskExecuteAutoscroll;
            }

            set
            {
                int_bTaskExecuteAutoscroll = value;
                OnPropertyChanged("bTaskExecuteAutoscroll");
            }

        }

        public degScrollToEndTaskExecute ScrollToEndTaskExecute { get; set; }
        




        // ______________User Options




        // Process max for hash codes

        private string int_ProcessHash;
        public string ProcessHash
        {
            get
            {
                return int_ProcessHash;
            }

            set
            {
                int_ProcessHash = value;
                OnPropertyChanged("ProcessHash");
            }

        }

        public DataSizeUnit dsuSelectedSizeProcessHash
        {
            get
            {
                if (SelectedSizeProcessHash.Content.ToString() == DataSizeUnit.kByte.ToString())
                    return DataSizeUnit.kByte;
                else if (SelectedSizeProcessHash.Content.ToString() == DataSizeUnit.mByte.ToString())
                    return DataSizeUnit.mByte;
                else if (SelectedSizeProcessHash.Content.ToString() == DataSizeUnit.gByte.ToString())
                    return DataSizeUnit.gByte;
                else
                    return DataSizeUnit.gByte;
            }

            set
            {
                if (value == DataSizeUnit.kByte)
                    SelectedSizeProcessHash = SelectedSizeProcessHashItems[0];
                else if (value == DataSizeUnit.mByte)
                    SelectedSizeProcessHash = SelectedSizeProcessHashItems[1];
                /*
                else if (value == DataSizeUnit.gByte)
                    SelectedSizeProcessHash = SelectedSizeProcessHashItems[2];
                 */
            }
        }




        public int intSelectedSizeProcessHash
        {
            get
            {
                checked // Overflow Exception DataTypes
                {
                    try
                    {
                        if (dsuSelectedSizeProcessHash == DataSizeUnit.kByte)
                        {
                            int ret = Convert.ToInt32(ProcessHash) * 1024;
                            if (ret > 1023 * 1024 * 1024)
                            {
                                MessageBox.Show("Max 1024mByte per Process!");
                                ret = 1023 * 1024 * 1024;
                            }
                            return ret;
                        }
                        else if (dsuSelectedSizeProcessHash == DataSizeUnit.mByte)
                        {
                            int ret = Convert.ToInt32(ProcessHash) * 1024 * 1024;
                            if (ret > 1023 * 1024 * 1024)
                            {
                                MessageBox.Show("Max 1024mByte per Process!");
                                ret = 1023 * 1024 * 1024;
                            }
                            return ret;
                        }
                        /*
                        else if (dsuSelectedSizeProcessHash == DataSizeUnit.gByte)
                            return Convert.ToInt32(ProcessHash) * 1024 * 1024 * 1024;
                         */
                    }

                    catch (Exception ex)
                    {
                        String message = "ProcessHash input fault! ";
                        if (ex is OverflowException)
                            message += "Max 1024mByte per Process! ";
                        MessageBox.Show(message + ex.Message);
                    }
                }

                return 1023 * 1024 * 1024;
            }

        }

        private System.Windows.Controls.ComboBoxItem int_SelectedSizeProcessHash;
        public System.Windows.Controls.ComboBoxItem SelectedSizeProcessHash
        {
            get
            {
                return int_SelectedSizeProcessHash;
            }

            set
            {
                int_SelectedSizeProcessHash = value;
                OnPropertyChanged("SelectedSizeProcessHash");
            }

        }


        private List<System.Windows.Controls.ComboBoxItem> int_SelectedSizeProcessHashItems;
        public List<System.Windows.Controls.ComboBoxItem> SelectedSizeProcessHashItems
        {
            get
            {
                return int_SelectedSizeProcessHashItems;
            }

            set
            {
                int_SelectedSizeProcessHashItems = value;
                OnPropertyChanged("SelectedSizeProcessHashItems");
            }

        }





        // File Filter Options

        private FileFilterOptions int_fileFilterOptions;
        public FileFilterOptions fileFilterOptions
        {
            get
            {
                return int_fileFilterOptions;
            }

            set
            {
                int_fileFilterOptions = value;
                OnPropertyChanged("fileFilterOptions");
            }

        }


        private FileFilterOptions int_PlaylistFileFilterOptions;
        public FileFilterOptions PlaylistFileFilterOptions
        {
            get
            {
                return int_PlaylistFileFilterOptions;
            }

            set
            {
                int_PlaylistFileFilterOptions = value;
                OnPropertyChanged("PlaylistFileFilterOptions");
            }

        }


        // Other Options

        private String int_strSelectSourcePath;
        public String strSelectSourcePath
        {
            get
            {
                return int_strSelectSourcePath;
            }

            set
            {
                int_strSelectSourcePath = NetworkPathing.Pathing.GetUNCPath(value);

                /*
                string root = System.IO.Path.GetPathRoot(int_strSelectSourcePath);
                if (int_strSelectSourcePath == root)
                {
                    int_strSelectSourcePath = int_strSelectSourcePath.Replace("\\", "");
                }
                */

                OnPropertyChanged("strSelectSourcePath");
            }

        }

        private String int_strSelectTargetPath;
        public String strSelectTargetPath
        {
            get
            {
                return int_strSelectTargetPath;
            }

            set
            {
                int_strSelectTargetPath = NetworkPathing.Pathing.GetUNCPath(value);

                /*
                string root = System.IO.Path.GetPathRoot(int_strSelectTargetPath);
                if (int_strSelectTargetPath == root)
                {
                    int_strSelectTargetPath = int_strSelectTargetPath.Replace("\\", "");
                }
                 */

                OnPropertyChanged("strSelectTargetPath");
            }

        }

        private bool int_bSelectTarget_Visible;
        public bool bSelectTarget_Visible
        {
            get
            {
                return int_bSelectTarget_Visible;
            }

            set
            {
                int_bSelectTarget_Visible = value;
                OnPropertyChanged("bSelectTarget_Visible");
            }

        }




        private bool int_bProcessHashCodes_Visible;
        public bool bProcessHashCodes_Visible
        {
            get
            {
                return int_bProcessHashCodes_Visible;
            }

            set
            {
                int_bProcessHashCodes_Visible = value;
                OnPropertyChanged("bProcessHashCodes_Visible");
                OnPropertyChanged("bSelectSourceVisibleResult");
            }

        }





        // Playlist Paths

        public List<string> GetAllPlaylistPaths()
        {
            List<string> listPlaylistPaths = new List<string>();

            if (bUpdatePlaylists)
            {
                if (bUseSameFolderPaths)
                {
                    listPlaylistPaths.Add( strSelectSourcePath );

                    if (bSelectTarget_Visible)
                        listPlaylistPaths.Add(strSelectTargetPath);
                }
                else
                {
                    if (bPlaylistPath1_Visible)
                        if ((strPlaylistPath1 != null) && strPlaylistPath1.Length > 0)
                            listPlaylistPaths.Add(strPlaylistPath1);

                    if (bPlaylistPath2_Visible)
                        if ((strPlaylistPath2 != null) && strPlaylistPath2.Length > 0)
                            listPlaylistPaths.Add(strPlaylistPath2);

                    if (bPlaylistPath3_Visible)
                        if ((strPlaylistPath3 != null) && strPlaylistPath3.Length > 0)
                            listPlaylistPaths.Add(strPlaylistPath3);

                    if (bPlaylistPath4_Visible)
                        if ((strPlaylistPath4 != null) && strPlaylistPath4.Length > 0)
                            listPlaylistPaths.Add(strPlaylistPath4);

                    if (bPlaylistPath5_Visible)
                        if ((strPlaylistPath5 != null) && strPlaylistPath5.Length > 0)
                            listPlaylistPaths.Add(strPlaylistPath5);
                }
            }

            return listPlaylistPaths;
        }

        private bool int_bUpdatePlaylists;
        public bool bUpdatePlaylists
        {
            get
            {
                return int_bUpdatePlaylists;
            }

            set
            {
                int_bUpdatePlaylists = value;
                OnPropertyChanged("bUpdatePlaylists");
            }
        }

        private bool int_bUseSameFolderPaths;
        public bool bUseSameFolderPaths
        {
            get
            {
                return int_bUseSameFolderPaths;
            }

            set
            {
                int_bUseSameFolderPaths = value;
                if (value == true)
                {
                    Playlist_Hide_all();
                }
                else
                    Button_AddPlaylistPath_Visibility = true;

                OnPropertyChanged("bUseSameFolderPaths");
            }
        }


        private void Playlist_Hide_all()
        {
            bPlaylistPath1_Visible = false;
            bPlaylistPath2_Visible = false;
            bPlaylistPath3_Visible = false;
            bPlaylistPath4_Visible = false;
            bPlaylistPath5_Visible = false;
            Button_AddPlaylistPath_Visibility = false;
        }



        private bool int_bPlaylistPath1_Visible;
        public bool bPlaylistPath1_Visible
        {
            get
            {
                return int_bPlaylistPath1_Visible;
            }

            set
            {
                int_bPlaylistPath1_Visible = value;
                OnPropertyChanged("bPlaylistPath1_Visible");
            }
        }
        private bool int_bPlaylistPath2_Visible;
        public bool bPlaylistPath2_Visible
        {
            get
            {
                return int_bPlaylistPath2_Visible;
            }

            set
            {
                int_bPlaylistPath2_Visible = value;
                OnPropertyChanged("bPlaylistPath2_Visible");
            }
        }
        private bool int_bPlaylistPath3_Visible;
        public bool bPlaylistPath3_Visible
        {
            get
            {
                return int_bPlaylistPath3_Visible;
            }

            set
            {
                int_bPlaylistPath3_Visible = value;
                OnPropertyChanged("bPlaylistPath3_Visible");
            }
        }
        private bool int_bPlaylistPath4_Visible;
        public bool bPlaylistPath4_Visible
        {
            get
            {
                return int_bPlaylistPath4_Visible;
            }

            set
            {
                int_bPlaylistPath4_Visible = value;
                OnPropertyChanged("bPlaylistPath4_Visible");
            }
        }
        private bool int_bPlaylistPath5_Visible;
        public bool bPlaylistPath5_Visible
        {
            get
            {
                return int_bPlaylistPath5_Visible;
            }

            set
            {
                int_bPlaylistPath5_Visible = value;
                OnPropertyChanged("bPlaylistPath5_Visible");
            }
        }

        private bool int_Button_AddPlaylistPath_Visibility;
        public bool Button_AddPlaylistPath_Visibility 
        { 
            get
            {
                return int_Button_AddPlaylistPath_Visibility;
            }

            set
            {
                int_Button_AddPlaylistPath_Visibility = value;
                OnPropertyChanged("Button_AddPlaylistPath_Visibility");
                OnPropertyChanged("Button_AddPlaylistPath_VisibilityResult");
            }
        }






        private String int_strPlaylistPath1;
        public String strPlaylistPath1
        {
            get
            {
                return int_strPlaylistPath1;
            }

            set
            {
                int_strPlaylistPath1 = NetworkPathing.Pathing.GetUNCPath(value);
                OnPropertyChanged("strPlaylistPath1");
            }

        }

        private String int_strPlaylistPath2;
        public String strPlaylistPath2
        {
            get
            {
                return int_strPlaylistPath2;
            }

            set
            {
                int_strPlaylistPath2 = NetworkPathing.Pathing.GetUNCPath(value);
                OnPropertyChanged("strPlaylistPath2");
            }

        }

        private String int_strPlaylistPath3;
        public String strPlaylistPath3
        {
            get
            {
                return int_strPlaylistPath3;
            }

            set
            {
                int_strPlaylistPath3 = NetworkPathing.Pathing.GetUNCPath(value);
                OnPropertyChanged("strPlaylistPath3");
            }

        }

        private String int_strPlaylistPath4;
        public String strPlaylistPath4
        {
            get
            {
                return int_strPlaylistPath4;
            }

            set
            {
                int_strPlaylistPath4 = NetworkPathing.Pathing.GetUNCPath(value);
                OnPropertyChanged("strPlaylistPath4");
            }

        }

        private String int_strPlaylistPath5;
        public String strPlaylistPath5
        {
            get
            {
                return int_strPlaylistPath5;
            }

            set
            {
                int_strPlaylistPath5 = NetworkPathing.Pathing.GetUNCPath(value);
                OnPropertyChanged("strPlaylistPath5");
            }

        }



        // ______________Propertys für die einfachere weiterverarbeitung







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






    public class FileFilterOptions : INotifyPropertyChanged
    {
        public FileFilterOptions()
        {
            //
            // int Breakpoint = 5;
        }


        public bool CheckIfFileOK(String path)
        {
            string name = System.IO.Path.GetFileName(path);
            string orgFilter = IncludeOnly.Replace(" ,", ",").Replace(", ",","); // Remove Spaces
            string[] arrOrgFilter = orgFilter.Split(',');

            // ________IncludeOnly___________

            bool bResult_IncludeOnly = false;

            if (bIncludeOnly == true)
            {
                string regexFilter = "^"; // start

                bool firstloop = true;
                foreach (string strFilter in arrOrgFilter)
                {
                    if (!firstloop)
                        regexFilter += "|";
                    firstloop = false;

                    regexFilter += "(?i)"; // Case insensitive

                    regexFilter += System.Text.RegularExpressions.Regex.Escape(strFilter).Replace(@"\*", ".*").Replace(@"\?", "."); // need to escape first, so dots are read as dots etc.
                }
                regexFilter += "$";  // end

                //System.Text.RegularExpressions.Regex.Escape(regexFilter);

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regexFilter);

                bResult_IncludeOnly = regex.IsMatch(name);
            }
            else
                bResult_IncludeOnly = true;



            // _______always Exclude_______


            bool bResult_AlwaysExclude = true;
            orgFilter = AlwaysExclude.Replace(" ,", ",").Replace(", ", ","); // Remove Spaces
            arrOrgFilter = orgFilter.Split(',');


            {
                string regexFilter = "^"; // start

                bool firstloop = true;
                foreach (string strFilter in arrOrgFilter)
                {
                    if (!firstloop)
                        regexFilter += "|";
                    firstloop = false;

                    regexFilter += "(?i)"; // Case insensitive

                    regexFilter += System.Text.RegularExpressions.Regex.Escape(strFilter).Replace(@"\*", ".*").Replace(@"\?", "."); // need to escape first, so dots are read as dots etc.
                }
                regexFilter += "$";  // end

                //System.Text.RegularExpressions.Regex.Escape(regexFilter);

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regexFilter);

                bResult_AlwaysExclude = regex.IsMatch(name);
            }

            /*
            if (bResult_AlwaysExclude == true)
            {
                int breakpoint = 5;
            }
             */

            return !bResult_AlwaysExclude && bResult_IncludeOnly;
        }



        public FileFilterOptions Clone()
        {
            FileFilterOptions clone = new FileFilterOptions();

            clone.bIncludeOnly = bIncludeOnly;
            clone.IncludeOnly = (string)IncludeOnly.Clone();

            clone.AlwaysExclude = (string)AlwaysExclude.Clone();

            return clone;
        }


        public void Update(FileFilterOptions Source)
        {
            bIncludeOnly = Source.bIncludeOnly;
            IncludeOnly = Source.IncludeOnly;

            AlwaysExclude = Source.AlwaysExclude;
        }


        // ______________Propertys für Anzeige des User Interface
        private bool int_bIncludeOnly;
        public bool bIncludeOnly
        {
            get
            {
                return int_bIncludeOnly;
            }

            set
            {
                int_bIncludeOnly = value;

                if (value == false)
                    IncludeOnly = "everything";
                else
                    IncludeOnly = "";

                OnPropertyChanged("bIncludeOnly");
            }

        }


        private String int_IncludeOnly;
        public String IncludeOnly
        {
            get
            {
                return int_IncludeOnly;
            }

            set
            {
                int_IncludeOnly = value;
                OnPropertyChanged("IncludeOnly");
            }

        }

        private String int_AlwaysExclude;
        public String AlwaysExclude
        {
            get
            {
                return int_AlwaysExclude;
            }

            set
            {
                int_AlwaysExclude = value;
                OnPropertyChanged("AlwaysExclude");
            }

        }



        // ______________Propertys für die einfachere weiterverarbeitung







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
