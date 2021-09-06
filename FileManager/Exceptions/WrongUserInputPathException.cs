using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.Exceptions
{
    [Serializable]
    class WrongUserInputPathException : Exception
    {
        public WrongUserInputPathException(string path) : base (String.Format("Wrong Path: '{0}'\n", path))
        {

        }
    }
}
