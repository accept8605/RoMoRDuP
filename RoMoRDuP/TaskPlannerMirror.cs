using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using RoMoRDuP.Tasks;
using RoMoRDuP.UserInterface;
using System.Threading;

namespace RoMoRDuP.TaskPlanner
{
    public enum enSyncDirection { None, PrimarySecondary, SecondaryPrimary/*, BothDirections*/ }
    

    public class SpecificOptionsTargetSource
    {
        //specific params for Source
        //string strPrimarySourcePath = userOptionsMirror.strSelectSourcePath
        //string strSecondarySourcePath = userOptionsMirror.strSelectTargetPath
        //bool OptionsMirrorDelete = userOptionsMirror.OptionsMirrorDeleteSourceTarget
        //enOptionsMirrorMovedRenaming enOptionsMerrorMovedRenamingSecondary = enOptionsMirrorMovedRenaming.Target
        //bool OptionsMirrorCopy = userOptionsMirror.OptionsMirrorCopySourceTarget
        //enOptionsMirrorMoveTo enOptionsMirrorMoveToSecondary = enOptionsMirrorMoveTo.Target
        //enOptionsMirrorDuplicates enOptionsMirrorDuplicatsSecondary = enOptionsMirrorDuplicates.RenameTarget

        public SpecificOptionsTargetSource(string strPrimaryPath, string strSecondaryPath,
                bool OptionsMirrorDelete, /*,enOptionsMirrorMovedRenaming enOptionsMerrorMovedRenamingSecondary, */
                bool OptionsMirrorCopy, /*, enOptionsMirrorMoveTo enOptionsMirrorMoveToSecondary,
                enOptionsMirrorMoveTo enOptionsMirrorMoveToPrimary,
                enOptionsMirrorDuplicates enOptionsMirrorDuplicatsSecondary, enOptionsMirrorUpdated enOptionsMirrorUpdatedSecondary, 
                enOptionsMirrorUpdated enOptionsMirrorUpdatedPrimary */

                enSyncDirection SyncDir,
                bool SyncBothWays,
                bool IsSecondary
            )

        {
            this.strPrimaryPath = strPrimaryPath;
            this.strSecondaryPath = strSecondaryPath;
            this.OptionsMirrorDelete = OptionsMirrorDelete;
            //this.enOptionsMirrorMovedRenamingSecondary = enOptionsMerrorMovedRenamingSecondary;
            this.OptionsMirrorCopy = OptionsMirrorCopy;
            /*
            this.enOptionsMirrorMoveToSecondary = enOptionsMirrorMoveToSecondary;
            this.enOptionsMirrorMoveToPrimary = enOptionsMirrorMoveToPrimary;
            this.enOptionsMirrorDuplicatesRenameSecondary = enOptionsMirrorDuplicatsSecondary;
            this.enOptionsMirrorUpdatedSecondary = enOptionsMirrorUpdatedSecondary;
            this.enOptionsMirrorUpdatedPrimary = enOptionsMirrorUpdatedPrimary;
             */
            this.SyncDirection = SyncDir;
            this.SyncBothWays = SyncBothWays;
            this.IsSecondary = IsSecondary;

        }

        public string strPrimaryPath{ get; set; }
        public string strSecondaryPath { get; set; }
        public bool OptionsMirrorDelete { get; set; }
        //public enOptionsMirrorMovedRenaming enOptionsMirrorMovedRenamingSecondary { get; set; }
        public bool OptionsMirrorCopy { get; set; }
        /*
        public enOptionsMirrorMoveTo enOptionsMirrorMoveToSecondary { get; set; }
        public enOptionsMirrorMoveTo enOptionsMirrorMoveToPrimary { get; set; }
        public enOptionsMirrorDuplicates enOptionsMirrorDuplicatesRenameSecondary { get; set; }

        public enOptionsMirrorUpdated enOptionsMirrorUpdatedSecondary { get; set; }
        public enOptionsMirrorUpdated enOptionsMirrorUpdatedPrimary { get; set; } */

        public enSyncDirection SyncDirection { get; set; }
        public bool SyncBothWays { get; set; }
        public bool IsSecondary { get; set; }
    }


    public class TaskPlannerMirror : TaskPlannerBase
    {
        public TaskPlannerMirror(UserInterface.UserInterfaceBase userOptions, TaskViews taskSource, TaskViews taskTarget, Tasks.Tasks tasks, FileLists fileLists, PlaylistUpdates.PlaylistUpdates playlistUpdates)
            : base(userOptions, taskSource, taskTarget, tasks, fileLists, playlistUpdates)
        {
            //

        }



        Thread thread_CreateMirror_Before_Actions;


        public void CreateMirror_Source_Target_Before_Actions()
        {
            if (thread_CreateMirror_Before_Actions == null)
            {
                thread_CreateMirror_Before_Actions = new Thread(new ThreadStart(int_CreateMirror_Source_Target_Before_Actions));
            }
            else
            {
                thread_CreateMirror_Before_Actions.Abort(); // TBD check if Abort valid - only if everything managed

                thread_CreateMirror_Before_Actions = new Thread(new ThreadStart(int_CreateMirror_Source_Target_Before_Actions));
            }


            thread_CreateMirror_Before_Actions.Start();


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       TaskPlanner.GlobalVar.wHourglass = new WHourglass("Creating mirror actions...");
                       TaskPlanner.GlobalVar.wHourglass.Owner = userOptions.mainWindow;

                       TaskPlanner.GlobalVar.wHourglass.ShowDialog();

                   }
                    ), null);
            }

        }




        //separate Thread
        private void int_CreateMirror_Source_Target_Before_Actions()
        {
            UserOptionsMirror userOptionsMirror = (UserOptionsMirror)userOptions;
           
            /*
            // Check if CrawlSource (if Copy Source->Target / Delete in Source / Rename in Source ) necessary
            if (userOptionsMirror.OptionsMirrorCopySourceTarget == true || userOptionsMirror.OptionsMirrorDeleteTargetSource == true
                || userOptionsMirror.OptionsMirrorMoveTo == enOptionsMirrorMoveTo.Target || userOptionsMirror.OptionsMirrorMovedRenaming == enOptionsMirrorMovedRenaming.Target
                || (userOptionsMirror.OptionsMirrorUpdated != enOptionsMirrorUpdated.Skip && userOptionsMirror.OptionsMirrorUpdated != enOptionsMirrorUpdated.Source)
                || (userOptionsMirror.OptionsMirrorDuplicates != enOptionsMirrorDuplicates.Skip && userOptionsMirror.OptionsMirrorDuplicates != enOptionsMirrorDuplicates.RenameSource)
                )
            */
            {
                enSyncDirection syncDirection;
                bool syncBothWays = false;
                bool isSecondary = false;

                if (userOptionsMirror.OptionsMirrorCopySourceTarget && userOptionsMirror.OptionsMirrorCopyTargetSource)
                {
                    syncDirection = enSyncDirection.PrimarySecondary;
                    syncBothWays = true;
                }
                else if (userOptionsMirror.OptionsMirrorCopySourceTarget)
                    syncDirection = enSyncDirection.PrimarySecondary;
                else if (userOptionsMirror.OptionsMirrorCopyTargetSource)
                {
                    syncDirection = enSyncDirection.SecondaryPrimary;
                    isSecondary = true;
                }
                else
                    syncDirection = enSyncDirection.None;


                SpecificOptionsTargetSource specificOptions = new SpecificOptionsTargetSource(userOptionsMirror.strSelectSourcePath, userOptionsMirror.strSelectTargetPath,
                    userOptionsMirror.OptionsMirrorDeleteTargetSource, userOptionsMirror.OptionsMirrorCopySourceTarget, syncDirection, syncBothWays,isSecondary
                    );

                CreateMirror_Before_Actions(taskSource.TaskViewBefore.Items[0],taskTarget.TaskViewBefore.Items[0],fileLists.SourceFileListBefore, fileLists.TargetFileListBefore, userOptionsMirror, specificOptions);
            }

            /*
            // Check if CrawlTarget (if Copy Target->Source / Delete in Target / Rename in Target ) necessary
            if (userOptionsMirror.OptionsMirrorCopyTargetSource == true || userOptionsMirror.OptionsMirrorDeleteSourceTarget == true
                || userOptionsMirror.OptionsMirrorMoveTo == enOptionsMirrorMoveTo.Source || userOptionsMirror.OptionsMirrorMovedRenaming == enOptionsMirrorMovedRenaming.Source
                || (userOptionsMirror.OptionsMirrorUpdated != enOptionsMirrorUpdated.Skip && userOptionsMirror.OptionsMirrorUpdated != enOptionsMirrorUpdated.Target)
                || (userOptionsMirror.OptionsMirrorDuplicates != enOptionsMirrorDuplicates.Skip && userOptionsMirror.OptionsMirrorDuplicates != enOptionsMirrorDuplicates.RenameTarget)
                )
            */
            {
                enSyncDirection syncDirection;
                bool syncBothWays = false;
                bool isSecondary = false;

                if (userOptionsMirror.OptionsMirrorCopySourceTarget && userOptionsMirror.OptionsMirrorCopyTargetSource)
                {
                    syncDirection = enSyncDirection.SecondaryPrimary;
                    syncBothWays = true;
                    isSecondary = true;
                }
                else if (userOptionsMirror.OptionsMirrorCopySourceTarget)
                {
                    syncDirection = enSyncDirection.SecondaryPrimary;
                    isSecondary = true;
                }
                else if (userOptionsMirror.OptionsMirrorCopyTargetSource)
                    syncDirection = enSyncDirection.PrimarySecondary;
                else
                    syncDirection = enSyncDirection.None;

                SpecificOptionsTargetSource specificOptions = new SpecificOptionsTargetSource(userOptionsMirror.strSelectTargetPath, userOptionsMirror.strSelectSourcePath,
                    userOptionsMirror.OptionsMirrorDeleteSourceTarget, userOptionsMirror.OptionsMirrorCopyTargetSource, syncDirection, syncBothWays, isSecondary
                    );

                CreateMirror_Before_Actions(taskTarget.TaskViewBefore.Items[0],taskSource.TaskViewBefore.Items[0],fileLists.TargetFileListBefore, fileLists.SourceFileListBefore, userOptionsMirror, specificOptions);
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



        private void CreateMirror_Before_Actions(TaskNodeViewModel PrimaryBaseNode, TaskNodeViewModel SecondaryBaseNode, List<FileListEntry> PrimaryFileList, List<FileListEntry> SecondaryFileList, UserOptionsMirror userOptionsMirror, SpecificOptionsTargetSource specificOptions)
        {

            // Crawl
            foreach (FileListEntry entry in PrimaryFileList)
            {
                // Delete old tasks
                entry.targetNode.RemoveAllTasks();


                // Check File Filters
                if (userOptionsMirror.fileFilterOptions.CheckIfFileOK(entry.Path))
                {

                    if (entry.targetNode.IsFile)
                    {

                        // Get Infos

                        // Get Parent folder paths for Copying/moving
                        string PrimaryParentPath = System.IO.Directory.GetParent(entry.Path).FullName.Replace(specificOptions.strPrimaryPath, ""); //UsefulMethods.ReplacePath(System.IO.Directory.GetParent(entry.Path).FullName,specificOptions.strPrimaryPath, "");
                        
                        /*
                        List<string> DuplicatesSecondaryParentPaths = new List<string>();
                        foreach (string duplPath in entry.targetNode.DuplicatesSourceTarget)
                        {
                            DuplicatesSecondaryParentPaths.Add(System.IO.Directory.GetParent(duplPath).FullName.Replace(specificOptions.strSecondaryPath, ""));
                        }

                        // Check Duplicate Same Folder between Source-Target for Copying
                        bool bDuplicateSameFolder = false;
                        foreach (string duplParentPath in DuplicatesSecondaryParentPaths)
                            if (duplParentPath == PrimaryParentPath)
                                bDuplicateSameFolder = true;
                         */

                        
                        List<string> DuplicatesSecondaryParentPaths = new List<string>();
                        bool bDuplicateSameFolder = false;
                        foreach (TaskNodeViewModel duplNode in entry.targetNode.DuplicatesSourceTargetNodes)
                        {
                            string duplParentPath = System.IO.Directory.GetParent(duplNode.Path_1_Original).FullName.Replace(specificOptions.strSecondaryPath, "");

                            DuplicatesSecondaryParentPaths.Add(duplParentPath);

                            if (duplParentPath == PrimaryParentPath)
                            {
                                bDuplicateSameFolder = true;
                                // Add ReferencedOtherFolder Info
                                entry.targetNode.ReferencedOtherFolderNode = duplNode;
                                duplNode.ReferencedOtherFolderNode = entry.targetNode;
                            }
                        }
                         


                        // Check if updated file
                        bool bUpdatedFile = false;
                        TaskNodeViewModel UpdatedNode = null;

                        TaskNodeViewModel SecondaryNode = UsefulMethods.FindNodeHierarchicalByPath(SecondaryBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entry.Path, specificOptions.strPrimaryPath), false, "");
                        if (SecondaryNode != null)
                            if (entry.targetNode.HashCode != SecondaryNode.HashCode)
                            {
                                bUpdatedFile = true;
                                UpdatedNode = SecondaryNode;
                            }


                        // old Version
                        /*
                        foreach (FileListEntry targetFile in SecondaryFileList)
                            if (UsefulMethods.GetPathWithoutSourceFolder(entry.Path,specificOptions.strPrimaryPath)  
                                    == UsefulMethods.GetPathWithoutSourceFolder(targetFile.Path,specificOptions.strSecondaryPath) )
                                if (entry.hash != targetFile.hash)
                                {
                                    bUpdatedFile = true;
                                    UpdatedFile = targetFile;
                                    break;
                                }
                        */




                        // Check if updated file is most recent
                        bool bUpdatedFilseIsMostRecent = false;     // Secondary file is older
                        if (bUpdatedFile)
                        {
                            System.IO.FileInfo fileInfoPrimaryUpdated = new System.IO.FileInfo(entry.Path);
                            System.IO.FileInfo fileInfoSecondaryUpdated = new System.IO.FileInfo(UpdatedNode.Path_1_Original);

                            if (fileInfoSecondaryUpdated.LastWriteTime > fileInfoPrimaryUpdated.LastWriteTime)
                                bUpdatedFilseIsMostRecent = true;                                           // Secondary file is newer
                        }


                        entry.targetNode.bUpdatedFile = bUpdatedFile;


                        // _________________Create Actions_______________


                        // Check Delete

                        if (
                            // Delete files that are in Source but not Target(Delete in Source)
                                    specificOptions.OptionsMirrorDelete//userOptionsMirror.OptionsMirrorDeleteSourceTarget
                                    && (entry.targetNode.DuplicatesSourceTarget.Count == 0)

                                    && !bUpdatedFile // Updated Files werden separat behandelt
                            )
                        {
                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       entry.targetNode.Path_5_Delete = entry.Path;
                                       entry.targetNode.task5 = enTasks.Delete;
                                       entry.targetNode.bActivated = true;
                                   }
                                    ), null);
                            }

                        }

                        else if (        // Updated files options can cause deleting
                                             ((bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.MostRecentDate) && (bUpdatedFilseIsMostRecent) && (specificOptions.OptionsMirrorDelete)) // Secondary File is most recent -> delete Primary skip copy
                                             || ((bUpdatedFile) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary) &&
                                                    (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.SyncDir) && (specificOptions.OptionsMirrorDelete)) // use Secondary -> delete primary skip copy  
                               )
                        {



                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       entry.targetNode.Path_5_Delete = entry.Path;

                                       entry.targetNode.task5 = enTasks.Delete;
                                       entry.targetNode.bActivated = true;

                                       entry.targetNode.Info = "Deleting this unused updated file";

                                   }
                                    ), null);
                            }

                        }


                        else // No Delete
                        {

                            // Check Copy
                            if (specificOptions.OptionsMirrorCopy || bUpdatedFile) // copying also possible from updated files
                            {

                                bool bNoDuplicatesWithDiffFilenames = false;

                                if (// Duplicates with Different FileNames in same folder options
                                        (userOptionsMirror.OptionsMirrorDuplicates == enOptionsMirrorDuplicates.CopyAnyway)
                                        || (entry.targetNode.DuplicatesSourceTarget.Count == 0) || (!bDuplicateSameFolder)
                                    )
                                    bNoDuplicatesWithDiffFilenames = true;


                                bool bMovedBackInsideSecondary = false;

                                if (// Dont copy file if its gonna be moved back inside Secondary
                                            (userOptionsMirror.OptionsMirrorMoveTo == enOptionsMirrorMoveTo.SyncDir) && ((specificOptions.SyncDirection == enSyncDirection.PrimarySecondary) || (specificOptions.SyncBothWays && specificOptions.IsSecondary))
                                            && (entry.targetNode.DuplicatesSourceSourceOrTT.Count == 0) && (entry.targetNode.DuplicatesSourceTarget.Count == 1) // only if only 1 example
                                           )
                                    bMovedBackInsideSecondary = true;


                                bool bUpdatedFilesNoCopying = false;
                                
                                if(// Updated files options can prevent copying
                                             ((bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.Skip))
                                             || ( (bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.SyncDir) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary) )
                                             || ( (bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.MostRecentDate) && (bUpdatedFilseIsMostRecent)) // Secondary File is most recent -> skip copy   
                                             || ((bUpdatedFile) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary) && !specificOptions.SyncBothWays) // Skip if not SyncDir
                                        )
                                    bUpdatedFilesNoCopying = true;



                                if (
                                        bNoDuplicatesWithDiffFilenames
                                        &&
                                        !bMovedBackInsideSecondary
                                        &&
                                        !bUpdatedFilesNoCopying
                                    )
                                {


                                    if (userOptions.mainWindow != null)
                                    {
                                        userOptions.mainWindow.Dispatcher.Invoke(
                                           (Action)(() =>
                                           {
                                               entry.targetNode.Path_2_Copy = UsefulMethods.ReplacePath(entry.Path, specificOptions.strPrimaryPath, specificOptions.strSecondaryPath);         /*entry.Path.Replace(specificOptions.strPrimaryPath, specificOptions.strSecondaryPath)*/
                                               entry.targetNode.task2 = enTasks.Copy;
                                               entry.targetNode.bActivated = true;

                                               entry.targetNode.Path_SetCopyOrAttributesSource = entry.targetNode.Path_1_Original;
                                               entry.targetNode.Path_CopyOrSetAttributesBase = specificOptions.strSecondaryPath;

                                               if (userOptionsMirror.OptionsMirrorSyncAttributes)
                                               {
                                                   // Set Attributes
                                                   entry.targetNode.Path_SetAttributes = entry.targetNode.Path_2_Copy;
                                                   
                                                   entry.targetNode.Path_SetAttributesSourceBase = entry.targetNode.basePath;
                                                   entry.targetNode.task7 = enTasks.SetAttributes;

                                                   System.IO.FileInfo fileInfo = new System.IO.FileInfo(entry.Path);
                                                   System.DateTime CreationDateTime = System.IO.File.GetCreationTime(entry.Path);
                                                   cAttributes fileAttributes = new cAttributes((fileInfo.Attributes & System.IO.FileAttributes.ReadOnly) != 0, (fileInfo.Attributes & System.IO.FileAttributes.Hidden) != 0, CreationDateTime);

                                                   entry.targetNode.SetFileAttributes = fileAttributes;
                                               }


                                               if (bUpdatedFile && ((userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.MostRecentDate)
                                                       && (!bUpdatedFilseIsMostRecent))) // Primary file is most recent
                                                   entry.targetNode.Info = "Most recent updated file!";

                                               if ((bUpdatedFile) && (specificOptions.SyncDirection == enSyncDirection.PrimarySecondary) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.SyncDir))
                                                   entry.targetNode.Info = "Using this updated file!";

                                           }
                                            ), null);
                                    }

                                }
                                else if (   bUpdatedFilesNoCopying
                                    /*
                                    // Info about Updated files options can prevent copying
                                             ((bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.Skip))
                                             || ((bUpdatedFile) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.SyncDir))
                                             || ((bUpdatedFile) && (userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.MostRecentDate) && (bUpdatedFilseIsMostRecent)) // Secondary File is most recent -> delete Primary skip copy
                                     */
                                        )
                                {

                                    if (userOptions.mainWindow != null)
                                    {
                                        userOptions.mainWindow.Dispatcher.Invoke(
                                           (Action)(() =>
                                           {
                                               entry.targetNode.Info = "No copy-unused updated file!";
                                           }
                                            ), null);
                                    }

                                }
                                else // SetAttributes Action always in CopyDirection!
                                {
                                    if (specificOptions.OptionsMirrorCopy && userOptionsMirror.OptionsMirrorSyncAttributes
                                        && !(specificOptions.SyncBothWays && specificOptions.IsSecondary)
                                        )
                                    {
                                        // Only for duplicates
                                        if (entry.targetNode.ComparePathToDuplicatesSourceTarget(UsefulMethods.ReplacePath(entry.Path, specificOptions.strPrimaryPath, specificOptions.strSecondaryPath)))
                                        {
                                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(entry.Path);
                                            System.DateTime CreationDateTime = System.IO.File.GetCreationTime(entry.Path);
                                            cAttributes PrimaryFileAttributes = new cAttributes((fileInfo.Attributes & System.IO.FileAttributes.ReadOnly) != 0, (fileInfo.Attributes & System.IO.FileAttributes.Hidden) != 0, CreationDateTime);

                                            string secondaryPath = UsefulMethods.ReplacePath(entry.Path, specificOptions.strPrimaryPath, specificOptions.strSecondaryPath);
                                            fileInfo = new System.IO.FileInfo(secondaryPath);
                                            if (fileInfo != null)
                                            {
                                                CreationDateTime = System.IO.File.GetCreationTime(secondaryPath);
                                                cAttributes SecondaryFileAttributes = new cAttributes((fileInfo.Attributes & System.IO.FileAttributes.ReadOnly) != 0, (fileInfo.Attributes & System.IO.FileAttributes.Hidden) != 0, CreationDateTime);

                                                if (PrimaryFileAttributes.Compare(SecondaryFileAttributes) == false)
                                                {
                                                    // Set Attributes if difference in Attributes
                                                    entry.targetNode.Path_SetAttributes = secondaryPath;
                                                    entry.targetNode.Path_CopyOrSetAttributesBase = specificOptions.strSecondaryPath;
                                                    entry.targetNode.Path_SetCopyOrAttributesSource = entry.Path;
                                                    entry.targetNode.Path_SetAttributesSourceBase = entry.targetNode.basePath;

                                                    entry.targetNode.task7 = enTasks.SetAttributes;
                                                    entry.targetNode.SetFileAttributes = PrimaryFileAttributes;

                                                    entry.targetNode.bActivated = true;
                                                }
                                            }
                                        }
                                    }
                                }

                            }



                            // Check Rename
                            if (
                                // Duplicates same Path renaming options
                                        (userOptionsMirror.OptionsMirrorDuplicates == enOptionsMirrorDuplicates.RenameSyncDir) && bDuplicateSameFolder && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary)
                                //&& (System.IO.Path.GetFileName(entry.Path) != System.IO.Path.GetFileName(entry.targetNode.DuplicatesSourceTarget[0])
                                        && !entry.targetNode.ComparePathToDuplicatesSourceTarget(UsefulMethods.ReplacePath(entry.Path, specificOptions.strPrimaryPath, specificOptions.strSecondaryPath)) // only if not the same as in Target

                               )
                            {

                                if (userOptions.mainWindow != null)
                                {
                                    userOptions.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           string targetPath = UsefulMethods.ReplacePath(entry.targetNode.DuplicatesSourceTarget[0], specificOptions.strSecondaryPath, specificOptions.strPrimaryPath);

                                           if (null == UsefulMethods.FindNodeHierarchicalByPath(PrimaryBaseNode, UsefulMethods.GetPathWithoutSourceFolder(targetPath, specificOptions.strPrimaryPath), true, specificOptions.strPrimaryPath))
                                           {
                                               entry.targetNode.task4 = enTasks.Rename;
                                               entry.targetNode.bActivated = true;

                                               // TBD Check if file with this name exists already first!
                                               entry.targetNode.Path_4_Rename = targetPath;
                                           }
                                       }
                                        ), null);
                                }

                            }

                                // Renameing is in Moved Task 
                            /*
                        else if (    // Moved Files Renaming options
                            // Only if only 1 example in Source/Target
                                    (entry.targetNode.DuplicatesSourceSourceOrTT.Count == 0) && (DuplicatesSecondaryParentPaths.Count == 1)
                            //Moved files renaming options - only if move Secondary to place like in Primary, but use Seconary name
                                    && (userOptionsMirror.OptionsMirrorMoveTo ==  enOptionsMirrorMoveTo.SyncDir)
                                    && (userOptionsMirror.OptionsMirrorMovedRenaming ==  enOptionsMirrorMovedRenaming.SyncDir) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary)
                                    && (System.IO.Path.GetFileName(entry.targetNode.DuplicatesSourceTarget[0]) != System.IO.Path.GetFileName(entry.Path))
                          )
                        {

                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       entry.targetNode.task4 = enTasks.Rename;
                                       entry.targetNode.bActivated = true;

                                       // TBD Check if file with this name exists already first!
                                       // use secondary name
                                       System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(entry.Path); // Primary Folder Path stays!
                                       entry.targetNode.Path_4_Rename = dirInfo.Parent.FullName + "\\" + System.IO.Path.GetFileName(entry.targetNode.DuplicatesSourceTarget[0]);
                                   }
                                    ), null);
                            }

                        }
                             */

                            else if (
                                // Updated Files renaming options
                                        (bUpdatedFile) && ((userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.SyncDir) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary))
                                                        || ((userOptionsMirror.OptionsMirrorUpdated == enOptionsMirrorUpdated.MostRecentDate) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary || specificOptions.SyncBothWays) && (bUpdatedFilseIsMostRecent)) // Secondary File is most recent -> rename primary skip copy
                               )
                            {
                                if (userOptions.mainWindow != null)
                                {
                                    userOptions.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           entry.targetNode.task4 = enTasks.Rename;
                                           entry.targetNode.bActivated = true;

                                           // TBD Check if file with this name exists already first! - maybe through Executer?

                                           entry.targetNode.Path_4_Rename = UsefulMethods.AddModificationDateTimeToFilename(entry.targetNode.Path_1_Original);
                                       }
                                        ), null);
                                }

                            }


                            // Check Move
                            /* // In Copy
                    
                             */
                            if ((userOptionsMirror.OptionsMirrorMoveTo == enOptionsMirrorMoveTo.SyncDir) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary)) // Move in Primary
                            {
                                if (!(entry.targetNode.task2 == enTasks.Copy)) // Don´t move if going to copied to secondary
                                {
                                    // Only if only 1 example in Source/Target
                                    if (entry.targetNode.DuplicatesSourceSourceOrTT.Count == 0)
                                        if (DuplicatesSecondaryParentPaths.Count == 1)
                                        {
                                            if (PrimaryParentPath != DuplicatesSecondaryParentPaths[0])
                                            {
                                                string finalname = System.IO.Path.GetFileName(entry.Path); // get name from Primary

                                                // Check Renaming
                                                if (    // Moved Files Renaming options
                                                    // Only if only 1 example in Source/Target - see above
                                                    //Moved files renaming options - only if move Secondary to place like in Primary, but use Seconary name
                                                        (userOptionsMirror.OptionsMirrorMoveTo == enOptionsMirrorMoveTo.SyncDir)
                                                        && (userOptionsMirror.OptionsMirrorMovedRenaming == enOptionsMirrorMovedRenaming.SyncDir) && (specificOptions.SyncDirection == enSyncDirection.SecondaryPrimary)
                                                        && (System.IO.Path.GetFileName(entry.targetNode.DuplicatesSourceTarget[0]) != System.IO.Path.GetFileName(entry.Path))
                                                    )
                                                {
                                                    // Rename Source to Target?
                                                    finalname = System.IO.Path.GetFileName(entry.targetNode.DuplicatesSourceTarget[0]);
                                                }

                                                if (userOptions.mainWindow != null)
                                                {
                                                    userOptions.mainWindow.Dispatcher.Invoke(
                                                       (Action)(() =>
                                                       {
                                                           if (DuplicatesSecondaryParentPaths[0].Length > 0)
                                                               entry.targetNode.Path_3_Move = specificOptions.strPrimaryPath + DuplicatesSecondaryParentPaths[0] + "\\" + finalname;
                                                           else
                                                               entry.targetNode.Path_3_Move = specificOptions.strPrimaryPath + "\\" + finalname;

                                                           entry.targetNode.task3 = enTasks.Move;
                                                           entry.targetNode.bActivated = true;
                                                       }
                                                        ), null);
                                                }


                                            }
                                        }
                                }
                            }
                            // Subfolders will only be created if target path does not exist(For Move/Copy by Task Executer - TBD)! Check after Copy and Move. Also creates parent folders if not exist.


                        }

                    } // entry ISFILE

                    else
                    {
                        // ///////////////////////ISFOLDER//////////////////////

                        // Check Delete

                        if (
                            // Delete folders that are in Primary but not Secondary(Delete in Primary)
                                    specificOptions.OptionsMirrorDelete
                                    && (null == UsefulMethods.FindNodeHierarchicalByPath(SecondaryBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entry.targetNode.Path_1_Original, entry.targetNode.basePath), false, ""))
                            )
                        {
                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       entry.targetNode.Path_5_Delete = entry.Path;
                                       entry.targetNode.task5 = enTasks.Delete;
                                       entry.targetNode.bActivated = true;
                                   }
                                    ), null);
                            }

                        }

                        else // No Delete
                        {
                            // Check Create Subfolder
                            if (
                                // Create folders that are in Primary but not Secondary(Create in Secondary)
                                    specificOptions.OptionsMirrorCopy
                                    && (null == UsefulMethods.FindNodeHierarchicalByPath(SecondaryBaseNode, UsefulMethods.GetPathWithoutSourceFolder(entry.targetNode.Path_1_Original, entry.targetNode.basePath), false, ""))
                                )
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {

                                       entry.targetNode.Path_2_Copy = UsefulMethods.ReplacePath(entry.Path, specificOptions.strPrimaryPath, specificOptions.strSecondaryPath);
                                       //entry.targetNode.Path //Path1 already OK
                                       entry.targetNode.task2 = enTasks.CreateSubfolder;
                                       entry.targetNode.bActivated = true;
                                   }
                                    ), null);

                            }
                        }

                    }

                }
                else // Filtered File
                {
                    if (userOptions.mainWindow != null)
                    {
                        userOptions.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               entry.targetNode.Info = "Filtered file!";
                           }
                            ), null);
                    }

                }

            }

        }



        //public enum enTasks { None, CreateSubfolder, Copy, Rename, Move, Delete, Overflow } // Order of biggestAction
        /*
            Path_1_Original = "some path";  // Create Subfolder without Move/Copy (not required for mirroring)/ Delete
            Path_2_Copy = "some path";      // Copy // CreateSubfolder for Copy
            Path_3_Move = "some path" ;     // Move // Create Subfolder for Move
            Path_4_Rename = "some path" ;   // Rename
         */


        // Actions setzen (nicht before nur After, aus before generiert!)

        /*
        enTasks action = enTasks.Delete;

        int n = 0;
        foreach (TaskNodeViewModel node in ParentNode.Children)
        {
            ++action;
            if (action > enTasks.Delete)
                action = enTasks.CreateSubfolder;

            node.task1 = action;

            if (action == enTasks.Delete)
                node.Info = "duplicate";
            if (action == enTasks.Copy)
                node.Info = "updated file";

            ++action;
            if (action > enTasks.Delete)
                action = enTasks.CreateSubfolder;
            node.task2 = action;

            if (action == enTasks.Delete)
                node.Info = "duplicate";
            if (action == enTasks.Copy)
                node.Info = "updated file";

            //String[] hashcodes = { "7e716d0e702df0505fc72e2b89467910", "a3cca2b2aa1e3b5b3b5aad99aae793c1", "7e716d0e702df0505fc72e38acf64984", "a3cca2b2aa1e3b5b3b5aad99a8529074", "7e716d0e702df050dc3680214467910", "a3cca2b2aa1e3b5b3b5acde4687074" };


            if (!node.IsFolder)
            {
                node.HashCode = GetFileHash(node.Path_1_Original);
            }

        }
         */


    }


}

