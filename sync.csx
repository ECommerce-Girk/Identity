#r "nuget: Tools, 2611.22.5"

using Tools;

var options = new Options
{
    SolutionRoot = Directory.GetCurrentDirectory(),
    SolutionName = "Identity"
};

CoreTypesSync.SyncAll(options);

Console.WriteLine("Synchronization completed!");