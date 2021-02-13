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
    /// Interaktionslogik für UCTaskViewMirrorParent.xaml
    /// </summary>
    public partial class UCTaskViewMirrorParent : UserControl
    {
        UCTaskViewParent ucParentSource = null;
        UCTaskViewParent ucParentTarget = null;

        Tasks.Tasks tasks;

        UserInterface.UserInterfaceBase userOptionsBase { get; set; }

        public UCTaskViewMirrorParent(Tasks.Tasks tasks)
        {
            this.tasks = tasks;

            this.userOptionsBase = tasks.userOptions.userOptionsMirror;

            InitializeComponent();

            if (ucParentSource == null)
            {
                ucParentSource = new UserControls.UCTaskViewParent(tasks, userOptionsBase, tasks.taskPlannerMirror, TaskViewParentType.Source, tasks.playlistUpdatesMirror);

                //Initialize start panel
                TaskViewSourcePanel.Children.Add(ucParentSource);
            }

            if (ucParentTarget == null)
            {
                ucParentTarget = new UserControls.UCTaskViewParent(tasks, userOptionsBase, tasks.taskPlannerMirror,TaskViewParentType.Target, tasks.playlistUpdatesMirror);

                //Initialize start panel
                TaskViewTargetPanel.Children.Add(ucParentTarget);
            }
;
        }

        public void TabControlChangeSelectedIndex(int index)
        {
            tabControl.SelectedIndex = index;
            ucParentSource.TabControlChangeSelectedIndex(index);
            ucParentTarget.TabControlChangeSelectedIndex(index);
        }

        public void UpdateView()
        {
            ucParentSource.UpdateView();
            ucParentTarget.UpdateView();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.Primitives.Selector item = sender as System.Windows.Controls.Primitives.Selector; //The sender is a type of TabItem...

            HeaderedContentControl selectedElement = item.SelectedItem as HeaderedContentControl;

            SelectionChanged(selectedElement);

            //UpdateView(); // TBD prüfen
        }

        public void SelectionChanged(HeaderedContentControl selectedElement)
        {
            if (selectedElement != null)
            {
                if ((string)selectedElement.Header == "SourceFolder")
                {
                    

                    if(ucParentSource.BeforeAfter_TabControl != null)
                        ucParentSource.SelectionChanged(ucParentSource.BeforeAfter_TabControl.SelectedItem as FrameworkElement);
                }
                else if ((string)selectedElement.Header == "TargetFolder")
                {
                    if (ucParentTarget.BeforeAfter_TabControl != null)
                        ucParentTarget.SelectionChanged(ucParentTarget.BeforeAfter_TabControl.SelectedItem as FrameworkElement);
                }
     
            }

        }
    }
}
