//----------------------------------------------------------------------------
//
// <copyright file="ObservableCollection.cs" company="Microsoft">
//    Copyright (C) 2003 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
//
// Description: Implementation of an Collection<t> implementing INotifyCollectionChanged
//              to notify listeners of dynamic changes of the list.
//
// See spec at [....]/connecteddata/Specs/Collection%20Interfaces.mht
//
// History:
//  11/22/2004 : [....] - created
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;

namespace RoMoRDuP.Tasks
{
    /// <summary>
    /// Implementation of a dynamic data collection based on generic Collection<T>,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    [Serializable()]
    public class CustomObservableCollection<t> : ObservableCollection<t>
    {
        //-----------------------------------------------------
        //
        //  Constructors
        //
        //-----------------------------------------------------

        #region Constructors
        /// <summary>
        /// Initializes a new instance of ObservableCollection that is empty and has default initial capacity.
        /// </summary>
        public CustomObservableCollection() : base() { }


        #endregion Constructors





        //-----------------------------------------------------
        //
        //  Protected Methods
        //
        //-----------------------------------------------------


        /// <summary>
        /// Called by base class Collection<T> when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            if (typeof(t) == typeof(TaskNodeViewModel))
            {
                if (this.Count > 0)
                {
                    TaskNodeViewModel vm = (TaskNodeViewModel)((object)this[0]);

                    if (vm.Parent != null)
                        vm.Parent.childrensNames = new Dictionary<string, List<TaskNodeViewModel>>();
                }
            }

            base.ClearItems();
        }

        /// <summary>
        /// Called by base class Collection<T> when an item is removed from list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            
            if (typeof(t) == typeof(TaskNodeViewModel))
            {
                TaskNodeViewModel vm = (TaskNodeViewModel)((object)this[index]);

                if(vm.Parent != null)
                    vm.Parent.childrensNames.Remove(vm.Name);
            }

            base.RemoveItem(index);
        }

        /// <summary>
        /// Called by base class Collection<T> when an item is added to list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void InsertItem(int index, t item)
        {
            base.InsertItem(index, item);

            if (typeof(t) == typeof(TaskNodeViewModel))
            {
                TaskNodeViewModel vm = (TaskNodeViewModel)((object)this[index]);

                if (vm.Parent != null)
                {
                    if(vm.Parent.childrensNames.ContainsKey(vm.Name))
                    {
                        vm.Parent.childrensNames[vm.Name].Add(vm);
                    }
                    else
                    {
                        vm.Parent.childrensNames.Add(vm.Name, new List<TaskNodeViewModel>(){vm});
                    }
                }
            }
        }

        /// <summary>
        /// Called by base class Collection<T> when an item is set in list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void SetItem(int index, t item)
        {
            TaskNodeViewModel oldvm = null;

            if (typeof(t) == typeof(TaskNodeViewModel))
            {
                oldvm = (TaskNodeViewModel)((object)this[index]);
            }

            base.SetItem(index, item);

            if (typeof(t) == typeof(TaskNodeViewModel))
            {
                TaskNodeViewModel newvm = (TaskNodeViewModel)((object)this[index]);

                if (newvm.Parent != null)
                {
                    newvm.Parent.childrensNames.Remove(oldvm.Name);
                    newvm.Parent.childrensNames.Add(newvm.Name, new List<TaskNodeViewModel>(){newvm});
                }
            }
        }




    }
}
