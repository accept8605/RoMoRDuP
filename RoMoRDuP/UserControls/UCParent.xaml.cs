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
    public delegate void degNext();
    public delegate void degBack();
    public delegate void degCancel();

    public enum enSelectedMode { Easy, Expert, None };
    public delegate void degSelectionModeChanged(enSelectedMode selectedMode);

    /// <summary>
    /// Interaktionslogik für UCParent.xaml
    /// </summary>
    public partial class UCParent : UserControl
    {
        degNext Next;
        degBack Back;
        degCancel Cancel;

        degSelectionModeChanged SelectionModeChanged;

        public UserInterface.UserInterfaceBase userOptions { get; set; }

        public UCParent(degNext next, degBack back, degCancel cancel, degSelectionModeChanged SelectionModeChanged, UserInterface.UserInterfaceBase userOptions)
        {
            this.userOptions = userOptions;

            this.SelectionModeChanged = SelectionModeChanged;

            InitializeComponent();

            Next = next;
            Back = back;
            Cancel = cancel;
        }

        public void TabControlChangeSelectedIndex(enSelectedMode mode)
        {
            if(mode != enSelectedMode.None )
                tabControl.SelectedIndex = (int)mode;
        }

        public Grid parentPanelEasy
        {
            get { return ParentPanelEasy; }
        }

        public Grid parentPanelExpert
        {
            get { return ParentPanelExpert; }
        }

        public String StepString
        {
            get { return Label_Step.Content.ToString(); }
            set { Label_Step.Content = value; }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.Primitives.Selector item = sender as System.Windows.Controls.Primitives.Selector; //The sender is a type of TabItem...

            FrameworkElement selectedElement = item.SelectedItem as FrameworkElement;

            if (selectedElement != null)
            {
                if (selectedElement.Name == "TabItem_EasyMode") // new SelectedView
                {
                    SelectionModeChanged(enSelectedMode.Easy);
                }
                else if (selectedElement.Name == "TabItem_ExpertMode") // new SelectedView
                {
                    SelectionModeChanged(enSelectedMode.Expert);
                }
            }

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
