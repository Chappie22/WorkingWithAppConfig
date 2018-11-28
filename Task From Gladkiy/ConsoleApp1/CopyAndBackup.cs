using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace ConsoleApp1
{
    public class CopyAndBackup
    {
        private static readonly NameValueCollection AppSettings = ConfigurationManager.AppSettings;
        private readonly ListDictionary _listDictionary = new ListDictionary();

        public CopyAndBackup()
        {
            foreach (var key in AppSettings.AllKeys)
            {
                _listDictionary[key] = AppSettings[key];                
            }
        }
        
        public void ShowAllKeys()
        {
            foreach (DictionaryEntry o in _listDictionary)
            {
                Console.WriteLine($"{o.Key} = {o.Value}");
            }
        }

        /// <summary>
        /// Copying all files that is in App.config to a new directory (also matched in AppConfig) Also creating a Backup Folder
        /// </summary>
        public void Copy()
        {
            var path = Convert.ToString(_listDictionary["target"]);
            
            var checkNewPath = new DirectoryInfo(path);
            if (checkNewPath.GetDirectories().Length != 0 && checkNewPath.GetFiles().Length != 0)
            {
                FolderCopy(path,_listDictionary["backup"].ToString());
            }
            
            if (_listDictionary.Count == 0) Console.WriteLine("No data");
            else
            {                
                foreach (DictionaryEntry o in _listDictionary)
                {
                    if ((string) o.Key == "target" || (string) o.Key == "backup") continue;
                    
                    var attributes = File.GetAttributes(o.Value.ToString());
                    if (attributes.HasFlag(FileAttributes.Directory)) FolderCopy(o.Value.ToString(),path);
                    else FileCopy(o.Value.ToString(),path);
                }                                
            }

        }

        private static void FileCopy(string sourcePath, string  directoryPath)
        {
            File.Copy(sourcePath,directoryPath,true);
           
        }

        private static void FolderCopy(string sourcePath, string directoryPath)
        {
            var sourcePathInfo = new DirectoryInfo(sourcePath);
            var directoryPathInfo = new DirectoryInfo(directoryPath);
            
            //Creating a directory and copying all files to a it
            var path = directoryPathInfo.CreateSubdirectory(sourcePathInfo.Name);
            foreach (var t in sourcePathInfo.GetFiles())
            {
                FileCopy(t.ToString(),path.ToString());
            }
                                    
            //Looking for folders, if there are some, working with their files
            //(Creating a sudirectories and copying all files to a it)
            var allFoldersIn = new DirectoryInfo[sourcePathInfo.GetDirectories().Length];
            if (allFoldersIn.Length != 0)
            {
                foreach (var t in sourcePathInfo.GetDirectories())
                {
                    FolderCopy(t.ToString(),path.ToString());
                }
            }
        }        
    }
}