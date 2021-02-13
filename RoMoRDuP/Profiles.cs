
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Text;
using System.Windows;

using System.ComponentModel;

using RoMoRDuP.UserInterface;


namespace RoMoRDuP.Profiles
{

    public class Profiles : INotifyPropertyChanged
    {
        public const string ROMORDUP_VERSION = "0.60"; // TBD UPDATE


        UserInterface.UserInterface userOptions;


        public Profiles(UserInterface.UserInterface userOptions)
        {
            CurrentProfilePath = "default.RMDPProf";

            this.userOptions = userOptions;
        }


        public void SaveProfile(string path)
        {
            try
            {

                // Version 
                Properties.Settings.Default.Version = ROMORDUP_VERSION;


                // Mirror SelectSource
                Properties.Settings.Default.MirrorProcessMax = Convert.ToInt32(userOptions.userOptionsMirror.ProcessHash);

                Properties.Settings.Default.MirrorProcessMaxType = Enum.GetName(typeof(DataSizeUnit),userOptions.userOptionsMirror.dsuSelectedSizeProcessHash);

                Properties.Settings.Default.MirrorSourceFolder = userOptions.userOptionsMirror.strSelectSourcePath;

                Properties.Settings.Default.MirrorTargetFolder = userOptions.userOptionsMirror.strSelectTargetPath;




                Properties.Settings.Default.MirrorFileFilterbIncludeOnly = userOptions.userOptionsMirror.fileFilterOptions.bIncludeOnly;

                Properties.Settings.Default.MirrorFileFilterInclude = userOptions.userOptionsMirror.fileFilterOptions.IncludeOnly;

                Properties.Settings.Default.MirrorFileFilterExclude = userOptions.userOptionsMirror.fileFilterOptions.AlwaysExclude;

                Properties.Settings.Default.MirrorFileFilterPlaylistbIncludeOnly = userOptions.userOptionsMirror.PlaylistFileFilterOptions.bIncludeOnly;

                Properties.Settings.Default.MirrorFileFilterPlaylistInclude = userOptions.userOptionsMirror.PlaylistFileFilterOptions.IncludeOnly;

                Properties.Settings.Default.MirrorFileFilterPlaylistExclude = userOptions.userOptionsMirror.PlaylistFileFilterOptions.AlwaysExclude;

                Properties.Settings.Default.MirrorbUpdatePlaylists = userOptions.userOptionsMirror.bUpdatePlaylists;

                Properties.Settings.Default.MirrorbUseSameFolderPathsPlaylists = userOptions.userOptionsMirror.bUseSameFolderPaths;


                // Mirror UserOptions

                Properties.Settings.Default.MirrorDuplicatesWithDiffFilenames = Enum.GetName(typeof(enOptionsMirrorDuplicates),userOptions.userOptionsMirror.OptionsMirrorDuplicates);

                Properties.Settings.Default.MirrorUpdatedFiles = Enum.GetName(typeof(enOptionsMirrorUpdated),userOptions.userOptionsMirror.OptionsMirrorUpdated);

                Properties.Settings.Default.MirrorMoveTo = Enum.GetName(typeof(enOptionsMirrorMoveTo),userOptions.userOptionsMirror.OptionsMirrorMoveTo);

                Properties.Settings.Default.MirrorMoveChangeFilename = Enum.GetName(typeof(enOptionsMirrorMovedRenaming),userOptions.userOptionsMirror.OptionsMirrorMovedRenaming);

                Properties.Settings.Default.MirrorbCopyMissingSouceTarget = userOptions.userOptionsMirror.OptionsMirrorCopySourceTarget;

                Properties.Settings.Default.MirrorbCopyMissingTargetSource = userOptions.userOptionsMirror.OptionsMirrorCopyTargetSource;

                Properties.Settings.Default.MirrorbDeleteTarget = userOptions.userOptionsMirror.OptionsMirrorDeleteTargetSource;

                Properties.Settings.Default.MirrorbDeleteSource = userOptions.userOptionsMirror.OptionsMirrorDeleteSourceTarget;


                Properties.Settings.Default.MirrorSyncAttributes = userOptions.userOptionsMirror.OptionsMirrorSyncAttributes;

                // Mirror UserOptions Easy

                Properties.Settings.Default.MirrorUserOptionsEasy = Enum.GetName(typeof(UserInterface.enOptionsMirrorEasy), userOptions.userOptionsMirror.OptionsMirrorEasy);



                // Mirror Selected Mode

                Properties.Settings.Default.MirrorModeSelectSource = Enum.GetName(typeof(UserControls.enSelectedMode), userOptions.userOptionsMirror.ModeSelectSource);

                Properties.Settings.Default.MirrorModeUserOptions = Enum.GetName(typeof(UserControls.enSelectedMode), userOptions.userOptionsMirror.ModeUserOptions);






                // RD SelectSource
                Properties.Settings.Default.RDProcessMax = Convert.ToInt32(userOptions.userOptionsRemoveDuplicates.ProcessHash);

                Properties.Settings.Default.RDProcessMaxType = Enum.GetName(typeof(DataSizeUnit), userOptions.userOptionsRemoveDuplicates.dsuSelectedSizeProcessHash);

                Properties.Settings.Default.RDSourceFolder = userOptions.userOptionsRemoveDuplicates.strSelectSourcePath;




                Properties.Settings.Default.RDFileFilterbIncludeOnly = userOptions.userOptionsRemoveDuplicates.fileFilterOptions.bIncludeOnly;

                Properties.Settings.Default.RDFileFilterInclude = userOptions.userOptionsRemoveDuplicates.fileFilterOptions.IncludeOnly;

                Properties.Settings.Default.RDFileFilterExclude = userOptions.userOptionsRemoveDuplicates.fileFilterOptions.AlwaysExclude;

                Properties.Settings.Default.RDFileFilterPlaylistbIncludeOnly = userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.bIncludeOnly;

                Properties.Settings.Default.RDFileFilterPlaylistInclude = userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.IncludeOnly;

                Properties.Settings.Default.RDFileFilterPlaylistExclude = userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.AlwaysExclude;

                Properties.Settings.Default.RDbUpdatePlaylists = userOptions.userOptionsRemoveDuplicates.bUpdatePlaylists;

                Properties.Settings.Default.RDbUseSameFolderPathsPlaylists = userOptions.userOptionsRemoveDuplicates.bUseSameFolderPaths;



                // RD Selected Mode

                Properties.Settings.Default.RDModeSelectSource = Enum.GetName(typeof(UserControls.enSelectedMode), userOptions.userOptionsRemoveDuplicates.ModeSelectSource);






                //___________________________________________________
                Properties.Settings.Default.SettingsKey = path;

                CurrentProfilePath = path;

                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to save " + ex.Message);
            }
             

        }


        public void LoadProfile(string path)
        {
            try
            {
                Properties.Settings.Default.SettingsKey = path;

                CurrentProfilePath = path;

                Properties.Settings.Default.Reload();



                // Mirror select Source
                userOptions.userOptionsMirror.ProcessHash = Properties.Settings.Default.MirrorProcessMax.ToString();

                userOptions.userOptionsMirror.dsuSelectedSizeProcessHash = (UserInterface.DataSizeUnit)Enum.Parse(typeof(UserInterface.DataSizeUnit),Properties.Settings.Default.MirrorProcessMaxType);

                userOptions.userOptionsMirror.strSelectSourcePath = Properties.Settings.Default.MirrorSourceFolder;

                userOptions.userOptionsMirror.strSelectTargetPath = Properties.Settings.Default.MirrorTargetFolder;





                userOptions.userOptionsMirror.fileFilterOptions.bIncludeOnly = Properties.Settings.Default.MirrorFileFilterbIncludeOnly;

                userOptions.userOptionsMirror.fileFilterOptions.IncludeOnly = Properties.Settings.Default.MirrorFileFilterInclude;

                userOptions.userOptionsMirror.fileFilterOptions.AlwaysExclude = Properties.Settings.Default.MirrorFileFilterExclude;

                userOptions.userOptionsMirror.PlaylistFileFilterOptions.bIncludeOnly = Properties.Settings.Default.MirrorFileFilterPlaylistbIncludeOnly;

                userOptions.userOptionsMirror.PlaylistFileFilterOptions.IncludeOnly = Properties.Settings.Default.MirrorFileFilterPlaylistInclude;

                userOptions.userOptionsMirror.PlaylistFileFilterOptions.AlwaysExclude = Properties.Settings.Default.MirrorFileFilterPlaylistExclude;

                userOptions.userOptionsMirror.bUpdatePlaylists = Properties.Settings.Default.MirrorbUpdatePlaylists;

                userOptions.userOptionsMirror.bUseSameFolderPaths = Properties.Settings.Default.MirrorbUseSameFolderPathsPlaylists;


                // Mirror UserOptions

                userOptions.userOptionsMirror.OptionsMirrorDuplicates = (enOptionsMirrorDuplicates)Enum.Parse(typeof(enOptionsMirrorDuplicates), Properties.Settings.Default.MirrorDuplicatesWithDiffFilenames);

                userOptions.userOptionsMirror.OptionsMirrorUpdated = (enOptionsMirrorUpdated)Enum.Parse(typeof(enOptionsMirrorUpdated), Properties.Settings.Default.MirrorUpdatedFiles);

                userOptions.userOptionsMirror.OptionsMirrorMoveTo = (enOptionsMirrorMoveTo)Enum.Parse(typeof(enOptionsMirrorMoveTo), Properties.Settings.Default.MirrorMoveTo);

                userOptions.userOptionsMirror.OptionsMirrorMovedRenaming = (enOptionsMirrorMovedRenaming)Enum.Parse(typeof(enOptionsMirrorMovedRenaming), Properties.Settings.Default.MirrorMoveChangeFilename);

                userOptions.userOptionsMirror.OptionsMirrorCopySourceTarget = Properties.Settings.Default.MirrorbCopyMissingSouceTarget;

                userOptions.userOptionsMirror.OptionsMirrorCopyTargetSource = Properties.Settings.Default.MirrorbCopyMissingTargetSource;

                userOptions.userOptionsMirror.OptionsMirrorDeleteTargetSource = Properties.Settings.Default.MirrorbDeleteTarget;

                userOptions.userOptionsMirror.OptionsMirrorDeleteSourceTarget = Properties.Settings.Default.MirrorbDeleteSource;

                userOptions.userOptionsMirror.OptionsMirrorSyncAttributes = Properties.Settings.Default.MirrorSyncAttributes;


                // Mirror UserOptions Easy

                userOptions.userOptionsMirror.OptionsMirrorEasy = (UserInterface.enOptionsMirrorEasy)Enum.Parse(typeof(UserInterface.enOptionsMirrorEasy), Properties.Settings.Default.MirrorUserOptionsEasy);


                // Mirror Selected Mode

                userOptions.userOptionsMirror.ModeSelectSource = (UserControls.enSelectedMode)Enum.Parse(typeof(UserControls.enSelectedMode), Properties.Settings.Default.MirrorModeSelectSource);

                userOptions.userOptionsMirror.ModeUserOptions = (UserControls.enSelectedMode)Enum.Parse(typeof(UserControls.enSelectedMode), Properties.Settings.Default.MirrorModeUserOptions);






                // RD select Source
                userOptions.userOptionsRemoveDuplicates.ProcessHash = Properties.Settings.Default.RDProcessMax.ToString();

                userOptions.userOptionsRemoveDuplicates.dsuSelectedSizeProcessHash = (UserInterface.DataSizeUnit)Enum.Parse(typeof(UserInterface.DataSizeUnit), Properties.Settings.Default.RDProcessMaxType);

                userOptions.userOptionsRemoveDuplicates.strSelectSourcePath = Properties.Settings.Default.RDSourceFolder;

   



                userOptions.userOptionsRemoveDuplicates.fileFilterOptions.bIncludeOnly = Properties.Settings.Default.RDFileFilterbIncludeOnly;

                userOptions.userOptionsRemoveDuplicates.fileFilterOptions.IncludeOnly = Properties.Settings.Default.RDFileFilterInclude;

                userOptions.userOptionsRemoveDuplicates.fileFilterOptions.AlwaysExclude = Properties.Settings.Default.RDFileFilterExclude;

                userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.bIncludeOnly = Properties.Settings.Default.RDFileFilterPlaylistbIncludeOnly;

                userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.IncludeOnly = Properties.Settings.Default.RDFileFilterPlaylistInclude;

                userOptions.userOptionsRemoveDuplicates.PlaylistFileFilterOptions.AlwaysExclude = Properties.Settings.Default.RDFileFilterPlaylistExclude;

                userOptions.userOptionsRemoveDuplicates.bUpdatePlaylists = Properties.Settings.Default.RDbUpdatePlaylists;

                userOptions.userOptionsRemoveDuplicates.bUseSameFolderPaths = Properties.Settings.Default.RDbUseSameFolderPathsPlaylists;


                // RD Selected Mode

                userOptions.userOptionsRemoveDuplicates.ModeSelectSource = (UserControls.enSelectedMode)Enum.Parse(typeof(UserControls.enSelectedMode), Properties.Settings.Default.RDModeSelectSource);

   



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to save " + ex.Message);
            }


        }



        private String int_CurrentProfilePath;
        public String CurrentProfilePath
        {
            get
            {
                return int_CurrentProfilePath;
            }

            set
            {
                int_CurrentProfilePath = value;
                OnPropertyChanged("CurrentProfilePath");
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

}