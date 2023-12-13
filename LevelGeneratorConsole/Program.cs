class Program
{
    static void Main()
    {
        // Create a panel with dimensions 3x3
        Panel puzzlePanel = new(3, 3);
        puzzlePanel.PrintPanel();
        // Create symbols
        Hexagon hexagon = new();
        Square square = new();
        Sun sun = new();
        puzzlePanel.SetStart(0, 0);
        puzzlePanel.SetEnd(2, 6);
        // Place symbols on the panel
        puzzlePanel.PlaceSymbol(hexagon, 4, 2);
        puzzlePanel.PlaceSymbol(square, 3, 3);
        puzzlePanel.PlaceSymbol(sun, 5, 1);

        // Print the current state of the panel
        puzzlePanel.PrintPanel();
    }
}
