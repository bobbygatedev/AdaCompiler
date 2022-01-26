using System.IO;

namespace AdaCompiler
{
   public class ParamRootDir : Param
   {
      public override string Name => "Root Dir";

      public override object Check(string[] args, ref int aIndex)
      {
         if (aIndex < args.Length)
         {
            var cnd_dir = args[aIndex++];

            if (Directory.Exists(cnd_dir)) { return Path.GetFullPath(cnd_dir); }
            else { throw new AdaCompiler.ErrorException(string.Format("Directory {0} not exists!", cnd_dir)); }
         }
         else
         {
            throw new AdaCompiler.ErrorException("Root directory expected!");
         }
      }
   }
}
