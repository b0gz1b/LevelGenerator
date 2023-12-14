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
            Debug.WriteLine(indexRow + " " + indexCol);
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

    public Tuple<int, int> RandomStartEnd()
    {
        // Draw if the start position is freely on a row or a column, the other one being -1 or the lenght complementing the other one
        Random random = new();
        int rowOrCol = random.Next(2);
        if (rowOrCol == 0)
        {
            // Set the row on which the start position will be placed
            int row = random.Next((grid.GetLength(0)+1)/2) * 2;
            // Set the column on which the start position will be placed
            int randomCol = random.Next(2);
            if (randomCol == 0)
            {
                return new Tuple<int, int>(row, 0);
            }
            else
            {
                return new Tuple<int, int>(row, grid.GetLength(1) - 1);
            }
        }
        else
        {
            // Set the column on which the start position will be placed
            int col = random.Next((grid.GetLength(1)+1)/2) * 2;
            // Set the row on which the start position will be placed
            int randomRow = random.Next(2);
            if (randomRow == 0)
            {
                return new Tuple<int, int>(0, col);
            }
            else
            {
                return new Tuple<int, int>(grid.GetLength(0) - 1, col);
            }
        }
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
                    // S for start, E for end, * in between
                    if(row == start.Item1 && col == start.Item2){
                        Console.Write("S ");
                    }
                    else if(row == end.Item1 && col == end.Item2){
                        Console.Write("E ");
                    }
                    else{
                        Console.Write("* ");
                    }
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

    public void SetStart(Tuple<int, int> tuple)
    {
        SetStart(tuple.Item1, tuple.Item2);
    }

    public void SetEnd(Tuple<int, int> tuple)
    {
        SetEnd(tuple.Item1, tuple.Item2);
    }
}