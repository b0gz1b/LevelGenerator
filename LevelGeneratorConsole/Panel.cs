using System.Diagnostics;

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

    public bool IsPointValid(int indexRow, int indexCol) => !(indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1);
    private void CheckStartEndValidity(int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if(!IsPointValid(indexRow, indexCol)){
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
        bool hasStartPrinted = false;
        bool hasEndPrinted = false;
        for (int row = -1; row <= grid.GetLength(0); row++)
        {
            for (int col = -1; col <= grid.GetLength(1); col++)
            {
                if (row == -1 || row == grid.GetLength(0) || col == -1 || col == grid.GetLength(1))
                {
                    // If the current position is the start or end position, print S or E on the top if it is on row 0 or the bottom if it is on the last row, then to the left if it is on column 0 or to the right if it is on the last column if not on the first or last row
                    // top <-> row == -1
                    // bottom <-> row == grid.GetLength(0)
                    // left <-> col == -1
                    // right <-> col == grid.GetLength(1)
                    if (start.Item1 == 0 && row == -1 && col == start.Item2 && !hasStartPrinted)
                    {
                        Console.Write("S ");
                        hasStartPrinted = true;
                    }
                    else if (start.Item2 == 0 && row == start.Item1 && col == -1 && !hasStartPrinted)
                    {
                        Console.Write("S ");
                        hasStartPrinted = true;
                    }
                    else if (start.Item1 == grid.GetLength(0) - 1 && row == grid.GetLength(0) && col == start.Item2 && !hasStartPrinted)
                    {
                        Console.Write("S ");
                        hasStartPrinted = true;
                    }
                    else if (start.Item2 == grid.GetLength(1) - 1 && row == start.Item1 && col == grid.GetLength(1) && !hasStartPrinted)
                    {
                        Console.Write("S ");
                        hasStartPrinted = true;
                    }
                    else if (end.Item1 == 0 && row == -1 && col == end.Item2 && !hasEndPrinted)
                    {
                        Console.Write("E ");
                        hasEndPrinted = true;
                    }
                    else if (end.Item2 == 0 && row == end.Item1 && col == -1 && !hasEndPrinted)
                    {
                        Console.Write("E ");
                        hasEndPrinted = true;
                    }
                    else if (end.Item1 == grid.GetLength(0) - 1 && row == grid.GetLength(0) && col == end.Item2 && !hasEndPrinted)
                    {
                        Console.Write("E ");
                        hasEndPrinted = true;
                    }
                    else if (end.Item2 == grid.GetLength(1) - 1 && row == end.Item1 && col == grid.GetLength(1) && !hasEndPrinted)
                    {
                        Console.Write("E ");
                        hasEndPrinted = true;
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

    public List<Tuple<int, int>> GetNeighbourNodes(int indexRow, int indexCol)
    {
        // Check if the node is within the bounds of the panel
        if(!IsPointValid(indexRow, indexCol)){
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }
        // Check if the node is a node
        if (indexCol % 2 != 0 || indexRow % 2 != 0)
        {
            throw new Exception("Invalid placement");
        }

        // Initialize the list of all possible neighbour nodes
        List<Tuple<int, int>> neighbourNodes = new();
        if(IsPointValid(indexRow - 2, indexCol)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow - 2, indexCol));
        }
        if(IsPointValid(indexRow + 2, indexCol)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow + 2, indexCol));
        }
        if(IsPointValid(indexRow, indexCol - 2)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow, indexCol - 2));
        }
        if(IsPointValid(indexRow, indexCol + 2)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow, indexCol + 2));
        }
        
        return neighbourNodes;
    }


    public Tuple<int, int> GetStart()
    {
        return start;
    }

    public Tuple<int, int> GetEnd()
    {
        return end;
    }

    public IPuzzleSymbol[,] GetGrid()
    {
        return grid;
    }

    public void PrintPath(List<Tuple<int, int>> points){
        // Just print the path on the panel without any other symbols or start/end positions
        bool[,] pathGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
        foreach(Tuple<int, int> point in points){
            pathGrid[point.Item1, point.Item2] = true;
        }
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if(pathGrid[row, col]){
                    Console.Write("~ ");
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