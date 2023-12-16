class Program
{
    static void Main()
    {
        // Create a panel with dimensions 3x3
        Panel puzzlePanel = new(4, 4);
        puzzlePanel.PrintPanel();
        // Create symbols
        Hexagon hexagon = new();
        Square square = new(0);
        Sun sun = new(0);
        Wall wall1 = new();
        Wall wall2 = new();
        // Place symbols on the panel
        puzzlePanel.PlaceSymbol(hexagon, 4, 2);
        puzzlePanel.PlaceSymbol(square, 3, 3);
        puzzlePanel.PlaceSymbol(sun, 5, 1);
        puzzlePanel.PlaceSymbol(wall1, 3, 6);
        puzzlePanel.PlaceSymbol(wall2, 0, 3);


        // Change the start and end positions
        puzzlePanel.RandomStartEndCOVR();
        // Print the current state of the panel
        puzzlePanel.PrintPanel();
        Console.WriteLine();
        // Generate a random path
        for(int i = 0; i < 10; i++){
            Path randomPath = Path.GenerateRandomPath(puzzlePanel);
            puzzlePanel.PrintRegions(randomPath.GetPoints());
            Console.WriteLine();
        }

    }
}
