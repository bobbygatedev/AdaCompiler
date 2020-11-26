using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaCompiler
{
   public class ErrorException : Exception
   {
      public ErrorException(string message) : base(message) { }
      public ErrorException(string message, params object[] pars) : base(string.Format(message,pars)) { }
   }
}
