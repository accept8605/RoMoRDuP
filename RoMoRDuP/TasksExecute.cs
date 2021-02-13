
// Always do Playlist Updates first (in case path changes afterwards)

// Be careful with updated files - first rename then copy

// Be sure to do the DELETING FIRST in source+target before doing any copying

// Subfolders will only be created if target path does not exist(For Move/Copy by Task Executer)! Check after Copy and Move. Also creates parent folders if not exist.

// check if File to rename to already exists! (especially before Moving)



using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.AccessControl;



namespace RoMoRDuP.TasksExecution
{

    public enum enTaskToProcess { DeleteFiles, Rename, CreateSubfolder, Move, Copy, CreateShortcut, SetAttributes, DeleteFolders, Overflow };

    public class TaskExecute
    {
        public TaskExecute(Tasks.FileLists fileLists, UserInterface.UserInterfaceBase userInterface, PlaylistUpdates.PlayListUpdatesModel playlistUpdatesModel)
        {
            this.fileLists = fileLists;

            this.playlistUpdatesModel = playlistUpdatesModel;

            this.userInterface = userInterface;
        }

        Thread threadTaskExecute;

        Tasks.FileLists fileLists;

        PlaylistUpdates.PlayListUpdatesModel playlistUpdatesModel { get; set; }

        UserInterface.UserInterfaceBase userInterface { get; set; }


        public void handleTaskExecute()
        {
            if (threadTaskExecute == null)
            {
                threadTaskExecute = new Thread(new ThreadStart(Execute));
            }
            else
            {
                threadTaskExecute.Abort(); // TBD check if Abort valid - only if everything managed

                threadTaskExecute = new Thread(new ThreadStart(Execute));
            }


            // Start Thread
            threadTaskExecute.Start();
        }



        public void CancelExecute()
        {
            try
            {
                threadTaskExecute.Abort();
                threadTaskExecute = null;
            }
            catch (Exception ex)
            {
                // TBD

            }

        }


        enum enMainState { GetTask, GetPathSource, GetPathTarget, ExecuteTask };

        // In separate Thread!!
        private void Execute()
        {

            if (userInterface.mainWindow != null)
            {
                userInterface.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       // Reset Output
                       userInterface.strTaskExecution = "";

                       // Reset Current Progress
                       userInterface.ProcessCurrentSizeTaskExecution = "0";
                   }
                    ), null);
            }



            try
            {

                enMainState mainState = enMainState.GetTask;


                List<string> listTasks = new List<string>();
                // Add Playlist Tasks
                listTasks = ProcessPlaylistUpdates(playlistUpdatesModel);
                // Add SourceList Tasks

                List<List<TaskPlanner.FileListEntry>> ListfileLists = new List<List<TaskPlanner.FileListEntry>>();
                ListfileLists.Add(fileLists.SourceFileListBefore);

                if (userInterface is UserInterface.UserOptionsMirror)
                    ListfileLists.Add(fileLists.TargetFileListBefore);

                List<string> listTasksAdd = ProcessFileLists(ListfileLists);

                foreach (string strAdd in listTasksAdd)
                    listTasks.Add(strAdd);


                if (userInterface.mainWindow != null)
                {
                    userInterface.mainWindow.Dispatcher.Invoke(
                       (Action)(() =>
                       {
                           userInterface.ProcessTargetSizeTaskExecution = listTasks.Count.ToString();
                       }
                        ), null);
                }


                int iProcessCurrentSize = 0; // Current Progress

                TaskExecutionClient.ExecutionClient executionClinet = new TaskExecutionClient.ExecutionClient();

                string strLine = "";
                
                foreach (string line in listTasks)
                {
                    strLine += line + "\n";

                    mainState++;

                    if (mainState == enMainState.ExecuteTask)
                    {
                        mainState = enMainState.GetTask;

                        string strInfo = executionClinet.ExecuteTaskString(strLine);
                        strLine = "";

                        if (strInfo.Length > 0)
                        {
                            if (strInfo != "EXIT")
                            {

                                if (userInterface.mainWindow != null)
                                {
                                    userInterface.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           userInterface.strTaskExecution += strInfo + "\n";
                                       }
                                        ), null);
                                }
                                
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                            break;
                    }


                    // Increment Current Progress
                    if (userInterface.mainWindow != null)
                    {
                        userInterface.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               iProcessCurrentSize++;
                               userInterface.ProcessCurrentSizeTaskExecution = iProcessCurrentSize.ToString();
                           }
                            ), null);
                    }

                } // foreach taskline


                if (userInterface.mainWindow != null)
                {
                    userInterface.mainWindow.Dispatcher.Invoke(
                       (Action)(() =>
                       {
                           userInterface.strTaskExecution += "Finished!" + "\n";
                       }
                        ), null);
                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine("The server throws the error: {0}", ex.Message);

                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        } // Execute


      



        public List<string> ProcessPlaylistUpdates(PlaylistUpdates.PlayListUpdatesModel playlistUpdatesModel)
        {
            List<string> listRet = new List<string>();

            foreach (PlaylistUpdates.PlayListUpdatesViewModel vm in playlistUpdatesModel.Items)
            {
                if (vm.UpdatePlaylist)
                {
                    if (vm.LeaveCopy)
                    {
                        AddTask(ref listRet, Tasks.enTasks.Rename, vm.PathOriginal, vm.PathRenameOrg);
                    }
                    else // no oldversion copy - Delete
                    {
                        AddTask(ref listRet, Tasks.enTasks.Delete, vm.PathOriginal, vm.PathOriginal);
                    }

                    AddTask(ref listRet, Tasks.enTasks.Copy, vm.PathTemp, vm.PathOriginal);
                }
            }
 

            return listRet;
        }


        


        public List<string> ProcessFileLists(List<List<TaskPlanner.FileListEntry>> ListFileLists)
        {
            List<string> listRet = new List<string>();

            enTaskToProcess taskToProcess = enTaskToProcess.DeleteFiles;

            for (taskToProcess = enTaskToProcess.DeleteFiles; taskToProcess <= enTaskToProcess.DeleteFolders; taskToProcess++)
            {

                foreach (List<TaskPlanner.FileListEntry> fileList in ListFileLists)
                {

                    if (taskToProcess == enTaskToProcess.DeleteFiles) // Delete Files
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if(entry.targetNode.IsFile)
                                if (entry.targetNode.bActivated)
                                {
                                    // 1 Delete
                                    if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                    {
                                        AddTask(ref listRet, Tasks.enTasks.Delete, entry.targetNode.Path_5_Delete, entry.targetNode.Path_5_Delete);
                                    }
                                }

                        }
                    }

                    if (taskToProcess == enTaskToProcess.Rename)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                {
                                    continue;
                                }
                                else
                                {
                                    // ELSE
                                    // 2 Rename
                                    if (entry.targetNode.task4 == Tasks.enTasks.Rename)
                                    {
                                        AddTask(ref listRet, Tasks.enTasks.Rename, entry.targetNode.Path_1_Original, entry.targetNode.Path_4_Rename);
                                    }

                                }
                            }
                        }
                    }

                    if (taskToProcess == enTaskToProcess.CreateSubfolder)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                {
                                    continue;
                                }
                                else
                                {
                                    // ELSE
                                    // 3 Move
                                    if (entry.targetNode.task2 == Tasks.enTasks.CreateSubfolder)
                                    {
                                        AddTask(ref listRet, Tasks.enTasks.CreateSubfolder, entry.targetNode.Path_2_Copy, entry.targetNode.Path_2_Copy);
                                    }
                                }
                            }
                        }
                    }


                    if (taskToProcess == enTaskToProcess.Move)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                {
                                    continue;
                                }
                                else
                                {
                                    // ELSE
                                    // 3 Move
                                    if (entry.targetNode.task3 == Tasks.enTasks.Move)
                                    {
                                        AddTask(ref listRet, Tasks.enTasks.Move, entry.targetNode.Path_1_Original, entry.targetNode.Path_3_Move);
                                    }
                                }
                            }
                        }
                    }


                    if (taskToProcess == enTaskToProcess.Copy)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                {
                                    continue;
                                }
                                else
                                {
                                    // ELSE
                                    // 4 Copy
                                    if (entry.targetNode.task2 == Tasks.enTasks.Copy)
                                    {
                                        if (entry.targetNode.task3 == Tasks.enTasks.Move)
                                            AddTask(ref listRet, Tasks.enTasks.Copy, entry.targetNode.Path_3_Move, entry.targetNode.Path_2_Copy);
                                        else if (entry.targetNode.task4 == Tasks.enTasks.Rename)
                                            AddTask(ref listRet, Tasks.enTasks.Copy, entry.targetNode.Path_4_Rename, entry.targetNode.Path_2_Copy);
                                        else
                                            AddTask(ref listRet, Tasks.enTasks.Copy, entry.targetNode.Path_1_Original, entry.targetNode.Path_2_Copy);
                                    }

                                }
                            }
                        }
                    }




                    if (taskToProcess == enTaskToProcess.CreateShortcut)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                // 5 CreateShortcut
                                if (entry.targetNode.task6 == Tasks.enTasks.CreateShortcut)
                                {
                                    AddTask(ref listRet, Tasks.enTasks.CreateShortcut, entry.targetNode.Path_CreateShortcutAt, entry.targetNode.Path_CreateShortcutTo);
                                }


                            }
                        }
                    }


                    if (taskToProcess == enTaskToProcess.SetAttributes)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.bActivated)
                            {
                                if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                {
                                    continue;
                                }
                                else
                                {
                                    // ELSE
                                    // SetAttributes
                                    if (entry.targetNode.task7 == Tasks.enTasks.SetAttributes)
                                    {
                                        string newAttributes = TaskPlanner.UsefulMethods.GetAttributesString(entry.targetNode.SetFileAttributes);

                                        AddTask(ref listRet, Tasks.enTasks.SetAttributes, entry.targetNode.Path_SetAttributes, newAttributes);
                                    }

                                }
                            }
                        }
                    }




                    if (taskToProcess == enTaskToProcess.DeleteFolders) // Delete Folders (if empty)
                    {
                        foreach (TaskPlanner.FileListEntry entry in fileList)
                        {
                            if (entry.targetNode.IsFolder)
                                if (entry.targetNode.bActivated)
                                {
                                    // 1 Delete
                                    if (entry.targetNode.task5 == Tasks.enTasks.Delete)
                                    {
                                        AddTask(ref listRet, Tasks.enTasks.Delete, entry.targetNode.Path_5_Delete, entry.targetNode.Path_5_Delete);
                                    }
                                }

                        }
                    }


                }

            }

            return listRet;
        }






        public void AddTask(ref List<string> listToEdit, Tasks.enTasks task, string sourcePath, string targetPath)
        {
            switch (task)
            {
                case Tasks.enTasks.CreateSubfolder:
                    listToEdit.Add("CreateSubfolder");
                    break;

                case Tasks.enTasks.Copy:
                    listToEdit.Add("Copy");
                    break;

                case Tasks.enTasks.Delete:
                    listToEdit.Add("Delete");
                    break;

                case Tasks.enTasks.Rename:
                    listToEdit.Add("Rename");
                    break;

                case Tasks.enTasks.Move:
                    listToEdit.Add("Move");
                    break;

                case Tasks.enTasks.SetAttributes:
                    listToEdit.Add("SetAttributes");
                    break;

                case Tasks.enTasks.CreateShortcut:
                    listToEdit.Add("CreateShortcut");
                    break;

                default:
                    return;
            }


            listToEdit.Add(sourcePath);
            listToEdit.Add(targetPath);

        }
    }


}