using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using System.Windows.Controls;
using RoMoRDuP.Tasks;
using System.Threading;

using System.ComponentModel;

namespace RoMoRDuP.TaskPlanner
{

    public class TaskPlannerRemoveDuplicates : TaskPlannerBase
    {
        TaskPlanner.DuplicatesModel DuplicatesModel { get; set; }


        public TaskPlannerRemoveDuplicates(UserInterface.UserInterfaceBase userOptions, TaskViews taskSource, TaskViews taskTarget, Tasks.Tasks tasks, Tasks.FileLists fileLists, TaskPlanner.DuplicatesModel Duplicates, PlaylistUpdates.PlaylistUpdates playlistUpdates)
            : base(userOptions, taskSource, taskTarget, tasks, fileLists, playlistUpdates)
        {
            this.DuplicatesModel = Duplicates;

        }



        protected override void Create_RD_Duplicate_Groups() // Aus TaskPlannerBase aufgerufen
        {
            // HashCodeTable
            ObservableCollection<DuplicatesViewModel> hashCodeCollection = new ObservableCollection<DuplicatesViewModel>();

            // Loop through filelist
            foreach(FileListEntry entry in fileLists.SourceFileListBefore)
            {
                if (entry.targetNode.IsFile)
                {
                    bool bFoundVM = false;

                    foreach (DuplicatesViewModel vm in hashCodeCollection)
                    {
                        if (vm.HashCode == entry.hash) // already in List
                        {
                            bFoundVM = true;
                            bool bFoundPath = false;
                            foreach (string path in vm.PathsOfDuplicates)
                            {
                                if (path == entry.targetNode.Path_1_Original)
                                {
                                    bFoundPath = true;
                                    break;
                                }
                            }

                            if (bFoundPath == false)
                            {
                                vm.PathsOfDuplicates.Add(entry.targetNode.Path_1_Original);
                                vm.fileListEntrysDuplicates.Add(entry);
                            }

                            break;
                        }
                    }

                    if (bFoundVM == false) // add entry to hashCollection
                    {
                        if (userOptions.fileFilterOptions.CheckIfFileOK(entry.targetNode.Path_1_Original))
                        {
                            DuplicatesViewModel newVM = new DuplicatesViewModel();
                            newVM.HashCode = entry.hash;
                            newVM.Name = entry.targetNode.Name;
                            newVM.PathsOfDuplicates.Add(entry.targetNode.Path_1_Original);
                            newVM.SelectedPath = newVM.PathsOfDuplicates[0];
                            newVM.fileListEntrysDuplicates.Add(entry);
                            hashCodeCollection.Add(newVM);
                        }
                        else // Filtered File
                        {
                            if (userOptions.mainWindow != null)
                            {
                                userOptions.mainWindow.Dispatcher.Invoke(
                                   (Action)(() =>
                                   {
                                       entry.targetNode.Info = "Filtered file!";
                                   }
                                    ), null);
                            }

                        }
                    }
                }
            }


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       // Add Items to VM
                       DuplicatesModel.Items = new ObservableCollection<DuplicatesViewModel>();
                       DuplicatesModel.bRemoveDuplicatesAll = true;
                       DuplicatesModel.bCreateShortcutAll = true;

                       foreach (DuplicatesViewModel addVM in hashCodeCollection)
                           if (addVM.PathsOfDuplicates.Count > 1)
                               DuplicatesModel.Items.Add(addVM);


                       DuplicatesModel.SelectedRemainingAll = enSelectedRemainingAll.shortest;

                   }
                    ), null);
            }


            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.Invoke(
                   (Action)(() =>
                   {
                       if (TaskPlanner.GlobalVar.wHourglass != null)
                           TaskPlanner.GlobalVar.wHourglass.Close();
                   }
                    ), null);
            }


            //int breakpoint = 5;

        }




        public void CreateRD_Before_Actions() // fast Method, no seperate thread
        {
            foreach (DuplicatesViewModel vm in DuplicatesModel.Items)
            {
                foreach (FileListEntry duplicateEntry in vm.fileListEntrysDuplicates)
                {

                    // zuerst zurücksetzen
                    duplicateEntry.targetNode.task5 = enTasks.Org;
                    duplicateEntry.targetNode.Path_5_Delete = "";

                    duplicateEntry.targetNode.task6 = enTasks.Org;
                    duplicateEntry.targetNode.Path_CreateShortcutAt = "";
                    duplicateEntry.targetNode.Path_CreateShortcutTo = "";

                    duplicateEntry.targetNode.bActivated = false;

                    if (vm.RemoveDuplicates) // Remove Duplicates from group
                    {
                        if (duplicateEntry.Path == vm.SelectedPath)
                        {
                            // Remaining File
                            duplicateEntry.targetNode.Info = "Remaining File";
                        }
                        else
                        {
                            // Duplicate to remove
                            duplicateEntry.targetNode.Info = "Duplicate to Remove";
                            duplicateEntry.targetNode.task5 = enTasks.Delete;
                            duplicateEntry.targetNode.Path_5_Delete = duplicateEntry.Path;
                            duplicateEntry.targetNode.bActivated = true;

                            // Create shortcut to Remaining
                            if (vm.CreateShortcut)
                            {
                                duplicateEntry.targetNode.task6 = enTasks.CreateShortcut;
                                duplicateEntry.targetNode.Path_CreateShortcutAt = duplicateEntry.Path + ".lnk";
                                duplicateEntry.targetNode.Path_CreateShortcutTo = vm.SelectedPath;

                            }
                        }
                    }
                    else // dont remove duplicates
                    {
                        duplicateEntry.targetNode.Info = "Unselected Group";
                    }

                }

            
            }


            //int breakpoint = 5;
        }


    }










    public enum enSelectedRemainingAll { first, last, shortest, longest }

    public class DuplicatesModel : INotifyPropertyChanged
    {
        public DuplicatesModel()
        {
            InitDuplicates();
        }

        private ObservableCollection<DuplicatesViewModel> int_Items;
        public ObservableCollection<DuplicatesViewModel> Items
        {
            get
            {
                return int_Items;
            }
            set
            {
                int_Items = value;
                OnPropertyChanged("Items");
            }
        }



        private void InitDuplicates()
        {
            Items = new ObservableCollection<DuplicatesViewModel>();

            bRemoveDuplicatesAll = true;
            bCreateShortcutAll = true;

            List<string> remainingAll = new List<string>();

            remainingAll.Add("Select first path");
            remainingAll.Add("Select last path");
            remainingAll.Add("Select shortest path");
            remainingAll.Add("Select longest path");

            listComboBoxRemainingAll = remainingAll;


            /*
            DuplicatesViewModel item = new DuplicatesViewModel();

            item.NumOfDuplicateList = 1;
            item.CreateShortcut = true;
            item.RemoveDuplicates = true;
            item.Name = String.Format("Duplicate {0}", item.NumOfDuplicateList);

            string[] input = { "one path", "another path", "one more path" };

            item.PathsOfDuplicates = new List<string>( input );

            Items.Add(item);
            Items.Add(item);
             */
        }



        private bool int_bRemoveDuplicatesAll;
        public bool bRemoveDuplicatesAll
        {
            get
            {
                return int_bRemoveDuplicatesAll;
            }
            set
            {
                int_bRemoveDuplicatesAll = value;

                foreach (DuplicatesViewModel vm in Items)
                {
                    vm.RemoveDuplicates = value;
                }

                OnPropertyChanged("bRemoveDuplicatesAll");
            }
        }

        private bool int_bCreateShortcutAll;
        public bool bCreateShortcutAll
        {
            get
            {
                return int_bCreateShortcutAll;
            }
            set
            {
                int_bCreateShortcutAll = value;

                foreach (DuplicatesViewModel vm in Items)
                {
                    vm.CreateShortcut = value;
                }

                OnPropertyChanged("bCreateShortcutAll");
            }
        }


        private List<string> int_listComboBoxRemainingAll;
        public List<string> listComboBoxRemainingAll
        {
            get
            {
                return int_listComboBoxRemainingAll;
            }
            set
            {
                int_listComboBoxRemainingAll = value;
                OnPropertyChanged("listComboBoxRemainingAll");
            }
        }

        // nur für interne verwendung
        private string int_ComboBoxRemainingAllSelected;
        public string ComboBoxRemainingAllSelected
        {
            get
            {
                return int_ComboBoxRemainingAllSelected;
            }
            set
            {
                int_ComboBoxRemainingAllSelected = value;
                OnPropertyChanged("ComboBoxRemainingAllSelected");

                // alle umstellen
                foreach (DuplicatesViewModel vm in Items)
                {
                    vm.ChangeSelectedPath(SelectedRemainingAll);
                }
            }
        }

        // externe verwendung
        public enSelectedRemainingAll SelectedRemainingAll
        {
            get
            {
                enSelectedRemainingAll ret = enSelectedRemainingAll.first;

                foreach (string str in listComboBoxRemainingAll)
                {
                    if (str == ComboBoxRemainingAllSelected)
                        return ret;

                    else
                        ret++;
                }

                return ret;
            }
            set
            {
                ComboBoxRemainingAllSelected = listComboBoxRemainingAll[(int)value];

            }
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
        
    }






    public class DuplicatesViewModel : INotifyPropertyChanged
    {
        public DuplicatesViewModel()
        {
            PathsOfDuplicates = new List<string>();
            Name = "init";
            fileListEntrysDuplicates = new List<FileListEntry>();

            RemoveDuplicates = true;
            CreateShortcut = true;
        }

        // general Data
        public String Name { get; set; }
        public List<String> PathsOfDuplicates { get; set; }
        //public int NumOfDuplicateList { get; set; }
        public string HashCode { get; set; }
        public List<FileListEntry> fileListEntrysDuplicates { get; set; }

        public void ChangeSelectedPath(enSelectedRemainingAll toSelect)
        {
            string result;

            switch (toSelect)
            {
                case enSelectedRemainingAll.first:
                    SelectedPath = PathsOfDuplicates[0];
                    break;

                case enSelectedRemainingAll.last:
                    SelectedPath = PathsOfDuplicates[PathsOfDuplicates.Count - 1];
                    break;

                case enSelectedRemainingAll.shortest:
                    result = PathsOfDuplicates[0];

                    foreach (string cur in PathsOfDuplicates)
                        if (cur.Length < result.Length)
                            result = cur;

                    SelectedPath = result;
                    break;

                case enSelectedRemainingAll.longest:
                    result = PathsOfDuplicates[0];

                    foreach (string cur in PathsOfDuplicates)
                        if (cur.Length > result.Length)
                            result = cur;

                    SelectedPath = result;
                    break;
            }

        }

        
        private string int_SelectedPath;
        public string SelectedPath
        {
            get
            {
                return int_SelectedPath;
            }
            set
            {
                int_SelectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }

        private bool int_RemoveDuplicates;
        public bool RemoveDuplicates
        {
            get
            {
                return int_RemoveDuplicates;
            }
            set
            {
                int_RemoveDuplicates = value;
                OnPropertyChanged("RemoveDuplicates");
            }
        }

        private bool int_CreateShortcut;
        public bool CreateShortcut
        {
            get
            {
                return int_CreateShortcut;
            }
            set
            {
                int_CreateShortcut = value;
                OnPropertyChanged("CreateShortcut");
            }
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
    }


}

