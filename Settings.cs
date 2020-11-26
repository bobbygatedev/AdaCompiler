using Gate.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdaCompiler
{
   /// <summary>
   /// tododo rinomina e tratta come project
   /// </summary>
   public class Settings
   {
      public const string ADA_PROP_NAME = "AdaProperties.json";

      /// <summary>
      /// tododo tira fuori
      /// </summary>
      public enum TargetTypeEnum
      {
         exe = 0,
      }

      public Settings() { }

      public string Main { get; set; } = "main";

      public TargetTypeEnum TargetType { get; set; } = TargetTypeEnum.exe;

      public string RootDir { get; set; }
      public string[] BuildList
      {
         get
         {
            var dir_inf = new DirectoryInfo(RootDir);
            var fls = dir_inf.EnumerateFiles("*.*", SearchOption.AllDirectories);

            return fls.Select(f => f.FullName).Where(f => my_IsAdaExtension(f) || my_IsCppExtension(f)).ToArray();
         }
      }

      public string[] BuildListCpp => BuildList.Where(i => my_IsCppExtension(i)).ToArray();

      public string[] AdsArray => Directory.EnumerateFiles(RootDir, "*.ads", SearchOption.AllDirectories).ToArray();

      private static bool my_IsAdaExtension(string item)
      {
         switch (Path.GetExtension(item).ToLower())
         {
            case ".ada":
            case ".adb":
               return true;

            default: return false;
         }
      }

      private static bool my_IsCppExtension(string item)
      {
         switch (Path.GetExtension(item).ToLower())
         {
            case ".cpp":
            case ".c":
               return true;

            default: return false;
         }
      }

      /// <summary>
      /// tododo aggiungi opzione in file .json
      /// </summary>
      public string OutputDirRelative { get; set; } = @"./obj";
      public string OutputDirAbsolute
      {
         get
         {
            var out_dir =
               OutputDirRelative.StartsWith("\\") || OutputDirRelative.StartsWith("/") ?
                  OutputDirRelative.Substring(1) : OutputDirRelative;

            return Path.GetFullPath(Path.Combine(RootDir, out_dir));
         }
      }

      public string TargetName => Path.GetFileNameWithoutExtension(Main) + TargetExtension;
      public string TargetPath => Path.Combine(OutputDirAbsolute, TargetName);

      public string TargetExtension
      {
         get
         {
            switch (TargetType)
            {
               case TargetTypeEnum.exe: return ".exe";

               default: throw new Crash();
            }
         }
      }

      public static Settings FromRootDir(string rootDir)
      {
         var dir_inf = new DirectoryInfo(rootDir);
         var jsn_inf = dir_inf.EnumerateFiles().FirstOrDefault(f => string.Compare(f.Name, ADA_PROP_NAME, true) == 0);

         if (jsn_inf != null) { return Settings.FromJson(jsn_inf.FullName); }
         else { return new Settings(); }
      }

      public static Settings FromJson(string jsonPath)
      {
         try
         {
            using (var tr = new StreamReader(jsonPath))
            {
               var jo = JsonConvert.DeserializeObject<JObject>(tr.ReadToEnd());
               var res = new Settings();

               res.Main = jo["main"].ToString();//tododo gestione errore
               res.TargetType = (TargetTypeEnum)Enum.Parse(typeof(TargetTypeEnum), jo["target"].ToString().Substring(1));
               res.RootDir = Directory.GetParent(jsonPath).FullName;

               return res;
            }
         }
         catch (IOException exc) { throw new AdaCompiler.ErrorException(string.Format("I/O error during reading of {0}: {1}", jsonPath, exc.Message)); }
         catch (Exception exc) { throw new Crash(-1, exc); }
      }
   }
}
