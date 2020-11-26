using Gate.Tools;
using Gate.Tools.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdaCompiler
{
   public class CompileHelperAda : CompileHelper
   {
      private static Regex my_Regex = new Regex(@"(?<n>\w+(\.\w+))?\:(?<l>\d+)\:(?<c>\d+)\:(?<w>\s*warning\s*\:)?", RegexOptions.Compiled);

      protected override string myGetCmdName(Settings settings, string sourcePath) => "gnat";

      protected override string myGetCmdLine(Settings settings, string sourcePath)
      {
         var mod_rel = my_GetModuleRelativePath(settings.RootDir, sourcePath);
         var cmd_lin = string.Format("compile -g {0} -D {1} {2}", my_GetISettings(settings), settings.OutputDirRelative, mod_rel);

         return cmd_lin;
      }

      protected override string myGetReworkedStdOutErr(Settings settings, string sourcePath, string stdOutput, string stdError)
      {
         var lns_i = TextStoreFile.FromString(stdError).Lines.Select(l => l.Content.Trim()).Where(l => l != "");
         var lns_o = new TextStoreFile();

         foreach (var ln in lns_i)
         {
            var mat = my_Regex.Match(ln);

            if (mat.Success && mat.Index == 0)
            {
               var fil = mat.Groups["n"].Value;
               var lin = int.Parse(mat.Groups["l"].Value);
               var col = int.Parse(mat.Groups["c"].Value);
               var wrn = mat.Groups["w"].Value;
               var dir_inf = new DirectoryInfo(settings.RootDir);
               var fls = dir_inf.EnumerateFiles(fil, SearchOption.AllDirectories).ToArray();

               switch (fls.Length)
               {
                  case 0: throw new AdaCompiler.ErrorException("File {0} not found in dir {1}!", fil, dir_inf.FullName);

                  case 1:
                     var err_str = wrn == "" ? "error ada" : "warning ada";

                     lns_o.AddLine(string.Format(
                        "{0}({1},{2}):{3}: {4}", fls[0].FullName, lin, col, err_str, ln.Substring(mat.Length).Trim()));
                     break;

                  default: throw new AdaCompiler.ErrorException("File {0} has more than instance in {1}!", fil, dir_inf.FullName);
               }
            }
            else if (ln == lns_i.Last() || !ln.ToLower().StartsWith("gnatmake:")) { lns_o.AddLine(ln); }
         }

         return lns_o.Content;
      }

      protected override bool myIsRecompileNecessary(Settings settings, string sourcePath)
      {
         var roo_dir_inf = new DirectoryInfo(settings.RootDir);
         var prs = myGetProducts(settings, sourcePath);
         var prs_dat = prs.Select(p => File.GetLastWriteTimeUtc(p)).ToArray();
         var src_dat = File.GetLastWriteTimeUtc(sourcePath);

         var res =
            prs.Any(p => !File.Exists(p)) ||
            prs_dat.Any(d => d < src_dat) ||
            settings.AdsArray.Any(a => prs_dat.Any(d => File.GetLastWriteTimeUtc(a) > d));

         return res;
      }

      protected override string[] myGetProducts(Settings settings, string sourcePath)
      {
         var o_fil = Path.Combine(settings.OutputDirAbsolute, Path.GetFileNameWithoutExtension(sourcePath) + ".o");
         var ali_fil = Path.Combine(settings.OutputDirAbsolute, Path.GetFileNameWithoutExtension(sourcePath) + ".ali");

         return new string[] { o_fil, ali_fil };
      }

      private static string my_GetISettings(Settings settings)
      {
         var roo_inf = new DirectoryInfo(settings.RootDir);
         var ads_drs = roo_inf.EnumerateFiles("*.ads", SearchOption.AllDirectories).ToArray();
         var res = string.Join(" ",
            ads_drs.Select(f => f.Directory.FullName).Distinct().
            Select(d => "-I" + my_GetModuleRelativePath(roo_inf.FullName, d)));

         return res;
      }
   }
}
