namespace AdaCompiler
{
   public class CmdClean : Cmd
   {
      public override string Discriminant => "clean";

      public override Param[] Params => new Param[] { new ParamRootDir() };

      protected override void myExecute(Param[] @params)
      {
         var roo_dir = (ParamRootDir)@params[0];
         var hlp = new BuildHelper();
         var stt = Settings.FromRootDir((string)roo_dir.Value);

         hlp.Build(stt, CompileActionEnum.clean);
      }
   }
}
