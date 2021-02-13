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

using System.Collections.ObjectModel;

namespace RoMoRDuP.UserControls
{

    public partial class UCOptimizeRenameOptions : UserControl
    {

        Tasks.TaskTreeViewModel TreeViewModel_After;
        Tasks.Tasks tasks;

        public UserInterface.Optimize.UserOptionsOptimize userOptionsOptimize;

        public ObservableCollection<UCOptimizeScriptEntry> ucScriptEntrys { get; set; }

        public UCOptimizeRenameOptions(Tasks.Tasks _tasks)
        {
            this.tasks = _tasks;

            ucScriptEntrys = new ObservableCollection<UCOptimizeScriptEntry>();

            userOptionsOptimize = tasks.userOptions.userOptionsOptimize;
            userOptionsOptimize.ucScriptEntrys = ucScriptEntrys;
            
            TreeViewModel_After = tasks.taskSource.TaskViewAfter;
            InitializeComponent();
        }


        public Tasks.TaskTreeViewModel TreeModel
        {
            get
            {
                return TreeViewModel_After;
            }
        }

        private void Click_Button_AddScriptEntry(object sender, RoutedEventArgs e)
        {
            ucScriptEntrys.Add(new UCOptimizeScriptEntry(new degDeleteScriptEntry(DeleteScriptEntry)));
        }

        public void DeleteScriptEntry(UCOptimizeScriptEntry ucScriptEntry)
        {
            ucScriptEntrys.Remove(ucScriptEntry);

        }

        private void Click_Button_ContractAll(object sender, RoutedEventArgs e)
        {
            foreach (UCOptimizeScriptEntry entry in ucScriptEntrys)
                entry.ContractAll();
        }
    }
}
