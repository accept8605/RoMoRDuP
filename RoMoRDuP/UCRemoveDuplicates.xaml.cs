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
using System.Windows.Shapes;

using System.ComponentModel;

using System.Threading;

namespace RoMoRDuP
{
    public enum enCurrentUCRemoveDuplicates { SelectSource, HashCodes, UserOptions, TaskView, ExecuteTasks }


    public partial class UCRemoveDuplicates : UCFeatures_BaseClass
    {

        UserControls.UCSelectSource ucSelectSource;
        UserControls.UCGenerateHashes ucGenerateHashes;
        UserControls.UCRemoveDuplicatesUserOptions ucRDUserOptions;
        UserControls.UCTaskViewParent ucTaskViewParent;
        UserControls.UCTaskExecute ucTaskExecute;

        UserControls.UCSelectSource ucSelectSourceEasy;
        UserControls.UCGenerateHashes ucGenerateHashesEasy;
        UserControls.UCRemoveDuplicatesUserOptions ucRDUserOptionsEasy;
        UserControls.UCTaskViewParent ucTaskViewParentEasy;
        UserControls.UCTaskExecute ucTaskExecuteEasy;


        enCurrentUCRemoveDuplicates CurrentUC;

        TasksExecution.TaskExecute taskExecute;


        public UCRemoveDuplicates(Tasks.Tasks tasks)
            : base(tasks)
        {

            userInterfaceBase = tasks.userOptions.userOptionsRemoveDuplicates;

            taskExecute = new TasksExecution.TaskExecute(tasks.fileLists, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsRemoveDuplicates, tasks.playlistUpdatesRemoveDuplicates.playlistUpdatesModel);

            if (ucParent == null)
            {
                ucParent = new UserControls.UCParent(new UserControls.degNext(Next),new UserControls.degBack(Back), new UserControls.degCancel(Cancel), new UserControls.degSelectionModeChanged(SelectionModeChanged), tasks.userOptions.userOptionsRemoveDuplicates);

                // Expert mode
                ucSelectSource = new UserControls.UCSelectSource(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsRemoveDuplicates);
                ucGenerateHashes = new UserControls.UCGenerateHashes(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsRemoveDuplicates);
                ucRDUserOptions = new UserControls.UCRemoveDuplicatesUserOptions(tasks);
                ucTaskViewParent = new UserControls.UCTaskViewParent(tasks, tasks.userOptions.userOptionsRemoveDuplicates, tasks.taskPlannerRemoveDuplicates,UserControls.TaskViewParentType.Source, tasks.playlistUpdatesRemoveDuplicates);
                ucTaskExecute = new UserControls.UCTaskExecute(tasks.userOptions.userOptionsRemoveDuplicates);

                InitializeComponent();

                //Initialize start panel
                ParentPanel.Children.Add(ucParent);

                ucParent.parentPanelExpert.Children.Add(ucSelectSource);
                ucParent.parentPanelExpert.Children.Add(ucGenerateHashes);
                ucParent.parentPanelExpert.Children.Add(ucRDUserOptions);
                ucParent.parentPanelExpert.Children.Add(ucTaskViewParent);
                ucParent.parentPanelExpert.Children.Add(ucTaskExecute);

                // Easy mode
                ucSelectSourceEasy = new UserControls.UCSelectSource(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsRemoveDuplicates);
                ucGenerateHashesEasy = new UserControls.UCGenerateHashes(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsRemoveDuplicates);
                ucRDUserOptionsEasy = new UserControls.UCRemoveDuplicatesUserOptions(tasks);
                ucTaskViewParentEasy = new UserControls.UCTaskViewParent(tasks, tasks.userOptions.userOptionsRemoveDuplicates, tasks.taskPlannerRemoveDuplicates, UserControls.TaskViewParentType.Source, tasks.playlistUpdatesRemoveDuplicates);
                ucTaskExecuteEasy = new UserControls.UCTaskExecute(tasks.userOptions.userOptionsRemoveDuplicates);

                ucParent.parentPanelEasy.Children.Add(ucSelectSourceEasy);
                ucParent.parentPanelEasy.Children.Add(ucGenerateHashesEasy);
                ucParent.parentPanelEasy.Children.Add(ucRDUserOptionsEasy);
                ucParent.parentPanelEasy.Children.Add(ucTaskViewParentEasy);
                ucParent.parentPanelEasy.Children.Add(ucTaskExecuteEasy);


                //Reset(); // On Main Window Call

            }
         
        }




        protected override void ApplyEasyModeChangesCurUC()
        {
            ApplyEasyModeChanges(CurrentUC);

        }

        protected void ApplyEasyModeChanges(enCurrentUCRemoveDuplicates currentUC)
        {
            if (currentUC == enCurrentUCRemoveDuplicates.SelectSource)
            {
                tasks.userOptions.userOptionsRemoveDuplicates.ApplyRDSelectSourceEasy();

            }
        }

        private void SaveMode(enCurrentUCRemoveDuplicates currentUC)
        {
            if (currentUC == enCurrentUCRemoveDuplicates.SelectSource)
            {
                tasks.userOptions.userOptionsRemoveDuplicates.ModeSelectSource = selectedMode;
            }


        }


        protected override void base_SaveMode()
        {
            SaveMode(CurrentUC);
        }


        private void LoadModeSelectionFromProfile(enCurrentUCRemoveDuplicates currentUC)
        {
            lastSelectedThisScreen = UserControls.enSelectedMode.None; // Workaround wegen bug

            if (currentUC == enCurrentUCRemoveDuplicates.SelectSource)
            {
                userInterfaceBase.bTabcontrolModeSelectEnabled = true;
                ucParent.TabControlChangeSelectedIndex(tasks.userOptions.userOptionsRemoveDuplicates.ModeSelectSource);

                lastSelectedThisScreen = tasks.userOptions.userOptionsRemoveDuplicates.ModeSelectSource;
            }
            else
            {
                userInterfaceBase.bTabcontrolModeSelectEnabled = false;
            }

        }


        


        public void Reset()
        {
            CurrentUC = enCurrentUCRemoveDuplicates.SelectSource;
            ShowCurrentUC();

        }

        public void CloseWindow()
        {
            Reset();

            userInterfaceBase.mainWindow.ShowMain();
        }


        public void Back() // UCParent Back
        {
            SaveMode(CurrentUC);

            if (selectedMode == UserControls.enSelectedMode.Easy)
                ApplyEasyModeChanges(CurrentUC);

            if (CurrentUC > enCurrentUCRemoveDuplicates.SelectSource)
                CurrentUC--;

            ShowCurrentUC();
        }




        public void Next() // UCParent Next
        {
            SaveMode(CurrentUC);

            if (selectedMode == UserControls.enSelectedMode.Easy)
                ApplyEasyModeChanges(CurrentUC);


            enCurrentUCRemoveDuplicates oldUC = CurrentUC;

            // weiter
            if (CurrentUC < enCurrentUCRemoveDuplicates.ExecuteTasks)
            {
                if (CurrentUC == enCurrentUCRemoveDuplicates.TaskView)
                {
                    //if (MessageBox.Show("Do you really want to proceed and execute the tasks?", "Proceed?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    WMBExecuteTasks mbExecuteTasks = new WMBExecuteTasks(userInterfaceBase.messageBoxUI);
                    mbExecuteTasks.Owner = userInterfaceBase.mainWindow;

                    if ((bool)mbExecuteTasks.ShowDialog())
                        CurrentUC++;
                }
                else if ((enCurrentUCRemoveDuplicates)((short)CurrentUC) == enCurrentUCRemoveDuplicates.HashCodes)
                {
                    if (userInterfaceBase.ProcessCurrentSize == userInterfaceBase.ProcessTargetSize) // nur weiter wenn fertig
                        CurrentUC++;
                }
                else if ((enCurrentUCRemoveDuplicates)((short)CurrentUC) == enCurrentUCRemoveDuplicates.SelectSource)
                {
                    if (System.IO.Directory.Exists(userInterfaceBase.strSelectSourcePath))
                        CurrentUC++;
                    else
                        MessageBox.Show("SelectSource: SourcePath does not Exist!");
                }
                else
                    CurrentUC++;
            }


            // Actions
            if ((CurrentUC == enCurrentUCRemoveDuplicates.HashCodes) && (oldUC != enCurrentUCRemoveDuplicates.HashCodes))
            {
                tasks.taskPlannerRemoveDuplicates.CreateFileTree();
                //tasks.playlistUpdatesRemoveDuplicates.SearchForPlaylists();
            }

            if ((CurrentUC == enCurrentUCRemoveDuplicates.UserOptions) && (oldUC != enCurrentUCRemoveDuplicates.UserOptions))
            {
                tasks.taskPlannerRemoveDuplicates.CreateDuplicateInfo();
                //tasks.taskPlannerRemoveDuplicates.Create_RD_Duplicate_Groups(); // Aus TaskPlannerBase aufgerufen
            }

            if ((CurrentUC == enCurrentUCRemoveDuplicates.TaskView) && (oldUC != enCurrentUCRemoveDuplicates.TaskView))
            {
                TaskPlanner.GlobalVar.lastSelectedView = TaskPlanner.enLastSelectedView.None;

                tasks.taskPlannerRemoveDuplicates.CreateRD_Before_Actions();

                tasks.taskPlannerRemoveDuplicates.CreateAfterViews();

                if (userInterfaceBase.bExpertOptions_Visible)
                    ucTaskViewParent.TabControlChangeSelectedIndex(0);
                else
                    ucTaskViewParentEasy.TabControlChangeSelectedIndex(0);

                //ucTaskViewParent.UpdateView(); // TBD automaticly done over EditViews from TAB Event?
                //ucTaskViewParentEasy.UpdateView();
            }

            

            // Show new Page
            ShowCurrentUC();

            if ((CurrentUC == enCurrentUCRemoveDuplicates.ExecuteTasks) && (oldUC != enCurrentUCRemoveDuplicates.ExecuteTasks))
            {
                tasks.taskPlannerRemoveDuplicates.EditViews(TaskPlanner.GlobalVar.lastSelectedView, true, false, null, false, taskExecute, null, false, UserControls.TaskViewParentType.Source);  // UpdateAfterViews // TBD not taskplannermirror
                //tasks.playlistUpdatesRemoveDuplicates.GeneratePlaylistUpdates();
            }
        }




        public void Cancel() // UCParent Cancel
        {
            taskExecute.CancelExecute();

            CloseWindow();
        }


        public void ShowCurrentUC()
        {
            LoadModeSelectionFromProfile(CurrentUC);

            CollapseAll();

            switch (CurrentUC)
            {
                case enCurrentUCRemoveDuplicates.SelectSource:
                    ucSelectSource.Visibility = Visibility.Visible;
                    ucSelectSourceEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Visible;

                    userInterfaceBase.BackButtonEnabled = false;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCRemoveDuplicates.HashCodes:
                    ucGenerateHashes.Visibility = Visibility.Visible;
                    ucGenerateHashesEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Hidden;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCRemoveDuplicates.UserOptions:
                    ucRDUserOptions.Visibility = Visibility.Visible;
                    ucRDUserOptionsEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Disabled;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCRemoveDuplicates.TaskView:
                    ucTaskViewParent.Visibility = Visibility.Visible;
                    ucTaskViewParentEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Disabled;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCRemoveDuplicates.ExecuteTasks:
                    ucTaskExecute.Visibility = Visibility.Visible;
                    ucTaskExecuteEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Disabled;

                    userInterfaceBase.BackButtonEnabled = false;
                    userInterfaceBase.NextButtonEnabled = false;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                default:

                    break;
            }

            ShowStepString();

        }

        private void ShowStepString()
        {
            String stepString = String.Format("Remove duplicates - Step {0} of {1} - {2}", (int)CurrentUC + 1, (int)enCurrentUCRemoveDuplicates.ExecuteTasks + 1, CurrentUC);

            ucParent.StepString = stepString;
        }


        private void CollapseAll()
        {
            ucSelectSource.Visibility = Visibility.Collapsed;
            ucGenerateHashes.Visibility = Visibility.Collapsed;
            ucRDUserOptions.Visibility = Visibility.Collapsed;
            ucTaskViewParent.Visibility = Visibility.Collapsed;
            ucTaskExecute.Visibility = Visibility.Collapsed;

            ucSelectSourceEasy.Visibility = Visibility.Collapsed;
            ucGenerateHashesEasy.Visibility = Visibility.Collapsed;
            ucRDUserOptionsEasy.Visibility = Visibility.Collapsed;
            ucTaskViewParentEasy.Visibility = Visibility.Collapsed;
            ucTaskExecuteEasy.Visibility = Visibility.Collapsed;
        }


    }
}
