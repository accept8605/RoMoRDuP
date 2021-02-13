using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Text;
using System.Windows;

using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;




namespace RoMoRDuP.UserInterface.Optimize
{
    public class UserOptionsOptimize : UserInterfaceBase
    {
        //public ObservableCollection<ScriptEntry> ScriptEntrys { get; set; }
        //has to be user Control ______DataContext______
        public ObservableCollection<UserControls.UCOptimizeScriptEntry> ucScriptEntrys { get; set; } // wird gesetzt in UCOptimizeRenameOptions

        public UserOptionsOptimize(Tasks.FileLists fileLists, UserOptionsType userOptionsType, RoMoRDuP.MainWindow mainWindow)
            : base(fileLists, userOptionsType, mainWindow)
        {
            bSelectTarget_Visible = false;

            bProcessHashCodes_Visible = false;


            fileFilterOptions.bIncludeOnly = true; // should be set last only when overwritten
            fileFilterOptions.IncludeOnly = "*.mp3, *.wma, *.wav, *.mpg, *.mkv, *.avi, *.mp4, *.wmv";
            fileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*, *.jpg, *.jpeg, *.png, *.bmp, *.gif, *.ico";


            PlaylistFileFilterOptions.bIncludeOnly = true; // should be set last only when overwritten
            PlaylistFileFilterOptions.IncludeOnly = "*.m3u, *.wpl, *.kpl";
            PlaylistFileFilterOptions.AlwaysExclude = "*[RoMoRDuP_Ignore]*";
            

        }
    }




    public class ScriptEntry // Is UserControl DataContext
    {
        public ScriptEntry()
        {
            conditionBase1 = null; // wird gesetzt wenn die condition hinzugefügt wird
            conditionBase2 = null;

            actionBase1 = null; // wird gesetzt wenn die action hinzugefügt wird
            actionBase2 = null;
        }


        // ______________Propertys für Anzeige des User Interface
        public ConditionBase conditionBase1 { get; set; }
        public ConditionBase conditionBase2 { get; set; }

        public ActionBase actionBase1 { get; set; }
        public ActionBase actionBase2 { get; set; }


        // ______________Propertys für die einfachere weiterverarbeitung


        /*
        // ______________Property Updates ?

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
         */


    }






    public class ConditionBase
    {
        public ConditionBase()
        {
            //

        }


        // ______________Propertys für Anzeige des User Interface
        private bool int_bIncludeOnly;
        public bool bIncludeOnly
        {
            get
            {
                return int_bIncludeOnly;
            }

            set
            {
                int_bIncludeOnly = value;

                OnPropertyChanged("bIncludeOnly");
            }

        }

        // ______________Propertys für die einfachere weiterverarbeitung

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





    public class ActionBase
    {
        public ActionBase()
        {
            //

        }


        // ______________Propertys für Anzeige des User Interface




        // ______________Propertys für die einfachere weiterverarbeitung



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
