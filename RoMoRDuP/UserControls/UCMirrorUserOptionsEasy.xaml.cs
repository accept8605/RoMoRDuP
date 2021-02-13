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
    /// Interaktionslogik für UCMirrorUserOptionsEasy.xaml
    /// </summary>
    public partial class UCMirrorUserOptionsEasy : UserControl
    {
        Tasks.Tasks tasks;

        public UserInterface.UserOptionsMirror userOptionsMirror { get; set; }

        public UCMirrorUserOptionsEasy(Tasks.Tasks tasks, UserInterface.UserOptionsMirror userOptionsMirror)
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




        private void Click_Button_SyncOneWayRA(object sender, RoutedEventArgs e)
        {
            userOptionsMirror.OptionsMirrorEasy = UserInterface.enOptionsMirrorEasy.SyncOneWayRemove;
        }

        private void Click_Button_SyncBothWays(object sender, RoutedEventArgs e)
        {
            userOptionsMirror.OptionsMirrorEasy = UserInterface.enOptionsMirrorEasy.SyncBothWays;
        }

        private void Click_Button_SyncOneWayKeep(object sender, RoutedEventArgs e)
        {
            userOptionsMirror.OptionsMirrorEasy = UserInterface.enOptionsMirrorEasy.SyncOneWayLeave;
        }

    }
}
