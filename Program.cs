using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdaCompiler
{
   /// <summary>
   /// todo:
   /// -fase 1 task di compilazione con success/fail/build
   /// -fase 2 folding keyword
   /// -fase 3 intellisense
   /// </summary>
   class Program
   {
      /// <summary>
      /// tododo:
      /// - completa task compilazione
      /// - allinea i task con task.vs.json
      /// </summary>
      /// <param name="args"></param>
      static void Main(string[] args)
      {
         try { Cmd.SearchAndExecute(args, new CmdBuild(), new CmdCompile(), new CmdClean(), new CmdRebuild()); }
         catch (ErrorException exc)
         {
            Console.WriteLine("Error:");
            Console.WriteLine(exc.Message);
         }
      }
   }
}
