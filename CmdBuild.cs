using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaCompiler
{

   public class CmdBuild : Cmd
   {
      public override string Discriminant => "build";

      public override Param[] Params => new Param[] { new ParamRootDir() };

      protected override void myExecute(Param[] @params)
      {
         var roo_dir = (ParamRootDir)@params[0];
         var hlp = new BuildHelper();
         var stt = Settings.FromRootDir((string)roo_dir.Value);

         hlp.Build(stt, CompileActionEnum.build);
      }
   }
}
