using Gate.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdaCompiler
{
   /// <summary>
   /// 
   /// </summary>
   public class BuildHelper
   {
      public void Build(Settings settings, CompileActionEnum compileAction)
      {
         var cmp_hlp_cpp = new CompileHelperCpp();
         var cmp_hlp_ada = new CompileHelperAda();

         //get files to be compiled (.cpp, .ada , .adb);
         var bui_lst = settings.BuildList;
         var is_ok = true;

         foreach (var itm in bui_lst)
         {
            var ext = Path.GetExtension(itm).ToLower();

            switch (ext)
            {
               case ".adb":
               case ".ada":
                  is_ok &= cmp_hlp_ada.Compile(settings, itm, compileAction);
                  break;

               case ".c":
               case ".cpp":
                  is_ok &= cmp_hlp_cpp.Compile(settings, itm, compileAction);
                  break;

               default: throw new AdaCompiler.ErrorException("Extension {0} not allowed!", ext);
            }
         }

         if (is_ok) { Link(settings, compileAction); }
      }

      public bool Link(Settings settings, CompileActionEnum compileAction)
      {
         if (compileAction == CompileActionEnum.clean)
         {
            if (File.Exists(settings.TargetPath))
            {
               try
               {
                  Console.WriteLine("Cleaning {0}..", settings.TargetName);
                  File.Delete(settings.TargetPath);
               }
               catch (System.IO.IOException exc) { Console.WriteLine("Can't clean {0}:\n{1}", settings.TargetName, exc.Message); }
               catch (Exception exc) { throw new Crash(-1, exc); }
            }

            return true;
         }

         if (myIsLinkNecessary(settings))
         {
            var cmd_lin = "";
            var exi = -1;
            var std_out = null as string;
            var std_err = null as string;
            var out_nam = Path.GetFileNameWithoutExtension(settings.Main);

            Directory.CreateDirectory(settings.OutputDirAbsolute);

            ////binding
            cmd_lin = out_nam + ".ali";
            Console.WriteLine("gnatbind {0}", cmd_lin);
            exi = CompileHelper.ExecuteCmdLine(settings, "gnatbind", cmd_lin, true, out std_out, out std_err);

            if (exi != 0)
            {
               Console.WriteLine(std_err);

               return false;
            }

            // linking
            cmd_lin = string.Format(
               "-o {0} {1}.ali {2}",
               settings.TargetName,
               out_nam,
               string.Join(" ", settings.BuildListCpp.Select(i => Path.GetFileNameWithoutExtension(i) + ".o")));
            Console.WriteLine("gnatlink {0}", cmd_lin);
            exi = CompileHelper.ExecuteCmdLine(settings, "gnatlink", cmd_lin, true, out std_out, out std_err);

            if (exi != 0)
            {
               Console.WriteLine(std_err);

               return false;
            }
            else
            {
               Console.WriteLine("Bind-Link successfull");

               return true;
            }
         }
         else
         {
            Console.WriteLine("Bind-Link not necessary.");

            return true;
         }
      }

      private bool myIsLinkNecessary(Settings settings)
      {
         var out_dat = File.GetLastWriteTimeUtc(settings.TargetPath);

         if (File.Exists(settings.TargetPath))
         {
            foreach (var itm in Directory.EnumerateFiles(settings.OutputDirAbsolute).
               Where(f => new string[] { ".ali", ".bin" }.Contains(Path.GetExtension(f).ToLower())))
            {
               var o_ali_dat = File.GetLastWriteTimeUtc(itm);

               if (o_ali_dat > out_dat) { return true; }
            }

            return false;
         }
         else { return true; }
      }
   }
}
