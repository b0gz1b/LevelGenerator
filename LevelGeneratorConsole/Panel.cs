using System;

public class Panel
{
    private readonly IPuzzleSymbol[,] grid;
    private Tuple<int, int> start;
    private Tuple<int, int> end;

    public Panel(int nRows, int nCols)
    {
        // Initialize the grid with the specified dimensions
        grid = new IPuzzleSymbol[2 * nRows + 1, 2 * nCols + 1];
        start = new Tuple<int, int>(grid.GetLength(0) - 1, grid.GetLength(1) - 1);
        end = new Tuple<int, int>(0, 0);
    }

    public void PlaceSymbol(IPuzzleSymbol symbol, int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }

        // Place the symbol on the panel
        grid[indexRow, indexCol] = symbol;

        // Call the symbol's PlaceSymbol method with the correct dimensions
        symbol.PlaceSymbol(indexCol, indexRow, grid.GetLength(1), grid.GetLength(0));
    }

    private void CheckStartEndValidity(int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }
        // Check if the placement is on a node
        if (indexCol % 2 != 0 || indexRow % 2 != 0)
        {
            throw new Exception("Invalid placement");
        }
        // Check if the placement is on an outer node
        if (indexCol != 0 && indexCol != grid.GetLength(1) - 1 && indexRow != 0 && indexRow != grid.GetLength(0) - 1)
        {
            throw new Exception("Invalid placement");
        }
    }
    public void SetStart(int indexRow, int indexCol)
    {
        // Check if the placement is valid
        CheckStartEndValidity(indexRow, indexCol);
        // Set the start position
        start = new Tuple<int, int>(indexRow, indexCol);
    }

    public void SetEnd(int indexRow, int indexCol)
    {
        // Check if the placement is valid
        CheckStartEndValidity(indexRow, indexCol);
        // Set the end position
        end = new Tuple<int, int>(indexRow, indexCol);
    }

    public void PrintPanel()
    {
        // Print the current state of the panel
        for (int row = -1; row <= grid.GetLength(0); row++)
        {
            for (int col = -1; col <= grid.GetLength(1); col++)
            {
                if (row == -1 || row == grid.GetLength(0) || col == -1 || col == grid.GetLength(1))
                {
                    if(row == start.Item1 + 1 && col == start.Item2)
                    {
                        Console.Write("S ");
                    }
                    else if (row == end.Item1 - 1 && col == end.Item2)
                    {
                        Console.Write("E ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                
                else if (grid[row, col] != null)
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