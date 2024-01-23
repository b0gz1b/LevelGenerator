using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

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
                if(panel.GetGrid()[row, col] is not null){
                    grid[row, col] = panel.GetGrid()[row, col].Copy();
                }
            }
        }
    }

    public Panel(string filePath)
    {
        // Read the JSON file
        string json = File.ReadAllText(filePath);

        // Parse the JSON file
        JObject jObject = JObject.Parse(json);

        // Get the grid size
        int nRows = ((int)jObject["Panel"]["GridSize"]["Rows"] - 1) / 2;
        int nCols = ((int)jObject["Panel"]["GridSize"]["Cols"] - 1) / 2;

        // Initialize the grid with the specified dimensions
        grid = new IPuzzleSymbol[2 * nRows + 1, 2 * nCols + 1];

        // Get the grid
        JArray jGrid = (JArray)jObject["Panel"]["Grid"];

        // Place the symbols on the panel
        foreach (JObject jSymbol in jGrid)
        {
            // Get the symbol type
            string symbolType = (string)jSymbol["Type"];

            // Get the symbol color
            int colorId = (int)jSymbol["ColorId"];

            // Get the symbol position
            int row = (int)jSymbol["Position"]["Row"];
            int col = (int)jSymbol["Position"]["Col"];

            // Place the symbol on the panel
            switch (symbolType)
            {
                case "Wall":
                    grid[row, col] = new Wall();
                    break;
                case "Hexagon":
                    grid[row, col] = new Hexagon();
                    break;
                case "Square":
                    grid[row, col] = new Square(colorId);
                    break;
                case "Sun":
                    grid[row, col] = new Sun(colorId);
                    break;
            }
        }
        start = new Tuple<int, int>((int)jObject["Panel"]["Start"]["Row"], (int)jObject["Panel"]["Start"]["Col"]);
        end = new Tuple<int, int>((int)jObject["Panel"]["End"]["Row"], (int)jObject["Panel"]["End"]["Col"]);
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

    public void RandomStartEndCOVR(){
        // For COVR we want only starts on the bottom row and ends on the top row
        Random random = new();
        int row = grid.GetLength(0) - 1;
        int col = random.Next((grid.GetLength(1)+1)/2) * 2;
        start = new Tuple<int, int>(row, col);
        SetStart(start);
        row = 0;
        col = random.Next((grid.GetLength(1)+1)/2) * 2;
        end = new Tuple<int, int>(row, col);
        SetEnd(end);
    }

    public void SetEnd(int indexRow, int indexCol)
    {
        // Check if the placement is valid
        CheckStartEndValidity(indexRow, indexCol);
        // Set the end position
        end = new Tuple<int, int>(indexRow, indexCol);
    }

    public void PrintPanel(bool printStartEnd = false)
    {
        // Print the current state of the panel
        for (int row = -1; row <= grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (printStartEnd && row == start.Item1 + 1 && col == start.Item2)
                    Console.Write("S ");
                else if (printStartEnd && row == end.Item1 -1 && col == end.Item2)
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
        List<Tuple<int, int>> neighbourNodes = new();
        if(IsPointValid(indexRow - 2, indexCol) && grid[indexRow - 1, indexCol] is not Wall){
            neighbourNodes.Add(new Tuple<int, int>(indexRow - 2, indexCol));
        }
        if(IsPointValid(indexRow + 2, indexCol) && grid[indexRow + 1, indexCol] is not Wall){
            neighbourNodes.Add(new Tuple<int, int>(indexRow + 2, indexCol));
        }
        if(IsPointValid(indexRow, indexCol - 2) && grid[indexRow, indexCol - 1] is not Wall){
            neighbourNodes.Add(new Tuple<int, int>(indexRow, indexCol - 2));
        }
        if(IsPointValid(indexRow, indexCol + 2) && grid[indexRow, indexCol + 1] is not Wall){
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
        List<Tuple<int, int>> neighbourPillars = new();
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
                        if(grid[row, col] is not null){
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

    public List<List<Tuple<int, int>>> GetRegions(List<Tuple<int,int>> path){
        // Get the regions a path splits the panel into
        // A region is a list of nodes that are not separated by the path, meaning a path can be drawn between any two nodes in the region regardless of the walls
        List<Tuple<int, int>> dfs_pillars(int indexRow, int indexCol){
            // Depth-first search to find all the pillars in the region
            List<Tuple<int, int>> pillars = new();
            Stack<Tuple<int, int>> stack = new();
            stack.Push(new Tuple<int, int>(indexRow, indexCol));
            while(stack.Count > 0){
                Tuple<int, int> current = stack.Pop();
                if(!pillars.Contains(current)){
                    pillars.Add(current);
                    foreach(Tuple<int, int> neighbour in GetNeighbourPillars(current.Item1, current.Item2)){
                        // find the edge between the current pillar and the neighbour pillar
                        Tuple<int, int> edge = new((current.Item1 + neighbour.Item1) / 2, (current.Item2 + neighbour.Item2) / 2);
                        if(!path.Contains(edge)){
                            stack.Push(neighbour);
                        }
                    }
                }
            }
            return pillars;

        }
        List<List<Tuple<int, int>>> regions = new();
        List<Tuple<int, int>> not_visited = new();
        for(int row = 1; row < grid.GetLength(0) - 1; row+=2){
            for(int col = 1; col < grid.GetLength(1) - 1; col+=2){
                not_visited.Add(new Tuple<int, int>(row, col));
            }
        }
        // Depth-first search to find all the regions using auxiliary function dfs_pillars
        while(not_visited.Count > 0){
            List<Tuple<int, int>> pillars = dfs_pillars(not_visited[0].Item1, not_visited[0].Item2);
            foreach(Tuple<int, int> pillar in pillars){
                not_visited.Remove(pillar);
            }
            regions.Add(pillars);
        }
        return regions;
    }

    public void PrintRegions(List<Tuple<int, int>> points){
        bool[,] pathGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
        List<List<Tuple<int, int>>> regions = GetRegions(points);
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
                            if(regions[i].Contains(new Tuple<int, int>(row, col))){
                                Console.Write(i + " ");
                            }
                        }
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

    internal void WriteToFile(string filePath)
    {
        try
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.AppendLine("{");
            jsonBuilder.AppendLine("\t\"Panel\": {");
            jsonBuilder.AppendLine($"\t\t\"GridSize\": {{\"Rows\": {grid.GetLength(0)}, \"Cols\": {grid.GetLength(1)}}},");
            jsonBuilder.AppendLine("\t\t\"Grid\": [");

            // Write grid
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (grid[row, col] is not null)
                    {
                        jsonBuilder.AppendLine("\t\t\t{");
                        IPuzzleSymbol symbol = grid[row, col];

                        if (symbol != null)
                        {
                            jsonBuilder.AppendLine($"\t\t\t\t\"Type\": \"{symbol.Name}\",");
                            jsonBuilder.AppendLine($"\t\t\t\t\"ColorId\": {symbol.GetColorId()},");
                            jsonBuilder.AppendLine($"\t\t\t\t\"Position\": {{\"Row\": {row}, \"Col\": {col}}}");
                        }

                        jsonBuilder.AppendLine("\t\t\t},");
                    }
                }
            }

            // Remove the trailing comma if there are items in the grid
            if (jsonBuilder.Length > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 3, 1);
            }

            jsonBuilder.AppendLine("\t\t],");
            jsonBuilder.AppendLine($"\t\t\"Start\": {{\"Row\": {start.Item1}, \"Col\": {start.Item2}}},");
            jsonBuilder.AppendLine($"\t\t\"End\": {{\"Row\": {end.Item1}, \"Col\": {end.Item2}}}");
            jsonBuilder.AppendLine("\t}");
            jsonBuilder.AppendLine("}");

            File.WriteAllText(filePath, jsonBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing JSON file: {e.Message}");
        }
    }

}