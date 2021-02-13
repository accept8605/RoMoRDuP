
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using System.Security.Cryptography;
using System.IO;

using RoMoRDuP.Tasks;
using System.Threading;

using System.Xml;
using System.Security;

namespace RoMoRDuP.TaskPlanner
{


    public static class UsefulMethods
    {
        public static string GetAttributesString(cAttributes attributes)
        {
            string strRet = "";

            strRet += attributes.ReadOnly.ToString() + ";" + attributes.Hidden.ToString() + ";" + attributes.strCreatedDateTime;

            return strRet;
        }


        public static TaskNodeViewModel FindNodeHierarchicalByPath(TaskNodeViewModel BaseNode, string PathWithoutSourceFolder, bool checkBasePath, string basePath)
        {
            if( (checkBasePath==false) || (basePath==BaseNode.basePath) )
                if (BaseNode != null)
                {
                    List<string> FolderAndFilenames = GetFolderAndFileNames(PathWithoutSourceFolder, "", true);
                    if (FolderAndFilenames == null)
                        return null;

                    TaskNodeViewModel curNode = BaseNode;

                    for (int level = 0; level < FolderAndFilenames.Count; level++)
                    {
                        TaskNodeViewModel newNode = null;

                        /* // old Version
                        foreach (TaskNodeViewModel node in curNode.Children)
                            if (node.Name == FolderAndFilenames[level])
                            {
                                newNode = node;

                                if (level < FolderAndFilenames.Count - 1)
                                    break;
                                else
                                    return newNode;
                            }
                         */


                        List<TaskNodeViewModel> newNodes = null;

                        if (curNode.childrensNames.ContainsKey(FolderAndFilenames[level]))
                            newNodes = curNode.childrensNames[FolderAndFilenames[level]];

                        if (newNodes != null)
                        {
                            newNode = newNodes[0];

                            if (level < FolderAndFilenames.Count - 1)
                                curNode = newNode;
                            else
                                return newNode;
                        }
                        else
                        {
                            return null;
                        }

                    }

                }

            return null;
        }


        public static bool FilePathInBasePath(string strFilePath, string basePath)
        {
            bool bResult = false;

            if (strFilePath.Contains(basePath))
            {

                string baseRemovedFromFilePath = strFilePath.Replace(basePath.TrimEnd('\\'), "");

                if (baseRemovedFromFilePath.Length > 0)
                {
                    if (baseRemovedFromFilePath[0] == '\\')
                        bResult = true;
                }

            }

            return bResult;
        }


        public static string PlaylistCombinePaths(string basePath, string filePath)
        {
            string strRet = "";

            List<string> listBasePath = GetFolderAndFileNames(basePath, Path.GetPathRoot(basePath), false);
            List<string> listFilePath = GetFolderAndFileNames(filePath, Path.GetPathRoot(filePath), false);

            if ((listBasePath == null) || (listFilePath == null))
                return "";

            List<string> listResult = new List<string>();

            bool bMatches = false;
            int iMatchIndex = 0;
            foreach (string strBasePathPart in listBasePath)
            {
                if (iMatchIndex >= listFilePath.Count)
                    break;

                if (strBasePathPart == listFilePath[iMatchIndex])
                {
                    iMatchIndex++;
                    bMatches = true;
                }
                else
                {
                    if (bMatches)
                    {
                        break;
                    }
                }

                listResult.Add(strBasePathPart);
            }


            for (; iMatchIndex < listFilePath.Count; iMatchIndex++)
                listResult.Add(listFilePath[iMatchIndex]);

            strRet = Path.GetPathRoot(basePath);

            strRet = strRet.TrimEnd('\\');

            foreach (string str in listResult)
                strRet += "\\" + str;


            return strRet;
        }


        public static string GetPathWithoutSourceFolder(string orgPath,string sourceFolder)
        {
            string ret = "";

            /*
            string root = System.IO.Path.GetPathRoot(orgPath);
            if (sourceFolder == root)
            {
                string rootWithoutBackslash = root.Replace("\\", "");
                ret = orgPath.Replace(rootWithoutBackslash, "");
            }
            else
            {
                ret = orgPath.Replace(sourceFolder, "");
            }
             */

            ret = orgPath.Replace(sourceFolder.TrimEnd('\\'), "");


            return ret;

        }


        public static string CombinePathWithSourceFolder(string toCombine, string sourceFolder)
        {
            string ret = "";

            ret = sourceFolder.TrimEnd('\\') + toCombine;

            return ret;
        }

        public static string ReplacePath(string orgPath, string ToReplace, string ReplaceWith)
        {
            string ret = orgPath.Replace(ToReplace.TrimEnd('\\'), ReplaceWith.TrimEnd('\\'));


            return ret;
        }


        public static string AddDateTimeToFilename(string originalPath)
        {
            string strRet = "";

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(originalPath);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(originalPath);

            strRet = dirInfo.Parent.FullName;
            strRet += "\\" + Path.GetFileNameWithoutExtension(originalPath);
            strRet += "_olderVersion";

            strRet += "_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            strRet += "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();

            strRet += fileInfo.Extension;

            return strRet;
        }

        public static string AddModificationDateTimeToFilename(string originalPath)
        {
            string strRet = "";

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(originalPath);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(originalPath);

            strRet = dirInfo.Parent.FullName;
            strRet += "\\" + Path.GetFileNameWithoutExtension(originalPath);
            strRet += "_olderVersion";

            strRet += "_" + fileInfo.LastWriteTime.Year.ToString() + "-" + fileInfo.LastWriteTime.Month.ToString() + "-" + fileInfo.LastWriteTime.Day.ToString();
            strRet += "_" + fileInfo.LastWriteTime.Hour.ToString() + "_" + fileInfo.LastWriteTime.Minute.ToString();

            strRet += fileInfo.Extension;

            return strRet;
        }


        public static List<string> GetFolderAndFileNames(string path, string SourceFolderPath, bool bThrowErrors)
        {
            try
            {
                List<string> strFolderNames = new List<string>();

                string shortenedPath = path;
                for (int i = 0; true; i++)
                {
                    DirectoryInfo dirInfo = System.IO.Directory.GetParent(shortenedPath);
                    if (dirInfo == null) // already on Root
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
            catch (Exception ex)
            {
                if(bThrowErrors)
                    MessageBox.Show("Error Filename: " + ex.Message);

                return null;
            }

        }


        public static string ConvertFileSizeToString(double filesize)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (Math.Abs(filesize) >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                filesize = filesize / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", filesize, sizes[order]);
            return result;
        }



        // Pinvoke for API function
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        public static bool DriveFreeBytes(string folderName, out ulong freespace)
        {
            freespace = 0;
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                freespace = free;
                return true;
            }
            else
            {
                return false;
            }
        }
    }




}