class LevelGeneratorProgram
{
    static void Main(string[] args)
    {
        // Generate a level with the parameters passed to the function
        // parse the args
        int n_rows = int.Parse(args[0]);
        int n_cols = int.Parse(args[1]);
        int n_walls = int.Parse(args[2]);
        int n_hexagons = int.Parse(args[3]);
        int n_colors = int.Parse(args[4]);
        List<int> n_square_by_color = new();
        List<int> n_sun_by_color = new();
        for (int i = 0; i < n_colors; i++)
        {
            n_square_by_color.Add(int.Parse(args[5 + i]));
        }
        for (int i = 0; i < n_colors; i++)
        {
            n_sun_by_color.Add(int.Parse(args[5 + n_colors + i]));
        }

        Path path = Generator.GenerateLevel(n_rows, n_cols, n_walls, n_hexagons, n_colors, n_square_by_color, n_sun_by_color);
        path.PrintPath();
        path.PrintPanel();
        path.WriteToFiles(args[5 + 2 * n_colors], args[6 + 2 * n_colors]);
    }
}
