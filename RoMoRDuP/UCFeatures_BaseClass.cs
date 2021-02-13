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

    public partial class UCFeatures_BaseClass : UserControl
    {
        protected Tasks.Tasks tasks;
        protected UserControls.UCParent ucParent;

        protected UserInterface.UserInterfaceBase userInterfaceBase;

        public UCFeatures_BaseClass() // nur für XAML Design Viewer
        {

        }

        public UCFeatures_BaseClass(Tasks.Tasks tasks)
        {
            this.tasks = tasks;

            lastSelectedMode = UserControls.enSelectedMode.Easy; // UserControls.enSelectedMode.None; // Problems with window closing
        }


        protected UserControls.enSelectedMode lastSelectedThisScreen { get; set; }

        protected UserControls.enSelectedMode selectedMode;
        protected UserControls.enSelectedMode lastSelectedMode;


        protected virtual void ApplyEasyModeChangesCurUC()
        {
            //

        }


        protected virtual void base_SaveMode()
        {
            //

        }



        // eigener Thread?
        protected void internalSelectionModeChanged()
        {
            /*
           UserControls.enSelectedMode newmode = UserControls.enSelectedMode.Easy;
           if (selectedMode == UserControls.enSelectedMode.Expert)
               newmode = UserControls.enSelectedMode.Easy;
           ucParent.TabControlChangeSelectedIndex(newmode);
           */

            //if (lastSelectedMode != selectedMode)
            {

                try
                {

                    if (selectedMode == UserControls.enSelectedMode.Expert)
                    {
                        if (lastSelectedThisScreen == UserControls.enSelectedMode.Easy)
                        {
                            MessageBoxResult result = MessageBoxResult.Cancel;
                            if (userInterfaceBase.mainWindow != null)
                            {
                                userInterfaceBase.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       result = MessageBox.Show("Do you want to apply the easy mode changes?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                                   }
                                    ), null);
                            }
                            
                            lastSelectedThisScreen = UserControls.enSelectedMode.None;

                            if (result == MessageBoxResult.Yes)
                            {
                                ApplyEasyModeChangesCurUC();
                                if (userInterfaceBase.mainWindow != null)
                                {
                                    userInterfaceBase.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           ucParent.TabControlChangeSelectedIndex(UserControls.enSelectedMode.Expert);
                                           base_SaveMode(); // Save selected Mode on Tab Switch
                                       }
                                        ), null);
                                }

                            }
                            else if (result == MessageBoxResult.No)
                            {
                                // do nothing
                                if (userInterfaceBase.mainWindow != null)
                                {
                                    userInterfaceBase.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           ucParent.TabControlChangeSelectedIndex(UserControls.enSelectedMode.Expert);
                                           base_SaveMode(); // Save selected Mode on Tab Switch
                                       }
                                        ), null);
                                }

                            }
                            else if (result == MessageBoxResult.Cancel)
                            {
                                lastSelectedMode = UserControls.enSelectedMode.Easy; // verhindert erneuten aufruf
                                if (userInterfaceBase.mainWindow != null)
                                {
                                    userInterfaceBase.mainWindow.Dispatcher.Invoke(
                                       (Action)(() =>
                                       {
                                           ucParent.TabControlChangeSelectedIndex(UserControls.enSelectedMode.Easy);
                                       }
                                        ), null);
                                }

                            }
                        }

                    }

                    if (selectedMode == UserControls.enSelectedMode.Easy)
                    {
                        lastSelectedThisScreen = UserControls.enSelectedMode.Easy;
                        base_SaveMode(); // Save selected Mode on Tab Switch
                    }



                    // Show EasyMode Options
                    if (userInterfaceBase.mainWindow != null)
                    {
                        userInterfaceBase.mainWindow.Dispatcher.Invoke(
                           (Action)(() =>
                           {
                               if (selectedMode == UserControls.enSelectedMode.Easy)
                                   userInterfaceBase.bExpertOptions_Visible = false;
                               else
                                   userInterfaceBase.bExpertOptions_Visible = true;
                           }
                            ), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);

                }
            }

            //lastSelectedMode = selectedMode;

        }




        Thread threadInternalSelectionModeChanged;

        protected void SelectionModeChanged(UserControls.enSelectedMode selectedMode)
        {
            

            this.selectedMode = selectedMode;

            if (lastSelectedMode != selectedMode) // Selection Changed Event has problems with MessageBox.Show and UserInterface changes without seperate Thread
            {
                if (threadInternalSelectionModeChanged == null)
                {
                    threadInternalSelectionModeChanged = new Thread(new ThreadStart(internalSelectionModeChanged));
                }
                else
                {
                    threadInternalSelectionModeChanged.Abort(); // TBD check if Abort valid - only if everything managed

                    threadInternalSelectionModeChanged = new Thread(new ThreadStart(internalSelectionModeChanged));
                }

                lastSelectedMode = selectedMode;

                threadInternalSelectionModeChanged.Start();
            }
        }



    }

}

