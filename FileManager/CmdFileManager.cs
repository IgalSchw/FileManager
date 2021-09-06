using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
    class CmdFileManager
    {
        public static string ShowMenu(string Path)
        {
            Console.WriteLine("ls   C:\\Source                             - Show Tree of directory");
            Console.WriteLine("cpd  C:\\Source D:\\Target                   - Copy Catalog");
            Console.WriteLine("cpf  C:\\Source.txt D:\\Target.txt           - Copy file");
            Console.WriteLine("rm   C:\\Source                             - Recursively delete Catalog");
            Console.WriteLine("rm   C:\\Source.txt                         - Remove file");
            Console.WriteLine("file C:\\source.txt                         - Get information (file)");
            Console.WriteLine("file C:\\source                             - Get information (Folder)");
            Console.WriteLine("cl                                         - clear screen");
            Console.WriteLine("next                                       - Move next page");
            Console.WriteLine("prev                                       - Move prev page");
            Console.WriteLine("ex                                         - Exit from the program");
            Console.WriteLine();
            Console.WriteLine("Please enter commands according to the system. To exit please press 'ex'");

            Console.WriteLine();
            Console.Write(Path);
           
            return  Console.ReadLine();
        } 
       
        public static string[] ValidateUserCommand(string cmd)
        {
            string[] words = cmd.Split(" ");

            bool res = Enum.IsDefined(typeof(CmdFileManager.commands), words[0]);

            if (!res)
                throw new InvalidUserCommandException(cmd);

            return words;
        }
        
        [Flags]
        public  enum commands
        {
            ls,
            cpf,
            cpd,
            rm,
            file,
            cl,
            next,
            prev,
            ex,
            wrongCMD
        }
    }

}
