using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace NRefactoryCUBuilder
{
    public class Solution : StructEx.Solution
    {
        public new IList<Project> Projects { get; private set; } 

        public Solution(string path)
        {
            Projects = new List<Project>();
            if (System.IO.File.Exists(path))
            {
                Path = path;
                foreach (var line in System.IO.File.ReadLines(Path))
                {
                    var match = MSBuildProjectDefinitionPattern.Match(line);
                    if (match.Success)
                    {
                        if (match.Groups["TypeGuid"].Value.ToUpperInvariant().Equals(CSharpProjectIdentifier))
                        {
                            Projects.Add(new Project(this,
                                                            match.Groups["Title"].Value,
                                                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path),
                                                                         match.Groups["Location"].Value)));
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Solution file wasn't found on the specified path", Path);
            }
        }

        ConcurrentDictionary<string, IUnresolvedAssembly> assemblyDict = new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);

        public IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            return assemblyDict.GetOrAdd(assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }

        public void CreateCompilationUnitsForAllPojects()
        {
            var solutionSnapshot = new DefaultSolutionSnapshot(Projects.Select(project => project.ProjectContent));
            foreach(var project in Projects)
            {
                project.Compilation = solutionSnapshot.GetCompilation(project.ProjectContent);
            }      
        }

    }
}
