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
using System.Threading;

using System.ComponentModel;

namespace RoMoRDuP
{
    public enum enCurrentUCMirror { SelectSource, HashCodes, UserOptions, TaskView, ExecuteTasks }


    public partial class UCMirrorFolders : UCFeatures_BaseClass
    {

        UserControls.UCSelectSource ucSelectSource;
        UserControls.UCGenerateHashes ucGenerateHashes;
        UserControls.UCMirrorUserOptions ucMirrorUserOptions;
        UserControls.UCTaskViewMirrorParent ucTaskViewMirrorParent;
        UserControls.UCTaskExecute ucTaskExecute;


        UserControls.UCSelectSource ucSelectSourceEasy;
        UserControls.UCGenerateHashes ucGenerateHashesEasy;
        UserControls.UCMirrorUserOptionsEasy ucMirrorUserOptionsEasy;
        UserControls.UCTaskViewMirrorParent ucTaskViewMirrorParentEasy;
        UserControls.UCTaskExecute ucTaskExecuteEasy;


        protected enCurrentUCMirror CurrentUC;

        TasksExecution.TaskExecute taskExecute;



        public UCMirrorFolders(Tasks.Tasks tasks)
            : base(tasks)
        {
            userInterfaceBase = tasks.userOptions.userOptionsMirror;

            InitializeComponent();

            taskExecute = new TasksExecution.TaskExecute(tasks.fileLists, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror, tasks.playlistUpdatesMirror.playlistUpdatesModel);

            if (ucParent == null)
            {
                ucParent = new UserControls.UCParent(new UserControls.degNext(Next), new UserControls.degBack(Back), new UserControls.degCancel(Cancel), new UserControls.degSelectionModeChanged(SelectionModeChanged), tasks.userOptions.userOptionsMirror);


                // ________Expert
                ucSelectSource = new UserControls.UCSelectSource(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);
                ucGenerateHashes = new UserControls.UCGenerateHashes(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);
                ucMirrorUserOptions = new UserControls.UCMirrorUserOptions(tasks, tasks.userOptions.userOptionsMirror);
                ucTaskViewMirrorParent = new UserControls.UCTaskViewMirrorParent(tasks);
                ucTaskExecute = new UserControls.UCTaskExecute((UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);

                //Initialize start panel
                ParentPanel.Children.Add(ucParent);

                ucParent.parentPanelExpert.Children.Add(ucSelectSource);
                ucParent.parentPanelExpert.Children.Add(ucGenerateHashes);
                ucParent.parentPanelExpert.Children.Add(ucMirrorUserOptions);
                ucParent.parentPanelExpert.Children.Add(ucTaskViewMirrorParent);
                ucParent.parentPanelExpert.Children.Add(ucTaskExecute);



                // ____________Easy mode
                ucSelectSourceEasy = new UserControls.UCSelectSource(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);
                ucGenerateHashesEasy = new UserControls.UCGenerateHashes(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);
                ucMirrorUserOptionsEasy = new UserControls.UCMirrorUserOptionsEasy(tasks, tasks.userOptions.userOptionsMirror);
                ucTaskViewMirrorParentEasy = new UserControls.UCTaskViewMirrorParent(tasks);
                ucTaskExecuteEasy = new UserControls.UCTaskExecute((UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsMirror);

                ucParent.parentPanelEasy.Children.Add(ucSelectSourceEasy);
                ucParent.parentPanelEasy.Children.Add(ucGenerateHashesEasy);
                ucParent.parentPanelEasy.Children.Add(ucMirrorUserOptionsEasy);
                ucParent.parentPanelEasy.Children.Add(ucTaskViewMirrorParentEasy);
                ucParent.parentPanelEasy.Children.Add(ucTaskExecuteEasy);


                //Reset(); // On Main Window Call
            }

        }



        protected override void ApplyEasyModeChangesCurUC()
        {
            ApplyEasyModeChanges(CurrentUC);

        }

        protected void ApplyEasyModeChanges(enCurrentUCMirror currentUC)
        {
            if (currentUC == enCurrentUCMirror.SelectSource)
            {
                tasks.userOptions.userOptionsMirror.ApplyMirrorSelectSourceEasy();

            }
            else if (currentUC == enCurrentUCMirror.UserOptions)
            {
                tasks.userOptions.userOptionsMirror.ApplyMirrorUserOptionsEasy();

            }
        }

        private void SaveMode(enCurrentUCMirror currentUC)
        {
            if (currentUC == enCurrentUCMirror.SelectSource)
            {
                tasks.userOptions.userOptionsMirror.ModeSelectSource = selectedMode;
            }
            else if (currentUC == enCurrentUCMirror.UserOptions)
            {
                tasks.userOptions.userOptionsMirror.ModeUserOptions = selectedMode;
            }

        }

        protected override void base_SaveMode()
        {
            SaveMode(CurrentUC);
        }



        private void LoadModeSelectionFromProfile(enCurrentUCMirror currentUC)
        {
            lastSelectedThisScreen = UserControls.enSelectedMode.None; // Workaround wegen bug

            if (currentUC == enCurrentUCMirror.SelectSource)
            {
                userInterfaceBase.bTabcontrolModeSelectEnabled = true;
                ucParent.TabControlChangeSelectedIndex(tasks.userOptions.userOptionsMirror.ModeSelectSource);

                lastSelectedThisScreen = tasks.userOptions.userOptionsMirror.ModeSelectSource;
            }
            else if (currentUC == enCurrentUCMirror.UserOptions)
            {
                userInterfaceBase.bTabcontrolModeSelectEnabled = true;
                ucParent.TabControlChangeSelectedIndex(tasks.userOptions.userOptionsMirror.ModeUserOptions);

                lastSelectedThisScreen = tasks.userOptions.userOptionsMirror.ModeUserOptions;
            }
            else
            {
                userInterfaceBase.bTabcontrolModeSelectEnabled = false;
            }




        }


        public void Reset()
        {
            CurrentUC = enCurrentUCMirror.SelectSource;
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

            if (CurrentUC > enCurrentUCMirror.SelectSource)
                CurrentUC--;

            ShowCurrentUC();
        }

        public void Next() // UCParent Next
        {
            SaveMode(CurrentUC);

            if (selectedMode == UserControls.enSelectedMode.Easy)
                ApplyEasyModeChanges(CurrentUC);


            enCurrentUCMirror oldUC = CurrentUC;

            // weiter
            if (CurrentUC < enCurrentUCMirror.ExecuteTasks)
            {
                if (CurrentUC == enCurrentUCMirror.TaskView)
                {
                    //if (MessageBox.Show("Do you really want to proceed and execute the tasks?", "Proceed?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    WMBExecuteTasks mbExecuteTasks = new WMBExecuteTasks(userInterfaceBase.messageBoxUI);
                    mbExecuteTasks.Owner = userInterfaceBase.mainWindow;

                    if ((bool)mbExecuteTasks.ShowDialog())
                        CurrentUC++;
                }
                else if ((enCurrentUCMirror)((short)CurrentUC) == enCurrentUCMirror.HashCodes)
                {
                    if (userInterfaceBase.ProcessCurrentSize == userInterfaceBase.ProcessTargetSize) // nur weiter wenn fertig
                        CurrentUC++;
                }
                else if ((enCurrentUCMirror)((short)CurrentUC) == enCurrentUCMirror.SelectSource)
                {
                    if (System.IO.Directory.Exists(userInterfaceBase.strSelectSourcePath))
                    {
                        if (System.IO.Directory.Exists(userInterfaceBase.strSelectTargetPath))
                        {
                            CurrentUC++;
                        }
                        else
                            MessageBox.Show("SelectSource: TargetPath does not Exist!");
                    }
                    else
                        MessageBox.Show("SelectSource: SourcePath does not Exist!");
                }
                else
                    CurrentUC++;
            }


            // Actions
            if ((CurrentUC == enCurrentUCMirror.HashCodes) && (oldUC != enCurrentUCMirror.HashCodes))
            {
                tasks.taskPlannerMirror.CreateFileTree();
                //tasks.playlistUpdatesMirror.SearchForPlaylists();
            }

            if ((CurrentUC == enCurrentUCMirror.TaskView) && (oldUC != enCurrentUCMirror.TaskView))
            {
                TaskPlanner.GlobalVar.lastSelectedView = TaskPlanner.enLastSelectedView.None;

                tasks.taskPlannerMirror.CreateMirror_Source_Target_Before_Actions();

                tasks.taskPlannerMirror.CreateAfterViews();

                if (userInterfaceBase.bExpertOptions_Visible)
                    ucTaskViewMirrorParent.TabControlChangeSelectedIndex(0);
                else
                    ucTaskViewMirrorParentEasy.TabControlChangeSelectedIndex(0);

                //ucTaskViewMirrorParent.UpdateView(); // TBD automaticly done over EditViews from TAB Event?
                //ucTaskViewMirrorParentEasy.UpdateView();
            }

            if ((CurrentUC == enCurrentUCMirror.UserOptions) && (oldUC != enCurrentUCMirror.UserOptions))
                tasks.taskPlannerMirror.CreateDuplicateInfo();

            // Show new Page
            ShowCurrentUC();

            if ((CurrentUC == enCurrentUCMirror.ExecuteTasks) && (oldUC != enCurrentUCMirror.ExecuteTasks))
            {
                tasks.taskPlannerMirror.EditViews(TaskPlanner.GlobalVar.lastSelectedView, true, false, null, false, taskExecute, null, false, UserControls.TaskViewParentType.Source);  // UpdateAfterViews // TBD not taskplannermirror
                //tasks.playlistUpdatesMirror.GeneratePlaylistUpdates();

                //taskExecute.handleTaskExecute(); //using EditView Now
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
                case enCurrentUCMirror.SelectSource:
                    ucSelectSource.Visibility = Visibility.Visible;
                    ucSelectSourceEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Visible;

                    userInterfaceBase.BackButtonEnabled = false;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCMirror.HashCodes:
                    ucGenerateHashes.Visibility = Visibility.Visible;
                    ucGenerateHashesEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Visible;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCMirror.UserOptions:
                    ucMirrorUserOptions.Visibility = Visibility.Visible;
                    ucMirrorUserOptionsEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Visible;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCMirror.TaskView:
                    ucTaskViewMirrorParent.Visibility = Visibility.Visible;
                    ucTaskViewMirrorParentEasy.Visibility = Visibility.Visible;
                    userInterfaceBase.ParentScrollBarVisibility = ScrollBarVisibility.Disabled;

                    userInterfaceBase.BackButtonEnabled = true;
                    userInterfaceBase.NextButtonEnabled = true;
                    userInterfaceBase.FinishedButtonVisibility = System.Windows.Visibility.Hidden;
                    break;

                case enCurrentUCMirror.ExecuteTasks:
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
            String stepString = String.Format("Mirror folders - Step {0} of {1} - {2}", (int)CurrentUC + 1, (int)enCurrentUCMirror.ExecuteTasks + 1, CurrentUC);

            ucParent.StepString = stepString;
        }

        private void CollapseAll()
        {
            ucSelectSource.Visibility = Visibility.Collapsed;
            ucGenerateHashes.Visibility = Visibility.Collapsed;
            ucMirrorUserOptions.Visibility = Visibility.Collapsed;
            ucTaskViewMirrorParent.Visibility = Visibility.Collapsed;
            ucTaskExecute.Visibility = Visibility.Collapsed;

            ucSelectSourceEasy.Visibility = Visibility.Collapsed;
            ucGenerateHashesEasy.Visibility = Visibility.Collapsed;
            ucMirrorUserOptionsEasy.Visibility = Visibility.Collapsed;
            ucTaskViewMirrorParentEasy.Visibility = Visibility.Collapsed;
            ucTaskExecuteEasy.Visibility = Visibility.Collapsed;
        }


    }
}
