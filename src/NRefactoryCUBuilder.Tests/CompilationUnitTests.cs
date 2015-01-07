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
        public void ShouldThrowExceptionWhenParsingASolutionWithABrokenSyntaxTree()
        {
            (new Action(() => CreateSolutionFor("BrokenSyntaxTree", "BrokenSyntaxTree.sln"))).Should().Throw<ParseException>();
        }

        [Test]
        public void ShouldThrowExceptionWhenSolutionFileIsMissing()
        {
            (new Action(() => new Solution("C:\\wrongpath.sln"))).Should().Throw<FileNotFoundException>();
        }

        private Solution CreateSolutionFor(string solutionFolderName, string solutionFileName)
        {
            var solutionPath = Path.Combine(Path.Combine(_fixturesBasePath, solutionFolderName), solutionFileName);
            return new Solution(solutionPath);
        }
    }
}
