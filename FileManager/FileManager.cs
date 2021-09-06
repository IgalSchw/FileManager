using FileManager.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace FileManager
{
    public class FileManager
    {
        public static string DefaultPath = string.Empty;
        private Directories directories = null;

        // first page by defaut
        public static int PageNumber = 1;
        // elements number default
        private const int elementsNumber = 3;

        public FileManager(string defaultPath, int pageNumber)
        {
            DefaultPath = defaultPath;
            PageNumber = pageNumber;
            Run(DefaultPath, PageNumber);
        }
        private void Run(string defaultPath, int pageNumber)
        {
            int totalPage;
            directories = new Directories();        
            totalPage = directories.ShowDirectory(defaultPath, pageNumber, elementsNumber);
            UserCommandInput(defaultPath, totalPage);
        }
               
        private void UserCommandInput(string path, int totalPage)
        {           
            string userInputCMD = string.Empty;

            string[] multiWordsCommand = null;

            bool boolExit = false;

            // set default command
            CmdFileManager.commands cmd = CmdFileManager.commands.next;
            directories = new Directories();

            do
            {
                // input user command
                try
                {
                    // show user command menu
                    string strCmd = CmdFileManager.ShowMenu(path);
                    
                    // for ls, cp, rm
                    multiWordsCommand = CmdFileManager.ValidateUserCommand(strCmd);
                    
                    cmd = (CmdFileManager.commands)Enum.Parse(typeof(CmdFileManager.commands), multiWordsCommand[0]);
                }

                // throw exception
                catch (InvalidUserCommandException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    
                    cmd = CmdFileManager.commands.wrongCMD; // not initialized command (to Default)
                }

                // Commands
                switch (cmd)
                {
                    #region show catalog by path
                    case CmdFileManager.commands.ls:

                        if (multiWordsCommand.Length > 1)
                        {
                            string targetPath = multiWordsCommand[1];

                            if (directories.ShowDirectory(targetPath, 1, 3) == 0)
                            {
                                Console.WriteLine("Wrong input or the path is not exist. Try again! To continue press any key.");
                                Console.ReadKey();
                                RefreshView(path, 1, 3);
                                break;
                            }
                            
                            PageNumber = 1;

                            path = targetPath + @"\";

                            if (path.Contains(@"\\") == true)
                            {
                                path = path.Replace(@"\\", @"\");
                            }
                        }

                        break;

                    #endregion

                    #region move next page
                    case CmdFileManager.commands.next:

                        if (totalPage > PageNumber)
                        {
                            PageNumber += 1;
                        }
                        break;
                    #endregion

                    #region move previous page
                    // move previous page
                    case CmdFileManager.commands.prev:

                        if (PageNumber > 1)
                        {
                            PageNumber -= 1;
                        }

                        break;
                    #endregion
                    
                    #region copy directory
                    case CmdFileManager.commands.cpd:

                        if (multiWordsCommand.Length > 2)
                        {
                            string sourceDirectoryPath = multiWordsCommand[1];
                            string destDirectoryPath = multiWordsCommand[2];

                            if (!CheckDirectoryPath(sourceDirectoryPath, destDirectoryPath))
                            {
                                Directories.CopyDirectory(sourceDirectoryPath, destDirectoryPath);
                                Console.WriteLine("The directory was copied succesfully!\n");
                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();                             
                            }
                            else
                            {
                                Console.WriteLine("Wrong input.Try again! To continue press any key.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input.Try again! To continue press any key.");
                            Console.ReadKey();                       
                        }

                        break;

                    #endregion

                    #region copy file
                    // multiwordsCommand[0] = command (cpf)
                    case CmdFileManager.commands.cpf:

                        if (multiWordsCommand.Length > 2)
                        {
                            string sourceFilePath = multiWordsCommand[1];
                            string destFilePath = multiWordsCommand[2];

                            if (!CheckFilePath(sourceFilePath, destFilePath))
                            {
                                Files.CopyFile(sourceFilePath, destFilePath);
                                Console.WriteLine("The file was copied succesfully!\n");
                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Wrong input.Try again! To continue press any key.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input.Try again! To continue press any key.");
                            Console.ReadKey();                         
                        }

                        break;
                    #endregion

                    #region remove directory recurisevly and remove specific file
                    case CmdFileManager.commands.rm:

                        if (multiWordsCommand.Length > 1)
                        {
                            string targetPath = multiWordsCommand[1];

                            if (!CheckDirectoryPath(targetPath))
                            {
                                // recursively
                                Directories.RecursiveDelete(new DirectoryInfo(targetPath));
                                Console.WriteLine("The directory was Deleted!\n");
                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();
                            }
                            else if (!CheckFilePath(targetPath))
                            {
                                Files.DeleteFile(targetPath);
                                Console.WriteLine("The file was Deleted!\n");
                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Wrong input.Try again! To continue press any key.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input.Try again! To continue press any key.");
                            Console.ReadKey();
                        }
                        break;
                    #endregion

                    #region Get File information and directory information
                    case CmdFileManager.commands.file:

                        if (multiWordsCommand.Length > 1)
                        {
                            string targetPath = multiWordsCommand[1];

                            if (!CheckDirectoryPath(targetPath))
                            {
                                DirectoryInfo di = null;

                                DrawTable.PrintLine();
                                DrawTable.PrintRow("Directory Name", "Created Time", "Created Date", "Size");
                                DrawTable.PrintLine();
                                di = Directories.GetDirectoryInformation(targetPath);
                                DrawTable.PrintRow(di.Name, di.CreationTime.ToShortTimeString(), di.CreationTime.ToShortDateString(), Directories.GetSizeFromDirectory(targetPath) + "KB");
                                DrawTable.PrintLine();

                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();
                            }
                            else if (!CheckFilePath(targetPath))
                            {
                                FileInfo fi = null;

                                DrawTable.PrintLine();
                                DrawTable.PrintRow("File Name", "Created Time", "Created Date", "Size");
                                DrawTable.PrintLine();
                                fi = Files.GetFileInformation(targetPath);
                                DrawTable.PrintRow(fi.Name, fi.CreationTime.ToShortTimeString(), fi.CreationTime.ToShortDateString(), (fi.Length / 1024) + "KB");
                                DrawTable.PrintLine();

                                Console.WriteLine("To continue press any key.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Wrong input.Try again! To continue press any key.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input.Try again! To continue press any key.");
                            Console.ReadKey();
                        }

                        break;
                    #endregion

                    #region exit
                    // close file manager application
                    case CmdFileManager.commands.ex:
                        boolExit = true;
                        break;
                    #endregion

                    #region Clear console
                    case CmdFileManager.commands.cl:
                        Console.Clear();
                        PageNumber = 1;
                        path = DefaultPath;
                        break;
                    #endregion

                    // default wrong command
                    #region default command
                    default:
                        Console.WriteLine("To continue press any key.");
                        Console.ReadKey();                     
                        break;
                    #endregion
                }


                RefreshView(path, PageNumber, elementsNumber);

            } while (!boolExit);

            Console.WriteLine();
            Console.WriteLine("Bye Bye! see you.");
            //Environment.Exit(0);
        }

        // for files
        private bool CheckFilePath(string? targetPath, string? sourcePath)
        {
            try
            {
                Files.CheckIfFileExist(targetPath);
                Files.CheckIfFileExistAndCreate(sourcePath);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the path of folder ");
                return true;
            }
            catch (WrongUserInputPathException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }

            return false;
        }

        // for specific file
        private bool CheckFilePath(string? filePath)
        {
            try
            {
                Files.CheckIfFileExist(filePath);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the path of folder ");
                return true;
            }
            catch (WrongUserInputPathException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }

            return false;
        }

        // for directories
        private bool CheckDirectoryPath(string? targetPath, string? sourcePath)
        {
            try
            {
                Directories.CheckIfDirectoryExist(targetPath);
                Directories.CheckIfDirectoryExistCreate(sourcePath);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the path of folder ");
                return true;
            }
            catch (WrongUserInputPathException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }

            return false;
        }

        // for specific directory
        private bool CheckDirectoryPath(string? directoryPath)
        {
            try
            {
                Directories.CheckIfDirectoryExist(directoryPath);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the path of folder ");
                return true;
            }
            catch (WrongUserInputPathException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }

            return false;
        }

        // refresh current catalog path
        private void RefreshView(string path, int pageNumber, int elementNumber)
        {
            DefaultPath = path;
            PageNumber = pageNumber;
            directories.ShowDirectory(path, pageNumber, elementNumber);
        }
    }
}
