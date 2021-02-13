using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Text;
using System.Windows;

using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using System.Collections.Generic;

using System.Threading;

namespace RoMoRDuP.PlaylistUpdates
{
    public class ValidPath
    {
        public string strValidPath { get; set; }

        public enPathType PathType { get; set; }

        public int iStart { get; set; }

        public int iEnd { get; set; }

    }

    public enum enPathType { absolute, relative };



    public class PlaylistUpdates : INotifyPropertyChanged
    {

        private PlayListUpdatesModel int_playlistUpdatesModel;
        public PlayListUpdatesModel playlistUpdatesModel 
        {
            get
            {
                return int_playlistUpdatesModel;
            }
            set
            {
                int_playlistUpdatesModel = value;
                OnPropertyChanged("playlistUpdatesModel");
            }
        }

        private UserInterface.UserInterfaceBase userOptions { get; set; }

        private List<List<string>> matrixListPlaylists;

        /*
        private List<TaskPlanner.FileListEntry> SourceFileListBefore;
        private List<TaskPlanner.FileListEntry> TargetFileListBefore; 
        */

        private Tasks.FileLists fileLists;



        public PlaylistUpdates(UserInterface.UserInterfaceBase userOptions, Tasks.FileLists fileLists)
        {
            this.userOptions = userOptions;

            this.fileLists = fileLists;
            /*
            this.SourceFileListBefore = SourceFileListBefore;
            this.TargetFileListBefore = TargetFileListBefore;
            */
            matrixListPlaylists = new List<List<string>>();

            playlistUpdatesModel = new PlayListUpdatesModel();

        }

        /*
        Thread threadGeneratePlaylistUpdates;

        public void GeneratePlaylistUpdates()
        {
            if (threadGeneratePlaylistUpdates == null)
            {
                threadGeneratePlaylistUpdates = new Thread(new ThreadStart(int_GeneratePlaylistUpdates));
            }
            else
            {
                threadGeneratePlaylistUpdates.Abort(); // TBD check if Abort valid - only if everything managed

                threadGeneratePlaylistUpdates = new Thread(new ThreadStart(int_GeneratePlaylistUpdates));
            }

            

            threadGeneratePlaylistUpdates.Start();

        }
         */


        ObservableCollection<PlayListUpdatesViewModel> old_PlaylistUpdatesItems;


        public void ThreadSave_SavePlaylists(string folderpath)
        {
            // 6: Save Playlist Updates
            foreach (RoMoRDuP.PlaylistUpdates.PlayListUpdatesViewModel vm in playlistUpdatesModel.Items)
            {
                string targetpath = "";

                try
                {
                    targetpath = folderpath + "\\" + Enum.GetName(typeof(UserInterface.UserOptionsType), userOptions.userOptionsType)
                                                            + "_" + System.IO.Path.GetFileName(vm.PathOriginal);
                    System.IO.File.Copy(vm.PathOriginal, targetpath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can´t copy to " + targetpath);
                }

                try
                {
                    targetpath = folderpath + "\\" + Enum.GetName(typeof(UserInterface.UserOptionsType), userOptions.userOptionsType)
                                                            + "_" + System.IO.Path.GetFileName(vm.PathTemp);
                    System.IO.File.Copy(vm.PathTemp, targetpath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can´t copy to " + targetpath);
                }

            }

        }



        //Own Thread
        public void ThreadSave_GeneratePlaylistUpdates() // over TaskPlannerBase
        {
            try
            {
                old_PlaylistUpdatesItems = playlistUpdatesModel.Items;
                playlistUpdatesModel.Items = new ObservableCollection<PlayListUpdatesViewModel>(); // Reset


                // Delete old Plalist Updates
                String TempPlaylistPath = TempPlaylistPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (System.IO.Directory.Exists(TempPlaylistPath + "\\RoMoRDuP\\") == false)
                    if (System.IO.Directory.CreateDirectory(TempPlaylistPath + "\\RoMoRDuP\\") == null)
                    {
                        MessageBox.Show("Cannot create Directory " + TempPlaylistPath + "\\RoMoRDuP\\");
                        return;
                    }

                if (System.IO.Directory.Exists(TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\") == true) // empty folder
                {
                    // delete
                    try
                    {
                        System.IO.DirectoryInfo downloadedMessageInfo = new System.IO.DirectoryInfo(TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\");

                        foreach (System.IO.FileInfo file in downloadedMessageInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (System.IO.DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                        {
                            dir.Delete(true);
                        }

                        //System.IO.Directory.Delete(TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Can´t delete " + TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\ ! " + ex.Message);
                    }
                }



                // Generate new Plalist Updates
                int iTempPlaylistIndex = 1;

                foreach (List<string> list in matrixListPlaylists)
                {
                    foreach (string PlaylistFilePath in list)
                    {
                        // load File into RAM
                        StringBuilder sb = new StringBuilder();
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(PlaylistFilePath, Encoding.Default))
                        {
                            String line;
                            // Read and display lines from the file until the end of 
                            // the file is reached.
                            while ((line = sr.ReadLine()) != null)
                            {
                                sb.AppendLine(line);
                            }
                        }
                        string strFileListOrgContent = sb.ToString();

                        List<ValidPath> AllValidPaths = GetAllValidPathsFromString(strFileListOrgContent, PlaylistFilePath);

                        {
                            TempPlaylistPath = TempPlaylistPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                            if (System.IO.Directory.Exists(TempPlaylistPath + "\\RoMoRDuP\\") == false)
                                if (System.IO.Directory.CreateDirectory(TempPlaylistPath + "\\RoMoRDuP\\") == null)
                                {
                                    MessageBox.Show("Cannot create Directory " + TempPlaylistPath + "\\RoMoRDuP\\");
                                    return;
                                }

                            if (System.IO.Directory.Exists(TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\") == false)
                                if (System.IO.Directory.CreateDirectory(TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\") == null)
                                {
                                    MessageBox.Show("Cannot create Directory " + TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\");
                                    return;
                                }

                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(PlaylistFilePath);
                            TempPlaylistPath = TempPlaylistPath + "\\RoMoRDuP\\PlaylistUpdates\\" + "Temp_" + iTempPlaylistIndex.ToString() + "_" + fileInfo.Name;
                            iTempPlaylistIndex++;
                        }

                        PlayListUpdatesViewModel PlaylistUpdatesEntry = null;

                        foreach (ValidPath validPath in AllValidPaths)
                        {
                            // regenerate all Before Actions already done in UCPartent

                            // check for changes
                            TaskPlanner.FileListEntry changingEntry = null;
                            changingEntry = CheckChangesValidPath(validPath, fileLists.SourceFileListBefore);

                            if (userOptions is UserInterface.UserOptionsMirror) // Only for Mirror
                            {
                                if (changingEntry == null)
                                    changingEntry = CheckChangesValidPath(validPath, fileLists.TargetFileListBefore);
                            }

                            // if changes create entry, set bool for create file in AppData/Playlist
                            if (changingEntry != null)
                            {
                                if (PlaylistUpdatesEntry == null)
                                {
                                    PlaylistUpdatesEntry = new PlayListUpdatesViewModel();
                                    PlaylistUpdatesEntry.PathOriginal = PlaylistFilePath;
                                    //PlaylistUpdatesEntry.PathRenameOrg = ;
                                    PlaylistUpdatesEntry.PathTemp = TempPlaylistPath;
                                }

                                PlaylistChange playlistChange = new PlaylistChange();

                                playlistChange.validPath = validPath;
                                playlistChange.changingEntry = changingEntry;

                                PlaylistUpdatesEntry.PlaylistChanges.Add(playlistChange);

                            }


                        }

                        // create file if changes
                        if (PlaylistUpdatesEntry != null)
                            if (PlaylistUpdatesEntry.PlaylistChanges.Count > 0)
                            {
                                // check if LeaveCopy/UpdatePlaylist Option was changed before
                                if (old_PlaylistUpdatesItems != null)
                                    foreach (PlayListUpdatesViewModel oldvm in old_PlaylistUpdatesItems)
                                    {
                                        if (oldvm.PathOriginal == PlaylistUpdatesEntry.PathOriginal)
                                        {
                                            PlaylistUpdatesEntry.LeaveCopy = oldvm.LeaveCopy;
                                            PlaylistUpdatesEntry.UpdatePlaylist = oldvm.UpdatePlaylist;
                                            break;
                                        }
                                    }


                                if (userOptions.mainWindow != null)
                                {
                                    userOptions.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           playlistUpdatesModel.Items.Add(PlaylistUpdatesEntry);
                                           CreateNewPlaylistFile(PlaylistFilePath, strFileListOrgContent, PlaylistUpdatesEntry);
                                       }
                                        ), null);
                                }

                            }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
        }


        private bool CreateNewPlaylistFile(string strOrgPlaylistPath ,string strOrgPlaylistContent, PlayListUpdatesViewModel PlaylistUpdatesEntry)
        {

            List<String> resultingFile = new List<string>();
            int lastIndex = 0;

            try
            {
                PlaylistChange lastPlaylistChange = null;

                foreach (PlaylistChange playlistChange in PlaylistUpdatesEntry.PlaylistChanges)
                {
                    lastPlaylistChange = playlistChange;

                    // part until changing path
                    resultingFile.Add(strOrgPlaylistContent.Substring(lastIndex, playlistChange.validPath.iStart - lastIndex));
                    lastIndex = playlistChange.validPath.iEnd +1;

                    string orgPath = strOrgPlaylistContent.Substring(playlistChange.validPath.iStart, playlistChange.validPath.iEnd - playlistChange.validPath.iStart +1);

                    // changing path
                    if (playlistChange.validPath.PathType == enPathType.absolute)
                    {
                        //resultingFile.Add(playlistChange.changingEntry.targetNode.otherView.GetFullPathFromNode());
                        string newPath = playlistChange.changingEntry.targetNode.GetPathAfterActionsFromNode(orgPath);

                        //if( newPath.Length == 0 )
                        //    foreach(string otherPaths in playlistChange.changingEntry.targetNode.DuplicatesSourceSourceOrTT)
                        // TBD

                        resultingFile.Add(newPath);
                    }
                    else // relative path
                    {
                        //resultingFile.Add(playlistChange.changingEntry.targetNode.otherView.GetFullPathFromNode());
                        string newPathAbsolute = playlistChange.changingEntry.targetNode.GetPathAfterActionsFromNode(orgPath);

                        //if( newPath.Length == 0 )
                        //    foreach(string otherPaths in playlistChange.changingEntry.targetNode.DuplicatesSourceSourceOrTT)
                        // TBD alternatives for delete

                        //System.IO.DirectoryInfo dirInfoPlaylistPath = new System.IO.DirectoryInfo(strOrgPlaylistPath);
                        //string strPlaylistBasePath = dirInfoPlaylistPath.FullName;

                        System.Uri uriFileAbsolute = new Uri(newPathAbsolute);
                        System.Uri uriBasePath = new Uri(strOrgPlaylistPath);

                        System.Uri uriRelative = uriBasePath.MakeRelativeUri(uriFileAbsolute);

                        resultingFile.Add(Uri.UnescapeDataString(uriRelative.ToString()));


                    }

                }

                if (lastPlaylistChange != null)
                {
                    if (lastPlaylistChange.validPath.iEnd < strOrgPlaylistContent.Length)
                    {
                        // last part
                        resultingFile.Add(strOrgPlaylistContent.Substring(lastPlaylistChange.validPath.iEnd+1, strOrgPlaylistContent.Length - (lastPlaylistChange.validPath.iEnd+1)));
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Internal error: " + ex.Message);

                return false;
            }


            try
            {
                // Write File
                if (resultingFile.Count > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(PlaylistUpdatesEntry.PathTemp, false, Encoding.Default);

                    foreach(string str in resultingFile)
                        writer.Write(str);

                    writer.Close();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error trying to write " + PlaylistUpdatesEntry.PathTemp + " " + ex.Message);
            }


            return false;
        }



        private TaskPlanner.FileListEntry CheckChangesValidPath(ValidPath validPath, List<TaskPlanner.FileListEntry> FileList)
        {

            // go through all Before Actions check for changes of validPath (check with absolute paths)

            foreach (TaskPlanner.FileListEntry fileListEntry in FileList)
            {
                if (fileListEntry.Path == validPath.strValidPath)
                {
                    if (fileListEntry.targetNode.AnyNodeActionThisNode > Tasks.enTasks.CreateSubfolder)
                        return fileListEntry;
                }

            }


            return null;
        }


        private List<ValidPath> GetAllValidPathsFromString(string strPlaylist, string basePath) // string can be any playlist format
        {
            // Boundary condidtions
            // The paths always contain a ".xxx" ending
            // The path can be inside ""(.wpl) or fill up its own line(.m3u) or after a "="(.kpl)
            // The path can be absolute or relative

            List<ValidPath> listValidPaths = new List<ValidPath>();

            // Procedure:
            // 1) advance the string until a "." is found
            for (int i = 0; i < strPlaylist.Length; i++)
            {
                if (strPlaylist[i] == '.')
                {
                    // 2) from there move back and forth until a invalid path char is found
                    char[] invalidChars = System.IO.Path.GetInvalidPathChars();

                    int iStart = i, iEnd = i;
                    if (GetBoundarys(strPlaylist, invalidChars, ref iStart, ref iEnd, false)) // Normally
                    {
                        string fullpath = "";
                        enPathType pathType = enPathType.absolute;
                        if(CheckFile(strPlaylist, iStart, iEnd, basePath, ref fullpath, ref pathType)) // 3) check if path ok
                        {
                            ValidPath validPath = new ValidPath();

                            validPath.strValidPath = fullpath;
                            validPath.iEnd = iEnd;
                            validPath.iStart = iStart;
                            validPath.PathType = pathType;

                            listValidPaths.Add(validPath);
                            i = iEnd + 1;
                            continue;
                        }
                    }

                    iStart = i;
                    iEnd = i;
                    if (GetBoundarys(strPlaylist, invalidChars, ref iStart, ref iEnd, true)) // In case of KPL
                    {
                        string fullpath = "";
                        enPathType pathType = enPathType.absolute;
                        if (CheckFile(strPlaylist, iStart, iEnd, basePath, ref fullpath, ref pathType)) // 3) check if path ok
                        {
                            ValidPath validPath = new ValidPath();

                            validPath.strValidPath = fullpath;
                            validPath.iEnd = iEnd;
                            validPath.iStart = iStart;
                            validPath.PathType = pathType;

                            listValidPaths.Add(validPath);
                            i = iEnd + 1;
                            continue;
                        }
                    }

                }
            }

            return listValidPaths;
        }


        private bool CheckFile(string strPlaylist, int iStart, int iEnd, string basePath, ref string fullPath, ref enPathType PathType)
        {
            // 3) get the path from that boundarys
            string orgPath = strPlaylist.Substring(iStart, iEnd - iStart + 1);
            fullPath = orgPath;

            // 4) check with fileinfo if the path is valid, add to list if valid
            bool bFileExists = false;


            if (!bFileExists) // absolute Paths
            {
                try
                {
                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(orgPath);
                    bFileExists = System.IO.File.Exists(orgPath);

                    PathType = enPathType.absolute;
                }
                catch (Exception ex)
                {
                    // continue
                }

            }

            if (!bFileExists) // M3U style paths - absolute Paths without Root
            {
                try
                {
                    //string basePathDir = System.IO.Path.GetDirectoryName(basePath);
                    fullPath = TaskPlanner.UsefulMethods.PlaylistCombinePaths(basePath, orgPath);
                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(fullPath);
                    bFileExists = System.IO.File.Exists(fullPath);

                    PathType = enPathType.absolute;
                }
                catch (Exception ex2)
                {
                    //continue;
                }
            }

            

            if (!bFileExists) // relative Paths
            {
                try
                {
                    string basePathDir = System.IO.Path.GetDirectoryName(basePath);
                    fullPath = System.IO.Path.Combine(basePathDir, orgPath);
                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(fullPath);
                    bFileExists = System.IO.File.Exists(fullPath);

                    PathType = enPathType.relative;
                }
                catch (Exception ex2)
                {
                    //continue;
                }
            }


            // 5) Path can be added
            return bFileExists;
        }


        private bool GetBoundarys(string strPlaylist, char[] invalidChars, ref int iStart, ref int iEnd, bool bKPLExtension)
        {
            if (!bKPLExtension)
            {
                for (; iStart >= 0; iStart--)
                    if (MatchChars(invalidChars, strPlaylist[iStart]))
                        break;

                if (iStart != -1)
                    iStart++;
                else
                    return false;
            }
            else
            {
                for (; iStart >= 0; iStart--)
                {

                    string substring = strPlaylist.Substring(iStart, iEnd - iStart + 1);

                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"File\d+=");
                    System.Text.RegularExpressions.Match m = regex.Match(substring);

                    if (m.Success == true)
                    {
                        int length = m.Length;
                        iStart += length;
                        break;
                    }



                    if (MatchChars(invalidChars, strPlaylist[iStart]))
                        break;
                }

                if (iStart < 0)
                    iStart = 0;

            }

            for (; iEnd < strPlaylist.Length; iEnd++)
                if (MatchChars(invalidChars, strPlaylist[iEnd]))
                    break;

            if (iEnd != strPlaylist.Length)
                iEnd--;
            else
                return false;

            return true;

        }


        private bool MatchChars(char[] CheckChars, char OrgChar)
        {
            foreach (char CheckChar in CheckChars)
            {
                if (CheckChar == OrgChar)
                    return true;
            }

            return false;
        }


        public void SearchForPlaylists()
        {
            matrixListPlaylists = new List<List<string>>();

            foreach (String path in userOptions.GetAllPlaylistPaths())
            {
                List<string> list = new List<string>();
                matrixListPlaylists.Add(list);

                Filewalker(path, ref list);
            }


            //int breakpoint = 5;
        }



        private List<string> Filewalker(string sDir, ref List<string> filelist)
        {

            try
            {
                foreach (string d in System.IO.Directory.GetDirectories(sDir))
                {
                    this.Filewalker(d, ref filelist);
                }

                foreach (string f in System.IO.Directory.GetFiles(sDir, "*"))
                {
                    if( userOptions.PlaylistFileFilterOptions.CheckIfFileOK( f ) )
                        filelist.Add(f);
                }

                return filelist;
            }
            catch (System.Exception ex)
            {
                //listBox1.Items.Add(ex.Message);

                return null;
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


    } // Playlist Updates














    public class PlayListUpdatesModel : INotifyPropertyChanged
    {
        public PlayListUpdatesModel()
        {
            InitPlaylistUpdates();

            bPlaylistUpdates = true;
            bLeaveCopy = true;
        }



        private bool int_bPlaylistUpdates;
        public bool bPlaylistUpdates
        {
            get
            {
                return int_bPlaylistUpdates;
            }
            set
            {
                int_bPlaylistUpdates = value;

                foreach (PlayListUpdatesViewModel vm in Items)
                {
                    vm.UpdatePlaylist = value;
                }

                OnPropertyChanged("bPlaylistUpdates");
            }
        }

        private bool int_bLeaveCopy;
        public bool bLeaveCopy
        {
            get
            {
                return int_bLeaveCopy;
            }
            set
            {
                int_bLeaveCopy = value;

                foreach (PlayListUpdatesViewModel vm in Items)
                {
                    vm.LeaveCopy = value;
                }

                OnPropertyChanged("bLeaveCopy");
            }
        }


        private ObservableCollection<PlayListUpdatesViewModel> int_Items;
        public ObservableCollection<PlayListUpdatesViewModel> Items
        {
            get
            {
                return int_Items;
            }
            set
            {
                int_Items = value;
                OnPropertyChanged("Items");
            }
        }




        private void InitPlaylistUpdates()
        {
            Items = new ObservableCollection<PlayListUpdatesViewModel>();

            /*
            PlayListUpdatesViewModel vm = new PlayListUpdatesViewModel();

            String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(MainWindow)).CodeBase);
            appdir = appdir.Replace("file:\\", "");


            vm.PathOriginal = appdir + "\\temp\\old.m3u";
            vm.PathTemp = appdir + "\\temp\\new.m3u";
            vm.PathRenameOrg = "-";

            vm.UpdatePlaylist = true;
            vm.LeaveCopy = true;

            Items.Add(vm);
             */

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








    public class PlayListUpdatesViewModel : INotifyPropertyChanged
    {
        public PlayListUpdatesViewModel()
        {
            PlaylistChanges = new List<PlaylistChange>();

            UpdatePlaylist = true;
            LeaveCopy = true;
        }

        // general Data
        private string int_PathOriginal;
        public String PathOriginal
        {
            get
            {
                return int_PathOriginal;
            }
            set
            {
                int_PathOriginal = value;
                OnPropertyChanged("PathOriginal");
                LeaveCopy = LeaveCopy; // To Update Path Rename
            }

        }


        public String PathTemp { get; set; }


        private String int_PathRenameOrg;
        public String PathRenameOrg
        {
            get
            {
                return int_PathRenameOrg;
            }
            set
            {
                int_PathRenameOrg = value;
                OnPropertyChanged("PathRenameOrg");
            }
        }



        private bool int_UpdatePlaylist;
        public bool UpdatePlaylist
        {
            get
            {
                return int_UpdatePlaylist;
            }
            set
            {
                int_UpdatePlaylist = value;
                OnPropertyChanged("UpdatePlaylist");
            }
        }




        private bool int_LeaveCopy;
        public bool LeaveCopy
        {
            get
            {
                return int_LeaveCopy;
            }
            set
            {
                int_LeaveCopy = value;

                if (int_LeaveCopy == true)
                {
                    if (PathOriginal != null)
                    {
                        PathRenameOrg = TaskPlanner.UsefulMethods.AddDateTimeToFilename(PathOriginal);
                    }
                }
                else
                {
                    PathRenameOrg = "";
                }

                OnPropertyChanged("LeaveCopy");
            }
        }

        // changes
        public List<PlaylistChange> PlaylistChanges { get; set; }




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


    public class PlaylistChange
    {
        public TaskPlanner.FileListEntry changingEntry { get; set; } // Action affecting Playlist

        public ValidPath validPath { get; set; } // Original Path in Playlist to be changed

    }

}