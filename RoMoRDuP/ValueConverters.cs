using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Text;
using System.Windows;

using System.Collections.Generic;

namespace RoMoRDuP.ValueConverters
{
    /*
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target is not a bool!");


            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target is not a bool!");


            return !(bool)value;
        }
    }
     */

    /*
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target is not a Visibility!");

            Visibility visRet = Visibility.Collapsed;

            if ((bool)value)
                visRet = Visibility.Visible;

            return visRet;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target is not a bool!");

            bool bRet = false;

            if ((Visibility)value == Visibility.Visible)
                bRet = true;

            return bRet;
        }
    }
     */




    public class ActionToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target is not a brush!");

            Tasks.enTasks task = (Tasks.enTasks)value;
            //public enum enTasks { None, CreateSubfolder, Copy, Rename, Move, Delete } // Order of biggestAction

            Brush brush = Brushes.Transparent;

            switch (task)
            {
                case Tasks.enTasks.Org:
                    brush = Brushes.Transparent ;
                    break;

                case Tasks.enTasks.CreateSubfolder :
                    brush = Brushes.Green ;
                    break;

                case Tasks.enTasks.Copy :
                    brush = Brushes.LimeGreen ;
                    break;

                case Tasks.enTasks.Rename :
                    brush = Brushes.SkyBlue ;
                    break;

                case Tasks.enTasks.SetAttributes:
                    brush = Brushes.MediumPurple;
                    break;

                case Tasks.enTasks.Move :
                    brush = Brushes.Yellow ;
                    break;

                case Tasks.enTasks.Delete:
                    brush = Brushes.OrangeRed ;
                    break;

                default:
                    break;
            }




            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }



    public class ActionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target is not a string!");

            Tasks.enTasks task = (Tasks.enTasks)value;


            String str = task.ToString();


            return str;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class PathToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            //if (targetType != typeof(String))
            //    throw new InvalidOperationException("The target is not a string!");

            String strSource = (string)value;

            return System.IO.Path.GetFileNameWithoutExtension(strSource);
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class ActionToStringAfterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target is not a string!");

            Tasks.enTasks task = (Tasks.enTasks)value;

            String str = "default";

            switch (task)
            {
                case Tasks.enTasks.Copy:
                    str = "Copied";
                    break;

                case Tasks.enTasks.CreateSubfolder:
                    str = "CreatedSubfolder";
                    break;

                case Tasks.enTasks.Delete:
                    str = "Deleted";
                    break;

                case Tasks.enTasks.SetAttributes:
                    str = "SetAttributes";
                    break;

                case Tasks.enTasks.Move:
                    str = "Moved";
                    break;

                case Tasks.enTasks.Org:
                    str = "Org";
                    break;

                case Tasks.enTasks.Rename:
                    str = "Renamed";
                    break;

                default:
                    break;
            }
            


            return str;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }





    public class ActionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target is not a visibility!");

            Tasks.enTasks task = (Tasks.enTasks)value;

            Visibility vis = Visibility.Collapsed;

            if (task > Tasks.enTasks.Org)
                vis = Visibility.Visible;

            return vis;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target is not a visibility!");

            String str = (String)value;

            Visibility vis = Visibility.Collapsed;

            if(str != null)
                if (str.Length > 0)
                    vis = Visibility.Visible;

            return vis;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }




    public class StringListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target is not a visibility!");

            List<String> listStr = (List<String>)value;

            Visibility vis = Visibility.Collapsed;

            if (listStr.Count > 0)
                    vis = Visibility.Visible;

            return vis;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }




    public class RadioBoolToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {

            if (targetType != typeof(bool?))
                throw new InvalidOperationException("The target is not a bool!");


            if (value is UserInterface.enOptionsMirrorDuplicates)
            {

                UserInterface.enOptionsMirrorDuplicates orgValue = (UserInterface.enOptionsMirrorDuplicates)value;

                String strPara = parameter.ToString();


                if (
                    (strPara == "Skip" && orgValue == UserInterface.enOptionsMirrorDuplicates.Skip) ||
                    //(strPara == "RenameTarget" && orgValue == UserInterface.enOptionsMirrorDuplicates.RenameTarget) ||
                    (strPara == "Copy" && orgValue == UserInterface.enOptionsMirrorDuplicates.CopyAnyway) ||
                    (strPara == "RenameSyncDir" && orgValue == UserInterface.enOptionsMirrorDuplicates.RenameSyncDir)
                    )
                    return true;
                else
                    return false;

            }
            else if (value is UserInterface.enOptionsMirrorUpdated)
            {

                UserInterface.enOptionsMirrorUpdated orgValue = (UserInterface.enOptionsMirrorUpdated)value;

                String strPara = parameter.ToString();


                if (
                    (strPara == "Skip" && orgValue == UserInterface.enOptionsMirrorUpdated.Skip) ||
                    (strPara == "MostRecentDate" && orgValue == UserInterface.enOptionsMirrorUpdated.MostRecentDate) ||
                    (strPara == "SyncDir" && orgValue == UserInterface.enOptionsMirrorUpdated.SyncDir) 
                    /*
                    (strPara == "Target" && orgValue == UserInterface.enOptionsMirrorUpdated.Target) ||
                    (strPara == "CopyAddNumber" && orgValue == UserInterface.enOptionsMirrorUpdated.RenameAddNumber) ||
                    (strPara == "CopyAddDate" && orgValue == UserInterface.enOptionsMirrorUpdated.RenameAddDate)
                     */
                    )
                    return true;
                else
                    return false;

            }
            else if (value is UserInterface.enOptionsMirrorMoveTo)
            {

                UserInterface.enOptionsMirrorMoveTo orgValue = (UserInterface.enOptionsMirrorMoveTo)value;

                String strPara = parameter.ToString();


                if (
                    (strPara == "MoveSyncDir" && orgValue == UserInterface.enOptionsMirrorMoveTo.SyncDir) ||
                    /*(strPara == "MoveTarget" && orgValue == UserInterface.enOptionsMirrorMoveTo.Target) || */
                    (strPara == "DontMove" && orgValue == UserInterface.enOptionsMirrorMoveTo.NoMoving)
                    )
                    return true;
                else
                    return false;

            }
            else if (value is UserInterface.enOptionsMirrorMovedRenaming)
            {

                UserInterface.enOptionsMirrorMovedRenaming orgValue = (UserInterface.enOptionsMirrorMovedRenaming)value;

                String strPara = parameter.ToString();


                if (
                    (strPara == "SyncDir" && orgValue == UserInterface.enOptionsMirrorMovedRenaming.SyncDir) ||
                    /*(strPara == "Target" && orgValue == UserInterface.enOptionsMirrorMovedRenaming.Target) ||*/
                    (strPara == "DontChange" && orgValue == UserInterface.enOptionsMirrorMovedRenaming.NoRenaming)
                    )
                    return true;
                else
                    return false;

            }
            else
                return false;


        }


        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {

            bool orgValue = (bool)value;

            String strPara = parameter.ToString();

            if (targetType == typeof(UserInterface.enOptionsMirrorDuplicates))
            {
                if (strPara == "Skip" && orgValue == true)
                    return UserInterface.enOptionsMirrorDuplicates.Skip;
               /* else if (strPara == "RenameTarget" && orgValue == true)
                    return UserInterface.enOptionsMirrorDuplicates.RenameTarget; */
                else if (strPara == "Copy" && orgValue == true)
                    return UserInterface.enOptionsMirrorDuplicates.CopyAnyway;
                else if (strPara == "RenameSyncDir" && orgValue == true)
                    return UserInterface.enOptionsMirrorDuplicates.RenameSyncDir;
                else
                    return null;
            }

            else if (targetType == typeof(UserInterface.enOptionsMirrorUpdated))
            {
                if (strPara == "Skip" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.Skip;
                else if (strPara == "MostRecentDate" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.MostRecentDate;
                else if (strPara == "SyncDir" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.SyncDir;
                    /*
                else if (strPara == "Target" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.Target;
                else if (strPara == "CopyAddNumber" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.RenameAddNumber;
                else if (strPara == "CopyAddDate" && orgValue == true)
                    return UserInterface.enOptionsMirrorUpdated.RenameAddDate;
                     */
                else
                    return null;
            }

            else if (targetType == typeof(UserInterface.enOptionsMirrorMoveTo))
            {
                if (strPara == "MoveSyncDir" && orgValue == true)
                    return UserInterface.enOptionsMirrorMoveTo.SyncDir;
                /*else if (strPara == "MoveTarget" && orgValue == true)
                    return UserInterface.enOptionsMirrorMoveTo.Target; */
                else if (strPara == "DontMove" && orgValue == true)
                    return UserInterface.enOptionsMirrorMoveTo.NoMoving;
                else
                    return null;
            }

            else if (targetType == typeof(UserInterface.enOptionsMirrorMovedRenaming))
            {
                if (strPara == "SyncDir" && orgValue == true)
                    return UserInterface.enOptionsMirrorMovedRenaming.SyncDir;
                    /*
                else if (strPara == "Target" && orgValue == true)
                    return UserInterface.enOptionsMirrorMovedRenaming.Target; */
                else if (strPara == "DontChange" && orgValue == true)
                    return UserInterface.enOptionsMirrorMovedRenaming.NoRenaming;
                else
                    return null;
            }

            else
                throw new InvalidOperationException("The target is not valid!");
        }
    }


    public class RadioBoolMirrorEasyToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {

            if (targetType != typeof(bool?))
                throw new InvalidOperationException("The target is not a bool!");


            if (value is UserInterface.enOptionsMirrorEasy)
            {

                UserInterface.enOptionsMirrorEasy orgValue = (UserInterface.enOptionsMirrorEasy)value;

                String strPara = parameter.ToString();


                if (
                    (strPara == "SyncOneWayLeave" && orgValue == UserInterface.enOptionsMirrorEasy.SyncOneWayLeave) ||
                    (strPara == "SyncBothWays" && orgValue == UserInterface.enOptionsMirrorEasy.SyncBothWays) ||
                    (strPara == "SyncOneWayRemove" && orgValue == UserInterface.enOptionsMirrorEasy.SyncOneWayRemove)
                    )
                    return true;
                else
                    return false;

            }
            else
                return false;


        }


        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {

            bool orgValue = (bool)value;

            String strPara = parameter.ToString();

            if (targetType == typeof(UserInterface.enOptionsMirrorEasy))
            {
                if (strPara == "SyncOneWayLeave" && orgValue == true)
                    return UserInterface.enOptionsMirrorEasy.SyncOneWayLeave;
                else if (strPara == "SyncBothWays" && orgValue == true)
                    return UserInterface.enOptionsMirrorEasy.SyncBothWays;
                else if (strPara == "SyncOneWayRemove" && orgValue == true)
                    return UserInterface.enOptionsMirrorEasy.SyncOneWayRemove;
                else
                    return null;
            }

            else
                throw new InvalidOperationException("The target is not valid!");
        }
    }


}