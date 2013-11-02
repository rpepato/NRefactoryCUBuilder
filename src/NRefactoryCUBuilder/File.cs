using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using NRefactoryCUBuilder.Exceptions;

namespace NRefactoryCUBuilder
{
    public class File : StructEx.File
    {
        public File(Project project, string name): base(project, name)
        {

            var parser = new CSharpParser(Utils.Convert.ToCompilerSettings(Project.CompilerSettings));
            SyntaxTree = parser.Parse(Content, Name);
            if (parser.HasErrors)
            {
                var exceptionMessage = string.Format("Error parsing {0}:\n", Name);
                exceptionMessage = parser.ErrorsAndWarnings.Aggregate(exceptionMessage, (current, error) => current + string.Format("{0} {1} \n", error.Region, error.ErrorType));
                throw new ParseException(exceptionMessage);
            }
            UnresolvedTypeSystemForFile = SyntaxTree.ToTypeSystem();
        }

        public CSharpUnresolvedFile UnresolvedTypeSystemForFile { get; private set; }

        private SyntaxTree SyntaxTree { get; set; }
    }
}
