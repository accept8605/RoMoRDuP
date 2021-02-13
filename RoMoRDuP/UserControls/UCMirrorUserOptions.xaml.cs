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
    /// Interaktionslogik für UCMirrorUserOptions.xaml
    /// </summary>
    public partial class UCMirrorUserOptions : UserControl
    {
        Tasks.Tasks tasks;

        public UserInterface.UserOptionsMirror userOptionsMirror { get; set; }

        public UCMirrorUserOptions(Tasks.Tasks tasks, UserInterface.UserOptionsMirror userOptionsMirror)
        {

            this.tasks = tasks;

            this.userOptionsMirror = userOptionsMirror;

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
