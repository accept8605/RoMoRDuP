using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using System.ComponentModel;
using System.IO;
using System.Collections.Generic;


namespace RoMoRDuP.Tasks
{
    public enum enTasks { Org, CreateSubfolder, CreateShortcut, SetAttributes, Copy, Rename, Move, Delete, Overflow } // Order of biggestAction

    public enum TaskViewType { Before, After }
    public enum TaskViewTarget { Source, Target }

    
    public class cAttributes : INotifyPropertyChanged
    {
        public cAttributes(bool ReadOnly, bool Hidden, System.DateTime dateTime)
        {
            this.ReadOnly = ReadOnly;
            this.Hidden = Hidden;
            this.CreatedDateTime = dateTime;
        }

        public bool Compare(cAttributes CompareTo)
        {
            bool bRet = false;

            DateTime dateTime1 = CreatedDateTime;
            DateTime dateTime2 = CompareTo.CreatedDateTime;

            bool bDateTimeOK = false;

            if((dateTime1.Year==dateTime2.Year)&&(dateTime1.Month==dateTime2.Month)&&(dateTime1.Day==dateTime2.Day)
                &&(dateTime1.Hour==dateTime2.Hour)&&(dateTime1.Minute==dateTime2.Minute)
                )
                bDateTimeOK = true;

            if ((ReadOnly == CompareTo.ReadOnly) && (Hidden == CompareTo.Hidden) && (bDateTimeOK))
                bRet = true;

            return bRet;
        }

        public void UpdateView()
        {
            OnPropertyChanged("ReadOnly");
            OnPropertyChanged("Hidden");
            OnPropertyChanged("CreatedDateTime");
            OnPropertyChanged("strCreatedDateTime");

        }

        bool int_ReadOnly;
        public bool ReadOnly
        {
            get
            {
                return int_ReadOnly;
            }
            set
            {
                int_ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        bool int_Hidden;
        public bool Hidden
        {
            get
            {
                return int_Hidden;
            }
            set
            {
                int_Hidden = value;
                OnPropertyChanged("Hidden");
            }
        }

        DateTime int_CreatedDateTime;
        public System.DateTime CreatedDateTime
        {
            get
            {
                return int_CreatedDateTime;
            }
            set
            {
                int_CreatedDateTime = value;
                OnPropertyChanged("CreatedDateTime");
                OnPropertyChanged("strCreatedDateTime");
            }
        }

        public string strCreatedDateTime
        {
            get
            {
                string strRet = "";

                strRet += "_" + CreatedDateTime.Year.ToString() + "-" + CreatedDateTime.Month.ToString() + "-" + CreatedDateTime.Day.ToString();
                strRet += "_" + CreatedDateTime.Hour.ToString() + "_" + CreatedDateTime.Minute.ToString();

                return strRet;
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




    public class TaskTreeViewModel : INotifyPropertyChanged
    {

        CustomObservableCollection<TaskNodeViewModel> int_Items;

        public CustomObservableCollection<TaskNodeViewModel> Items  // The Observable Collection only takes care of its subelements, not of itself, so you still 
            //need OnPropertyChanged for the Observ. Collection element if you want to Assign it new
        {
            get
            {
                return int_Items;
            }
            set
            {
                int_Items = value;

                if(int_Items != null)
                    if (int_Items.Count > 0)
                        if(int_Items[0] != null)
                            int_Items[0].IsExpanded = true;

                OnPropertyChanged("Items");
            }
        }





        /*
        public void UpdateParents()
        {
            if (Items.Count == 1)
            {
                TaskNodeViewModel curParent = Items[0];

                NodeWalker(curParent, 0);
            }

        }


        private void NodeWalker(TaskNodeViewModel parentNode, int level)
        {
            try
            {
                foreach (TaskNodeViewModel child in parentNode.Children)
                {
                    if (child.IsFolder)
                    {
                        child.Parent = parentNode;

                        int newlevel = level + 1;
                        this.NodeWalker(child, newlevel);
                    }
                }

                foreach (TaskNodeViewModel child in parentNode.Children)
                {
                    if (child.IsFile)
                    {
                        child.Parent = parentNode;
                    }
                }

            }
            catch (System.Exception ex)
            {
                //listBox1.Items.Add(ex.Message);
            }

        }
         */


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







    public class TaskNodeViewModel : INotifyPropertyChanged
    {
        // The Observable Collection only means that added and removed items from it are noticed
        public TaskNodeViewModel(TaskNodeViewModel parent, int ID, TaskViewType taskViewType, string BasePath, cAttributes OrgFileAttributes) 
        {
            //Show = true;
            this.taskViewType = taskViewType;

            this.basePath = BasePath;

            this.OrgFileAttributes = OrgFileAttributes;

            this.SetFileAttributes = null;

            ParentOfThis = parent;
            IdOfThis = ID;

            DuplicatesSourceSourceOrTT = new List<string>();
            DuplicatesSourceTarget = new List<string>();

            DuplicatesSourceSourceOrTTNodes = new List<TaskNodeViewModel>();
            DuplicatesSourceTargetNodes = new System.Collections.Generic.List<TaskNodeViewModel>();

            Path_1_Original = "";
            Path_2_Copy = "";
            Path_3_Move = "";
            Path_4_Rename = "";
            Path_5_Delete = "";
            Path_CreateShortcutAt = "";
            Path_CreateShortcutTo = "";

            IsExpanded = false;

            bUpdatedFile = false;
            bManuallyModified = false;

            bActivated = true;

            childrensNames = new Dictionary<string, List<TaskNodeViewModel>>();
        }


        public TaskNodeViewModel ReferencedOtherFolderNode { get; set; } // SourceBeforeNode -> TargetBeforeNode / TargetBeforeNode -> SourceBeforeNode



        private bool int_IsExpanded;
        public bool IsExpanded
        {
            get
            {
                return int_IsExpanded;
            }
            set
            {
                int_IsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }


        public string basePath { get; set; }

        public TaskViewType taskViewType { get; set; }

        //public TaskNodeViewModel otherView { get; set; } // does not work right

        public bool bUpdatedFile { get; set; }
        public bool bManuallyModified { get; set; }


        // _______________Propertys

        // Internal Info
        private int IdOfThis;
        public int iId
        {
            get { return IdOfThis; }
        }



        // general Data
        String int_Name;
        public String Name // Original Path/Create Subfolder path
        {
            get
            {
                return int_Name;
            }
            set
            {
                int_Name = value;
                OnPropertyChanged("Name");
            }
        }


        private CustomObservableCollection<TaskNodeViewModel> int_Children;
        public CustomObservableCollection<TaskNodeViewModel> Children
        {
            get
            {
                return int_Children;
            }
            set
            {
                int_Children = value;

                OnPropertyChanged("Children");
                UpdateBiggestAction(0,enTasks.Org);
                OnPropertyChanged("IsFolder");
                OnPropertyChanged("IsFile");
            }

        }


        public Dictionary<string,List<TaskNodeViewModel>> childrensNames { get; set; }




        private TaskNodeViewModel ParentOfThis;
        public TaskNodeViewModel Parent
        {
            get { return ParentOfThis; }

            set { 
                ParentOfThis = value; 
            }
        }


        bool int_bActivated;
        public bool bActivated // Original Path/Create Subfolder path
        {
            get
            {
                return int_bActivated;
            }
            set
            {
                int_bActivated = value;

                if (IsFolder)
                    foreach (TaskNodeViewModel node in Children)
                        node.bActivated = value;

                if (value == true)
                    ActivateParentsOfThisNode();

                OnPropertyChanged("bActivated");
            }
        }


        public void ActivateParentsOfThisNode()
        {
            int_bActivated = true;
            OnPropertyChanged("bActivated");

            TaskNodeViewModel curNode = this;

            curNode = curNode.Parent;
            if (curNode == null)
                return;

            curNode.ActivateParentsOfThisNode();
        }



        public List<String> DuplicatesSourceSourceOrTT { get; set; }
        public List<String> DuplicatesSourceTarget { get; set; }

        public List<TaskNodeViewModel> DuplicatesSourceSourceOrTTNodes { get; set; }
        public List<TaskNodeViewModel> DuplicatesSourceTargetNodes { get; set; }

        private string int_Info;
        public string Info    // Info-string for the user about the Actions
        {
            get
            {
                return int_Info;
            }
            set
            {
                int_Info = value;
                OnPropertyChanged("Info");
            }
        }

        public string HashCode { get; set; }


        public bool ComparePathToDuplicatesSourceTarget(string strFilename)
        {
            bool bRet = false;

            foreach (string str in DuplicatesSourceTarget)
                if (str == strFilename)
                {
                    bRet = true;
                    break;
                }

            return bRet;
        }

        public bool CompareStringToDuplicatesSourceSourceOrTT(string strFilename)
        {
            bool bRet = false;

            foreach (string str in DuplicatesSourceSourceOrTT)
                if (System.IO.Path.GetFileName(str) == strFilename)
                {
                    bRet = true;
                    break;
                }

            return bRet;
        }



        // GetPathFromNode for AfterView manual Editing
        public string GetFullPathFromNode()
        {
            return basePath + GetPathFromNode();
        }

        public string GetPathFromNode()
        {
            string pathFromNode = "";

            List<string> strFolderAndFileNames = new List<string>();

            TaskNodeViewModel curNode = this;

            while (true)
            {
                strFolderAndFileNames.Add(curNode.Name);
                curNode = curNode.Parent;
                if (curNode == null)
                    break;
            }

            for (int i = strFolderAndFileNames.Count - 2; i >= 0; i--)
            {
                pathFromNode += "\\" + strFolderAndFileNames[i];
            }

            return pathFromNode;
        }


        // Get Path after Actions

        public string GetPathAfterActionsFromNode(string originalPath)
        {
            string pathFromNode = originalPath;

            if (bActivated)
            {
                switch (AnyNodeActionThisNode)
                {
                    case enTasks.Delete:
                        if (DuplicatesSourceSourceOrTTNodes != null)
                            foreach (TaskNodeViewModel vm in DuplicatesSourceSourceOrTTNodes)
                            {
                                if (vm.task5 != enTasks.Delete)
                                {
                                    pathFromNode = vm.GetFullPathFromNode();
                                    break;
                                }
                            }
                        break;

                    case enTasks.Move:
                        pathFromNode = Path_3_Move;
                        break;

                    case enTasks.Rename:
                        pathFromNode = Path_4_Rename;
                        break;

                }
            }

            return pathFromNode;
        }
       


        //___________Tasks_______________ (need to update automaticly)
        //public bool Show { get; set; }

        String int_Path_1_Original;
        public String Path_1_Original // Original Path/Create Subfolder path
        {
            get
            {
                return int_Path_1_Original;
            }
            set
            {
                int_Path_1_Original = value;
                OnPropertyChanged("Path_1_Original");
            }
        }
        String int_Path_2_Copy;
        public String Path_2_Copy // Original Path/Create Subfolder path
        {
            get
            {
                return int_Path_2_Copy;
            }
            set
            {
                int_Path_2_Copy = value;
                OnPropertyChanged("Path_2_Copy");
            }
        }
        String int_Path_3_Move;
        public String Path_3_Move // Original Path/Create Subfolder path
        {
            get
            {
                return int_Path_3_Move;
            }
            set
            {
                int_Path_3_Move = value;
                OnPropertyChanged("Path_3_Move");
            }
        }
        String int_Path_4_Rename;
        public String Path_4_Rename // Original Path/Create Subfolder path
        {
            get
            {
                return int_Path_4_Rename;
            }
            set
            {
                int_Path_4_Rename = value;
                OnPropertyChanged("Path_4_Rename");
            }
        }

        String int_Path_5_Delete;
        public String Path_5_Delete // Original Path/Create Subfolder path
        {
            get
            {
                return int_Path_5_Delete;
            }
            set
            {
                int_Path_5_Delete = value;
                OnPropertyChanged("Path_5_Delete");
            }
        }

        String int_Path_CreateShortcutAt;
        public String Path_CreateShortcutAt
        {
            get
            {
                return int_Path_CreateShortcutAt;
            }
            set
            {
                int_Path_CreateShortcutAt = value;
                OnPropertyChanged("Path_CreateShortcutAt");
            }
        }

        String int_Path_CreateShortcutTo;
        public String Path_CreateShortcutTo
        {
            get
            {
                return int_Path_CreateShortcutTo;
            }
            set
            {
                int_Path_CreateShortcutTo = value;
                OnPropertyChanged("Path_CreateShortcutTo");
            }
        }

        String int_Path_SetCopyOrAttributesSource;
        public String Path_SetCopyOrAttributesSource
        {
            get
            {
                return int_Path_SetCopyOrAttributesSource;
            }
            set
            {
                int_Path_SetCopyOrAttributesSource = value;
                OnPropertyChanged("Path_SetCopyOrAttributesSource");
            }
        }

        String int_Path_SetAttributesSourceBase;
        public String Path_SetAttributesSourceBase
        {
            get
            {
                return int_Path_SetAttributesSourceBase;
            }
            set
            {
                int_Path_SetAttributesSourceBase = value;
                OnPropertyChanged("Path_SetAttributesSourceBase");
            }
        }


        String int_Path_SetAttributes;
        public String Path_SetAttributes
        {
            get
            {
                return int_Path_SetAttributes;
            }
            set
            {
                int_Path_SetAttributes = value;
                OnPropertyChanged("Path_SetAttributes");
            }
        }

        String int_Path_CopyOrSetAttributesBase;
        public String Path_CopyOrSetAttributesBase
        {
            get
            {
                return int_Path_CopyOrSetAttributesBase;
            }
            set
            {
                int_Path_CopyOrSetAttributesBase = value;
            }
        }


        cAttributes int_SetFileAttributes;
        public cAttributes SetFileAttributes
        {
            get
            {
                return int_SetFileAttributes;
            }
            set
            {
                int_SetFileAttributes = value;
                OnPropertyChanged("SetFileAttributes");

                if(int_SetFileAttributes != null)
                    int_SetFileAttributes.UpdateView();
            }
        }


        cAttributes int_OrgFileAttributes;
        public cAttributes OrgFileAttributes
        {
            get
            {
                return int_OrgFileAttributes;
            }
            set
            {
                int_OrgFileAttributes = value;
                OnPropertyChanged("OrgFileAttributes");
            }
        }


        public void RemoveAllTasks()
        {
            task1 = enTasks.Org;
            task2 = enTasks.Org;
            task3 = enTasks.Org;
            task4 = enTasks.Org;
            task5 = enTasks.Org;
            task6 = enTasks.Org;
            task7 = enTasks.Org;

        }



        public enTasks int_task1;  // Org
        public enTasks task1 // Original Path
        {
            get
            {
                return int_task1;
            }
            set
            {
                int_task1 = value;
                OnPropertyChanged("task1");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task1_With");
            }
        }

        public enTasks int_task2; // Copy / CreateSubfolder
        public enTasks task2 
        {
            get
            {
                return int_task2;
            }
            set
            {
                int_task2 = value;
                OnPropertyChanged("task2");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task2_With");
            }
        }

        public enTasks int_task3;  // Move
        public enTasks task3 
        {
            get
            {
                return int_task3;
            }
            set
            {
                int_task3 = value;
                OnPropertyChanged("task3");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task3_With");
            }
        }

        public enTasks int_task4; // Rename
        public enTasks task4
        {
            get
            {
                return int_task4;
            }
            set
            {
                int_task4 = value;
                OnPropertyChanged("task4");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task4_With");
            }
        }

        public enTasks int_task5; // Delete
        public enTasks task5
        {
            get
            {
                return int_task5;
            }
            set
            {
                int_task5 = value;
                OnPropertyChanged("task5");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task5_With");
            }
        }

        public enTasks int_task6; // CreateShortcut
        public enTasks task6
        {
            get
            {
                return int_task6;
            }
            set
            {
                int_task6 = value;
                OnPropertyChanged("task6");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task6_With");
            }
        }

        public enTasks int_task7; // SetAttributes
        public enTasks task7
        {
            get
            {
                return int_task7;
            }
            set
            {
                int_task7 = value;
                OnPropertyChanged("task7");
                OnPropertyChanged("AnyNodeActionThisNode");
                UpdateBiggestAction(0, enTasks.Org);

                OnPropertyChanged("task7_With");
            }
        }

        const int TaskViewColumnWith = 180;


        public int task1_With
        {
            get
            {
                if (task1 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task2_With
        {
            get
            {
                if (task2 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task3_With
        {
            get
            {
                if (task3 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task4_With
        {
            get
            {
                if (task4 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task5_With
        {
            get
            {
                if (task5 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task6_With
        {
            get
            {
                if (task6 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int task7_With
        {
            get
            {
                if (task7 == enTasks.Org)
                    return 0;
                else
                    return TaskViewColumnWith;
            }
        }

        public int hashcode_With
        {
            get
            {
                if (HashCode != null)
                    if (HashCode.Length > 0)
                        return TaskViewColumnWith;

                    return 0;
            }
        }





        //___________Methods____________

        public double RequiredSpaceThisNode()
        {
            double doubRet = 0;


            if (IsFile)
            {
                // Copy->AddSpace
                if (task2 == enTasks.Copy)
                {
                    if (TaskPlanner.UsefulMethods.FilePathInBasePath(Path_2_Copy, basePath))//Path_2_Copy.Contains(basePath))
                    {
                        FileInfo fileInfo = new FileInfo(Path_1_Original);
                        doubRet += fileInfo.Length;
                    }

                }

                // Delete -> RemoveSpace
                if (task5 == enTasks.Delete)
                {
                    if (TaskPlanner.UsefulMethods.FilePathInBasePath(Path_5_Delete, basePath))//Path_5_Delete.Contains(basePath))
                    {
                        FileInfo fileInfo = new FileInfo(Path_5_Delete);
                        doubRet -= fileInfo.Length;
                    }

                }
            }

            return doubRet;
        }


        public TaskNodeViewModel Clone(TaskNodeViewModel newParent, int newID)
        {
            // Parent, ID
            TaskNodeViewModel clone = CloneFor_CopyTask(newParent, newID);

            clone.DuplicatesSourceSourceOrTT = DuplicatesSourceSourceOrTT;
            clone.DuplicatesSourceTarget = DuplicatesSourceTarget;


            //clone.Show = Show;
            clone.Children = Children;

            

            // Tasks
            if (Path_1_Original != null)
                clone.Path_1_Original = String.Copy(Path_1_Original);
            if (Path_2_Copy != null)
                clone.Path_2_Copy = String.Copy(Path_2_Copy);
            if (Path_3_Move != null)
                clone.Path_3_Move = String.Copy(Path_3_Move);
            if (Path_4_Rename != null)
                clone.Path_4_Rename = String.Copy(Path_4_Rename);
            if (Path_5_Delete != null)
                clone.Path_5_Delete = String.Copy(Path_5_Delete);


            clone.task1 = task1; // Create Folder 
            clone.task2 = task2; // Copy 
            clone.task3 = task3; // Move 
            clone.task4 = task4; // Rename 
            clone.task5 = task5; // Delete 


            return clone;
        }


        public TaskNodeViewModel CloneFor_AfterView(TaskNodeViewModel newParent, int newID)
        {
            // Parent, ID

            TaskNodeViewModel clone = CloneFor_CopyTask(newParent, newID);

            //clone.task1 = task1; // Create Folder - tasks are set in EditAfterViewsFromActionsBeforeList
            //clone.task2 = task2; // Copy - dont put in after view - can be different Base Path
            //clone.task3 = task3; // Move - tasks are set in EditAfterViewsFromActionsBeforeList
            //clone.task4 = task4; // Rename - tasks are set in EditAfterViewsFromActionsBeforeList
            //clone.task5 = task5; // Delete - tasks are set in EditAfterViewsFromActionsBeforeList

            //if (task1 != enTasks.Org || task3 != enTasks.Org || task4 != enTasks.Org || task5 != enTasks.Org)
            //    clone.bActivated = bActivated;

            clone.DuplicatesSourceSourceOrTT = DuplicatesSourceSourceOrTT;
            clone.DuplicatesSourceTarget = DuplicatesSourceTarget;


            return clone;
        }


        public TaskNodeViewModel CloneFor_CopyTask(TaskNodeViewModel newParent, int newID)
        {
            // Parent, ID
            TaskNodeViewModel clone = new TaskNodeViewModel(newParent, newID, taskViewType, basePath, OrgFileAttributes);

            // general data
            clone.Name = String.Copy(Name);
            //clone.Show = Show;
            //clone.Children = Children; // without children

            clone.basePath = String.Copy(basePath);

            clone.bUpdatedFile = bUpdatedFile;
            clone.bManuallyModified = bManuallyModified;

            // Duplicate Info Incorrect beim Clonen für Copy
            //clone.DuplicatesSourceSourceOrTT = DuplicatesSourceSourceOrTT;
            //clone.DuplicatesSourceTarget = DuplicatesSourceTarget;

            if (HashCode != null)
                clone.HashCode = String.Copy(HashCode);

            if (Info != null)
                clone.Info = String.Copy(Info);

            return clone;
        }



        public void RemoveChild(TaskNodeViewModel child)
        {
            Children.Remove(child);
        }

        public void RemoveSelf()
        {
            if (Parent != null)
                Parent.Children.Remove(this);
        }

        public void UpdateBiggestAction(int level, enTasks newAction)
        {

            if (level == 0)
            {
                biggestAction = GetBiggestActionFromNodeAndDirectChildren();
                newAction = biggestAction;
            }
            else
                biggestAction = (enTasks)Math.Max((int)biggestAction, (int)newAction);

            if (Parent != null)
                Parent.UpdateBiggestAction(++level, newAction);

        }

        // Actions
        /*
        public enTasks biggestAction // can change view
        {
            get
            {
                enTasks action = (enTasks)Math.Max((int)task1, Math.Max((int)task2, Math.Max((int)task3, Math.Max((int)task4, Math.Max((int)task5, Math.Max((int)task6, (int)task7))))));

                if (Children != null)
                    foreach (TaskNodeViewModel node in Children)
                        action = (enTasks)Math.Max((int)node.biggestAction, (int)action);
                 
                return action;
            }
        }
         */

        private enTasks int_sBiggestAction;
        public enTasks biggestAction
        {
            get
            {
                return int_sBiggestAction;
            }
            set
            {
                int_sBiggestAction = value;
                OnPropertyChanged("biggestAction");
            }
        }


        public enTasks GetBiggestActionFromNodeAndDirectChildren()
        {
            enTasks action = AnyNodeActionThisNode;

            if (Children != null)
                foreach (TaskNodeViewModel node in Children)
                    action = (enTasks)Math.Max((int)node.biggestAction, (int)action);

            return action;
        }



        public enTasks AnyNodeActionThisNode // can change view
        {
            get
            {
                enTasks action = (enTasks)Math.Max((int)task1, Math.Max((int)task2, Math.Max((int)task3, Math.Max((int)task4, Math.Max((int)task5, Math.Max((int)task6, (int)task7))))));

                return action;
            }
        }

        public bool IsFolder
        {
            get
            {
                if (Children == null)
                    return false;

                return true;
            }
        }

        public bool IsFile
        {
            get
            {
                if (Children == null)
                    return true;

                return false;
            }
        }


        // ______________Property Updates

        //public bool bPropertyChangedActivated { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            //if (bPropertyChangedActivated)
            {
                PropertyChangedEventHandler handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }



}