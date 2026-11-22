#r "nuget: Tools, 2611.22.6"

using Tools;

var options = new Options
{
    SolutionRoot = Directory.GetCurrentDirectory(),
    SolutionName = "Identity"
};

CoreTypesSync.SyncAll(options);
ControllerSync.Sync(options);

Console.WriteLine("Synchronization completed!");