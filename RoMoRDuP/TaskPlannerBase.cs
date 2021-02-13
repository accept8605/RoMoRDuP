using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using System.Security.Cryptography;
using System.IO;

using RoMoRDuP.Tasks;
using System.Threading;

using System.Xml;
using System.Security;


namespace RoMoRDuP.TaskPlanner
{

    public enum enLastSelectedView { None, SourceBefore, SourceAfter, TargetBefore, TargetAfter, GenerateNew }

    public static class GlobalVar
    {
        public static uint readByteLimit{ get; set; }

        public static ulong TargetByteSize { get; set; }

        public static enLastSelectedView lastSelectedView { get; set; }

        public static WHourglass wHourglass { get; set; }
    }


    public class FileListEntry
    {
        public FileListEntry(string Path, uint FileSize, ref TaskNodeViewModel targetNode)
        {
            this.Path = Path;

            this.int_SizeToProcess = Math.Min(FileSize, GlobalVar.readByteLimit);

            this.targetNode = targetNode;
        }

        private string int_Path;
        public string Path
        {
            get
            {
                return int_Path;
            }
            set
            {
                int_Path = value;
            }
        }

        uint int_SizeToProcess;
        public uint SizeToProcess 
        {
            get { return int_SizeToProcess; }
        }

        // reference to TaskNodeViewModel from ViewModel
        public TaskNodeViewModel targetNode { get; set; }



        // The hash is created later
        public string hash { get; set; }
    }



    public class TaskPlannerBase
    {
        protected UserInterface.UserInterfaceBase userOptions;

        public TaskViews taskSource;
        public TaskViews taskTarget;

        PlaylistUpdates.PlaylistUpdates playlistUpdates { get; set; }

        Tasks.Tasks tasks;

        public TaskPlannerBase(UserInterface.UserInterfaceBase userOptions, TaskViews taskSource, TaskViews taskTarget, Tasks.Tasks tasks, FileLists fileLists, PlaylistUpdates.PlaylistUpdates playlistUpdates)
        {
            this.userOptions = userOptions;
            this.taskSource = taskSource;
            this.taskTarget = taskTarget;

            this.playlistUpdates = playlistUpdates;

            int_lastSelectedView = enLastSelectedView.GenerateNew;

            this.fileLists = fileLists;

            this.tasks = tasks;
        }

        // for Threads
        Thread threadProcessFileTree;
        Thread threadSourceProcessFileList;
        Thread threadTargetProcessFileList;

        Thread thread_CreateDuplicateInfo;

        Thread thread_EditViews;

        ulong doubProcessCurrentSize;

        // These Filelist are actually used from Tasks-Class!!!
        /*
        protected List<FileListEntry> SourceFileList;  // always before
        protected List<FileListEntry> TargetFileList;   // always before

        protected List<FileListEntry> SourceFileListAfter;
        protected List<FileListEntry> TargetFileListAfter;
         */

        protected FileLists fileLists;

        public int IDAfterSource = 0;
        public int IDAfterTarget = 0;
        public int IDBeforeSource = 0;
        public int IDBeforeTarget = 0;


        // _________________________Methods_______________________

        // _________________________Create File Tree_______________________

        public void CreateFileTree()
        {
            GlobalVar.readByteLimit = (uint)userOptions.intSelectedSizeProcessHash;


            //generate HashCodes and ProcessedDataSize 
            if (threadProcessFileTree == null)
            {
                threadProcessFileTree = new Thread(new ThreadStart(ProcessFileTree));
            }
            else
            {
                threadProcessFileTree.Abort(); // TBD check if Abort valid - only if everything managed

                threadProcessFileTree = new Thread(new ThreadStart(ProcessFileTree));
            }


            threadProcessFileTree.Start();


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       TaskPlanner.GlobalVar.wHourglass = new WHourglass("Creating file tree...");
                       TaskPlanner.GlobalVar.wHourglass.Owner = userOptions.mainWindow;
                       TaskPlanner.GlobalVar.wHourglass.ShowDialog();

                   }
                    ), null);
            }

        }


        // In seperate Thread!
        private void ProcessFileTree()
        {
            IDBeforeSource = 0;    

            GlobalVar.TargetByteSize = 0;
            doubProcessCurrentSize = 0;


            // Create FileInfo

            //____Source Folder
            //tasks.SourceFileListBefore = new List<FileListEntry>();
            //SourceFileList = tasks.SourceFileListBefore;
            fileLists.SourceFileListBefore = new List<FileListEntry>();

            // Filetree
            TaskNodeViewModel ParentNode = Filewalker(userOptions.strSelectSourcePath, userOptions.strSelectSourcePath, taskSource, null, ref IDBeforeSource, 0, fileLists.SourceFileListBefore); // FileWalker creates list of all files
            taskSource.TaskViewBefore.Items = new CustomObservableCollection<TaskNodeViewModel>() { ParentNode };

            // Create TargetDataSize
            foreach (FileListEntry entry in fileLists.SourceFileListBefore)
                GlobalVar.TargetByteSize += entry.SizeToProcess;

            //userOptions.ProcessTargetSize = GlobalVar.TargetByteSize.ToString(); // see below
            // doubProcessCurrentSize = 0; // see up

            //generate HashCodes and ProcessedDataSize 
            if (threadSourceProcessFileList == null)
            {
                threadSourceProcessFileList = new Thread(new ThreadStart(ProcessSourceFileList));
            }
            else
            {
                threadSourceProcessFileList.Abort(); // TBD check if Abort valid - only if everything managed

                threadSourceProcessFileList = new Thread(new ThreadStart(ProcessSourceFileList));

                doubProcessCurrentSize = 0;
            }
            //CreateDuplicateInfo is separate Function!


            if (userOptions is UserInterface.UserOptionsMirror) // TargetFolder only for Mirror
            {
                // _____Target Folder
                //tasks.TargetFileListBefore = new List<FileListEntry>();
                //TargetFileList = tasks.TargetFileListBefore;

                fileLists.TargetFileListBefore = new List<FileListEntry>();

                // Filetree
                IDBeforeTarget = 0;
                ParentNode = Filewalker(userOptions.strSelectTargetPath, userOptions.strSelectTargetPath, taskTarget, null, ref IDBeforeTarget, 0, fileLists.TargetFileListBefore);

                taskTarget.TaskViewBefore.Items = new CustomObservableCollection<TaskNodeViewModel>() { ParentNode };


                // Create TargetDataSize
                foreach (FileListEntry entry in fileLists.TargetFileListBefore)
                    GlobalVar.TargetByteSize += entry.SizeToProcess;

                //doubProcessCurrentSize = 0; // see up

                //generate HashCodes and ProcessedDataSize 
                if (threadTargetProcessFileList == null)
                {
                    threadTargetProcessFileList = new Thread(new ThreadStart(ProcessTargetFileList));
                }
                else
                {
                    threadTargetProcessFileList.Abort(); // TBD check if Abort valid - only if everything managed

                    threadTargetProcessFileList = new Thread(new ThreadStart(ProcessTargetFileList));

                    doubProcessCurrentSize = 0;
                }

                //CreateDuplicateInfo is separate Function!
            }

            userOptions.ProcessTargetSize = GlobalVar.TargetByteSize.ToString();


            // Start Threads
            threadSourceProcessFileList.Start();

            if (userOptions is UserInterface.UserOptionsMirror) // TargetFolder only for Mirror
                threadTargetProcessFileList.Start();




            playlistUpdates.SearchForPlaylists(); // Playlist Updates




            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       if (TaskPlanner.GlobalVar.wHourglass != null)
                           TaskPlanner.GlobalVar.wHourglass.Close();

                   }
                    ), null);
            }
            

        }





        // In separate Thread!!
        ulong ulProcessCurrentSizeSource;
        private void ProcessSourceFileList()
        {
            ulProcessCurrentSizeSource = 0;
            doubProcessCurrentSize = 0;
            //generate HashCodes and ProcessedDataSize 
            foreach (FileListEntry entry in fileLists.SourceFileListBefore)
            {
                if (entry.targetNode.IsFile)
                {
                    entry.hash = GetFileHash(entry.Path);
                    entry.targetNode.HashCode = entry.hash;

                    ulProcessCurrentSizeSource += entry.SizeToProcess;
                    doubProcessCurrentSize = ulProcessCurrentSizeSource + ulProcessCurrentSizeTarget;
                    userOptions.ProcessCurrentSize = doubProcessCurrentSize.ToString();
                }
            }

        }

        // In separate Thread!!
        ulong ulProcessCurrentSizeTarget;
        private void ProcessTargetFileList()
        {
            ulProcessCurrentSizeTarget = 0;
            doubProcessCurrentSize = 0;
            //generate HashCodes and ProcessedDataSize 
            foreach (FileListEntry entry in fileLists.TargetFileListBefore)
            {
                if (entry.targetNode.IsFile)
                {
                    entry.hash = GetFileHash(entry.Path);
                    entry.targetNode.HashCode = entry.hash;

                    ulProcessCurrentSizeTarget += entry.SizeToProcess;
                    doubProcessCurrentSize = ulProcessCurrentSizeSource + ulProcessCurrentSizeTarget;
                    userOptions.ProcessCurrentSize = doubProcessCurrentSize.ToString();
                }
            }

        }

        
        // Achtung: die Actions setzen die spezifischen TaskPlanner, nicht die Basisklasse!!

        private TaskNodeViewModel Filewalker(string basePath, string sDir, TaskViews taskViews, TaskNodeViewModel parentNode, ref int ID, int level, List<FileListEntry> FileList)
        {
            try
            {
                TaskNodeViewModel newParent = EditTaskView(basePath, parentNode, null, TreeAction.AddDir, sDir, sDir, ref ID, FileList, TaskViewType.Before, enTasks.Org, null, false, false);
                newParent.basePath = basePath;

                foreach (string d in System.IO.Directory.GetDirectories(sDir))
                {
                    int newlevel = level + 1;
         
                    // see Above
                    //EditTaskView(basePath, newParent, null, TreeAction.AddDir, d, d, ref ID, FileList, TaskViewType.Before, enTasks.Org, null, false, false);

                    this.Filewalker(basePath, d, taskViews, newParent, ref ID, newlevel, FileList);
                }

                foreach (string f in System.IO.Directory.GetFiles(sDir, "*"))
                {
                    //lstFilesFound.Items.Add(f);

                    EditTaskView(basePath, newParent, null, TreeAction.AddFile, f, f, ref ID, FileList, TaskViewType.Before, enTasks.Org, null, false, false);
                }

                return newParent;
            }
            catch (System.Exception ex)
            {
                //listBox1.Items.Add(ex.Message);

                if (userOptions.mainWindow != null)
                {
                    userOptions.mainWindow.Dispatcher.Invoke(
                       (Action)(() =>
                       {
                           MessageBox.Show("Can´t process file tree: " + ex.Message);
                       }
                        ), null);
                }

                return null;
            }

        }

        static string GetFileHash(string filename)
        {
            //byte[] data = File.ReadAllBytes(filename);
            byte[] data;

            byte[] arrFileSize;

            try
            {
                System.IO.FileInfo fileinfo = new FileInfo(filename);

                long lFileSize = fileinfo.Length;
                arrFileSize = BitConverter.GetBytes(lFileSize);

                data = new byte[GlobalVar.readByteLimit + arrFileSize.Length];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                MessageBox.Show("Out of Memory! " + ex.Message + " Used Memory = " + process.PrivateMemorySize64);
                return "";
            }

            try
            {
                int iBytesRead = 0;

                using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    //reader.BaseStream.Seek(50, SeekOrigin.Begin);
                    iBytesRead = reader.Read(data, 0, (int)GlobalVar.readByteLimit);
                }

                for (int i = 0; i < arrFileSize.Length; i++)
                {
                    data[iBytesRead++] = arrFileSize[i];        // Calculate the FileSize Byte Count into hash for additional security

                }


                byte[] hash = MD5.Create().ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Can´t access file! " + ex.Message);

                return "";
            }
        }


        public void CreateLogFiles(string strFolderPath, string strNameAddition)
        {
            CreateLogFile(taskSource.TaskViewBefore, strFolderPath, strNameAddition + "SourceBefore.tvlog");
            CreateLogFile(taskSource.TaskViewAfter, strFolderPath, strNameAddition + "SourceAfter.tvlog");

            if(userOptions is UserInterface.UserOptionsMirror) // Only for Mirror
            {
                CreateLogFile(taskTarget.TaskViewBefore, strFolderPath, strNameAddition + "TargetBefore.tvlog");
                CreateLogFile(taskTarget.TaskViewAfter, strFolderPath, strNameAddition + "TargetAfter.tvlog");
            }
        }

        private void CreateLogFile(TaskTreeViewModel vm, string strFolderPath, string strNameAddition)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode myRoot;

            myRoot = doc.CreateElement("Root");
            doc.AppendChild(myRoot);

            if(vm.Items.Count>0)
                if(vm.Items[0]!=null)
                    CreateLogFileRecursive(doc, ref myRoot, vm.Items[0], 0);

            string targetDocPath = "";
            try
            {
                targetDocPath = strFolderPath + "\\" + strNameAddition;
                doc.Save(targetDocPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can´t save to " + targetDocPath);
            }

        }


        private void CreateLogFileRecursive(XmlDocument xmlDoc, ref XmlNode xmlParentNode, TaskNodeViewModel tvParentNode, int level)
        {
            try
            {
                foreach (TaskNodeViewModel d in tvParentNode.Children)
                {
                    if (d.IsFolder)
                    {
                        int newlevel = level + 1;

                        XmlNode subParentNode = xmlDoc.CreateElement( XmlConvert.EncodeName(d.Name) );
                        xmlParentNode.AppendChild(subParentNode);

                        this.CreateLogFileRecursive(xmlDoc, ref subParentNode, d, newlevel);
                    }
                }

                foreach (TaskNodeViewModel d in tvParentNode.Children)
                {
                    if (d.IsFile)
                    {
                        XmlNode fileNode = xmlDoc.CreateElement( XmlConvert.EncodeName(d.Name));
                        xmlParentNode.AppendChild(fileNode);

                        AddFileDataToXmlNode(xmlDoc, ref fileNode, d);
                    }
                }

                return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while creating tvLog: " + ex.Message);
                return;
            }

        }

        private void AddFileDataToXmlNode(XmlDocument xmlDoc, ref XmlNode xmlFileNode, TaskNodeViewModel tvFileNode)
        {
            XmlNode dataNode = null;
            XmlAttribute attribute = null;

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task1"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task1));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_1_Original);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task2"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task2));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_2_Copy);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task3"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task3));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_3_Move);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task4"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task4));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_4_Rename);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task5"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task5));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_5_Delete);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task6"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task6));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_CreateShortcutTo) ;
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Task7"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("TaskType"));
            attribute.InnerText = SecurityElement.Escape(Enum.GetName(typeof(enTasks), tvFileNode.task7));
            dataNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Path"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Path_SetAttributes);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);


            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Info"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Content"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.Info);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Hashcode"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Content"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.HashCode);
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);


            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("UpdatedFile"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Content"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.bUpdatedFile.ToString());
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("ManuallyModified"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Content"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.bManuallyModified.ToString());
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

            dataNode = xmlDoc.CreateElement(XmlConvert.EncodeName("Activated"));
            attribute = xmlDoc.CreateAttribute(XmlConvert.EncodeName("Content"));
            attribute.InnerText = SecurityElement.Escape(tvFileNode.bActivated.ToString());
            dataNode.Attributes.Append(attribute);
            xmlFileNode.AppendChild(dataNode);

        }


        // _________________________After View <=> Before View_______________________

        // Copy After View from Before View
        public void CreateAfterViews()
        {
            //____Source Folder
            if (taskSource.TaskViewBefore.Items != null)
            {
                if (taskSource.TaskViewBefore.Items.Count == 1)
                {
                    IDAfterSource = 0;

                    //tasks.SourceFileListAfter = new List<FileListEntry>();
                    //SourceFileListAfter = tasks.SourceFileListAfter;
                    fileLists.SourceFileListAfter = new List<FileListEntry>();


                    // Filetree
                    TaskNodeViewModel ParentNode = CreateAfterViewsFromBeforeList(userOptions.strSelectSourcePath, taskSource.TaskViewBefore.Items[0], null, ref IDAfterSource, 0, fileLists.SourceFileListAfter);
                    taskSource.TaskViewAfter.Items = new CustomObservableCollection<TaskNodeViewModel>() { ParentNode };
                }

                if (userOptions is UserInterface.UserOptionsMirror)
                {
                    //____Target Folder
                    if (taskTarget.TaskViewBefore.Items.Count == 1)
                    {
                        IDAfterTarget = 0;

                        //tasks.TargetFileListAfter = new List<FileListEntry>();
                        //TargetFileListAfter = tasks.TargetFileListAfter;
                        fileLists.TargetFileListAfter = new List<FileListEntry>();


                        // Filetree
                        TaskNodeViewModel ParentNode = CreateAfterViewsFromBeforeList(userOptions.strSelectTargetPath, taskTarget.TaskViewBefore.Items[0], null, ref IDAfterTarget, 0, fileLists.TargetFileListAfter);
                        taskTarget.TaskViewAfter.Items = new CustomObservableCollection<TaskNodeViewModel>() { ParentNode };
                    }
                }
            }
        }

        // Copy After View from Before View
        public TaskNodeViewModel CreateAfterViewsFromBeforeList(string basePath, TaskNodeViewModel orgBeforeViewParentNode, TaskNodeViewModel AfterViewParentNodeToEdit, ref int ID, int level, List<FileListEntry> FileList2Create) // Both After Views being created through 2 method calls
        {
            try
            {
                // 1 Schritt: Copy orginal FileTree without Actions - set path orginal
                /*
                 *separate Method!:
                // 2 Schritt: Edit FileList by Actions of specific Filelist Source/TargetBefore (this method)
                 */
                // TBD Check if file with name exists


                bool FolderTaskActivated = false;
                //string FolderTaskPath = "";
                //enTasks FolderTask = enTasks.Org;

                // Folder Tasks
                if (orgBeforeViewParentNode.task2 == enTasks.CreateSubfolder)
                {
                    // FolderTask
                    FolderTaskActivated = orgBeforeViewParentNode.bActivated;
                    //FolderTaskPath = orgBeforeViewParentNode.Path_2_Copy;     // Done by EditViews!!
                    //FolderTask = orgBeforeViewParentNode.task2;                // Done by EditViews!!
                }   
                else if (orgBeforeViewParentNode.task5 == enTasks.Delete)
                {
                    // FolderTask
                    FolderTaskActivated = orgBeforeViewParentNode.bActivated;
                    //FolderTaskPath = orgBeforeViewParentNode.Path_5_Delete;        // Done by EditViews!!
                    //FolderTask = orgBeforeViewParentNode.task5;                    // Done by EditViews!!
                }

                TaskNodeViewModel newParent2Edit = EditTaskView(basePath, AfterViewParentNodeToEdit, orgBeforeViewParentNode, TreeAction.AddDir, orgBeforeViewParentNode.Path_1_Original, orgBeforeViewParentNode.Path_1_Original, ref ID, FileList2Create, TaskViewType.After, enTasks.Org, null, FolderTaskActivated, true);

                foreach (TaskNodeViewModel d in orgBeforeViewParentNode.Children)
                {
                    if (d.IsFolder)
                    {
                        int newlevel = level + 1;
                        this.CreateAfterViewsFromBeforeList(basePath, d, newParent2Edit, ref ID, newlevel, FileList2Create);
                    }

                }

                foreach (TaskNodeViewModel d in orgBeforeViewParentNode.Children)
                {
                    if (d.IsFile)
                    {
                        TaskNodeViewModel newnode = EditTaskView(basePath, newParent2Edit, d, TreeAction.AddFile, d.Path_1_Original, d.Path_1_Original, ref ID, FileList2Create, TaskViewType.After, enTasks.Org, null, d.bActivated, true); // Check
                    }
                }


                return newParent2Edit;
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }



        // Edit Views
        enLastSelectedView int_lastSelectedView;

        bool int_bPlaylistUpdates;
        //bool int_bDoubleUpdate;
        string int_SaveLogFolderPath;
        bool int_bSaveLogPlaylistUpdates;
        bool int_bSaveLogTV;
        TasksExecution.TaskExecute int_taskExecute;
        UserControls.degUpdateView delegateUpdateView;
        bool int_bUpdateSpaceData;
        UserControls.TaskViewParentType int_taskViewParentType;

        // for Manual-Editing-Mirroring with Referenced Recreation
        bool RecreationReferencedView_Source = false;
        bool RecreationReferencedView_Target = false;


        public bool EditViews(enLastSelectedView lastSelectedView, bool PlaylistUpdates/*, bool DoubleUpdate*/, bool SaveLogPlaylistUpdates,
            string SaveLogFolderPath, bool SaveLogTV, TasksExecution.TaskExecute taskExecute, UserControls.degUpdateView delegateUpdateView, bool bUpdateSpaceData, UserControls.TaskViewParentType taskViewParentType) // Update TaskViews in both ways
        {
            if (thread_EditViews != null)
                if (thread_EditViews.IsAlive)
                    return false;


            int_lastSelectedView = lastSelectedView;
            int_bPlaylistUpdates = PlaylistUpdates;
            //int_bDoubleUpdate = DoubleUpdate;
            int_SaveLogFolderPath = SaveLogFolderPath;
            int_bSaveLogPlaylistUpdates = SaveLogPlaylistUpdates;
            int_bSaveLogTV = SaveLogTV;
            int_taskExecute = taskExecute;
            this.delegateUpdateView = delegateUpdateView;
            int_bUpdateSpaceData = bUpdateSpaceData;
            int_taskViewParentType = taskViewParentType;

            //generate HashCodes and ProcessedDataSize 
            if (thread_EditViews == null)
            {
                thread_EditViews = new Thread(new ThreadStart(int_EditViews));
            }
            else
            {
                thread_EditViews.Abort(); // TBD check if Abort valid - only if everything managed

                thread_EditViews = new Thread(new ThreadStart(int_EditViews));
            }


            thread_EditViews.Start();

            try
            {
                if (userOptions.mainWindow != null)
                {
                    userOptions.mainWindow.Dispatcher.BeginInvoke(  // While the visual Tree is being updated, The Dispatcher is disabled
                       (Action)(() =>
                       {
                           TaskPlanner.GlobalVar.wHourglass = new WHourglass("Editing views...");
                           TaskPlanner.GlobalVar.wHourglass.Owner = userOptions.mainWindow;

                           TaskPlanner.GlobalVar.wHourglass.ShowDialog();

                       }
                        ), null);
                }

            }
            catch (Exception ex) 
            {
                
            }


            return true;
        }


        // seperate Thread
        private void int_EditViews() // Update TaskViews in both ways
        {
            enLastSelectedView lastSelectedView = int_lastSelectedView;

            //////////////////////CheckMultipleSamePaths////////////////
            if ((lastSelectedView == enLastSelectedView.TargetAfter) || (lastSelectedView == enLastSelectedView.SourceAfter) || (lastSelectedView == enLastSelectedView.None))
            {
                // Check same Paths
                if ((taskSource.TaskViewBefore.Items != null) && (taskSource.TaskViewAfter.Items != null))
                {
                    CheckForMultipleSamePaths(taskSource.TaskViewBefore.Items[0], taskSource.TaskViewAfter.Items[0]);
                    if (userOptions is UserInterface.UserOptionsMirror)
                    {
                        CheckForMultipleSamePaths(taskTarget.TaskViewBefore.Items[0], taskTarget.TaskViewAfter.Items[0]);
                    }
                }

            }


            

            ///////////////////////Update Before View from AfterView///////////////////
            if ((lastSelectedView == enLastSelectedView.SourceAfter) || (lastSelectedView == enLastSelectedView.None))
            {
                // Update Parents - now done by NodeMove in childAfter UC
                //taskSource.TaskViewAfter.UpdateParents();

                // Create both Before actions
                // Source
                if (EditActionsBeforeFromAfterView(taskSource.TaskViewBefore.Items[0], fileLists.SourceFileListBefore, fileLists.SourceFileListAfter, TaskViewTarget.Source, TaskViewTarget.Source))
                {
                    // RecreationReferencedView - Referenced From BeforeBaseNode - TargetBefore 
                    RecreationReferencedView_Target = true;

                }
                if (userOptions is UserInterface.UserOptionsMirror)
                {
                    // Target
                    if( EditActionsBeforeFromAfterView(taskTarget.TaskViewBefore.Items[0], fileLists.TargetFileListBefore, fileLists.SourceFileListAfter, TaskViewTarget.Target, TaskViewTarget.Source) )
                    {
                        // RecreationReferencedView - Referenced From BeforeBaseNode - SourceBefore
                        RecreationReferencedView_Source = true;
                    }
                }
            }
            if (userOptions is UserInterface.UserOptionsMirror)
            {
                if ((lastSelectedView == enLastSelectedView.TargetAfter) || (lastSelectedView == enLastSelectedView.None))
                {
                    //taskTarget.TaskViewAfter.UpdateParents();

                    // Create both Before actions
                    if (EditActionsBeforeFromAfterView(taskSource.TaskViewBefore.Items[0], fileLists.SourceFileListBefore, fileLists.TargetFileListAfter, TaskViewTarget.Source, TaskViewTarget.Target))
                    {
                        // RecreationReferencedView - Referenced From BeforeBaseNode - TargetBefore
                        RecreationReferencedView_Target = true;
                    }
                    if ( EditActionsBeforeFromAfterView(taskTarget.TaskViewBefore.Items[0], fileLists.TargetFileListBefore, fileLists.TargetFileListAfter, TaskViewTarget.Target, TaskViewTarget.Target) )
                    {
                        // RecreationReferencedView - Referenced From BeforeBaseNode - SourceBefore
                        RecreationReferencedView_Source = true;
                    }
                }
            }




            ///////////////////Edit After Views/////////////////
            // Always do this, because Mirroring Renames in AfterView
            //if ((lastSelectedView == enLastSelectedView.SourceBefore) || (lastSelectedView == enLastSelectedView.TargetBefore) || (lastSelectedView == enLastSelectedView.None))
            {
                if (lastSelectedView != enLastSelectedView.None)
                    CreateAfterViews(); // Delete existing entrys (bActivated deselected etc.)

                // Create both after Views
                if (taskSource.TaskViewAfter.Items != null)
                {

                    if (userOptions is UserInterface.UserOptionsMirror)
                    {
                        // Create Target
                        if (RecreationReferencedView_Target)
                        {
                            if (taskTarget.TaskViewAfter.Items.Count > 0)
                                EditAfterViewsFromActionsBeforeList(taskTarget.TaskViewBefore.Items[0], taskTarget.TaskViewAfter.Items[0], fileLists.TargetFileListBefore, fileLists.TargetFileListAfter, TaskViewTarget.Target, TaskViewTarget.Target, ref IDAfterTarget);

                            if (taskSource.TaskViewAfter.Items.Count > 0)
                                EditAfterViewsFromActionsBeforeList(taskTarget.TaskViewBefore.Items[0], taskSource.TaskViewAfter.Items[0], fileLists.TargetFileListBefore, fileLists.SourceFileListAfter, TaskViewTarget.Target, TaskViewTarget.Source, ref IDAfterSource);
                        }
                    }


                    // Source
                    if (taskSource.TaskViewAfter.Items.Count > 0)
                        EditAfterViewsFromActionsBeforeList(taskSource.TaskViewBefore.Items[0],taskSource.TaskViewAfter.Items[0],fileLists.SourceFileListBefore, fileLists.SourceFileListAfter, TaskViewTarget.Source, TaskViewTarget.Source, ref IDAfterSource);

                    if (userOptions is UserInterface.UserOptionsMirror)
                    {
                        if (taskTarget.TaskViewAfter.Items.Count > 0)
                            EditAfterViewsFromActionsBeforeList(taskSource.TaskViewBefore.Items[0],taskTarget.TaskViewAfter.Items[0],fileLists.SourceFileListBefore, fileLists.TargetFileListAfter, TaskViewTarget.Source, TaskViewTarget.Target, ref IDAfterTarget);


                        // Recreate Target?
                        if ((RecreationReferencedView_Source == true) || (RecreationReferencedView_Target==false)) // one of those two has to be executed
                        {
                            if (taskTarget.TaskViewAfter.Items.Count > 0)
                                EditAfterViewsFromActionsBeforeList(taskTarget.TaskViewBefore.Items[0], taskTarget.TaskViewAfter.Items[0], fileLists.TargetFileListBefore, fileLists.TargetFileListAfter, TaskViewTarget.Target, TaskViewTarget.Target, ref IDAfterTarget);

                            if (taskSource.TaskViewAfter.Items.Count > 0)
                                EditAfterViewsFromActionsBeforeList(taskTarget.TaskViewBefore.Items[0], taskSource.TaskViewAfter.Items[0], fileLists.TargetFileListBefore, fileLists.SourceFileListAfter, TaskViewTarget.Target, TaskViewTarget.Source, ref IDAfterSource);
                        }
                    }
                }
            }


            


            //////////////////////////Other Tasks//////////////////////
            if (int_bPlaylistUpdates)
            {
                playlistUpdates.ThreadSave_GeneratePlaylistUpdates();
            }

            if (int_bSaveLogPlaylistUpdates)
            {
                playlistUpdates.ThreadSave_SavePlaylists(int_SaveLogFolderPath);
            }

            /*
            if (int_bDoubleUpdate)
            {
                //
                int_lastSelectedView = TaskPlanner.enLastSelectedView.SourceBefore;
                int_bPlaylistUpdates = false;
                int_bDoubleUpdate = false;
                int_bPlaylistUpdates = false;
                int_bSaveLogPlaylistUpdates = false;
                int_taskExecute = null;

                int_EditViews();

                return;
            }
             */

            if (delegateUpdateView != null)
            {
                delegateUpdateView();
            }

            if (int_bUpdateSpaceData)
            {
                userOptions.UpdateTaskViewSpaceData(int_taskViewParentType); 
            }


            if (int_bSaveLogTV)
            {
                // 7: Save TaskView Log
                CreateLogFiles(int_SaveLogFolderPath, Enum.GetName(typeof(UserInterface.UserOptionsType), userOptions.userOptionsType) + "_");
            }

            if (int_taskExecute != null) // Execute Tasks
            {
                int_taskExecute.handleTaskExecute();
            }


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       if (TaskPlanner.GlobalVar.wHourglass != null)
                           TaskPlanner.GlobalVar.wHourglass.Close();

                   }
                    ), null);
            }
        }









        public bool CheckForMultipleSamePaths(TaskNodeViewModel BeforeBaseNode, TaskNodeViewModel AfterBaseNode)
        {
            return RecursiveCheckForMultipleSamePaths(BeforeBaseNode, AfterBaseNode);
        }


        public bool RecursiveCheckForMultipleSamePaths(TaskNodeViewModel BeforeBaseNode, TaskNodeViewModel CUR_AfterBaseNode)
        {
            bool bRet = false;

            if (CUR_AfterBaseNode != null && BeforeBaseNode != null)
            {
                for (int i = 0; i < CUR_AfterBaseNode.Children.Count; i++) // Compare SourceFileList-Files to themselves
                {
                    // 1: Go through each folder in After View
                    if (CUR_AfterBaseNode.Children[i].IsFolder) // Is Folder
                        RecursiveCheckForMultipleSamePaths(BeforeBaseNode, CUR_AfterBaseNode.Children[i]);

                    // 2: Compare every Filename to every Filename in After View
                    if(CUR_AfterBaseNode.Children[i].IsFile)
                        for (int n = i + 1; n < CUR_AfterBaseNode.Children.Count; n++) // Don´t need to do the same comparison twice!
                        {
                            if(CUR_AfterBaseNode.Children[n].IsFile)
                                if (CUR_AfterBaseNode.Children[i].Name == CUR_AfterBaseNode.Children[n].Name)
                                {
                                    if ((CUR_AfterBaseNode.Children[i].task5 == enTasks.Org) && (CUR_AfterBaseNode.Children[n].task5 == enTasks.Org))
                                    {
                                        bRet = true;

                                        if (CUR_AfterBaseNode.Children[i].Path_1_Original.Contains(CUR_AfterBaseNode.Path_1_Original)) // Original File
                                        {
                                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(CUR_AfterBaseNode.Children[n].Name);
                                            String strRet = Path.GetFileNameWithoutExtension(CUR_AfterBaseNode.Children[n].Name);
                                            strRet += "_additionalVersion";
                                            strRet += "_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                                            strRet += "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
                                            strRet += fileInfo.Extension;

                                            CUR_AfterBaseNode.Children[n].Name = strRet;
                                        }
                                        else
                                        {
                                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(CUR_AfterBaseNode.Children[i].Name);
                                            String strRet = Path.GetFileNameWithoutExtension(CUR_AfterBaseNode.Children[i].Name);
                                            strRet += "_additionalVersion";
                                            strRet += "_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                                            strRet += "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
                                            strRet += fileInfo.Extension;

                                            CUR_AfterBaseNode.Children[i].Name = strRet;
                                        }
                                    }
                                }

                        }
                }

            }

            return bRet;
        }


        // Edit After View from before actions
        public void EditAfterViewsFromActionsBeforeList(TaskNodeViewModel BeforeBaseNode, TaskNodeViewModel AfterBaseNode, List<FileListEntry> FileListBefore, List<FileListEntry> FileListAfter, TaskViewTarget FileListBeforeType, TaskViewTarget FileListAfterType, ref int ID) // Both After Views being edited through 2 method calls
        {


            string strSelectPathAfter = null;
            if (FileListAfterType == TaskViewTarget.Source)
                strSelectPathAfter = userOptions.strSelectSourcePath;
            else
                strSelectPathAfter = userOptions.strSelectTargetPath;


            string strSelectPathBefore = null;
            if (FileListBeforeType == TaskViewTarget.Source)
                strSelectPathBefore = userOptions.strSelectSourcePath;
            else
                strSelectPathBefore = userOptions.strSelectTargetPath;




            try
            {
                // 1 Schritt: Copy orginal FileTree without Actions - set path orginal (not this method, seperate method)

                // 2 Schritt: Edit FileList by Actions of specific Filelist Source/Target_Before (this method) (task1-task4)
                foreach (FileListEntry entryBefore in FileListBefore)
                {

                    if (entryBefore.targetNode.bActivated == true)
                    {

                        TaskNodeViewModel entryAfterNode;


                        // TBD Test
                        /*
                        DateTime dtBefore = DateTime.Now;
                        DateTime dtSubBefore1 = DateTime.Now;
                        DateTime dtSubBefore2 = DateTime.Now;
                        DateTime dtSubBefore3 = DateTime.Now;

                        DateTime dtSubAfter1 = DateTime.Now;
                        DateTime dtSubAfter2 = DateTime.Now;
                        DateTime dtSubAfter3 = DateTime.Now;
                        */

                        // If delete for this file: set delete
                        // If rename for this file: Rename + set renamed
                        // can crawl fileListAfter for these

                        // OLD Version
                        /*
                        foreach (FileListEntry entryAfter in FileListAfter)
                         */

                        int buffer_ID = ID;

                        if (userOptions.mainWindow != null)
                        {
                            userOptions.mainWindow.Dispatcher.Invoke(
                               (Action)(() =>
                               {

                                   //dtSubBefore3 = DateTime.Now;
                                   entryAfterNode = UsefulMethods.FindNodeHierarchicalByPath(AfterBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entryBefore.Path, entryBefore.targetNode.basePath), true, entryBefore.targetNode.basePath);

                                   if (entryAfterNode != null)
                                   {
                                       if (entryBefore.Path == entryAfterNode.Path_1_Original)
                                       {
                                           if (entryBefore.targetNode.task5 == enTasks.Delete)
                                           {
                                               entryAfterNode.task5 = enTasks.Delete;
                                               entryAfterNode.Path_5_Delete = entryBefore.targetNode.Path_5_Delete;
                                           }

                                           else if (entryBefore.targetNode.task4 == enTasks.Rename)
                                           {
                                               entryAfterNode.task4 = enTasks.Rename;
                                               entryAfterNode.Path_4_Rename = entryBefore.targetNode.Path_4_Rename;
                                               entryAfterNode.Name = System.IO.Path.GetFileName(entryBefore.targetNode.Path_4_Rename);
                                               //entryAfter.targetNode.bActivated = true;  //not necessary because inheritted from before view
                                           }

                                           // If move: move + set moved; maybe create subfolders first
                                           // need movePathInTree//AddPathInTree method for this one?
                                           else if (entryBefore.targetNode.task3 == enTasks.Move)
                                           {

                                               if (
                                                       UsefulMethods.FilePathInBasePath(entryBefore.targetNode.Path_3_Move, strSelectPathAfter)//entryBefore.targetNode.Path_3_Move.Contains(strSelectPathAfter)
                                                   )
                                               {
                                                   // old Version
                                                   /*
                                                   foreach (FileListEntry entryAfter in FileListAfter)
                                                       if (UsefulMethods.GetPathWithoutSourceFolder(entryBefore.Path, strSelectPathBefore) == UsefulMethods.GetPathWithoutSourceFolder(entryAfter.Path, strSelectPathAfter) )
                                                       {
                                                       */


                                                   AddPathInTree(entryAfterNode, AfterBaseNode, FileListAfter, entryBefore.targetNode.Path_3_Move, entryBefore.targetNode.Path_1_Original, ref buffer_ID, TaskViewType.After, FileListAfterType, enTasks.Move, entryBefore.targetNode.Path_3_Move, entryBefore.targetNode.bActivated);

                                                   entryAfterNode.RemoveSelf();

                                               }

                                           }

                                       }
                                   }

                                   //dtSubAfter3 = DateTime.Now;



                                   // If Copy: Check copy target, if Parent ok create entry + set copied, maybe create subfolders first
                                   // need AddPathInTree method for this one?
                                   if (entryBefore.targetNode.task2 == enTasks.Copy || entryBefore.targetNode.task2 == enTasks.CreateSubfolder)
                                   {
                                       if (
                                               UsefulMethods.FilePathInBasePath(entryBefore.targetNode.Path_2_Copy, strSelectPathAfter)//entryBefore.targetNode.Path_2_Copy.Contains(strSelectPathAfter)
                                           )
                                       {

                                           if (entryBefore.targetNode.task4 == enTasks.Rename)
                                           {
                                               string bufPath = entryBefore.targetNode.Path_2_Copy;
                                               bufPath = System.IO.Directory.GetParent(bufPath).FullName;
                                               System.IO.FileInfo fileInfo = new FileInfo(entryBefore.targetNode.Path_4_Rename);
                                               bufPath += "\\" + fileInfo.Name;
                                               entryBefore.targetNode.Path_2_Copy = bufPath;
                                           }

                                           if (entryBefore.targetNode.task3 == enTasks.Move)
                                           {
                                               string bufPath = UsefulMethods.GetPathWithoutSourceFolder(entryBefore.targetNode.Path_3_Move, entryBefore.targetNode.basePath); // Move Path is always in its same Base Path

                                               entryBefore.targetNode.Path_2_Copy = UsefulMethods.CombinePathWithSourceFolder(bufPath, entryBefore.targetNode.Path_CopyOrSetAttributesBase);
                                           }


                                           //dtSubBefore1 = DateTime.Now;
                                           // Copy can set bActivated!! (because not inheritted from BeforeView)
                                           AddPathInTree(entryBefore.targetNode, AfterBaseNode, FileListAfter, entryBefore.targetNode.Path_2_Copy, entryBefore.targetNode.Path_1_Original, ref buffer_ID, TaskViewType.After, FileListAfterType, entryBefore.targetNode.task2, entryBefore.targetNode.Path_2_Copy, entryBefore.targetNode.bActivated);
                                           //dtSubAfter1 = DateTime.Now;
                                       }

                                   }

                                   if (entryBefore.targetNode.task7 == enTasks.SetAttributes)
                                   {
                                       string newSetAttributesPath = entryBefore.targetNode.Path_SetAttributes;

                                       // has to be done on mirroring
                                       if (entryBefore.targetNode.task4 == enTasks.Rename)
                                       {
                                           newSetAttributesPath = System.IO.Directory.GetParent(newSetAttributesPath).FullName;
                                           System.IO.FileInfo fileInfo = new FileInfo(entryBefore.targetNode.Path_4_Rename);
                                           newSetAttributesPath += "\\" + fileInfo.Name;
                                       }

                                       if (entryBefore.targetNode.task3 == enTasks.Move)
                                       {
                                           newSetAttributesPath = UsefulMethods.GetPathWithoutSourceFolder(entryBefore.targetNode.Path_3_Move, entryBefore.targetNode.basePath); // Move Path is always in its same Base Path

                                           newSetAttributesPath = UsefulMethods.CombinePathWithSourceFolder(newSetAttributesPath, entryBefore.targetNode.Path_CopyOrSetAttributesBase);
                                       }

                                       // In Case of ReferencedSourceNode Mirroring the afternodes of ReferencedNodes have to be created instantly
                                       entryBefore.targetNode.Path_SetAttributes = newSetAttributesPath;

                                       entryAfterNode = UsefulMethods.FindNodeHierarchicalByPath(AfterBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entryBefore.targetNode.Path_SetAttributes, entryBefore.targetNode.Path_CopyOrSetAttributesBase), true, entryBefore.targetNode.Path_CopyOrSetAttributesBase);

                                       //dtSubBefore2 = DateTime.Now;
                                       if (entryAfterNode != null)
                                       //if (entryBefore.targetNode.Path_SetAttributes == entryAfterNode.Path_1_Original)
                                       {
                                           entryBefore.targetNode.Path_SetAttributes = newSetAttributesPath; // UpdatePath

                                           entryAfterNode.task7 = enTasks.SetAttributes;
                                           entryAfterNode.Path_SetAttributes = entryBefore.targetNode.Path_SetAttributes;
                                           entryAfterNode.Path_SetCopyOrAttributesSource = entryBefore.targetNode.Path_SetCopyOrAttributesSource;
                                           entryAfterNode.Path_SetAttributesSourceBase = entryBefore.targetNode.Path_SetAttributesSourceBase;
                                           entryAfterNode.Path_CopyOrSetAttributesBase = entryBefore.targetNode.Path_CopyOrSetAttributesBase;
                                           entryAfterNode.SetFileAttributes = entryBefore.targetNode.SetFileAttributes;

                                           // SetAttributes can set bActivated!! (because not inheritted from BeforeView)
                                           entryAfterNode.bActivated = entryBefore.targetNode.bActivated;
                                       }
                                       //dtSubAfter2 = DateTime.Now;


                                   }



                               }), null);

                            ID = buffer_ID;

                        }

                        // TBD Test
                        /*
                        DateTime dtAfter = DateTime.Now;

                        System.TimeSpan dtResult = dtAfter - dtBefore;

                        System.TimeSpan dtResultSub1 = dtSubAfter1 - dtSubBefore1;
                        System.TimeSpan dtResultSub2 = dtSubAfter2 - dtSubBefore2;
                        System.TimeSpan dtResultSub3 = dtSubAfter3 - dtSubBefore3;

                        short breakpoint = 5;
                        */
                    }

                } // -bActivated

                return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("EditAfterViewsFromActionsBeforeList: " + ex.Message);

                return;
            }

        }

        //public enum enTasks { None, CreateSubfolder, Copy, Rename, Move, Delete, Overflow } // Order of biggestAction
        /*
            Path_1_Original = "some path";  // Create Subfolder without Move/Copy (not required for mirroring)/ Delete
            Path_2_Copy = "some path";      // Copy // CreateSubfolder for Copy
            Path_3_Move = "some path" ;     // Move // Create Subfolder for Move
            Path_4_Rename = "some path" ;   // Rename
         */

        public bool EditActionsBeforeFromAfterView(TaskNodeViewModel BeforeBaseNode, List<FileListEntry> FileListBeforeToEdit, List<FileListEntry> FileListAfter2Use, TaskViewTarget FileListBeforeType, TaskViewTarget FileListAfterType) // Both Before Actions being edited through 2 Method calls
        {
            bool bRet_RecreationReferencedView = false;

            string strSelectPathAfter = null;
            if (FileListAfterType == TaskViewTarget.Source)
                strSelectPathAfter = userOptions.strSelectSourcePath;
            else
                strSelectPathAfter = userOptions.strSelectTargetPath;


            string strSelectPathBefore = null;
            if (FileListBeforeType == TaskViewTarget.Source)
                strSelectPathBefore = userOptions.strSelectSourcePath;
            else
                strSelectPathBefore = userOptions.strSelectTargetPath;



            if ((FileListAfter2Use != null) && (FileListBeforeToEdit != null))
                foreach (FileListEntry entryAfter in FileListAfter2Use)
                {
                    ////////////////////////////////// normale Tasks /////////////////////
                    if (!((entryAfter.targetNode.task2 == enTasks.Copy) || (entryAfter.targetNode.task7 == enTasks.SetAttributes)))
                    {
                        TaskNodeViewModel entryBeforeNode = UsefulMethods.FindNodeHierarchicalByPath(BeforeBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entryAfter.targetNode.Path_1_Original, entryAfter.targetNode.basePath), true, entryAfter.targetNode.basePath);

                        // old version - slow
                        /*
                        foreach (FileListEntry entryBefore in FileListBeforeToEdit)
                        {
                            if (entryBefore.Path == entryAfter.targetNode.Path_1_Original)
                         */

                        if (entryBeforeNode != null)
                        {
                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       // ________Check activated
                                       if (entryAfter.targetNode.AnyNodeActionThisNode != enTasks.Org)
                                           entryBeforeNode.bActivated = entryAfter.targetNode.bActivated; // Set Activated, but only if Action (for example if TargetAfter Action, but no SourceAfter action -> Dont deactivate, if not really deactivated by user


                                       // Step1: GetPathFrom Node, compare to Path_1_Original
                                       string pathFromAfterNode = entryAfter.targetNode.GetPathFromNode();
                                       if (pathFromAfterNode != UsefulMethods.GetPathWithoutSourceFolder(entryAfter.Path, strSelectPathAfter)/*entryAfter.Path.Replace(strSelectPathAfter, "")*/)
                                       {
                                           // Step 2: If different, browse FileListbefore for Path_1_Original, check rename/move action, add if neccessary
                                           // ________Edit Actions

                                           List<string> strFolderAndFileNamesAfter = UsefulMethods.GetFolderAndFileNames(UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), strSelectPathAfter), strSelectPathAfter, true);
                                           List<string> strFolderAndFileNamesBefore = UsefulMethods.GetFolderAndFileNames(entryBeforeNode.Path_1_Original, strSelectPathBefore, true);

                                           if ((strFolderAndFileNamesAfter != null) && (strFolderAndFileNamesBefore != null))
                                           // Updated Files nicht auf BeforeView zurückführen da Copy+Rename actions gleichzeitig möglich
                                               if (entryBeforeNode.HashCode == entryAfter.targetNode.HashCode)
                                               {
                                                   entryBeforeNode.bManuallyModified = true;
                                                   entryAfter.targetNode.bManuallyModified = true;

                                                   /*
                                                   // Check if Copied - see below
                                                   if (entryAfter.targetNode.task2 == enTasks.Copy) // should already be activated
                                                   {
                                                       entryBeforeNode.Path_2_Copy = UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), strSelectPathAfter);

                                                   }
                                                    */
                                                   // Moved or Renamed
                                                   {
                                                       bool hasMoved = false;
                                                       // Check Rename/Move Actions
                                                       if (strFolderAndFileNamesAfter.Count == strFolderAndFileNamesBefore.Count)
                                                       {
                                                           for (int i = 0; i < strFolderAndFileNamesBefore.Count - 1; i++)
                                                               if (strFolderAndFileNamesBefore[i] != strFolderAndFileNamesAfter[i])
                                                               {
                                                                   hasMoved = true;
                                                                   break;
                                                               }
                                                       }
                                                       else
                                                           hasMoved = true;

                                                       if (hasMoved)
                                                       {
                                                           if (entryAfter.targetNode.task3 != enTasks.Move) // wenn neuer Task activated setzen
                                                               entryBeforeNode.bActivated = true;

                                                           // Move action
                                                           entryBeforeNode.task3 = enTasks.Move;
                                                           entryBeforeNode.Path_3_Move = UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), strSelectPathAfter);

                                                           // Move Action in Other Folder
                                                           if (entryBeforeNode.ReferencedOtherFolderNode != null)
                                                           {
                                                               if (entryBeforeNode.ReferencedOtherFolderNode.task3 != enTasks.Move) // wenn neuer Task activated setzen
                                                                   entryBeforeNode.ReferencedOtherFolderNode.bActivated = true;

                                                               entryBeforeNode.ReferencedOtherFolderNode.task3 = enTasks.Move;
                                                               entryBeforeNode.ReferencedOtherFolderNode.Path_3_Move = UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), entryBeforeNode.ReferencedOtherFolderNode.basePath);

                                                               // Create Its afterviewNode Right away because afterview note might not be found else

                                                               // _________Return Info for Recreation OtherView_________
                                                               bRet_RecreationReferencedView = true;

                                                           }
                                                       }
                                                       else
                                                       {
                                                           if (entryAfter.targetNode.task4 != enTasks.Rename) // wenn neuer Task activated setzen
                                                               entryBeforeNode.bActivated = true;

                                                           // Rename Action
                                                           entryBeforeNode.task4 = enTasks.Rename;
                                                           entryBeforeNode.Path_4_Rename = /*strSelectPathBefore + entryAfter.targetNode.GetPathFromNode()*/ UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), strSelectPathBefore);

                                                           // Rename Action in Other Folder
                                                           if (entryBeforeNode.ReferencedOtherFolderNode != null)
                                                           {
                                                               if (entryBeforeNode.ReferencedOtherFolderNode.task4 != enTasks.Rename) // wenn neuer Task activated setzen
                                                                   entryBeforeNode.ReferencedOtherFolderNode.bActivated = true;

                                                               entryBeforeNode.ReferencedOtherFolderNode.task4 = enTasks.Rename;

                                                               string bufPath = System.IO.Directory.GetParent(entryBeforeNode.ReferencedOtherFolderNode.Path_1_Original).FullName;
                                                               System.IO.FileInfo fileInfo = new FileInfo(entryBeforeNode.Path_4_Rename);
                                                               bufPath += "\\" + fileInfo.Name;
                                                               entryBeforeNode.ReferencedOtherFolderNode.Path_4_Rename = bufPath;

                                                               // Create Its afterviewNode Right away because afterview note might not be found else
                                                               // _________Return Info for Recreation OtherView_________
                                                               bRet_RecreationReferencedView = true;
                                                           }

                                                       }
                                                   }

                                               }

                                       } // // Step1: GetPathFrom Node, compare to Path_1_Original
                                   }
                                    ), null);
                            } // Main Window Dispatcher


                        }

                    }


                    ////////////////////////////////// spezielle Tasks /////////////////////
                    if ((entryAfter.targetNode.task7 == enTasks.SetAttributes) || (entryAfter.targetNode.task2 == enTasks.Copy))
                    {
                        if (entryAfter.targetNode.Path_SetAttributesSourceBase != null)
                        {
                            TaskNodeViewModel entryBeforeNode = UsefulMethods.FindNodeHierarchicalByPath(BeforeBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entryAfter.targetNode.Path_SetCopyOrAttributesSource, entryAfter.targetNode.Path_SetAttributesSourceBase), true, entryAfter.targetNode.Path_SetAttributesSourceBase);

                            if (entryBeforeNode != null)    // SetAttributes
                            {
                                if (userOptions.mainWindow != null)
                                {
                                    userOptions.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           if (entryAfter.targetNode.task2 == enTasks.Copy) // should already be activated
                                           {
                                               entryBeforeNode.Path_2_Copy = UsefulMethods.CombinePathWithSourceFolder(entryAfter.targetNode.GetPathFromNode(), strSelectPathAfter);

                                               if (entryAfter.targetNode.task7 == enTasks.SetAttributes)
                                               {
                                                   entryBeforeNode.Path_SetAttributes = entryBeforeNode.Path_2_Copy;   // does not work for mirrored Manual editing actions
                                               }

                                               entryBeforeNode.bActivated = entryAfter.targetNode.bActivated;
                                           }
                                           // ________Check activated
                                           if (entryAfter.targetNode.task7 == enTasks.SetAttributes)
                                           {
                                               entryBeforeNode.bActivated = entryAfter.targetNode.bActivated; // Set Activated, but only if Action (for example if TargetAfter Action, but no SourceAfter action -> Dont deactivate, if not really deactivated by user
                                           }

                                       }), null);
                                }

                            }
                        }
                    }


                }

            return bRet_RecreationReferencedView;
        }





        // _________________________Duplicate Info_______________________


        public void CreateDuplicateInfo()
        {
            

            //generate HashCodes and ProcessedDataSize 
            if (thread_CreateDuplicateInfo == null)
            {
                thread_CreateDuplicateInfo = new Thread(new ThreadStart(int_CreateDuplicateInfo));
            }
            else
            {
                thread_CreateDuplicateInfo.Abort(); // TBD check if Abort valid - only if everything managed

                thread_CreateDuplicateInfo = new Thread(new ThreadStart(int_CreateDuplicateInfo));
            }


            thread_CreateDuplicateInfo.Start();


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       TaskPlanner.GlobalVar.wHourglass = new WHourglass("Creating duplicate info...");
                       TaskPlanner.GlobalVar.wHourglass.Owner = userOptions.mainWindow;

                       TaskPlanner.GlobalVar.wHourglass.ShowDialog();

                   }
                    ), null);
            }
        }


        protected virtual void Create_RD_Duplicate_Groups() 
        {
            // virtual
        }

        // separate Thread
        private void int_CreateDuplicateInfo()
        {
            // Create list of all duplicates out of list of all files

            CreateDuplicateInfoSourceSource();
            CreateDuplicateInfoSourceTarget();

            if (this is TaskPlannerRemoveDuplicates)
            {
                Create_RD_Duplicate_Groups();
            }

            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       if (TaskPlanner.GlobalVar.wHourglass != null)
                           TaskPlanner.GlobalVar.wHourglass.Close();
                   }
                    ), null);
            }
        }

        private void CreateDuplicateInfoSourceSource()
        {
            // Delete existing Duplicate Info
            foreach (FileListEntry entry in fileLists.SourceFileListBefore)
            {
                if (userOptions.mainWindow != null)
                {
                    userOptions.mainWindow.Dispatcher.Invoke(
                       (Action)(() =>
                       {
                           entry.targetNode.DuplicatesSourceSourceOrTT = new List<string>();
                       }
                        ), null);
                }

            }

            //////////////////// OLD Version ///////////////
            /*
            // Source Folder
            for (int i = 0; i < fileLists.SourceFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
            {
                for (int n = i + 1; n < fileLists.SourceFileListBefore.Count; n++) // Don´t need to do the same comparison twice!
                {
                    if (fileLists.SourceFileListBefore[i].targetNode.IsFile && fileLists.SourceFileListBefore[n].targetNode.IsFile)
                        if (fileLists.SourceFileListBefore[i].hash == fileLists.SourceFileListBefore[n].hash)
                        {
                            if (userOptions.mainWindow != null)
                            {
                                // do something
                            }
                        }

                }
            }
             */

            // Create Lookup Source
            Dictionary<string, List<FileListEntry>> lookupSource = new Dictionary<string,List<FileListEntry>>();

            for (int i = 0; i < fileLists.SourceFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
            {
                if (fileLists.SourceFileListBefore[i].targetNode.IsFile)
                {
                    if (lookupSource.ContainsKey(fileLists.SourceFileListBefore[i].hash))
                    {
                        FileListEntry entry = fileLists.SourceFileListBefore[i];
                        lookupSource[fileLists.SourceFileListBefore[i].hash].Add(entry);
                    }
                    else
                    {
                        List<FileListEntry> list = new List<FileListEntry>();
                        list.Add(fileLists.SourceFileListBefore[i]);
                        lookupSource.Add(fileLists.SourceFileListBefore[i].hash, list);
                    }
                }
            }
            

            // Source Folder
            for (int i = 0; i < fileLists.SourceFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
            {
                /*
                for (int n = i + 1; n < fileLists.SourceFileListBefore.Count; n++) // Don´t need to do the same comparison twice!
                {
                    if (fileLists.SourceFileListBefore[i].targetNode.IsFile && fileLists.SourceFileListBefore[n].targetNode.IsFile)
                        if (fileLists.SourceFileListBefore[i].hash == fileLists.SourceFileListBefore[n].hash)
                        {
                 */

                if (fileLists.SourceFileListBefore[i].targetNode.IsFile)
                {
                    if (lookupSource[fileLists.SourceFileListBefore[i].hash] != null)
                        if (lookupSource[fileLists.SourceFileListBefore[i].hash].Count > 1)
                        {

                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       //fileLists.SourceFileListBefore[i].targetNode.DuplicatesSourceSourceOrTT.Add(fileLists.SourceFileListBefore[n].Path);
                                       //fileLists.SourceFileListBefore[n].targetNode.DuplicatesSourceSourceOrTT.Add(fileLists.SourceFileListBefore[i].Path);
                                       if (lookupSource[fileLists.SourceFileListBefore[i].hash][0] == fileLists.SourceFileListBefore[i])
                                           lookupSource[fileLists.SourceFileListBefore[i].hash][0].targetNode.DuplicatesSourceSourceOrTT.Add(lookupSource[fileLists.SourceFileListBefore[i].hash][1].Path);
                                       else
                                           lookupSource[fileLists.SourceFileListBefore[i].hash][1].targetNode.DuplicatesSourceSourceOrTT.Add(lookupSource[fileLists.SourceFileListBefore[i].hash][0].Path);

                                       // Add Nodes
                                       if (lookupSource[fileLists.SourceFileListBefore[i].hash][0] == fileLists.SourceFileListBefore[i])
                                           lookupSource[fileLists.SourceFileListBefore[i].hash][0].targetNode.DuplicatesSourceSourceOrTTNodes.Add(lookupSource[fileLists.SourceFileListBefore[i].hash][1].targetNode);
                                       else
                                           lookupSource[fileLists.SourceFileListBefore[i].hash][1].targetNode.DuplicatesSourceSourceOrTTNodes.Add(lookupSource[fileLists.SourceFileListBefore[i].hash][0].targetNode);

                                   }
                                    ), null);
                            }

                        }
                }
                        

                
            }



            if (userOptions is UserInterface.UserOptionsMirror)
            {
                // Delete existing Duplicate Info
                foreach (FileListEntry entry in fileLists.TargetFileListBefore)
                {

                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               entry.targetNode.DuplicatesSourceSourceOrTT = new List<string>();
                           }
                            ), null);
                    }
                }


  
                // Create Lookup Target
                Dictionary<string, List<FileListEntry>> lookupTarget = new Dictionary<string, List<FileListEntry>>();

                for (int i = 0; i < fileLists.TargetFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
                {
                    if (fileLists.TargetFileListBefore[i].targetNode.IsFile)
                    {
                        if (lookupTarget.ContainsKey(fileLists.TargetFileListBefore[i].hash))
                        {
                            FileListEntry entry = fileLists.TargetFileListBefore[i];
                            lookupTarget[fileLists.TargetFileListBefore[i].hash].Add(entry);
                        }
                        else
                        {
                            List<FileListEntry> list = new List<FileListEntry>();
                            list.Add(fileLists.TargetFileListBefore[i]);
                            lookupTarget.Add(fileLists.TargetFileListBefore[i].hash, list);
                        }
                    }
                }




                for (int i = 0; i < fileLists.TargetFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
                {

                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               // Delete existing Duplicate Info
                               fileLists.TargetFileListBefore[i].targetNode.DuplicatesSourceSourceOrTT = new List<string>();

                           }
                            ), null);
                    }



                    /*
                    for (int n = i + 1; n < fileLists.TargetFileListBefore.Count; n++) // Don´t need to do the same comparison twice!
                    {
                        if (fileLists.TargetFileListBefore[i].targetNode.IsFile && fileLists.TargetFileListBefore[n].targetNode.IsFile)
                            if (fileLists.TargetFileListBefore[i].hash == fileLists.TargetFileListBefore[n].hash)
                            {
                     */

                    if (fileLists.TargetFileListBefore[i].targetNode.IsFile)
                    {
                        if (lookupTarget[fileLists.TargetFileListBefore[i].hash] != null)
                            if (lookupTarget[fileLists.TargetFileListBefore[i].hash].Count > 1)
                            {

                                if (userOptions.mainWindow != null)
                                {
                                    userOptions.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           //fileLists.TargetFileListBefore[i].targetNode.DuplicatesSourceSourceOrTT.Add(fileLists.TargetFileListBefore[n].Path);
                                           //fileLists.TargetFileListBefore[n].targetNode.DuplicatesSourceSourceOrTT.Add(fileLists.TargetFileListBefore[i].Path);


                                           if (lookupTarget[fileLists.TargetFileListBefore[i].hash][0] == fileLists.TargetFileListBefore[i])
                                               lookupTarget[fileLists.TargetFileListBefore[i].hash][0].targetNode.DuplicatesSourceSourceOrTT.Add(lookupTarget[fileLists.TargetFileListBefore[i].hash][1].Path);
                                           else
                                               lookupTarget[fileLists.TargetFileListBefore[i].hash][1].targetNode.DuplicatesSourceSourceOrTT.Add(lookupTarget[fileLists.TargetFileListBefore[i].hash][0].Path);

                                           // Add Nodes
                                           if (lookupTarget[fileLists.TargetFileListBefore[i].hash][0] == fileLists.TargetFileListBefore[i])
                                               lookupTarget[fileLists.TargetFileListBefore[i].hash][0].targetNode.DuplicatesSourceSourceOrTTNodes.Add(lookupTarget[fileLists.TargetFileListBefore[i].hash][1].targetNode);
                                           else
                                               lookupTarget[fileLists.TargetFileListBefore[i].hash][1].targetNode.DuplicatesSourceSourceOrTTNodes.Add(lookupTarget[fileLists.TargetFileListBefore[i].hash][0].targetNode);

                                       }
                                        ), null);
                                }
                            }
                    }
                }
            }

        }




        private void CreateDuplicateInfoSourceTarget()
        {
            if (userOptions is UserInterface.UserOptionsMirror)
            {
                // Delete existing Duplicate Info
                foreach (FileListEntry entry in fileLists.SourceFileListBefore)
                {
                    
                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               entry.targetNode.DuplicatesSourceTarget = new List<string>();

                           }
                            ), null);
                    }
                }

                foreach (FileListEntry entry in fileLists.TargetFileListBefore)
                {
                    
                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               entry.targetNode.DuplicatesSourceTarget = new List<string>();

                           }
                            ), null);
                    }
                }



                // Create Lookup Target
                Dictionary<string, List<FileListEntry>> lookupTarget = new Dictionary<string, List<FileListEntry>>();

                for (int i = 0; i < fileLists.TargetFileListBefore.Count; i++) // Compare SourceFileList-Files to themselves
                {
                    if (fileLists.TargetFileListBefore[i].targetNode.IsFile)
                    {
                        if (lookupTarget.ContainsKey(fileLists.TargetFileListBefore[i].hash))
                        {
                            FileListEntry entry = fileLists.TargetFileListBefore[i];
                            lookupTarget[fileLists.TargetFileListBefore[i].hash].Add(entry);
                        }
                        else
                        {
                            List<FileListEntry> list = new List<FileListEntry>();
                            list.Add(fileLists.TargetFileListBefore[i]);
                            lookupTarget.Add(fileLists.TargetFileListBefore[i].hash, list);
                        }
                    }
                }



                for (int i = 0; i < fileLists.SourceFileListBefore.Count; i++) // Compare SourceFileList-Files to TargetFileList-Files
                {
                    //for (int n = 0; n < fileLists.TargetFileListBefore.Count; n++)
                    //{

                    if (fileLists.SourceFileListBefore[i].targetNode.IsFile)
                    {
                        if (lookupTarget.ContainsKey(fileLists.SourceFileListBefore[i].hash))
                        {
                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       //fileLists.SourceFileListBefore[i].targetNode.DuplicatesSourceTarget.Add(fileLists.TargetFileListBefore[n].Path);
                                       //fileLists.TargetFileListBefore[n].targetNode.DuplicatesSourceTarget.Add(fileLists.SourceFileListBefore[i].Path);

                                       if (lookupTarget[fileLists.SourceFileListBefore[i].hash] != null)
                                           foreach (FileListEntry entryTarget in lookupTarget[fileLists.SourceFileListBefore[i].hash])
                                           {
                                               // Add Target To Source
                                               fileLists.SourceFileListBefore[i].targetNode.DuplicatesSourceTarget.Add(entryTarget.Path);
                                               fileLists.SourceFileListBefore[i].targetNode.DuplicatesSourceTargetNodes.Add(entryTarget.targetNode);


                                               //Add Source To Target
                                               entryTarget.targetNode.DuplicatesSourceTarget.Add(fileLists.SourceFileListBefore[i].Path);
                                               entryTarget.targetNode.DuplicatesSourceTargetNodes.Add(fileLists.SourceFileListBefore[i].targetNode);
                                           }


                                   }
                                    ), null);
                            }

                        }
                    }
                }
            }

        }





        // _________________________Edit Views/Actions_______________________

        public enum TreeAction {AddFile, AddDir}

        private TaskNodeViewModel EditTaskView(string basePath, TaskNodeViewModel parentNode, TaskNodeViewModel referencedSourceNode, TreeAction treeAction, string path, string OriginPath, ref int ID, List<FileListEntry> FileList, TaskViewType taskViewType, enTasks TargetNodeTask, string TaskPath, bool Activated, bool CreateAfterView)
        {
            try
            {

                if (treeAction == TreeAction.AddDir)
                {
                    TaskNodeViewModel subParent;

                    /* // Directories werden nicht mehr referenziert, da u.U. eine Datei als Ordnername hinzugefügt wird
                    if (referencedSourceNode != null)
                    {
                        subParent = referencedSourceNode.CloneForAfterView(parentNode, ID++);
                        //subParent.otherView = referencedSourceNode;
                        //referencedSourceNode.otherView = subParent;
                    }
                    else
                     */
                    subParent = new TaskNodeViewModel(parentNode, ID++, taskViewType, basePath, new cAttributes(false, false, Directory.GetCreationTime(path))) { Name = Path.GetFileName(path), Path_1_Original = path };

                    // since its a new dir, add Observable collection immediately
                    subParent.Children = new CustomObservableCollection<TaskNodeViewModel>();

                    // Add subParent(Dir) to parentNode(Dir)

                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               if (parentNode != null) // If not first node itself
                               {
                                   if (parentNode.Children == null)
                                       parentNode.Children = new CustomObservableCollection<TaskNodeViewModel>();

                                   parentNode.Children.Add(subParent);

                                   UpdateTargetNodeActions(subParent, TargetNodeTask, TaskPath, OriginPath, Activated);

                                   // Update List of all Files - CHANGED 15-02-15 TBD CHECK
                                   if (taskViewType == TaskViewType.Before)
                                       FileList.Add(new FileListEntry(path, 0, ref subParent));
                                   else
                                       FileList.Add(new FileListEntry(path, 0, ref subParent)); // FileLength is only relevant for before View
                               }

                           }
                            ), null);
                    }


                    return subParent;

                }


                else if (treeAction == TreeAction.AddFile)  // CreateFile Node
                {

                    int bufferID = ID;

                    TaskNodeViewModel newNode = null;

                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {

                               try
                               {

                                   // Add File to parentNode (Dir)
                                   if (parentNode != null) // If not first node itself, cant add files to first node
                                   {
                                       if (parentNode.Children == null)
                                           parentNode.Children = new CustomObservableCollection<TaskNodeViewModel>();

                                       if (referencedSourceNode != null)
                                       {
                                           if ((TargetNodeTask == enTasks.Copy) && (CreateAfterView == false))
                                           {
                                               newNode = referencedSourceNode.CloneFor_CopyTask(parentNode, bufferID++);
                                               // neuer Base-Path
                                               newNode.basePath = basePath;
                                           }
                                           else
                                               newNode = referencedSourceNode.CloneFor_AfterView(parentNode, bufferID++);

                                           //newNode.otherView = referencedSourceNode;
                                           //referencedSourceNode.otherView = newNode;

                                           // Update Name by path
                                           newNode.Name = Path.GetFileName(path);
                                       }
                                       else
                                       {
                                           // Create New File Node

                                           FileInfo fileInfo = new FileInfo(path);
                                           System.DateTime CreationDateTime = File.GetCreationTime(path);
                                           cAttributes fileAttributes = new cAttributes((fileInfo.Attributes & System.IO.FileAttributes.ReadOnly) != 0, (fileInfo.Attributes & System.IO.FileAttributes.Hidden) != 0, CreationDateTime);

                                           newNode = new TaskNodeViewModel(parentNode, bufferID++, taskViewType, basePath, fileAttributes) { Name = Path.GetFileName(path), Path_1_Original = path };
                                       }

                                       UpdateTargetNodeActions(newNode, TargetNodeTask, TaskPath, OriginPath, Activated);

                                       parentNode.Children.Add(newNode);

                                       // Update List of all Files
                                       if (taskViewType == TaskViewType.Before)
                                           FileList.Add(new FileListEntry(path, (uint)(new System.IO.FileInfo(path).Length), ref newNode));
                                       else
                                           FileList.Add(new FileListEntry(path, 0, ref newNode)); // FileLength is only relevant for before View
                                   }
                               }
                               catch (Exception ex)
                               {
                                   MessageBox.Show("EditTaskView: " + path + " : " + ex.Message);

                                   newNode = null;
                               }
                           }
                            ), null);
                    }


                    ID = bufferID;

                    return newNode;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("EditTaskView: " + path + " : " + ex.Message);
            }

            return null;
        }

        private void AddPathInTree(TaskNodeViewModel referencedSourceNode, TaskNodeViewModel parentNode2Edit, List<FileListEntry> FileList2Edit, string path, string OriginPath, ref int ID, TaskViewType taskViewType, TaskViewTarget taskViewTarget, enTasks TargetNodeTask, string TaskPath, bool Activated)
        {

            string SourceFolderPath = null;
            if (taskViewTarget == TaskViewTarget.Source)
                SourceFolderPath = userOptions.strSelectSourcePath;
            else if (taskViewTarget == TaskViewTarget.Target)
                SourceFolderPath = userOptions.strSelectTargetPath;


            List<string> strFolderAndFileNames = UsefulMethods.GetFolderAndFileNames(path, SourceFolderPath, true);
            if (strFolderAndFileNames == null)
                return;

            // find Parentnode
            TaskNodeViewModel curNode = parentNode2Edit;

            if (strFolderAndFileNames.Count > 1)
            {
                int iFolderNameIndex = 0;

                for (iFolderNameIndex = 0; iFolderNameIndex < strFolderAndFileNames.Count - 1; iFolderNameIndex++)
                {
                    // old Version
                    /*
                    bool bFound = false;

                    foreach (TaskNodeViewModel node in curNode.Children)
                        if (node.Name == strFolderAndFileNames[iFolderNameIndex])
                        {
                            curNode = node;
                            bFound = true;

                            break;
                        }

                    if (bFound == false)
                        break; // Folder not Found
                     */

                    List<TaskNodeViewModel> newnodes = null;

                    if (curNode.childrensNames.ContainsKey(strFolderAndFileNames[iFolderNameIndex]))
                        newnodes = curNode.childrensNames[strFolderAndFileNames[iFolderNameIndex]];

                    if (newnodes != null)
                        curNode = newnodes[0];
                    else
                        break;

                }

                // Create Folders that doesnt exist!
                for (; iFolderNameIndex < strFolderAndFileNames.Count - 1; iFolderNameIndex++)
                {
                    curNode = EditTaskView(curNode.basePath, curNode, referencedSourceNode, TreeAction.AddDir, strFolderAndFileNames[iFolderNameIndex], strFolderAndFileNames[iFolderNameIndex], ref ID, FileList2Edit, taskViewType, enTasks.CreateSubfolder, strFolderAndFileNames[iFolderNameIndex], true, false);
                }

            }


            List<TaskNodeViewModel> nodes = null;

            if (curNode.childrensNames.ContainsKey(System.IO.Path.GetFileName(path)))
                nodes = curNode.childrensNames[System.IO.Path.GetFileName(path)];

            if (nodes != null) // Update File Actions if Exist
            {
                TaskNodeViewModel node = nodes[0];

                // nur falls hash gleich
                if (referencedSourceNode != null)
                    if ((node.HashCode != referencedSourceNode.HashCode) || (node.Path_1_Original != referencedSourceNode.Path_1_Original))
                        goto PROCESS_TASK; // add file anyway even if same name - will be renamed by editView

                if (userOptions.mainWindow != null)
                {
                    userOptions.mainWindow.Dispatcher.Invoke(
                       (Action)(() =>
                       {
                           UpdateTargetNodeActions(node, TargetNodeTask, TaskPath, OriginPath, Activated);

                       }
                        ), null);
                }

                return; // sonst ABBRUCH
            }

            if (TargetNodeTask == enTasks.CreateSubfolder) // Create Subfolder Action
            {
                curNode = EditTaskView(curNode.basePath, curNode, referencedSourceNode, TreeAction.AddDir, path, path, ref ID, FileList2Edit, taskViewType, enTasks.CreateSubfolder, TaskPath, true, false);

                return;
            }


        PROCESS_TASK:

            if (TargetNodeTask != enTasks.CreateSubfolder)
            {
                // Add Files if Any
                EditTaskView(curNode.basePath, curNode, referencedSourceNode, TreeAction.AddFile, path, OriginPath, ref ID, FileList2Edit, taskViewType, TargetNodeTask, TaskPath, Activated, false);
            }
        }



        private void UpdateTargetNodeActions(TaskNodeViewModel targetNode, enTasks TargetNodeTask, string TaskPath, string OriginPath, bool Activated)
        {
            targetNode.Path_1_Original = OriginPath;

            targetNode.bActivated = Activated;

            if (TargetNodeTask != enTasks.Org)
            {
                switch (TargetNodeTask)
                {
                    case enTasks.CreateSubfolder:
                        targetNode.task2 = enTasks.CreateSubfolder;
                        targetNode.Path_2_Copy = TaskPath;
                        break;

                    case enTasks.Copy:
                        targetNode.task2 = enTasks.Copy;
                        targetNode.Path_2_Copy = TaskPath;
                        break;

                    case enTasks.Delete:
                        targetNode.task5 = enTasks.Delete;
                        targetNode.Path_5_Delete = TaskPath;
                        break;

                    case enTasks.Move:
                        targetNode.task3 = enTasks.Move;
                        targetNode.Path_3_Move = TaskPath;
                        break;

                    case enTasks.Rename:
                        targetNode.task4 = enTasks.Rename;
                        targetNode.Path_4_Rename = TaskPath;
                        break;

                    default:
                        break;
                }

            }

        }
    }



}

