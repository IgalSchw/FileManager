using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
    [Serializable]
    class InvalidUserCommandException : Exception
    {
        public InvalidUserCommandException(string UserCommand) : base (String.Format("Invalid user command {0}\n", UserCommand))
        {

        }
    }
}
