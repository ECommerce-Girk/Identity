
class Program
{    
    public const string SolutionName = "Identity";
    public static string SolutionRoot => AppDomain.CurrentDomain.BaseDirectory.Split([SolutionName], default)[0] + SolutionName;

    public static void Main()
    {
        ValueObjectGenerator.Generate();
    }
}
