using System;
using System.Runtime.Serialization;

namespace WindowsFormsApp1
{
    [Serializable]
    internal class InvalidDirectorySelected : Exception
    {
        public InvalidDirectorySelected()
        {
        }

        public InvalidDirectorySelected(string message) : base(message)
        {
        }

        public InvalidDirectorySelected(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidDirectorySelected(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}