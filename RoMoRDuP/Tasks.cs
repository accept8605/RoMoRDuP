using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace RoMoRDuP.Tasks
{

    public class FileLists
    {
        public List<TaskPlanner.FileListEntry> SourceFileListBefore;  // always before
        public List<TaskPlanner.FileListEntry> TargetFileListBefore;   // always before

        public List<TaskPlanner.FileListEntry> SourceFileListAfter;
        public List<TaskPlanner.FileListEntry> TargetFileListAfter;
    }

    public class Tasks
    {
        // Resulting Tasks
        public TaskViews taskSource { get; set; }
        public TaskViews taskTarget { get; set; }

        public FileLists fileLists { get; set; }

        public TaskPlanner.TaskPlannerOptimize taskPlannerOptimize { get; set; }
        public TaskPlanner.TaskPlannerMirror taskPlannerMirror { get; set; }
        public TaskPlanner.TaskPlannerRemoveDuplicates taskPlannerRemoveDuplicates { get; set; }

        // Set UserOptions
        public UserInterface.UserInterface userOptions { get; set; }

        // Generated Duplicates-Lists
        public TaskPlanner.DuplicatesModel DuplicatesModel { get; set; }

        public WHourglass wHourglass { get; set; }

        // Playlist Updates
        public PlaylistUpdates.PlaylistUpdates playlistUpdatesMirror { get; set; }
        public PlaylistUpdates.PlaylistUpdates playlistUpdatesRemoveDuplicates { get; set; }
        public PlaylistUpdates.PlaylistUpdates playlistUpdatesOptimize { get; set; }

        public Tasks(RoMoRDuP.MainWindow mainWindow)
        {
            taskSource = new TaskViews();
            taskTarget = new TaskViews();

            fileLists = new FileLists();

            wHourglass = new WHourglass("TBD should not be displayed");

            userOptions = new UserInterface.UserInterface(fileLists, mainWindow);


            playlistUpdatesMirror = new PlaylistUpdates.PlaylistUpdates(userOptions.userOptionsMirror, fileLists);
            playlistUpdatesRemoveDuplicates = new PlaylistUpdates.PlaylistUpdates(userOptions.userOptionsRemoveDuplicates, fileLists);
            playlistUpdatesOptimize = new PlaylistUpdates.PlaylistUpdates(userOptions.userOptionsOptimize, fileLists);


            taskPlannerOptimize = new TaskPlanner.TaskPlannerOptimize(userOptions.userOptionsOptimize, taskSource, taskTarget, this, fileLists, playlistUpdatesOptimize);
            taskPlannerMirror = new TaskPlanner.TaskPlannerMirror(userOptions.userOptionsMirror, taskSource, taskTarget, this, fileLists, playlistUpdatesMirror);

            DuplicatesModel = new TaskPlanner.DuplicatesModel();

            taskPlannerRemoveDuplicates = new TaskPlanner.TaskPlannerRemoveDuplicates(userOptions.userOptionsRemoveDuplicates, taskSource, taskTarget, this, fileLists, DuplicatesModel, playlistUpdatesRemoveDuplicates);



            
        }


    }





    // ________________________TaskViews
    public class TaskViews
    {
        public TaskTreeViewModel TaskViewBefore { get; set; }
        public TaskTreeViewModel TaskViewAfter { get; set; }

        //TaskNodeViewModel TVBefore_root, TVBefore_level1, TVBefore_level2;
        //TaskNodeViewModel TVAfter_root, TVAfter_level1, TVAfter_level2;

        //public int TVBefore_curID { get; set; }
        //public int TVAfter_curID { get; set; } 
        /*
         // IN TASKPLANNERBASE
         public int IDAfterSource = 0;
        public int IDAfterTarget = 0;
        public int IDBeforeSource = 0;
        public int IDBeforeTarget = 0;
         */


        public TaskViews()
        {
            InitTaskViewAfter();
            InitTaskViewBefore();

            //TVBefore_curID = 0;
            //TVAfter_curID = 0;
        }


        private void InitTaskViewBefore()
        {
            TaskViewBefore = new TaskTreeViewModel();

            /*
            TVBefore_level1 = new TaskNodeViewModel(TVBefore_root, ++TVBefore_curID) { Name = "Level1" };

            TVBefore_level1.Children =
                new ObservableCollection<TaskNodeViewModel>
                { // Children of Level 1
                     new TaskNodeViewModel(TVBefore_level1, ++TVBefore_curID){ Name = "Level2"}
                };



            TVBefore_root = new TaskNodeViewModel(null, ++TVBefore_curID) { Name = "Root" };

            TVBefore_root.Children =
                new ObservableCollection<TaskNodeViewModel>
                { // Children of Root

                    TVBefore_level1,

                    new TaskNodeViewModel(TVBefore_root, ++TVBefore_curID) { Name = "Level1b", Children = null },

                    new TaskNodeViewModel(TVBefore_root, ++TVBefore_curID) { Name = "Level1c", Children = null },

                    new TaskNodeViewModel(TVBefore_root, ++TVBefore_curID) { Name = "Level1d", Children = null }

                };


            TaskViewBefore.Items = new ObservableCollection<TaskNodeViewModel>() { TVBefore_root };


            // Actions setzen


            enTasks action = enTasks.Delete;

            int n = 0;
            foreach (TaskNodeViewModel node in TVBefore_root.Children)
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

                String[] hashcodes = { "7e716d0e702df0505fc72e2b89467910", "a3cca2b2aa1e3b5b3b5aad99aae793c1", "7e716d0e702df0505fc72e38acf64984", "a3cca2b2aa1e3b5b3b5aad99a8529074", "7e716d0e702df050dc3680214467910", "a3cca2b2aa1e3b5b3b5acde4687074" };


                if (!node.IsFolder)
                    node.HashCode = hashcodes[n++];

                if (n > hashcodes.Length)
                    n = 0;
            }
             
             */

        }




        private void InitTaskViewAfter()
        {
            TaskViewAfter = new TaskTreeViewModel();

            /*
            TVAfter_level1 = new TaskNodeViewModel(TVAfter_root, ++TVAfter_curID) { Name = "TVAfter_Level1" };

            TVAfter_level1.Children =
                new ObservableCollection<TaskNodeViewModel>
                { // Children of Level 1
                     new TaskNodeViewModel(TVAfter_level1, ++TVAfter_curID){ Name = "TVAfter_Level2"}
                };

            

            TVAfter_root = new TaskNodeViewModel(null, ++TVAfter_curID) { Name = "TVAfter_Root" };

            TVAfter_root.Children =
                new ObservableCollection<TaskNodeViewModel>
                { // Children of Root

                    TVAfter_level1,

                    new TaskNodeViewModel(TVAfter_root, ++TVAfter_curID) { Name = "TVAfter_Level1b", Children = null },

                    new TaskNodeViewModel(TVAfter_root, ++TVAfter_curID) { Name = "TVAfter_Level1c", Children = null },

                    new TaskNodeViewModel(TVAfter_root, ++TVAfter_curID) { Name = "TVAfter_Level1d", Children = null }

                };


            TaskViewAfter.Items = new ObservableCollection<TaskNodeViewModel>() { TVAfter_root };


            // Actions setzen

            enTasks action = enTasks.Delete;

            int n = 0;
            foreach (TaskNodeViewModel node in TVAfter_root.Children)
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

                String[] hashcodes = { "7e716d0e702df0505fc72e2b89467910", "a3cca2b2aa1e3b5b3b5aad99aae793c1", "7e716d0e702df0505fc72e38acf64984", "a3cca2b2aa1e3b5b3b5aad99a8529074", "7e716d0e702df050dc3680214467910", "a3cca2b2aa1e3b5b3b5acde4687074" };


                if (!node.IsFolder)
                    node.HashCode = hashcodes[n++];

                if (n > hashcodes.Length)
                    n = 0;
             
            }
             */

        }
    }



}