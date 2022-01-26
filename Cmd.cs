using System;
using System.Linq;

namespace AdaCompiler
{
   public abstract class Cmd
   {
      public static void SearchAndExecute(string[] args, params Cmd[] cmds)
      {
         if (args.Length > 0)
         {
            var cmd = cmds.FirstOrDefault(c => string.Compare(c.Discriminant, args[0], true) == 0);

            if (cmd != null) { cmd.Execute(args); }
            else { Console.WriteLine("Not any input argument!"); }
         }
      }

      public void Execute(string[] args)
      {
         var p_i = 0;
         var a_i = 1;
         var prs = Params;

         foreach (var par in prs) { par.Value = null; }

         for (; a_i < args.Length && p_i < prs.Length; p_i++) { prs[p_i].Value = prs[p_i].Check(args, ref a_i); }

         if (a_i < args.Length) { Console.WriteLine("Too many in-parameters!"); }
         else if (p_i < prs.Length) { Console.WriteLine("Not enough many in-parameters!"); }
         else { myExecute(prs); }
      }

      protected abstract void myExecute(Param[] @params);

      public abstract Param[] Params { get; }

      public abstract string Discriminant { get; }
   }
}

