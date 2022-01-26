namespace AdaCompiler
{
   public abstract class Param
   {
      /// <summary>
      /// 
      /// </summary>
      public abstract string Name { get; }

      /// <summary>
      /// 
      /// </summary>
      public object Value { get; set; }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="args"></param>
      /// <param name="aIndex"></param>
      /// <returns></returns>
      /// <exception cref="AdaCompiler.ErrorException"></exception>
      public abstract object Check(string[] args, ref int aIndex);
   }
}
