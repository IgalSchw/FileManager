using FileManager.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManager
{
    public  class Directories
    {
        private const string cross = " ├─";
        private const string corner = " └─";
        private const string vertical = " │ ";

        private static int tableWidth = 73;

        public Directories()
        { 
            
        }

        // show catalog by path
        public int ShowDirectory(string sDir, int pageNumber, int elementsNumber)
        {
            //pageNumber = 2;
            //elementsNumber = 5;
            Console.Clear();
            
            DirectoryInfo DirInfo = new DirectoryInfo(sDir);

            Console.WriteLine("_________________________________________________________________________");
            Console.WriteLine("Current Catalog:\n");
                        
            Console.WriteLine("+ " + sDir);

            try
            {
                // N
                foreach (string dir in Directory.GetDirectories(sDir).Skip((pageNumber - 1) * 3).Take(elementsNumber))
                {
                    try
                    {
                        if (IsDirectoryEmpty(dir)) // is has subdirectories
                        {
                            Console.WriteLine(cross + " + " + dir.Replace(sDir, ""));

                            foreach (string subDir in Directory.GetDirectories(dir)) // sub directories
                            {
                                if (IsDirectoryEmpty(subDir))
                                {
                                    Console.WriteLine(vertical + "       " + cross + " + " + subDir.Replace(dir, ""));

                                    foreach (var subsubDir in Directory.GetDirectories(subDir))
                                    {
                                        if (IsDirectoryEmpty(subsubDir))
                                        {
                                            Console.WriteLine(vertical + "       " + vertical + "     " + cross + " + " + subsubDir.Replace(subDir, ""));
                                        }
                                        else
                                        {
                                            //Console.WriteLine(_vertical + "                " + _corner + subsubDir.Replace(subDir, ""));
                                            Console.WriteLine(vertical + "       " + vertical + "     " + cross + "   " + subsubDir.Replace(subDir, ""));
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(vertical + "       " + cross + "   " + subDir.Replace(dir, ""));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(cross + "   " + dir.Replace(sDir, ""));
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // Add log file
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            Console.WriteLine();
            Console.WriteLine("_________________________________________________________________________");

            Console.WriteLine("Files In Current Catalog:");
            DrawTable.PrintLine();
            DrawTable.PrintRow("File Name", "Created Time", "Created Date", "Size");
            DrawTable.PrintLine();
            Console.WriteLine("");

            foreach (var item in Files.GetAllFilesInCurrenctDirectory(sDir))
            {
                DrawTable.PrintRow(item.Name,item.CreationTime.ToShortTimeString(), item.CreationTime.ToShortDateString(), (item.Length/1024).ToString()+ " KB");
                DrawTable.PrintLine();
            }

            Console.WriteLine("-Page " + (pageNumber) + " From " + ((Directory.GetDirectories(sDir).Length /elementsNumber) + 1) + "-");
            Console.WriteLine();
            
            return (Directory.GetDirectories(sDir).Length / elementsNumber) + 1;

        }

        // check if directory exist
        public static void CheckIfDirectoryExistCreate(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    throw new WrongUserInputPathException(path);
                }                
            }         
        }

        // only check if source directory exist
        public static void CheckIfDirectoryExist(string path)
        {
            if (Directory.Exists(path))
            { }
            else
            {
                throw new WrongUserInputPathException(path);
            }
        }

        // copy all directory to destination directory
        public static async void CopyDirectory(string source, string destination)
        {
            foreach (string filename in Directory.EnumerateFiles(source))
            {
                using (FileStream SourceStream = File.Open(filename, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destination + filename.Substring(filename.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }
            }
        }

        // recursively delete folder
        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            baseDir.Delete(true);
        }
                
        //check if directory has any file or folder
        public bool IsDirectoryEmpty(string path)
        {
            return Directory.EnumerateFileSystemEntries(path).Any();
        }

        // return directory informmation of specific folder
        public static DirectoryInfo GetDirectoryInformation(string path)
        {
            StringBuilder strBuild = new StringBuilder();
            //DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo di = new DirectoryInfo(path);
            
            return di;
        }
      
        // check size of directory
        public static long GetSizeFromDirectory(string dirPath)
        {
            long currentSize = 0, subDirSize = 0;
            IEnumerable<string> directories = null;
            IEnumerable<string> files = null;

            try
            {
                files = Directory.EnumerateFiles(dirPath);

                // get the sizeof all files in the current directory
                currentSize = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();

                directories = Directory.EnumerateDirectories(dirPath);
            }
            catch (UnauthorizedAccessException ex)
            { 
                // write log
            }

            try
            {
                // get the size of all files in all subdirectories
                subDirSize = (from directory in directories select GetSizeFromDirectory(directory)).Sum();
            }
            catch (ArgumentNullException ex)
            { 
                // write log
            }

            return currentSize + subDirSize;
        }



        /*
        // get size of drive
        public long GetSizeOfDrive(string sDir)
        {
            string[] GetDrives = Environment.GetLogicalDrives();
       
            foreach (string item in GetDrives)
            {
                string drive;
                drive = item;
                DriveInfo GetInfo = new DriveInfo(item);
                if (GetInfo.DriveType == DriveType.CDRom || GetInfo.DriveType == DriveType.Network)
                {
                    continue;
                }
                if (sDir.ToLower() == item.ToLower()) // drive root
                {
                    return GetInfo.TotalSize - GetInfo.AvailableFreeSpace;
                }
            }

            return -1; // else -1
        }

        */

    }
}
