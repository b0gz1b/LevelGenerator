class Program
{
    static void Main()
    {
        int n_cols = 4;
        int n_rows = 4;
        int n_walls = 3;
        int n_hexagons = 3;
        int n_colors = 2;
        List<int> n_square_by_color = new(){4, 4};
        List<int> n_sun_by_color = new(){1, 1};
        Path path = Generator.GenerateLevel(n_rows, n_cols, n_walls, n_hexagons, n_colors, n_square_by_color, n_sun_by_color);
        path.PrintPath();
        path.WriteToFiles("level.json", "solution.csv");
    }
}
