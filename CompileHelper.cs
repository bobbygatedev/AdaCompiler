using Gate.Tools;
using System;
using System.Diagnostics;
using System.IO;

namespace AdaCompiler
{
   public abstract class CompileHelper
   {
      protected abstract string myGetCmdName(Settings settings, string sourcePath);
      protected abstract string[] myGetProducts(Settings settings, string sourcePath);
      protected abstract bool myIsRecompileNecessary(Settings settings, string sourcePath);
      protected abstract string myGetReworkedStdOutErr(Settings settings, string sourcePath, string stdOutput, string stdError);
      protected abstract string myGetCmdLine(Settings settings, string sourcePath);

      public bool Compile(Settings settings, string sourcePath, CompileActionEnum compileAction)
      {
         if (compileAction == CompileActionEnum.clean)
         {
            var prs = myGetProducts(settings, sourcePath);

            foreach (var prd in prs)
            {
               try 
               {
                  if (File.Exists(prd))
                  {
                     Console.WriteLine("Cleaning {0}.." , Path.GetFileName(prd));
                     File.Delete(prd);
                  }                  
               }
               catch (System.IO.IOException exc)
               {
                  Console.WriteLine("I/O error during deletion of file" + sourcePath + ":");
                  Console.WriteLine(exc.Message);
               }
               catch (Exception exc) { throw new Crash(-1, exc); }
            }

            return true;
         }
         else if (compileAction == CompileActionEnum.rebuild || myIsRecompileNecessary(settings, sourcePath))
         {
            var std_out = null as string;
            var std_err = null as string;
            var exi = -1;

            Directory.CreateDirectory(settings.OutputDirAbsolute);

            exi = ExecuteCmdLine(
               settings,
               myGetCmdName(settings, sourcePath),
               myGetCmdLine(settings, sourcePath),
               false,
               out std_out,
               out std_err);

            var rew_out = myGetReworkedStdOutErr(settings, sourcePath, std_out, std_err);

            Console.WriteLine(rew_out);

            return exi == 0;
         }
         else { return true; }
      }

      public static int ExecuteCmdLine(
         Settings settings, string cmd, string cmdLine, bool isForLink, out string stdOutput, out string stdError)
      {
         var psi = new ProcessStartInfo();

         psi.FileName = cmd;
         psi.Arguments = cmdLine;
         psi.UseShellExecute = false;
         psi.RedirectStandardOutput = true;
         psi.RedirectStandardError = true;
         psi.WorkingDirectory = isForLink ? settings.OutputDirAbsolute : settings.RootDir;

         var pro = Process.Start(psi);
         pro.Start();
         pro.WaitForExit();

         stdOutput = pro.StandardOutput.ReadToEnd();
         stdError = pro.StandardError.ReadToEnd();

         return pro.ExitCode;
      }

      protected static string my_GetModuleRelativePath(string rootDir, string sourcePath)
      {
         var roo_ful = Path.GetFullPath(rootDir);
         var src_ful = Path.GetFullPath(sourcePath);
         var rel = "." + src_ful.Substring(roo_ful.Length).Replace("\\", "/");

         return rel;
      }
   }
}
