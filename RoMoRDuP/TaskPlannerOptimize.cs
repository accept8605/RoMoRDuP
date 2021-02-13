using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using RoMoRDuP.Tasks;

namespace RoMoRDuP.TaskPlanner
{

    public class TaskPlannerOptimize : TaskPlannerBase
    {
        public SimilarTermsModel similarTermsModel { get; set; }

        public TaskPlannerOptimize(UserInterface.UserInterfaceBase userOptions, TaskViews taskSource, TaskViews taskTarget, Tasks.Tasks tasks, Tasks.FileLists fileLists, PlaylistUpdates.PlaylistUpdates playlistUpdates)
            : base(userOptions, taskSource, taskTarget,tasks, fileLists, playlistUpdates)
        {
            similarTermsModel = new SimilarTermsModel();

        }


    }


    public class SimilarTermsModel
    {
        public SimilarTermsModel()
        {
            InitDuplicates();
        }

        public ObservableCollection<SimilarTermsViewModel> Items { get; set; }

        private void InitDuplicates()
        {
            Items = new ObservableCollection<SimilarTermsViewModel>();

            SimilarTermsViewModel item = new SimilarTermsViewModel();

            item.bUnify = true;

            VersionOfTermViewModel vm = new VersionOfTermViewModel();
            vm.term = "one version";
            item.VersionsOfTerm.Add(vm);

            vm = new VersionOfTermViewModel();
            vm.term = "another version";
            item.VersionsOfTerm.Add(vm);

            vm = new VersionOfTermViewModel();
            vm.term = "one more version";
            item.VersionsOfTerm.Add(vm);


            Items.Add(item);

        }
    }

    public class SimilarTermsViewModel
    {
        public SimilarTermsViewModel()
        {
            VersionsOfTerm = new List<VersionOfTermViewModel>();
            term = "some term";
        }

        // general Data
        public String term { get; set; }
        public List<VersionOfTermViewModel> VersionsOfTerm { get; set; }
        public bool bUnify { get; set; }
    }

    public class VersionOfTermViewModel
    {
        public string term { get; set; }

        public VersionOfTermViewModel()
        {
            term = "init";
        }

    }


}
