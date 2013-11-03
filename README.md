NRefactoryCUBuilder (NRCUB)
===================

Creates [NRefactory](https://github.com/icsharpcode/NRefactory) Compilation Units for all CSharp Projects on your .net solution


If you plan to use NRefactory to inspect your source code, one of the basic tasks you'll need to deal with is the creation of compilation units. NRefactoryCUBuilder (NRCUB) simplify this task by enabling you to build Compilation Units for all projects in a .net solution. All you need to do is to provide the solutions' path.


### Usage

```csharp

using NRefactoryCUBuilder;
using NRefactoryCUBuilder.Exceptions;

// ...

try
{
  var solution = new Solution(@"C:\solutionFile.sln");
  foreach(var project in solution.Projects)
  {
    var projectCompilationUnit = project.Compilation;
  
    // do some stuff with project compilation and nrefactory
  }
}
catch (ParseError parseEx)
{
  Console.WriteLine(string.Format("error on parsing: {0}", parseEx.Message);
}

// ...

```

## Limitations

The current version only works with C# projects (including class libraries, console, windows forms, asp.net mvc, and other project types)

## Inspiration and References

This project is largely inspired on the [work](http://www.codeproject.com/Articles/408663/Using-NRefactory-for-analyzing-Csharp-code) and samples provided by [Daniel Grunwald](http://www.danielgrunwald.de/). Please, check the original article for more information.
