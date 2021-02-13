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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCTaskViewParent.xaml
    /// </summary>
    /// 

    public enum TaskViewParentType { Source, Target }
    public delegate void degUpdateAfterView();
    public delegate void degUpdateView();

    public partial class UCTaskViewParent : UserControl
    {

        Tasks.Tasks tasks;

        public TabControl BeforeAfter_TabControl { get; set; }

        UserControls.UCTaskViewChildBefore ucTaskViewChildBefore;
        UserControls.UCTaskViewChildAfter ucTaskViewChildAfter;
        UserControls.UCPlaylistUpdates ucPlaylistUpdates;

        TaskViewParentType taskViewParentType;

        TaskPlanner.TaskPlannerBase taskPlannerBase { get; set; }

        public UserInterface.UserInterfaceBase userOptionsBase { get; set; }

        public UCTaskViewParent(Tasks.Tasks tasks, UserInterface.UserInterfaceBase userOptionsBase, TaskPlanner.TaskPlannerBase taskPlannerBase,TaskViewParentType taskViewParentType, PlaylistUpdates.PlaylistUpdates playlistUpdates)
        {
            this.taskViewParentType = taskViewParentType;

            this.userOptionsBase = userOptionsBase;

            this.taskPlannerBase = taskPlannerBase;

            InitializeComponent();

            this.BeforeAfter_TabControl = tabControl;

            this.tasks = tasks;

            ucTaskViewChildAfter = new UCTaskViewChildAfter(tasks, taskViewParentType, taskPlannerBase, new degUpdateAfterView(UpdateAfterView), userOptionsBase);
            ucTaskViewChildBefore = new UCTaskViewChildBefore(tasks, taskViewParentType, userOptionsBase);
            ucPlaylistUpdates = new UCPlaylistUpdates(playlistUpdates);

            TaskViewParent_After.Children.Add(ucTaskViewChildAfter);
            TaskViewParent_Before.Children.Add(ucTaskViewChildBefore);
            TaskViewParent_Playlists.Children.Add(ucPlaylistUpdates);
        }

        public void UpdateView()
        {
            ucTaskViewChildBefore.UpdateView();
            ucTaskViewChildAfter.UpdateView();
        }



        private void Click_Button_SaveLog(object sender, RoutedEventArgs e)
        {
            // 1: MessageBox
            MessageBox.Show("Specify path of a log-file folder.\nYou should have write access there!\nMultiple files will get added!\nAlways create a new Folder, dont save in an existing folder!");

            // 2: Show OpenFolder Dialog
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;

                if (!System.IO.Directory.EnumerateFileSystemEntries(path).Any()) // Folder is empty?
                {

                    // 4: Save Profile
                    tasks.userOptions.profiles.SaveProfile(path + "\\Profile.RMDPProf");

                    // 5: Update TaskView + Playlist Updates
                    taskPlannerBase.EditViews(TaskPlanner.GlobalVar.lastSelectedView, true, true, path, true, null, null, false, TaskViewParentType.Source);  // UpdateAfterViews
                   
                }
                else
                {
                    MessageBox.Show("Folder is not empty!");

                }
            }

        }

        public void TabControlChangeSelectedIndex(int index)
        {
            tabControl.SelectedIndex = index;
        }

        private void Click_Button_BugReport(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.Primitives.Selector item = sender as System.Windows.Controls.Primitives.Selector; //The sender is a type of TabItem...

            FrameworkElement selectedElement = item.SelectedItem as FrameworkElement;

            SelectionChanged(selectedElement);
        }

        bool PlaylistUpdatesActive = false;

        public void SelectionChanged(FrameworkElement selectedElement)
        {

            if (selectedElement != null)
            {

                TaskPlanner.enLastSelectedView selectedView = TaskPlanner.enLastSelectedView.None;
                
                if (selectedElement.Name == "BeforeView") // new SelectedView
                {
                    PlaylistUpdatesActive = false;

                    if (taskViewParentType == TaskViewParentType.Source)
                    {
                        selectedView = TaskPlanner.enLastSelectedView.SourceBefore;
                        userOptionsBase.visSource = System.Windows.Visibility.Visible;
                        userOptionsBase.visTarget = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        selectedView = TaskPlanner.enLastSelectedView.TargetBefore;
                        userOptionsBase.visSource = System.Windows.Visibility.Collapsed;
                        userOptionsBase.visTarget = System.Windows.Visibility.Visible;
                    }


                    if ( (selectedView!= TaskPlanner.enLastSelectedView.None) && (selectedView != TaskPlanner.GlobalVar.lastSelectedView) )
                    {
                        if (taskPlannerBase.EditViews(TaskPlanner.GlobalVar.lastSelectedView, false, false, null, false, null, new degUpdateView(UpdateView), true, taskViewParentType)) // UpdateAfterViews
                        {
                            //userOptionsBase.UpdateTaskViewSpaceData(taskViewParentType); // UpdateSpaceData
                            TaskPlanner.GlobalVar.lastSelectedView = selectedView;  // in EditViews
                        }
                    }

                }
                else if (selectedElement.Name == "AfterView") // new SelectedView
                {
                    PlaylistUpdatesActive = false;

                    if (taskViewParentType == TaskViewParentType.Source)
                    {
                        selectedView = TaskPlanner.enLastSelectedView.SourceAfter;
                        userOptionsBase.visSource = System.Windows.Visibility.Visible;
                        userOptionsBase.visTarget = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        selectedView = TaskPlanner.enLastSelectedView.TargetAfter;
                        userOptionsBase.visSource = System.Windows.Visibility.Collapsed;
                        userOptionsBase.visTarget = System.Windows.Visibility.Visible;
                    }

                    if ((selectedView != TaskPlanner.enLastSelectedView.None) && (selectedView != TaskPlanner.GlobalVar.lastSelectedView))
                    {
                        if (taskPlannerBase.EditViews(TaskPlanner.GlobalVar.lastSelectedView, false, false, null, false, null, new degUpdateView(UpdateView), true, taskViewParentType))  // UpdateAfterViews
                        {
                            //userOptionsBase.UpdateTaskViewSpaceData(taskViewParentType); // UpdateSpaceData
                            TaskPlanner.GlobalVar.lastSelectedView = selectedView;
                        }
                    }

                    //ucTaskViewChildAfter.UpdateView(); // TBD prüfen
                }
                else if (selectedElement.Name == "PlaylistUpdates")
                {
                    if(!PlaylistUpdatesActive)
                    {
                        taskPlannerBase.EditViews(TaskPlanner.GlobalVar.lastSelectedView, true, false, null, false, null, null, true, taskViewParentType);  // UpdateAfterViews
                        //userOptionsBase.UpdateTaskViewSpaceData(taskViewParentType); // UpdateSpaceData

                        //ucPlaylistUpdates.playlistUpdates.GeneratePlaylistUpdates();
                    }

                    PlaylistUpdatesActive = true;
                }
            }

        }



        public void UpdateAfterView()
        {
            // TBD not takplannermirror
            taskPlannerBase.EditViews(TaskPlanner.GlobalVar.lastSelectedView, false, false, null, false, null, new degUpdateView(ucTaskViewChildAfter.UpdateView), true, taskViewParentType);  // Update beforeViews by Afterviews
            //taskPlannerBase.EditViews(TaskPlanner.enLastSelectedView.SourceBefore); // UpdateAfterView by beforeView

            //userOptionsBase.UpdateTaskViewSpaceData(taskViewParentType); // UpdateSpaceData

            //ucTaskViewChildAfter.UpdateView();
        }


        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = true;
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            ((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = false;
        }




    }
}
