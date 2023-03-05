using System;

namespace AzyWorks.System.Exceptions
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string caller, string statement) : base ($"Assertion called by {caller} failed: {statement}") { }
    }
}