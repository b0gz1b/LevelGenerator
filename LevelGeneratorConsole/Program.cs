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
        // Place symbols on the panel
        puzzlePanel.PlaceSymbol(hexagon, 4, 2);
        puzzlePanel.PlaceSymbol(square, 3, 3);
        puzzlePanel.PlaceSymbol(sun, 5, 1);

        // Change the start and end positions
        puzzlePanel.SetStart(puzzlePanel.RandomStartEnd());
        puzzlePanel.SetEnd(puzzlePanel.RandomStartEnd());
        // Print the current state of the panel
        puzzlePanel.PrintPanel();
        Console.WriteLine();
        // Generate a random path
        for(int i = 0; i < 10; i++){
            Path randomPath = Path.GenerateRandomPath(puzzlePanel);
            randomPath.PrintPath();
            Console.WriteLine();
        }
        // Print the random path
        // randomPath.PrintPath();
    }
}
