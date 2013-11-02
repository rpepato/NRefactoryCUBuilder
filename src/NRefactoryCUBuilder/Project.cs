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
