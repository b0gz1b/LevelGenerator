using System;

public class Panel
{
    private IPuzzleSymbol[,] grid;

    public Panel(int nRows, int nCols)
    {
        // Initialize the grid with the specified dimensions
        grid = new IPuzzleSymbol[2 * nRows + 1, 2 * nCols + 1];
    }

    public void PlaceSymbol(IPuzzleSymbol symbol, int indexCol, int indexRow)
    {
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            throw new ArgumentOutOfRangeException("Invalid placement coordinates");
        }

        // Place the symbol on the panel
        grid[indexRow, indexCol] = symbol;

        // Call the symbol's PlaceSymbol method with the correct dimensions
        symbol.PlaceSymbol(indexCol, indexRow, grid.GetLength(1), grid.GetLength(0));
    }

    public void PrintPanel()
    {
        // Print the current state of the panel
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] != null)
                {
                    Console.Write(grid[row, col].GetSymbol() + " ");
                }
                else
                {
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        Console.Write(". ");
                    }
                    else if (row % 2 != col % 2)
                    {
                        if (row % 2 == 0)
                        {
                            Console.Write("- ");
                        }
                        else
                        {
                            Console.Write("| ");
                        }
                    }
                    else
                    {
                        Console.Write("X ");
                    }
                }
            }
            Console.WriteLine();
        }
    }
}