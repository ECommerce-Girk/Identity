#r "nuget: Tools, 2611.23.10"

using Tools;

var options = new Options
{
    SolutionRoot = Directory.GetCurrentDirectory(),
    SolutionName = "Identity"
};

CoreTypesSync.SyncAll(options);
ControllerSync.Sync(options);

Console.WriteLine("Synchronization completed!");