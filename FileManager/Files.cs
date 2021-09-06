using FileManager.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManager
{
    public class Files
    {
        // return all files of current catalog
        public static FileInfo[] GetAllFilesInCurrenctDirectory(string sDir)
        {
            DirectoryInfo d = new DirectoryInfo(sDir); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files

            return Files;
        }
        
        // copy file
        public static void CopyFile(string source, string destination)
        {
            // with overwrite
           File.Copy(source, destination,true);
        }

        // check if file exist if not create it for destination
        public static void CheckIfFileExistAndCreate(string path)
        {
            if (!File.Exists(path))
            {
                try
                {
                    File.Create(path).Dispose();
                }
                catch
                {
                    throw new WrongUserInputPathException(path);
                }            
            }
        }

        // only check if source file exist
        public static void CheckIfFileExist(string path)
        {
            if (File.Exists(path))
            {}
            else
            {
                throw new WrongUserInputPathException(path);
            }
        }

        //delete file
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        // get information about file
        public static FileInfo GetFileInformation(string path)
        {
            StringBuilder strBuild = new StringBuilder();
            //DirectoryInfo di = new DirectoryInfo(path);
            
            FileInfo fi = new FileInfo(path);

            return fi;
        }
    }
}
