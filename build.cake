#tool "Squirrel.Windows"
#tool nuget:?package=GitVersion.CommandLine&Version=3.6.5
#addin Cake.Squirrel

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Package");

///////////////////////////////////////////////////////////////////////////////
// PATHS & VARS
///////////////////////////////////////////////////////////////////////////////
var configuration = "Release";
var pWindowJaxProject = new FilePath("src/pWindowJax.csproj");
var pWindowJaxNuspec = new FilePath("pWindowJax.nuspec");
var buildOutputDirectory = new DirectoryPath($"src/bin/{configuration}/net471/");
var artifactsDirectory = new DirectoryPath("artifacts");
var version = GitVersion();

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Compile")
.Does(() => {
   MSBuild(pWindowJaxProject, new MSBuildSettings() {
      Restore = true,
      Configuration = configuration,
      Verbosity = Verbosity.Minimal,
      ArgumentCustomization = args => {
         args.Append($"/p:AssemblyVersion={version.AssemblySemVer}");
         args.Append($"/p:AssemblyFileVersion={version.AssemblySemVer}");
         args.Append($"/p:AssemblyInformationalVersion={version.FullSemVer}");
         return args;
      }
   });
});

Task("Package")
.IsDependentOn("Compile")
.Does(() => {
   CleanDirectory(artifactsDirectory);
   var nugetPackage = artifactsDirectory.CombineWithFilePath($"pWindowJax.{version.MajorMinorPatch}.nupkg");

   NuGetPack(pWindowJaxNuspec, new NuGetPackSettings() {
      Version = version.MajorMinorPatch,
      OutputDirectory = artifactsDirectory,

   });

   Squirrel(nugetPackage, new SquirrelSettings(){
      ReleaseDirectory = artifactsDirectory.Combine("Releases")

   });
});

RunTarget(target);