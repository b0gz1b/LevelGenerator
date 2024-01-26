using System;
using System.Collections.Generic;

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

    public Panel(Panel panel){
        // Initialize the grid with the specified dimensions
        grid = new IPuzzleSymbol[panel.GetGrid().GetLength(0), panel.GetGrid().GetLength(1)];
        start = panel.GetStart();
        end = panel.GetEnd();
        for(int row = 0; row < grid.GetLength(0); row++){
            for(int col = 0; col < grid.GetLength(1); col++){
                if(panel.GetGrid()[row, col] != null){
                    grid[row, col] = panel.GetGrid()[row, col].Copy();
                }
            }
        }
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

    public void SetStart(Tuple<int, int> tuple)
    {
        SetStart(tuple.First, tuple.Second);
    }

    public void SetEnd(Tuple<int, int> tuple)
    {
        SetEnd(tuple.First, tuple.Second);
    }

    public void RandomStartEndCOVR(){
        // For COVR we want only starts on the bottom row and ends on the top row
        System.Random random = new System.Random();
        int row = 0;
        int col = random.Next(1,(grid.GetLength(1)+1)/2 - 1) * 2;
        start = new Tuple<int, int>(row, col);
        SetStart(start);
        row = grid.GetLength(0) - 1;
        col = random.Next(1,(grid.GetLength(1)+1)/2 - 1) * 2;
        end = new Tuple<int, int>(row, col);
        SetEnd(end);
    }

    private void CheckStartEndValidity(int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if(!IsPointValid(indexRow, indexCol)){
            // UnityEngine.Debug.Log(indexRow + " " + indexCol);
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

    public Tuple<int, int> RandomStartEnd()
    {
        // Draw if the start position is freely on a row or a column, the other one being -1 or the lenght complementing the other one
        System.Random random = new System.Random();
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

    public bool IsPointValid(int indexRow, int indexCol){
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            return false;
        }
        return true;
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

    public void RemoveSymbol(int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }

        // Remove the symbol from the panel
        grid[indexRow, indexCol] = default!;
    }

    public IPuzzleSymbol GetSymbol(int indexRow, int indexCol)
    {
        // Check if the placement is within the bounds of the panel
        if (indexCol < 0 || indexCol > grid.GetLength(1) - 1 || indexRow < 0 || indexRow > grid.GetLength(0) - 1)
        {
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }

        // Return the symbol from the panel
        return grid[indexRow, indexCol];
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

        // Initialize the list of all possible neighbour nodes not separated by a wall
        List<Tuple<int, int>> neighbourNodes = new List<Tuple<int, int>>();
        if(IsPointValid(indexRow - 2, indexCol) && !(grid[indexRow - 1, indexCol] is Wall)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow - 2, indexCol));
        }
        if(IsPointValid(indexRow + 2, indexCol) && !(grid[indexRow + 1, indexCol] is Wall)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow + 2, indexCol));
        }
        if(IsPointValid(indexRow, indexCol - 2) && !(grid[indexRow, indexCol - 1] is Wall)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow, indexCol - 2));
        }
        if(IsPointValid(indexRow, indexCol + 2) && !(grid[indexRow, indexCol + 1] is Wall)){
            neighbourNodes.Add(new Tuple<int, int>(indexRow, indexCol + 2));
        }
        
        return neighbourNodes;
    }

    public List<Tuple<int, int>> GetNeighbourPillars(int indexRow, int indexCol)
    {
        // Check if the point is within the bounds of the panel
        if(!IsPointValid(indexRow, indexCol)){
            throw new ArgumentOutOfRangeException("indexCol, indexRow");
        }
        // Check if the point is a pillar
        if (indexCol % 2 == 0 || indexRow % 2 == 0)
        {
            throw new Exception("Invalid placement");
        }

        // Initialize the list of all possible neighbour pillars
        List<Tuple<int, int>> neighbourPillars = new List<Tuple<int, int>>();
        if(IsPointValid(indexRow - 2, indexCol)){
            neighbourPillars.Add(new Tuple<int, int>(indexRow - 2, indexCol));
        }
        if(IsPointValid(indexRow + 2, indexCol)){
            neighbourPillars.Add(new Tuple<int, int>(indexRow + 2, indexCol));
        }
        if(IsPointValid(indexRow, indexCol - 2)){
            neighbourPillars.Add(new Tuple<int, int>(indexRow, indexCol - 2));
        }
        if(IsPointValid(indexRow, indexCol + 2)){
            neighbourPillars.Add(new Tuple<int, int>(indexRow, indexCol + 2));
        }
        
        return neighbourPillars;
    }

    public List<Tuple<int, int>> GetHexagonPositions()
    {
        // Initialize the list of all hexagon positions
        List<Tuple<int, int>> hexagonPositions = new List<Tuple<int, int>>();
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] is Hexagon)
                {
                    hexagonPositions.Add(new Tuple<int, int>(row, col));
                }
            }
        }
        return hexagonPositions;
    }

    public List<Tuple<int, int>> GetWallPositions()
    {
        // Initialize the list of all wall positions
        List<Tuple<int, int>> wallPositions = new List<Tuple<int, int>>();
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] is Wall)
                {
                    wallPositions.Add(new Tuple<int, int>(row, col));
                }
            }
        }
        return wallPositions;
    }

    public List<Tuple<int, int>> GetSunPositions()
    {
        // Initialize the list of all sun positions
        List<Tuple<int, int>> sunPositions = new List<Tuple<int, int>>();
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] is Sun)
                {
                    sunPositions.Add(new Tuple<int, int>(row, col));
                }
            }
        }
        return sunPositions;
    }

    public List<Tuple<int, int>> GetSquarePositions()
    {
        // Initialize the list of all square positions
        List<Tuple<int, int>> squarePositions = new List<Tuple<int, int>>();
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] is Square)
                {
                    squarePositions.Add(new Tuple<int, int>(row, col));
                }
            }
        }
        return squarePositions;
    }

    public List<List<Tuple<int, int>>> GetRegions(List<Tuple<int,int>> path)
    {
        // Get the regions a path splits the panel into
        // A region is a list of nodes that are not separated by the path, meaning a path can be drawn between any two nodes in the region regardless of the walls
        List<Tuple<int, int>> dfs_pillars(int indexRow, int indexCol){
            // Depth-first search to find all the pillars in the region
            List<Tuple<int, int>> pillars = new List<Tuple<int, int>>();
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            stack.Push(new Tuple<int, int>(indexRow, indexCol));
            // Write the path
            // foreach(Tuple<int, int> point in path){
            //     Console.WriteLine(point.First + " " + point.Second);
            // }
            while(stack.Count > 0){ 
                // Console.WriteLine("stack.Count: " + stack.Count);
                Tuple<int, int> current = stack.Pop();
                if(!Utils.TupleListContains(pillars, current)){
                    pillars.Add(current);
                    foreach(Tuple<int, int> neighbour in GetNeighbourPillars(current.First, current.Second)){
                        // find the edge between the current pillar and the neighbour pillar
                        Tuple<int, int> edge = new Tuple<int, int>((current.First + neighbour.First) / 2, (current.Second + neighbour.Second) / 2);
                        
                        if(!Utils.TupleListContains(path, edge)){
                            stack.Push(neighbour);
                        }
                    }
                }
            }
            return pillars;

        }
        List<List<Tuple<int, int>>> regions = new List<List<Tuple<int, int>>>();
        List<Tuple<int, int>> not_visited = new List<Tuple<int, int>>();
        for(int row = 1; row < grid.GetLength(0) - 1; row+=2){
            for(int col = 1; col < grid.GetLength(1) - 1; col+=2){
                not_visited.Add(new Tuple<int, int>(row, col));
            }
        }
        // Depth-first search to find all the regions using auxiliary function dfs_pillars
        while(not_visited.Count > 0){
            // Console.WriteLine("not_visited.Count: " + not_visited.Count);
            List<Tuple<int, int>> pillars = dfs_pillars(not_visited[0].First, not_visited[0].Second);
            foreach(Tuple<int, int> pillar in pillars){
                Utils.TupleListRemove(not_visited, pillar);
            }
            regions.Add(pillars);
        }
        return regions;
    }

    public void PrintPanel(bool printStartEnd = false)
    {
        // Print the current state of the panel
        for (int row = -1; row <= grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (printStartEnd && row == start.First + 1 && col == start.Second)
                    Console.Write("S ");
                else if (printStartEnd && row == end.First -1 && col == end.Second)
                    Console.Write("E ");
                else if (row == -1 || row == grid.GetLength(0))
                    Console.Write("  ");
                else if (grid[row, col] != null)
                    Console.Write(grid[row, col].GetSymbol() + " ");
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
                        Console.Write("O ");
                    }
                }
            }
            Console.WriteLine();
        }
    }

    public void PrintRegions(List<Tuple<int, int>> points)
    {
        // Debug
        bool[,] pathGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
        List<List<Tuple<int, int>>> regions = GetRegions(points);
        foreach(Tuple<int, int> point in points){
            pathGrid[point.First, point.Second] = true;
        }
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if(pathGrid[row, col]){
                    // S for start, E for end, * in between
                    if(row == start.First && col == start.Second){
                        Console.Write("S ");
                    }
                    else if(row == end.First && col == end.Second){
                        Console.Write("E ");
                    }
                    else{
                        Console.Write("* ");
                    }
                }
                else
                {
                    if (grid[row, col] is Wall)
                    {
                        Console.Write(grid[row, col].GetSymbol() + " ");
                    }
                    else if (row % 2 == 0 && col % 2 == 0)
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
                        for(int i = 0; i < regions.Count; i++){
                            if(Utils.TupleListContains(regions[i], new Tuple<int, int>(row, col))){
                                Console.Write(i + " ");
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
        }
    }

    public void PrintPath(List<Tuple<int, int>> points){
        // Just print the path on the panel without any other symbols or start/end positions
        bool[,] pathGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
        foreach(Tuple<int, int> point in points){
            pathGrid[point.First, point.Second] = true;
        }
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if(pathGrid[row, col]){
                    // S for start, E for end, * in between
                    if(row == start.First && col == start.Second){
                        Console.Write("S ");
                    }
                    else if(row == end.First && col == end.Second){
                        Console.Write("E ");
                    }
                    else if (grid[row, col] is Hexagon){
                        Console.Write(grid[row, col].GetSymbol() + " ");
                    }
                    else{
                        Console.Write("* ");
                    }
                }
                else
                {
                    if (grid[row, col] is Wall)
                    {
                        Console.Write(grid[row, col].GetSymbol() + " ");
                    }
                    else if (row % 2 == 0 && col % 2 == 0)
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
                        if(grid[row, col] != null){
                            Console.Write(grid[row, col].GetSymbol() + " ");
                        }
                        else{
                            Console.Write("O ");
                        }
                    }
                }
            }
            Console.WriteLine();
        }
    }

    public void WriteToFile(string filePath){
        // Print the panel to a file with the start and end positions
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
        {
            for (int row = -1; row <= grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (row == start.First && col == start.Second)
                        file.Write("S ");
                    else if (row == end.First && col == end.Second)
                        file.Write("E ");
                    else if (row == -1 || row == grid.GetLength(0))
                        file.Write("  ");
                    else if (grid[row, col] != null)
                        file.Write(grid[row, col].GetSymbol() + " ");
                    else
                    {
                        if (row % 2 == 0 && col % 2 == 0)
                        {
                            file.Write(". ");
                        }
                        else if (row % 2 != col % 2)
                        {
                            if (row % 2 == 0)
                            {
                                file.Write("- ");
                            }
                            else
                            {
                                file.Write("| ");
                            }
                        }
                        else
                        {
                            file.Write("O ");
                        }
                    }
                }
                file.WriteLine();
            }
        }
    }
}