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
    public delegate void degDeleteScriptEntry(UCOptimizeScriptEntry ucScriptEntry);

    public partial class UCOptimizeScriptEntry : UserControl
    {
       UserInterface.Optimize.ScriptEntry scriptEntry { get; set; }

       degDeleteScriptEntry DeleteThisScriptEntry;

       UCOptimizeScriptEntryBase ucCondition1;
       UCOptimizeScriptEntryBase ucCondition2;


       public UCOptimizeScriptEntry(degDeleteScriptEntry DeleteThisScriptEntry)
        {
            // DataContext der UserControl ist scriptEntry

            this.DeleteThisScriptEntry = DeleteThisScriptEntry;

            scriptEntry = new UserInterface.Optimize.ScriptEntry();
            this.DataContext = scriptEntry;

            InitializeComponent();
        }


        // nur für Designer
       public UCOptimizeScriptEntry()
       {
           InitializeComponent();
       }



        private void Click_Button_AddCondtion(object sender, RoutedEventArgs e)
        {
            if (ucCondition1 == null)
            {
                if (ComboBox_SelectCondition.SelectedValue != null)
                {
                    switch ((ComboBox_SelectCondition.SelectedValue as ComboBoxItem).Content.ToString())
                    {
                        case "Find":
                            ucCondition1 = new UCOptimizeScriptEntryFind();
                            break;

                        case "If":
                            ucCondition1 = new UCOptimizeScriptEntryIF();
                            break;

                        case "Find same char in a row":
                            ucCondition1 = new UCOptimizeScriptEntrySameChars();
                            break;

                        case "Find number":
                            ucCondition1 = new UCOptimizeScriptEntryFindNumber();
                            break;

                        default:
                            ucCondition1 = new UCOptimizeScriptEntryFind();
                            break;
                    }
                }
                else
                    ucCondition1 = new UCOptimizeScriptEntryFind();


                ParentPanel_Condition1.Children.Add(ucCondition1);
            }
            else if (ucCondition2 == null)
            {

                if (ucCondition1.GetType() == typeof(UCOptimizeScriptEntryIF))
                {
                    if (ComboBox_SelectCondition.SelectedValue != null)
                    {
                        switch ((ComboBox_SelectCondition.SelectedValue as ComboBoxItem).Content.ToString())
                        {
                            case "Find":
                                ucCondition2 = new UCOptimizeScriptEntryFind();
                                break;

                            case "If":
                                ucCondition2 = new UCOptimizeScriptEntryIF();
                                break;

                            case "Find same char in a row":
                                ucCondition2 = new UCOptimizeScriptEntrySameChars();
                                break;

                            case "Find number":
                                ucCondition2 = new UCOptimizeScriptEntryFindNumber();
                                break;

                            default:
                                ucCondition2 = new UCOptimizeScriptEntryFind();
                                break;
                        }
                    }

                    if (ucCondition2 != null)
                        ParentPanel_Condition2.Children.Add(ucCondition2);
                }
                else
                    MessageBox.Show("Only possible after first condition = If");
            }

        }

        private void Click_Button_AddAction(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void Click_Button_RemoveScriptEntry(object sender, RoutedEventArgs e)
        {
            DeleteThisScriptEntry(this);
        }

        private void Click_Button_RemoveCondition1(object sender, RoutedEventArgs e)
        {
            ParentPanel_Condition1.Children.Remove(ucCondition1);
            ucCondition1 = null;
        }

        private void Click_Button_RemoveCondition2(object sender, RoutedEventArgs e)
        {
            ParentPanel_Condition2.Children.Remove(ucCondition2);
            ucCondition2 = null;
        }

        public void ContractAll()
        {
            if(ucCondition1!=null)
                ucCondition1.ContractExpander();
            if(ucCondition2!=null)
                ucCondition2.ContractExpander();
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
