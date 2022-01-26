using Gate.Tools;
using Gate.Tools.Text;
using System.IO;
using System.Linq;

namespace AdaCompiler
{
   public class CompileHelperCpp : CompileHelper
   {
      protected override string myGetCmdName(Settings settings, string sourcePath) => Path.GetExtension(sourcePath) == ".cpp" ? "g++" : "gcc";

      protected override string myGetCmdLine(Settings settings, string sourcePath)
      {
         var obj_dir_rel = "./obj";
         var f_nam_no_ext = Path.GetFileNameWithoutExtension(sourcePath);
         var src_rel_pth = my_GetModuleRelativePath(settings.RootDir, sourcePath);
         var cmd_lin = string.Format("-g -c -o {0}/{1}.o {2}", obj_dir_rel, f_nam_no_ext, src_rel_pth);
         var ext = Path.GetExtension(sourcePath);

         return cmd_lin;
      }

      protected override string myGetReworkedStdOutErr(Settings settings, string sourcePath, string stdOutput, string stdError) => 
         stdError == "" ? $"Compilation of {Path.GetFileName(sourcePath)} successfull." : stdError;

      protected override bool myIsRecompileNecessary(Settings settings, string sourcePath)
      {
         var o_fil = myGetProducts(settings, sourcePath)[0];
         var o_dat = File.GetLastWriteTimeUtc(o_fil);
         var src_dat = File.GetLastWriteTimeUtc(sourcePath);

         if (!File.Exists(o_fil) || o_dat < src_dat) { return true; }
         else
         {
            var std_out = null as string;
            var std_err = null as string;

            var exi = ExecuteCmdLine(settings, myGetCmdName(settings, sourcePath), "-M " + sourcePath, false, out std_out, out std_err);

            if (exi != 0) { throw new Crash(); }

            var fil = TextStoreFile.FromString(std_out);

            foreach (var ln in fil.Lines.Skip(1))
            {
               if (ln.Content.Trim() != "")
               {
                  var ln_cnt = ln.Content.Trim();

                  ln_cnt = ln_cnt.Substring(0, ln_cnt.Length - 1).Trim();

                  var h_dat = File.GetLastWriteTimeUtc(ln_cnt);

                  if (h_dat > src_dat) { return true; }
               }
            }

            return false;
         }
      }

      protected override string[] myGetProducts(Settings settings, string sourcePath) =>
         new string[] { Path.Combine(settings.OutputDirAbsolute, Path.GetFileNameWithoutExtension(sourcePath) + ".o") };
   }
}
