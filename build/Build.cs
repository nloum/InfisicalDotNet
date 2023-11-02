using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Test), nameof(Push) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    GitHubActions GitHubActions => GitHubActions.Instance;
    
    [Solution]
    readonly Solution Solution;
    [GitVersion(Framework = "net7.0")]
    GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";
    IEnumerable<Project> TestProjects => Solution.GetAllProjects("*.Tests");
    [Parameter("NuGet server URL.")]
    readonly string NugetSource = "https://api.nuget.org/v3/index.json";
    [Parameter("API Key for the NuGet server.")]
    readonly string NugetApiKey;
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .EnableNoRestore()
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
            );
        });
    
    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .ResetVerbosity()
                .SetProcessArgumentConfigurator(args => args.Add("--collect:\"XPlat Code Coverage\""))
                .SetResultsDirectory(TestResultDirectory)
                .When(IsServerBuild, _ => _
                    .EnableUseSourceLink())
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLoggers($"trx;LogFileName={v.Name}.trx")
                    .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml")));
        });

    string CoverageReportDirectory => ArtifactsDirectory / "coverage-report";

    Target Coverage => _ => _
        .DependsOn(Test)
        .TriggeredBy(Test)
        .Consumes(Test)
        //.Produces(CoverageReportArchive)
        .Executes(() =>
        {
	        var package = NuGetPackageResolver.GetGlobalInstalledPackage("dotnet-reportgenerator-globaltool", "5.1.26", null);
	        //var settings = new GitVersionSettings().SetToolPath( package.Directory / "tools/netcoreapp3.1/any/gitversion.dll");

            // TestResultDirectory.GlobFiles("**/*.xml").ForEach(x =>
            //     x.Move(TestResultDirectory / $"{x.Parent.Name}-coverage.cobertura.xml"));
            
	        ReportGenerator(_ => _
                .SetProcessToolPath(package.Directory / "tools/net7.0/any/ReportGenerator.dll")
                .SetReports(TestResultDirectory / "**/*.xml")
                .SetReportTypes(ReportTypes.HtmlInline)
                .SetTargetDirectory(CoverageReportDirectory)
                .SetFramework("net7.0"));
            //
            // CompressZip(
            //     directory: CoverageReportDirectory,
            //     archiveFile: CoverageReportArchive,
            //     fileMode: FileMode.Create);
        });
    
    Target Pack => _ => _
        .DependsOn(Compile)
		//.Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetPack(s => s
                .EnableNoRestore()
                .EnableNoBuild()
				.SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersionV2)
				.SetIncludeSymbols(true)
				.SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
            );
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
				.SetSource(NugetSource)
				.SetApiKey(NugetApiKey)
				.SetSkipDuplicate(true)
				.CombineWith(ArtifactsDirectory.GlobFiles("*.nupkg"), (s, v) => s
					.SetTargetPath(v)
				)
            );
        });
}
