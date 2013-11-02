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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace NRefactoryCUBuilder
{
    public class Project : StructEx.Project
    {
        public new IList<File> Files { get; private set; } 

        public ICompilation Compilation { get; set; }

        public IProjectContent ProjectContent { get; private set; } 

        public Project(StructEx.Solution solution, string title, string fileName) : base(solution, title, fileName)
        {
            Files = new List<File>();
            ProjectContent = new CSharpProjectContent();
            ResolvedReferencedAssemblies = new List<IUnresolvedAssembly>();

            ProjectContent = ProjectContent.SetAssemblyName(AssemblyName);
            ProjectContent = ProjectContent.SetProjectFileName(fileName);

            ProjectContent = ProjectContent.SetCompilerSettings(Utils.Convert.ToCompilerSettings(CompilerSettings));

            foreach (var sourceCodeFile in MsBuildProject.GetItems("Compile"))
            {
                Files.Add(new File(this, Path.Combine(MsBuildProject.DirectoryPath, sourceCodeFile.EvaluatedInclude)));
            }

            var files =
                Files.Select(f => f.UnresolvedTypeSystemForFile);
            ProjectContent = ProjectContent.AddOrUpdateFiles(files);

            foreach (var projectReference in ReferencedProjects)
            {
                ProjectContent = ProjectContent.AddAssemblyReferences(new[] { new ProjectReference(projectReference) });
            }

            ResolveAssemblies();
        }

        public IList<IUnresolvedAssembly> ResolvedReferencedAssemblies { get; private set; }

        private void ResolveAssemblies()
        {
            foreach (string assemblyFile in ReferencedAssemblies)
            {
                IUnresolvedAssembly assembly = ((Solution) Solution).LoadAssembly(assemblyFile);
                ProjectContent = ProjectContent.AddAssemblyReferences(new[] { assembly });
                ResolvedReferencedAssemblies.Add(assembly);
            }
        }
    }
}
