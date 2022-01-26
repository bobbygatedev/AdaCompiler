using System;

namespace AdaCompiler
{
   public class ErrorException : Exception
   {
      public ErrorException(string message) : base(message) { }
      public ErrorException(string message, params object[] pars) : base(string.Format(message,pars)) { }
   }
}
