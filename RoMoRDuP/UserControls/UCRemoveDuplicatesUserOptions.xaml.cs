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
    /// Interaktionslogik für UCRemoveDuplicatesUserOptions.xaml
    /// </summary>
    public partial class UCRemoveDuplicatesUserOptions : UserControl
    {
        public TaskPlanner.DuplicatesModel duplicates { get; set; }

        Tasks.Tasks tasks;

        public UCRemoveDuplicatesUserOptions(Tasks.Tasks tasks)
        {
            this.tasks = tasks;

            duplicates = tasks.DuplicatesModel;

            InitializeComponent();
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
