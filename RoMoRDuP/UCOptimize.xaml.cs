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


namespace RoMoRDuP
{
    enum enCurrentUCOptimize { SelectSource, DataCollecting, UserRenamingOptions, UserGroupOptions, UserSelectGrouping, TaskView, ExecuteTasks }


    public partial class UCOptimize : UCFeatures_BaseClass
    {
        UserControls.UCParent ucParent;
        UserControls.UCSelectSource ucSelectSource;
        UserControls.UCDataCollecting ucDataCollecting;
        UserControls.UCOptimizeRenameOptions ucOptimizeRenameOptions;
        UserControls.UCOptimizeGroupOptions ucOptimizeGroupOptions;
        UserControls.UCOptimizeSelectGrouping ucOptimizeSelectGrouping;
        UserControls.UCTaskViewParent ucTaskViewParent;

        enCurrentUCOptimize CurrentUC;

        Tasks.Tasks tasks;

        public UCOptimize(Tasks.Tasks tasks)
        {
            this.tasks = tasks;

            InitializeComponent(); 

            if (ucParent == null)
            {
                ucParent = new UserControls.UCParent(new UserControls.degNext(Next),new UserControls.degBack(Back), new UserControls.degCancel(Cancel), new UserControls.degSelectionModeChanged(SelectionModeChanged), tasks.userOptions.userOptionsOptimize);
                ucSelectSource = new UserControls.UCSelectSource(tasks, (UserInterface.UserInterfaceBase)tasks.userOptions.userOptionsOptimize);
                ucDataCollecting = new UserControls.UCDataCollecting();
                ucOptimizeRenameOptions = new UserControls.UCOptimizeRenameOptions(tasks);
                ucOptimizeGroupOptions = new UserControls.UCOptimizeGroupOptions();
                ucOptimizeSelectGrouping = new UserControls.UCOptimizeSelectGrouping(tasks);
                ucTaskViewParent = new UserControls.UCTaskViewParent(tasks, tasks.userOptions.userOptionsOptimize, tasks.taskPlannerOptimize,UserControls.TaskViewParentType.Source, tasks.playlistUpdatesOptimize);

                InitializeComponent();

                //Initialize start panel
                ParentPanel.Children.Add(ucParent);

                ucParent.parentPanelExpert.Children.Add(ucSelectSource);
                ucParent.parentPanelExpert.Children.Add(ucDataCollecting);
                ucParent.parentPanelExpert.Children.Add(ucOptimizeRenameOptions);
                ucParent.parentPanelExpert.Children.Add(ucOptimizeGroupOptions);
                ucParent.parentPanelExpert.Children.Add(ucOptimizeSelectGrouping);
                ucParent.parentPanelExpert.Children.Add(ucTaskViewParent);
                Reset();
            }
         
        }


        void SelectionModeChanged(UserControls.enSelectedMode selectedMode)
        {
            //

        }


        public void Reset()
        {
            CurrentUC = enCurrentUCOptimize.SelectSource;
            ShowCurrentUC();

        }

        public void CloseWindow()
        {
            Reset();

            userInterfaceBase.mainWindow.ShowMain(); // TBD
        }


        public void Back() // UCParent Back
        {
            if (CurrentUC > enCurrentUCOptimize.SelectSource)
                CurrentUC--;

            ShowCurrentUC();
        }

        public void Next() // UCParent Next
        {
            if (CurrentUC < enCurrentUCOptimize.ExecuteTasks)
            {
                if (CurrentUC == enCurrentUCOptimize.TaskView)
                {
                    if (MessageBox.Show("Do you really want to proceed and execute the tasks?", "Proceed?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        CurrentUC++;
                }
                else
                    CurrentUC++;
            }

            ShowCurrentUC();
        }

        public void Cancel() // UCParent Cancel
        {
            CloseWindow();
        }


        public void ShowCurrentUC()
        {
            CollapseAll();

            switch (CurrentUC)
            {
                //enum enCurrentUCOptimize { SelectSource, DataCollecting, UserRenamingOptions, UserGroupOptions, UserSelectGrouping, TaskView, ExecuteTasks }

                case enCurrentUCOptimize.SelectSource:
                    ucSelectSource.Visibility = Visibility.Visible;
                    break;

                case enCurrentUCOptimize.DataCollecting:
                    ucDataCollecting.Visibility = Visibility.Visible;
                    break;

                case enCurrentUCOptimize.UserRenamingOptions:
                    ucOptimizeRenameOptions.Visibility = Visibility.Visible;
                    break;

                case enCurrentUCOptimize.UserGroupOptions:
                    ucOptimizeGroupOptions.Visibility = Visibility.Visible;
                    break;

                case enCurrentUCOptimize.UserSelectGrouping:
                    ucOptimizeSelectGrouping.Visibility = Visibility.Visible;
                    break;

                case enCurrentUCOptimize.TaskView:
                    ucTaskViewParent.Visibility = Visibility.Visible;
                    break;

                default:

                    break;
            }

            ShowStepString();

        }

        private void ShowStepString()
        {
            String stepString = String.Format("Optimize - Step {0} of {1} - {2}", (int)CurrentUC + 1, (int)enCurrentUCOptimize.ExecuteTasks + 1, CurrentUC);

            ucParent.StepString = stepString;
        }

        private void CollapseAll()
        {
            ucSelectSource.Visibility = Visibility.Collapsed;
            ucDataCollecting.Visibility = Visibility.Collapsed;
            ucOptimizeRenameOptions.Visibility = Visibility.Collapsed;
            ucOptimizeGroupOptions.Visibility = Visibility.Collapsed;
            ucOptimizeSelectGrouping.Visibility = Visibility.Collapsed;
            ucTaskViewParent.Visibility = Visibility.Collapsed;
        }


    }
}
