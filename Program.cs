using System;

namespace AdaCompiler
{
   /// <summary>
   /// todo:
   /// -phase 1 task di compilazione con success/fail/build
   /// -phase 2 folding keyword
   /// -phase 3 intellisense
   /// </summary>
   class Program
   {
      /// <summary>
      /// tododo:
      /// - complete task compilazione
      /// - align i task con task.vs.json
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
