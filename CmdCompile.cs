using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdaCompiler
{
   public class CmdCompile : Cmd
   {
      public override string Discriminant => "compile";

      public override Param[] Params => new Param[] { new ParamRootDir(), new ParamInFile() };

      protected override void myExecute(Param[] @params)
      {
         var roo_dir = (ParamRootDir)@params[0];
         var roo_dir_pth = (string)roo_dir.Value;
         var fil = (ParamInFile)@params[1];
         var fil_pth = (string)fil.Value;

         if (File.Exists(fil_pth))
         {
            var stt = Settings.FromRootDir(roo_dir_pth);

            switch (Path.GetExtension(fil_pth).ToLower())
            {
               case ".c":
               case ".cpp":
                  var cpp_hlp = new CompileHelperCpp();

                  cpp_hlp.Compile(stt, fil_pth, CompileActionEnum.build);
                  break;

               case ".adb":
               case ".ada":
               case ".ads":
                  var ada_hlp = new CompileHelperAda();

                  ada_hlp.Compile(stt, fil_pth, CompileActionEnum.build);
                  break;

               default:
                  Console.WriteLine("Extension {0} not supported!", Path.GetExtension(fil_pth));
                  break;
            }
         }
         else { Console.WriteLine("File {0} not existing", fil_pth); }
      }
   }
}
