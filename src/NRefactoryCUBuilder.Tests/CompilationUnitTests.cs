using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.TypeSystem;
using NRefactoryCUBuilder.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NRefactoryCUBuilder.Tests
{
    [TestFixture]
    public class CompilationUnitTests
    {
        private string _fixturesBasePath;

        [SetUp]
        public void SetUp()
        {
            var uri = new Uri(Assembly.GetAssembly(typeof(CompilationUnitTests)).CodeBase);
            DirectoryInfo directory = new DirectoryInfo(uri.AbsolutePath);
            _fixturesBasePath = Path.Combine(directory.Parent.Parent.Parent.FullName, "SolutionFixtures");
        }

        [Test]
        public void ShouldResolveReferencedAssemblies()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            foreach(var assembly in solution.Projects.First().ResolvedReferencedAssemblies)
            {
                assembly.Should().Be.InstanceOf<IUnresolvedAssembly>();
            }

            solution.Projects.First().ResolvedReferencedAssemblies.Any(f => f.AssemblyName == "Microsoft.CSharp").Should().Be.True();           
        }

        [Test]
        public void ShouldPresetNullAsDefaultForProjectsCompilation()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            foreach (var project in solution.Projects)
            {
                project.Compilation.Should().Be.Null();
            }
        }

        [Test]
        public void ShouldCreateCompilationForAllProjectsOnTheSolution()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            solution.CreateCompilationUnitsForAllPojects();
            foreach (var project in solution.Projects)
            {
                project.Compilation.Should().Not.Be.Null();
            }
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void ShouldThrowExceptionWhenParsingASolutionWithABrokenSyntaxTree()
        {
            var solution = CreateSolutionFor("BrokenSyntaxTree", "BrokenSyntaxTree.sln");
        }

        private Solution CreateSolutionFor(string solutionFolderName, string solutionFileName)
        {
            var solutionPath = Path.Combine(Path.Combine(_fixturesBasePath, solutionFolderName), solutionFileName);
            return new Solution(solutionPath);
        }
    }
}
