using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;



namespace TaskExecutionClient
{

    enum enMainState { GetTask, GetPathSource, GetPathTarget, ExecuteTask };
    enum enTask { None, CreateSubfolder, SetAttributes, Copy, Rename, Move, Delete, CreateShortcut }

    public class ExecutionClient
    {


        public string ExecuteTaskString(string strServer)
        {

            // __________Execute Tasks__________

            enMainState mainState = enMainState.GetTask;
            enTask curTask = enTask.None;
            string strPathSource = "";
            string strPathTarget = "";

            try
            {

                //Console.WriteLine("Receives {0} bytes; Message: \"{1}\"",
                //    cbBytesRead, strMessage);

                string[] listReceived = strServer.Split('\n');

                foreach (string line in listReceived)
                {
                    if (line.Length > 0)
                    {
                        string strline = line.TrimEnd('\0');
                        strline = strline.TrimStart('\0');

                        Console.WriteLine("[CLIENT] Echo: " + strline);


                        // Process Message
                        switch (mainState)
                        {
                            case enMainState.GetTask:
                                curTask = GetTask(strline);
                                mainState = enMainState.GetPathSource;
                                break;

                            case enMainState.GetPathSource:
                                strPathSource = strline;
                                mainState = enMainState.GetPathTarget;
                                break;

                            case enMainState.GetPathTarget:
                                strPathTarget = strline;
                                mainState = enMainState.ExecuteTask;
                                break;

                            default:
                                mainState = enMainState.GetTask;
                                break;

                        }


                        if (mainState == enMainState.ExecuteTask)
                        {

                            // Send one message to the pipe.

                            string strAnswer = ExecuteTask(curTask, strPathSource, strPathTarget);


                            return strAnswer;
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("The client throws the error: {0}", ex.Message);
            }


            return null;

        }



        private static string ExecuteTask(enTask Task, string strPathSource, string strPathTarget)
        {
            string strRet = "";

            if (Task != enTask.None)
            {

                try
                {

                    switch (Task)
                    {
                        case enTask.CreateSubfolder:
                            if (System.IO.Directory.CreateDirectory(strPathTarget) == null)
                            {
                                strRet = "ERROR while trying: " + "CreatedSubfolder: At " + strPathTarget
                                        + " - Message: Cannot create folder";
                            }
                            else
                                strRet = "CreatedSubfolder: At " + strPathTarget;

                            break;


                        case enTask.SetAttributes:

                            string[] arrStrAttributes = strPathTarget.Split(';');

                            if (arrStrAttributes.Length > 2)
                            {
                                strRet = "SetAttributes: File " + strPathSource + " to attributes " + strPathTarget;

                                bool ReadOnly = bool.Parse(arrStrAttributes[0]);
                                bool Hidden = bool.Parse(arrStrAttributes[1]);

                                /*strRet += "_" + CreatedDateTime.Year.ToString() + "-" + CreatedDateTime.Month.ToString() + "-" + CreatedDateTime.Day.ToString();
                                strRet += "_" + CreatedDateTime.Hour.ToString() + "_" + CreatedDateTime.Minute.ToString();*/

                                string[] arrStrDateTime = arrStrAttributes[2].Split('_');

                                if (arrStrDateTime.Length > 3)
                                {
                                    string[] arrStrDate = arrStrDateTime[1].Split('-');

                                    if (arrStrDate.Length > 2)
                                    {
                                        // Split OK
                                        string strYear = arrStrDate[0];
                                        string strMonth = arrStrDate[1];
                                        string strDay = arrStrDate[2];

                                        string strHour = arrStrDateTime[2];
                                        string strMin = arrStrDateTime[3];

                                        DateTime CreatedDateTime = new DateTime(int.Parse(strYear), int.Parse(strMonth), int.Parse(strDay), int.Parse(strHour),
                                            int.Parse(strMin), 0);


                                        // Set Creation Time
                                        FileAttributes attributes = File.GetAttributes(strPathSource);
                                        File.SetAttributes(strPathSource, attributes & ~FileAttributes.ReadOnly);

                                        File.SetCreationTime(strPathSource, CreatedDateTime);  // May not be write Protected for this



                                        // Set Attributes

                                        if (ReadOnly)
                                            attributes = attributes | FileAttributes.ReadOnly;
                                        else
                                            attributes = attributes & ~FileAttributes.ReadOnly;

                                        if (Hidden)
                                            attributes = attributes | FileAttributes.Hidden;
                                        else
                                            attributes = attributes & ~FileAttributes.Hidden;

                                        File.SetAttributes(strPathSource, attributes);

                                    }

                                }

                            }
                            else
                            {
                                strRet = "ERROR while trying: " + "SetAttributes: File " + strPathSource + " to attributes" + strPathTarget
                                        + " - Message: Invalid arguments";
                            }

                            break;


                        case enTask.Copy:
                            AddFolderPathsThatDontExist(strPathTarget);
                            strRet = "COPIED: From " + strPathSource + " to " + strPathTarget;
                            File.Copy(strPathSource, strPathTarget);
                            break;

                        case enTask.Delete:
                            // Be careful here
                            // get the file attributes for file or directory
                            FileAttributes attr = File.GetAttributes(strPathSource);

                            //detect whether its a directory or file
                            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                //MessageBox.Show("Its a directory");

                                if (IsDirectoryEmpty(strPathSource))
                                {
                                    strRet = "DELETED: " + strPathSource;

                                    DirectoryInfo di = new DirectoryInfo(strPathSource);
                                    di.Attributes = FileAttributes.Normal;

                                    Directory.Delete(strPathSource);
                                }
                                else
                                {
                                    strRet = "ERROR while trying: " + "DELETED: " + strPathSource
                                        + " - Message: This is not a file or empty Dir";
                                }
                            }
                            else
                            {
                                // Is a file
                                //MessageBox.Show("Its a file");
                                strRet = "DELETED: " + strPathSource;

                                File.SetAttributes(strPathSource, FileAttributes.Normal); // for write protected files
                                File.Delete(strPathSource);
                            }

                            break;

                        case enTask.Move:
                            AddFolderPathsThatDontExist(strPathTarget);
                            strRet = "MOVED: From " + strPathSource + " to " + strPathTarget;
                            File.Move(strPathSource, strPathTarget);
                            break;

                        case enTask.Rename:
                            strRet = "RENAMED: From " + strPathSource + " to " + strPathTarget;
                            File.Move(strPathSource, strPathTarget);
                            break;

                        case enTask.CreateShortcut:
                            AddFolderPathsThatDontExist(strPathTarget);
                            strRet = "CREATED_SHORTCUT: At " + strPathSource + " to " + strPathTarget;

                            addShortcut(strPathSource, strPathTarget);
                            break;

                        default:
                            break;

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CLIENT] Error: " + ex.Message);
                    strRet = "ERROR while trying: " + strRet + " - Message: " + ex.Message;
                }

            }
            else
            {
                strRet = "Invalid Task";
            }



            return strRet + "\n";

        }

        private static enTask GetTask(string line)
        {

            enTask curTask = enTask.None;


            switch (line)
            {
                case "CreateSubfolder":
                    curTask = enTask.CreateSubfolder;
                    break;

                case "SetAttributes":
                    curTask = enTask.SetAttributes;
                    break;

                case "Copy":
                    curTask = enTask.Copy;
                    break;

                case "Delete":
                    curTask = enTask.Delete;
                    break;

                case "Rename":
                    curTask = enTask.Rename;
                    break;

                case "Move":
                    curTask = enTask.Move;
                    break;

                case "CreateShortcut":
                    curTask = enTask.CreateShortcut;
                    break;

                default:
                    curTask = enTask.None;
                    break;
            }

            return curTask;
        }



        private static void AddFolderPathsThatDontExist(string path)
        {
            List<string> strFolderAndFileNames = GetFolderAndFileNames(path, Path.GetPathRoot(path));

            string strExistingPath = Path.GetPathRoot(path);

            // find Parentnode
            if (strFolderAndFileNames.Count > 1)
            {
                int iFolderNameIndex = 0;

                for (iFolderNameIndex = 0; iFolderNameIndex < strFolderAndFileNames.Count - 1; iFolderNameIndex++)
                {
                    if (Directory.Exists(strExistingPath + "\\" + strFolderAndFileNames[iFolderNameIndex]))
                    {
                        // OK
                    }
                    else
                    {
                        if (System.IO.Directory.CreateDirectory(strExistingPath + "\\" + strFolderAndFileNames[iFolderNameIndex]) == null)
                        {
                            Console.WriteLine("[CLIENT] Cannot create Directory " + strExistingPath + "\\" + strFolderAndFileNames[iFolderNameIndex]);
                            return;
                        }
                        else
                            Console.WriteLine("[CLIENT] Created Directory " + strExistingPath + "\\" + strFolderAndFileNames[iFolderNameIndex]);
                    }

                    strExistingPath += "\\" + strFolderAndFileNames[iFolderNameIndex];
                }

            }
        }



        public static List<string> GetFolderAndFileNames(string path, string SourceFolderPath)
        {
            List<string> strFolderNames = new List<string>();


            string shortenedPath = path;
            for (int i = 0; true; i++)
            {
                DirectoryInfo dirInfo = System.IO.Directory.GetParent(shortenedPath);

                if (dirInfo == null)
                    break;

                shortenedPath = dirInfo.FullName;

                if (shortenedPath.Contains(SourceFolderPath))
                {
                    strFolderNames.Add(dirInfo.Name);
                }
                else
                    break;
            }

            // find Parentnode
            List<string> strResult = new List<string>();

            if (strFolderNames.Count > 1)
            {
                int iFolderNameIndex = strFolderNames.Count - 2; // Count -2 because Highest Index = Count -1 is the ParentSourceFolderName

                for (; iFolderNameIndex >= 0; iFolderNameIndex--)
                {
                    strResult.Add(strFolderNames[iFolderNameIndex]);
                }
            }

            strResult.Add(System.IO.Path.GetFileName(path));


            return strResult;
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }


        private static void addShortcut(string atPath, string toPath)
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                var lnk = shell.CreateShortcut(atPath);
                try
                {
                    lnk.TargetPath = toPath;
                    lnk.IconLocation = "shell32.dll, 1";
                    lnk.Save();
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
            }
        }

    }
}
