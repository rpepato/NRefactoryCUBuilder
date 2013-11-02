//The MIT License (MIT)

//Copyright (c) 2013 Roberto Pepato

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
