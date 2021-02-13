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

using System.ComponentModel;

namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCTaskViewChildBefore.xaml
    /// </summary>
    public partial class UCTaskViewChildBefore : UserControl, INotifyPropertyChanged
    {

        Tasks.TaskTreeViewModel int_TreeViewModel_Before;

        public Tasks.TaskTreeViewModel TreeModel
        {
            get
            {
                return int_TreeViewModel_Before;
            }
            set
            {
                int_TreeViewModel_Before = value;
                OnPropertyChanged("TreeModel");
            }
        }


        Tasks.Tasks tasks;

        TaskViewParentType taskViewParentType;

        UserInterface.UserInterfaceBase userOptions;

        public UCTaskViewChildBefore(Tasks.Tasks _tasks, TaskViewParentType taskViewParentType, UserInterface.UserInterfaceBase userOptions)
        {
            this.tasks = _tasks;

            this.userOptions = userOptions;

            this.taskViewParentType = taskViewParentType;

            InitializeComponent();
        }


        public void UpdateView()
        {
            
            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.BeginInvoke( // Invoke does not always work
                   (Action)(() =>
                   {
                       if (taskViewParentType == TaskViewParentType.Source)
                       {
                           TreeModel = tasks.taskSource.TaskViewBefore;

                           TreeModel.Items = tasks.taskSource.TaskViewBefore.Items;


                       }
                       else if (taskViewParentType == TaskViewParentType.Target)
                       {
                           TreeModel = tasks.taskTarget.TaskViewBefore;

                           TreeModel.Items = tasks.taskTarget.TaskViewBefore.Items;
                       }

                       TreeView_Before.UpdateLayout();
                   }
                    ), null);
            }
            


            //ExpandTreeView(TreeView_Before);



        }

        /*
        private void ExpandTreeView(ItemsControl control)
        {
            try
            {
                foreach (object o in TreeModel.Items)
                {
                    control.UpdateLayout();
                    TreeViewItem item = (TreeViewItem)control.ItemContainerGenerator.ContainerFromItem(o);
                    control.UpdateLayout();
                    item.IsExpanded = true;
                    //ExpandTreeView(item);
                }

            }
            catch (Exception ex)
            {
                //

            }
        }
         */



        private void TS_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineUp();
            }
            else
            {
                scrollviewer.LineDown();
            }
            e.Handled = true;
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

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            //((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = true; // TBD Buggy - ToolTipp is shown empty
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            //((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = false; // TBD Buggy - ToolTipp is shown empty
        }



        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }

                if (sender != null)
                {
                    Tasks.TaskNodeViewModel node = ((TreeViewItem)sender).DataContext as Tasks.TaskNodeViewModel;

                    if (node != null)
                    {
                        // Datei Öffnen
                        try
                        {
                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                            process.EnableRaisingEvents = false;
                            process.StartInfo.FileName = node.Path_1_Original;
                            process.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Cant execute " + node.Path_1_Original + " : " + ex.Message);
                        }
                    }
                }

            }
        }





    }

}
