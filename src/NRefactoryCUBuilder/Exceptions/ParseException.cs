using System;

namespace NRefactoryCUBuilder.Exceptions
{
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {
        }
    }
}
