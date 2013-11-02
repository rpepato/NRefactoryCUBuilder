using ICSharpCode.NRefactory.CSharp;

namespace NRefactoryCUBuilder.Utils
{
    public static class Convert
    {
        public static CompilerSettings ToCompilerSettings(StructEx.CompilerSettings settings)
        {
            var compilerSettings = new CompilerSettings
                                       {
                                           AllowUnsafeBlocks = settings.AllowUnsafeBlocks,
                                           CheckForOverflow = settings.CheckForOverflow
                                       };
            foreach(var symbol in settings.ConditionalSymbols)
            {
                compilerSettings.ConditionalSymbols.Add(symbol);
            }
            return compilerSettings;
        }
    }
}
