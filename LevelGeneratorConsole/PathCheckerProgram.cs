class PathCheckerProgram
{
    static void Main(string[] args)
    {
        string filePanelPath = args[0];
        string filePointsPath = args[1];
        Path path = new Path(filePanelPath, filePointsPath);
        int[] result = path.isPathValid();
        Console.WriteLine(result[0] + " " + result[1] + " " + result[2]);
    }
}