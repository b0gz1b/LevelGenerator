

class Path{
    private Panel panel;
    private List<Tuple<int, int>> points;

    public Path(Panel panel){
        this.panel = panel;
        points = new List<Tuple<int, int>>();
    }

    public Path(string filePanelPath, string filePointsPath){
        panel = new Panel(filePanelPath);
        points = new List<Tuple<int, int>>();
        using StreamReader file = new StreamReader(filePointsPath);
        string line;
        while((line = file.ReadLine()!) != null){
            string[] coordinates = line.Split(',');
            points.Add(new Tuple<int, int>(int.Parse(coordinates[0]), int.Parse(coordinates[1])));
        }
    }

    public bool AddNode(int indexRow, int indexCol){
        if(!panel.IsPointValid(indexRow, indexCol)){
            return false; // The node is not within the bounds of the panel
        }
        if(indexCol % 2 != 0 || indexRow % 2 != 0){
            return false; // The node is not a node
        }
        // Check if the placement is next to the last point in the path
        if(points.Count > 0){
            Tuple<int, int> pointToBeAdded = new(indexRow, indexCol);
            Tuple<int, int> last = points[^1];
            if(points.Contains(pointToBeAdded)){
                return false; // The node is already in the path
            }
            else if(last.Item1 == panel.GetEnd().Item1 && last.Item2 == panel.GetEnd().Item2)
            {
                return false; // The path is already complete
            }
            else if(Math.Abs(last.Item1 - indexRow) + Math.Abs(last.Item2 - indexCol) != 2)
            {
                return false; // The placement is not next to the last point in the path
            }
            // Compute the index of the edge to be added to link the last point in the path to the new point
            Tuple<int, int> edgeToBeAdded = new((last.Item1 + indexRow) / 2, (last.Item2 + indexCol) / 2);
            // Check if the edge to be added is already in the path
            if(points.Contains(edgeToBeAdded))
            {
                return false; // The edge to be added is already in the path
            }
            // Add the edge and the node to the path
            points.Add(edgeToBeAdded);
            points.Add(pointToBeAdded);
        }
        else{
            // Check if the placement is on the start position
            if(indexRow != panel.GetStart().Item1 || indexCol != panel.GetStart().Item2)
            {
                return false; // The placement is not on the start position
            }
            // Add the node to the path
            points.Add(new Tuple<int, int>(indexRow, indexCol));
        }
        return true;
    }

    public void AddNodeList(List<Tuple<int, int>> nodeList){
        foreach(Tuple<int, int> node in nodeList){
            AddNode(node.Item1, node.Item2);
        }
    }

    public void RemoveLastNode(){
        points.RemoveAt(points.Count - 1);
        if(points.Count > 0){
            points.RemoveAt(points.Count - 1);
        }
    }
    public Tuple<int, int> GetLastNode(){
        return points[^1];
    }

    public void PrintPath(){
        panel.PrintPath(points);
    }

    public void PrintPanel(){
        panel.PrintPanel();
    }

    public void PrintRegions(){
        panel.PrintRegions(points);
    }

    public void SetPanel(Panel panel){
        this.panel = panel;
    }

    public List<Tuple<int, int>> GetPoints(){
        // Return a copy of the list of points
        return new List<Tuple<int, int>>(points);
    }

    public static Path GenerateRandomPath(Panel panel){
        // Create a path
        Path path = new(panel);
        // Add the start position to the path
        path.AddNode(panel.GetStart().Item1, panel.GetStart().Item2);
        List<Tuple<int, int>> neighbourNodes = panel.GetNeighbourNodes(panel.GetStart().Item1, panel.GetStart().Item2);
        List<Tuple<int, int>> invalidNeighbourNodes = new();
        while(path.GetLastNode().Item1 != panel.GetEnd().Item1 || path.GetLastNode().Item2 != panel.GetEnd().Item2){
            // Add a random neighbour node to the path
            Random random = new();
            int randomIndex = random.Next(neighbourNodes.Count);
            
            while(neighbourNodes.Count > 0 && !path.AddNode(neighbourNodes[randomIndex].Item1, neighbourNodes[randomIndex].Item2)){
                neighbourNodes.RemoveAt(randomIndex);
                randomIndex = random.Next(neighbourNodes.Count);  
            }
            if(neighbourNodes.Count == 0){
                // Debug.Print("No neighbour nodes left, backtrack");
                // Remove the last node and edge from the path, while storing the last node
                Tuple<int, int> lastNode = path.GetLastNode();
                invalidNeighbourNodes.Add(lastNode);
                path.RemoveLastNode();
                // Update the list of neighbour nodes to the new last node, while removing the last node from the list
                neighbourNodes = panel.GetNeighbourNodes(path.GetLastNode().Item1, path.GetLastNode().Item2);
                foreach(Tuple<int, int> invalidNeighbourNode in invalidNeighbourNodes){
                    neighbourNodes.Remove(invalidNeighbourNode);
                }
                if(neighbourNodes.Count == 0){
                    invalidNeighbourNodes = new();
                }
            }
            else{
                // Update the list of neighbour nodes
                neighbourNodes = panel.GetNeighbourNodes(neighbourNodes[randomIndex].Item1, neighbourNodes[randomIndex].Item2);
            }
        }
        return path;
    }

    public void WriteToFiles(string filePanelPath, string filePointsPath)
    {
        panel.WriteToFile(filePanelPath);
        using StreamWriter file = new StreamWriter(filePointsPath);
        foreach (Tuple<int, int> point in points)
        {
            file.WriteLine(point.Item1 + "," + point.Item2);
        }

    }

    internal int[] isPathValid()
    {
        int[] result = new int[3] { 0, 0, 0 };

        // get the grid
        IPuzzleSymbol[,] grid = panel.GetGrid();
        // get regions
        List<List<Tuple<int, int>>> regions = panel.GetRegions(points);
        // check if all squares of a region have the same color
        foreach (List<Tuple<int, int>> region in regions)
        {
            int colorId = -1;
            foreach (Tuple<int, int> square in region)
            {
                if (grid[square.Item1, square.Item2] != null && grid[square.Item1, square.Item2].Name == "Square"){
                    if(colorId == -1)
                    {
                        colorId = grid[square.Item1, square.Item2].GetColorId();
                    }
                    else if (colorId != grid[square.Item1, square.Item2].GetColorId())
                    {
                        // Console.WriteLine("colorId: " + colorId + " != " + grid[square.Item1, square.Item2].GetColorId());
                        result[1]++;
                    }
                }
            }
        }
        // check if for all suns of a region, you can pair it with exactly one other symbol of the same color
        foreach (List<Tuple<int, int>> region in regions)
        {
            foreach (Tuple<int, int> sun in region)
            {
                if (grid[sun.Item1, sun.Item2] != null && grid[sun.Item1, sun.Item2].Name == "Sun")
                {
                    int colorId = grid[sun.Item1, sun.Item2].GetColorId();
                    int count = 1;
                    foreach (Tuple<int, int> symbol in region)
                    {
                        if (grid[symbol.Item1, symbol.Item2] != null && (grid[symbol.Item1, symbol.Item2].Name == "Square" || grid[symbol.Item1, symbol.Item2].Name == "Sun"))
                        {
                            if (grid[symbol.Item1, symbol.Item2].GetColorId() == colorId && symbol != sun)
                            {
                                count++;
                            }
                        }
                    }
                    if (count != 2)
                    {
                        // Console.WriteLine("count: " + count + " != 2");
                        result[2]++;
                    }
                }
            }
        }
        // check if all hexagons are on the path
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                Tuple<int, int> point = new(i, j);
                if (grid[point.Item1, point.Item2] != null && grid[point.Item1, point.Item2].Name == "Hexagon")
                {
                    if (!points.Contains(point))
                    {
                        // Console.WriteLine("Hexagon not on path");
                        result[0]++;
                    }
                }
            }
        }
        return result;
    }
}