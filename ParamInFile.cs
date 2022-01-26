using System;
using System.IO;

namespace AdaCompiler
{
   public class ParamInFile : Param
   {
      public override string Name => throw new NotImplementedException();

      public override object Check(string[] args, ref int aIndex)
      {
         if (aIndex < args.Length)
         {
            var in_fil = args[aIndex++];

            if (File.Exists(in_fil)) { return Path.GetFullPath(in_fil); }
            else { throw new AdaCompiler.ErrorException(string.Format("File {0} not exists!", in_fil)); }
         }
         else { throw new AdaCompiler.ErrorException("In-File expected!"); }
      }
   }
}
