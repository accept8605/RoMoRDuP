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

using System.ComponentModel;


namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCTaskViewChildAfter.xaml
    /// </summary>
    public partial class UCTaskViewChildAfter : UserControl, INotifyPropertyChanged
    {
        Tasks.TaskTreeViewModel TreeViewModel_After;

        public Tasks.TaskTreeViewModel TreeModel
        {
            get
            {
                return TreeViewModel_After;
            }
            set
            {
                TreeViewModel_After = value;
                OnPropertyChanged("TreeModel");
            }
        }


        Dictionary<Tasks.TaskNodeViewModel, string> NVM_TreeView_After_selectedItems =
            new Dictionary<Tasks.TaskNodeViewModel, string>();

        const double const_DragMoveDist = 10.0;


        Tasks.Tasks tasks;

        UserInterface.UserInterfaceBase userOptions;

        TaskViewParentType taskViewParentType;

        TaskPlanner.TaskPlannerBase taskPlannerBase { get; set; }

        degUpdateAfterView updateAfterView { get; set; }

        public UCTaskViewChildAfter(Tasks.Tasks _tasks, TaskViewParentType taskViewParentType, TaskPlanner.TaskPlannerBase taskPlannerBase, degUpdateAfterView updateAfterView, UserInterface.UserInterfaceBase userOptions)
        {
            this.tasks = _tasks;

            this.userOptions = userOptions;

            this.taskViewParentType = taskViewParentType;

            this.updateAfterView = updateAfterView;

            this.taskPlannerBase = taskPlannerBase;

            InitializeComponent();

            TreeView_After.SelectedItemChanged +=
                new RoutedPropertyChangedEventHandler<object>(TreeView_After_SelectedItemChanged);

            TreeView_After.Focusable = true;

        }


        public void UpdateView()
        {

            if (userOptions.mainWindow != null)
            {
                userOptions.mainWindow.Dispatcher.BeginInvoke( // Invoke does not always work
                   (Action)(() =>
                   {
                       if (taskViewParentType == TaskViewParentType.Source)
                       {
                           TreeModel = tasks.taskSource.TaskViewAfter;

                           TreeModel.Items = tasks.taskSource.TaskViewAfter.Items;
                       }
                       else if (taskViewParentType == TaskViewParentType.Target)
                       {
                           TreeModel = tasks.taskTarget.TaskViewAfter;

                           TreeModel.Items = tasks.taskTarget.TaskViewAfter.Items;
                       }

                       TreeView_After.UpdateLayout();
                   }
                    ), null);
            }

        }





        //______________________DragAndDrop____________________________

        Point _lastMouseDown;
        Tasks.TaskNodeViewModel _target;

        List<Tasks.TaskNodeViewModel> draggedItems = null;


        private void TreeView_After_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(TreeView_After);
            }

        }


        private bool CheckGridSplitter(UIElement element)
        {
            if (element is GridSplitter)
            {
                return true;
            }

            GridSplitter GridSplitter = FindParent<GridSplitter>(element);

            if (GridSplitter != null)
            {
                return true;
            }
            return false;

        }


        private void TreeView_After_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    UIElement element = e.OriginalSource as UIElement;
                    bool bGridSplitter = CheckGridSplitter(element);


                    Point currentPosition = e.GetPosition(TreeView_After);

                    double movedX = 0,movedY =0;
                    if (!((_lastMouseDown.X == -1) && (_lastMouseDown.Y == -1)))
                    {
                        movedX = Math.Abs(currentPosition.X - _lastMouseDown.X);
                        movedY = Math.Abs(currentPosition.Y - _lastMouseDown.Y);
                    }

                    if ( (movedX > const_DragMoveDist) ||
                        (movedY > const_DragMoveDist) )
                    {
                        //draggedItem = (Tasks.TaskNodeViewModel)TreeView_After.SelectedItem;
                        if (NVM_TreeView_After_selectedItems.Count > 0)
                        {
                            draggedItems = new List<Tasks.TaskNodeViewModel>();
                            foreach (Tasks.TaskNodeViewModel vm in NVM_TreeView_After_selectedItems.Keys)
                                draggedItems.Add(vm);
                        }
                        else
                            draggedItems = null;

                        if ( (draggedItems != null) && !bGridSplitter)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(TreeView_After, TreeView_After.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                
                                // A Move drop was accepted
                                /*
                                bool Correct_drop = true;
                                foreach( Tasks.TaskNodeViewModel vm in draggedItems )
                                    if(vm.Name == _target.Name)
                                        Correct_drop = false;
                                */

                                //private bool CheckDropTarget(List<Tasks.TaskNodeViewModel> _sourceItems, Tasks.TaskNodeViewModel _targetItem)
                                

                                if (CheckDropTarget(draggedItems,_target))
                                {
                                    MoveItem(draggedItems, _target);
                                    _target = null;
                                    draggedItems = null;
                                }      
                                 

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TaskView MouseMove: " + ex.Message);

                draggedItems = null;
            }
        }

        private void TreeView_After_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(TreeView_After);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    Tasks.TaskNodeViewModel item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(draggedItems, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("TaskView DragOver: " + ex.Message);
                e.Effects = DragDropEffects.None;
            }
        }
        private void TreeView_After_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                Tasks.TaskNodeViewModel TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItems != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TaskView Drop: " + ex.Message);
                e.Effects = DragDropEffects.None;
            }



        }

        private bool CheckDropTarget(List<Tasks.TaskNodeViewModel> _sourceItems, Tasks.TaskNodeViewModel _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = true;

            /*
            foreach( Tasks.TaskNodeViewModel vm in _sourceItems )
                if(vm.Name == _targetItem.Name)
                     _isEqual = false;
             */

            // Drop possible if: target==folder and target!=Parent and target!=self
            foreach( Tasks.TaskNodeViewModel vm in _sourceItems )
                if( (vm.Parent == _targetItem) || (vm ==_targetItem) )
                    _isEqual = false;

            if (_targetItem.IsFolder == false)
                _isEqual = false;


            return _isEqual;

        }

        /*
        private void CopyItem(List<Tasks.TaskNodeViewModel> _sourceItems, Tasks.TaskNodeViewModel _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop the selected items into " + _targetItem.Name + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach(Tasks.TaskNodeViewModel vm in _sourceItems)
                        _targetItem.Children.Add(vm);
                }
                catch (Exception)
                {

                }
            }

        }
         */


        private void MoveItem(List<Tasks.TaskNodeViewModel> _sourceItems, Tasks.TaskNodeViewModel _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to move the selected items into " + _targetItem.Name + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Tasks.TaskNodeViewModel vm in _sourceItems)
                    {
                        // ___SOURCE
                        if (taskViewParentType == TaskViewParentType.Source) // Update FileListsAfter
                        {
                            Tasks.TaskNodeViewModel newNode = vm.Clone(_targetItem, taskPlannerBase.IDAfterSource++);

                            _targetItem.Children.Add(newNode);

                            foreach (TaskPlanner.FileListEntry entry in tasks.fileLists.SourceFileListAfter)
                            {
                                if (entry.targetNode == vm)
                                {
                                    entry.targetNode = newNode; // Update FileList
                                }
                            }

                        }
                        // ___TARGET
                        else
                        {
                            Tasks.TaskNodeViewModel newNode = vm.Clone(_targetItem, taskPlannerBase.IDAfterTarget++);

                            _targetItem.Children.Add(newNode);

                            foreach (TaskPlanner.FileListEntry entry in tasks.fileLists.TargetFileListAfter)
                            {
                                if (entry.targetNode == vm)
                                {
                                    entry.targetNode = newNode; // Update FileList
                                }
                            }
                        }



                        // Von Original Parent entfernen
                        //foreach (Tasks.TaskNodeViewModel vm in _sourceItems)
                        //{
                            vm.RemoveSelf();
                        //}
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("TaskView MoveItem: " + ex.Message);
                }

                InitializeComponent();
            }

        }
         

        private Tasks.TaskNodeViewModel GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem UIContainer = FindParent<TreeViewItem>(element);
            Tasks.TaskNodeViewModel NVContainer = null;

            if (UIContainer != null)
            {
                NVContainer = UIContainer.DataContext as Tasks.TaskNodeViewModel;
            }
            return NVContainer;
        }


        private static Parent FindParent<Parent>(DependencyObject child)
                where Parent : DependencyObject
        {
            DependencyObject parentObject = child;
            parentObject = VisualTreeHelper.GetParent(parentObject);

            //check if the parent matches the type we're looking for
            if (parentObject is Parent || parentObject == null)
            {
                return parentObject as Parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<Parent>(parentObject);
            }
        }










        //______________________MultiSelect____________________________

        TreeViewItem lastSelected_TreeViewAfterItem = new TreeViewItem();

        // a set of all selected items
        Dictionary<TreeViewItem, string> TreeView_After_selectedItems =
            new Dictionary<TreeViewItem, string>();


        // true only while left ctrl key is pressed
        bool CtrlPressed
        {
            get
            {
                return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl);
            }
        }

        bool ShiftPressed
        {
            get
            {
                return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift);
            }
        }



        // deselects the tree item
        void Deselect(TreeViewItem treeViewItem)
        {
            //if (treeViewItem.DataContext == {DisconnectedItem} )
            //    return;
            try
            {
                if (treeViewItem != null)
                    if ((treeViewItem.DataContext as Tasks.TaskNodeViewModel) != null)
                {
                    treeViewItem.Background = Brushes.White;// change background and foreground colors
                    treeViewItem.Foreground = Brushes.Black;
                    TreeView_After_selectedItems.Remove(treeViewItem); // remove the item from the selected items set
                    NVM_TreeView_After_selectedItems.Remove(treeViewItem.DataContext as Tasks.TaskNodeViewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TaskView Deselect: " + ex.Message);

                // {DisconnectedItems} bug in WPF
                NVM_TreeView_After_selectedItems =new Dictionary<Tasks.TaskNodeViewModel, string>();
                TreeView_After_selectedItems = new Dictionary<TreeViewItem, string>();
            }
        }

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        void ChangeSelectedState(TreeViewItem treeViewItem)
        {

            if (Select(treeViewItem))
                ;
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }

        // use MouseButtonUp Event instead
        void TreeView_After_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem treeViewItem = lastSelected_TreeViewAfterItem;

            if (treeViewItem == null)
                return;

            // prevent the WPF tree item selection 
            treeViewItem.IsSelected = false;

        }

        TreeViewItem LastShift_treeViewItem = null;

        enum enShiftState { Idle, Item1, Item2 }

        private void TreeView_After_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!(
                    (Math.Abs(e.GetPosition(TreeView_After).X - _lastMouseDown.X) > const_DragMoveDist) ||
                        (Math.Abs(e.GetPosition(TreeView_After).Y - _lastMouseDown.Y) > const_DragMoveDist)
                    ))
                {
                    TreeViewItem treeViewItem = lastSelected_TreeViewAfterItem;

                    if (treeViewItem == null)
                        return;


                    treeViewItem.Focus();

                    if (!CtrlPressed && !ShiftPressed) // Deselect everything
                    {
                        LastShift_treeViewItem = null;

                        List<TreeViewItem> selectedTreeViewItemList = new List<TreeViewItem>();
                        foreach (TreeViewItem treeViewItem1 in TreeView_After_selectedItems.Keys)
                        {
                            selectedTreeViewItemList.Add(treeViewItem1);
                        }

                        foreach (TreeViewItem treeViewItem1 in selectedTreeViewItemList)
                        {
                            Deselect(treeViewItem1);
                        }
                    }
                    else if (ShiftPressed) // Select between first and Second Shift
                    {
                        // alle items zwischen alter/neuer selection aktivieren

                        if (LastShift_treeViewItem != null)
                        {
                            // Get all children elements of parent
                            TreeViewItem UIContainerCurrent = FindParent<TreeViewItem>(treeViewItem);
                            TreeViewItem UIContainerLast = FindParent<TreeViewItem>(LastShift_treeViewItem);

                            enShiftState shiftState = enShiftState.Idle;

                            if (UIContainerCurrent == UIContainerLast)
                            {
                                List<TreeViewItem> selectedTreeViewItemList = new List<TreeViewItem>();
                                foreach (TreeViewItem tv in FindDirectVisualChildren<TreeViewItem>(UIContainerCurrent))
                                {
                                    // Get element 1
                                    if (tv == LastShift_treeViewItem)
                                        shiftState = enShiftState.Item1;

                                    if (tv == treeViewItem && shiftState==enShiftState.Item1)
                                        shiftState = enShiftState.Item2;

                                    // Select everything until element 2  
                                    if ( shiftState == enShiftState.Item1)
                                        selectedTreeViewItemList.Add(tv);
                                }

                                // If element 2 not found, discard changes
                                if( shiftState == enShiftState.Item2 )
                                    foreach (TreeViewItem treeViewItem1 in selectedTreeViewItemList)
                                    {
                                        Select(treeViewItem1);
                                    }
                            }
                        }

                        
                    }
                    LastShift_treeViewItem = lastSelected_TreeViewAfterItem;

                    ChangeSelectedState(treeViewItem);

                }
            }


            _lastMouseDown = new Point(-1, -1); // Set up for not dragged


        }

        public static IEnumerable<T> FindDirectVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            bool success = false;
            int MaxLevel = 0;

            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        success = true;
                        yield return (T)child;
                    }
                }

                if( !success )
                {
                    IEnumerable<T> ret = null;
                    while(MaxLevel < 20)
                    {
                        MaxLevel++;
                        ret = FindVisualChildren<T>(depObj, MaxLevel, 0);
                        if (ret.Count<T>() > 0)
                            break;
                    }

                    foreach (T val in ret)
                        yield return val;
                }
            }
        }


        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj, int MaxLevel, int CurLevel) where T : DependencyObject
        {
            bool success = false;

            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    if (++CurLevel < MaxLevel)
                    {
                        foreach (T childOfChild in FindVisualChildren<T>(child, MaxLevel, CurLevel))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }

        }



        bool Select(TreeViewItem treeViewItem)
        {
            try
            {
                if (!TreeView_After_selectedItems.ContainsKey(treeViewItem) && !NVM_TreeView_After_selectedItems.ContainsKey(treeViewItem.DataContext as Tasks.TaskNodeViewModel))
                { // select
                    treeViewItem.Background = Brushes.Black; // change background and foreground colors
                    treeViewItem.Foreground = Brushes.White;
                    try
                    {
                        TreeView_After_selectedItems.Add(treeViewItem, null); // add the item to selected items
                        NVM_TreeView_After_selectedItems.Add(treeViewItem.DataContext as Tasks.TaskNodeViewModel, null);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("TaskView Select: A element with this Key does already exist" + ex.Message);
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("TaskView Select: The selected item got disconnected? " + ex.Message);
                return false;
            }

        }


        private void TreeView_After_TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;

            // set the last tree view item selected variable which may be used elsewhere as there is no other way I have found to obtain the TreeViewItem container (may be null)
            lastSelected_TreeViewAfterItem = tvi;
        }

        private void TreeView_After_TreeViewItemUnSelected(object sender, RoutedEventArgs e)
        {
            //lastSelected_TreeViewAfterItem = null;
        }







        // ____________ Mouse wheel_____________________



        private void TS_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineUp();
            }
            else
            {
                scrollviewer.LineDown();
            }
            e.Handled = true;
        }


        // ______________Other_____________________


        private void Button_UpdateView_Click(object sender, RoutedEventArgs e)
        {
            updateAfterView();
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

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            //((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = true; // TBD Buggy?
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            //((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = false; // TBD Buggy?
        }


        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is TreeViewItem)
            {
                TreeViewItem tvi = sender as TreeViewItem;

                if (lastSelected_TreeViewAfterItem != tvi) // only if selected
                {
                    return;
                }

                if (sender != null)
                {
                    Tasks.TaskNodeViewModel node = ((TreeViewItem)sender).DataContext as Tasks.TaskNodeViewModel;

                    if (node != null)
                    {
                        // Datei Öffnen
                        try
                        {
                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                            process.EnableRaisingEvents = false;
                            process.StartInfo.FileName = node.Path_1_Original;
                            process.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("TaskView: Cant execute " + node.Path_1_Original + " : " + ex.Message);
                        }
                    }
                }

            }
        }



    }
}
