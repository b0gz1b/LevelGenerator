using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
class PlayerPath{
    private Panel panel;
    private List<Tuple<int, int>> points;

    public PlayerPath(Panel panel){
        this.panel = panel;
        points = new List<Tuple<int, int>>();
    }

    public PlayerPath(Panel panel, List<Tuple<int, int>> points){
        this.panel = panel;
        this.points = points;
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
            Tuple<int, int> pointToBeAdded = new Tuple<int, int>(indexRow, indexCol);
            Tuple<int, int> last = Utils.Last(points);
            if(Utils.TupleListContains(points, pointToBeAdded)){
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
            Tuple<int, int> edgeToBeAdded = new Tuple<int, int>((last.Item1 + indexRow) / 2, (last.Item2 + indexCol) / 2);
            // Check if the edge to be added is already in the path
            if(Utils.TupleListContains(points, edgeToBeAdded))
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
            AddNode(node.First, node.Second);
        }
    }

    public void RemoveLastNode(){
        points.RemoveAt(points.Count - 1);
        if(points.Count > 0){
            points.RemoveAt(points.Count - 1);
        }
    }
    public Tuple<int, int> GetLastNode(){
        return points[points.Count - 1];
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

    public Panel GetPanel(){
        return panel;
    }

    public List<Tuple<int, int>> GetPoints(){
        // Return a copy of the list of points
        return new List<Tuple<int, int>>(points);
    }

    public static PlayerPath GenerateRandomPath(Panel panel){
        // Create a path
        PlayerPath path = new PlayerPath(panel);
        System.Random random = new System.Random();
        // Add the start position to the path
        path.AddNode(panel.GetStart().First, panel.GetStart().Second);
        List<Tuple<int, int>> neighbourNodes = panel.GetNeighbourNodes(panel.GetStart().First, panel.GetStart().Second);
        List<Tuple<int, int>> invalidNeighbourNodes = new List<Tuple<int, int>>();
        // while the last node in the path is not the end position
        while(path.GetLastNode().First != panel.GetEnd().First || path.GetLastNode().Second != panel.GetEnd().Second){
            // Add a random neighbour node to the path
            int randomIndex = random.Next(0, neighbourNodes.Count);
            bool is_added = false;
            while(neighbourNodes.Count > 0 && !is_added){
                is_added = path.AddNode(neighbourNodes[randomIndex].First, neighbourNodes[randomIndex].Second);
                // Console.WriteLine("is_added: " + is_added);
                if(!is_added){
                    neighbourNodes.RemoveAt(randomIndex);
                    randomIndex = random.Next(0, neighbourNodes.Count);
                }
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
                    Utils.TupleListRemove(neighbourNodes, invalidNeighbourNode);
                }
                if(neighbourNodes.Count == 0){
                    invalidNeighbourNodes = new List<Tuple<int, int>>();
                }
            }
            else{
                // Update the list of neighbour nodes
                neighbourNodes = panel.GetNeighbourNodes(neighbourNodes[randomIndex].Item1, neighbourNodes[randomIndex].Item2);
            }
            // Debug: print the path on one line
            // Console.Write("Path: ");
            // foreach(Tuple<int, int> point in path.GetPoints()){
            //     Console.Write("(" + point.First + ", " + point.Second + ") ");
            // }
            // Console.WriteLine();

            // Debug: print the invalid neighbour nodes on one line
            // Console.Write("Invalid neighbour nodes: ");
            // foreach(Tuple<int, int> point in invalidNeighbourNodes){
            //     Console.Write("(" + point.First + ", " + point.Second + ") ");
            // }
            // Console.WriteLine();
        }
        return path;
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
                if (grid[square.First, square.Second] != null && grid[square.First, square.Second].Name == "Square"){
                    if(colorId == -1)
                    {
                        colorId = grid[square.First, square.Second].GetColorId();
                    }
                    else if (colorId != grid[square.First, square.Second].GetColorId())
                    {
                        // Console.WriteLine("colorId: " + colorId + " != " + grid[square.First, square.Second].GetColorId());
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
                if (grid[sun.First, sun.Second] != null && grid[sun.First, sun.Second].Name == "Sun")
                {
                    int colorId = grid[sun.First, sun.Second].GetColorId();
                    int count = 1;
                    foreach (Tuple<int, int> symbol in region)
                    {
                        if (grid[symbol.First, symbol.Second] != null && (grid[symbol.First, symbol.Second].Name == "Square" || grid[symbol.First, symbol.Second].Name == "Sun"))
                        {
                            if (grid[symbol.First, symbol.Second].GetColorId() == colorId && symbol != sun)
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
                Tuple<int, int> point = new Tuple<int, int>(i, j);
                if (grid[point.First, point.Second] != null && grid[point.First, point.Second].Name == "Hexagon")
                {
                    if (!Utils.TupleListContains(points, point))
                    {
                        // Console.WriteLine("Hexagon not on path");
                        result[0]++;
                    }
                }
            }
        }
        return result;
    }

    public int[] BuggyRulesSuns(){
        int[] result = new int[3] { 0, 0, 0 };
        // First rule: separating the suns in equal groups but not groups of 2
        // Second rule: avoiding to touch the suns with the line
        // Third rule: grouping by 2 but ignoring the color
        IPuzzleSymbol[,] grid = panel.GetGrid();
        List<List<Tuple<int, int>>> regions = panel.GetRegions(points);
        int nbSunsColor = panel.GetSunColors().Count;
        int[,] sunCount = new int[regions.Count, nbSunsColor];
        for (int i = 0; i < regions.Count; i++)
        {
            for (int j = 0; j < panel.GetSunColors().Count; j++)
            {
                sunCount[i, j] = 0;
            }
        }
        for (int i = 0; i < regions.Count; i++)
        {
            List<Tuple<int, int>> region = regions[i];
            foreach (Tuple<int, int> sun in region)
            {
                if (grid[sun.First, sun.Second] != null && grid[sun.First, sun.Second].Name == "Sun")
                {
                    int colorId = grid[sun.First, sun.Second].GetColorId();
                    if (sunCount[i, colorId] == 0)
                    {
                        int count = 1;
                        foreach (Tuple<int, int> symbol in region)
                        {
                            if (grid[symbol.First, symbol.Second] != null && ( grid[symbol.First, symbol.Second].Name == "Sun"))
                            {
                                if (grid[symbol.First, symbol.Second].GetColorId() == colorId && symbol != sun)
                                {
                                    count++;
                                }
                            }
                        }
                        sunCount[i, colorId] += count;
                    }
                }
            }
        }
        // Check if for each color there is the same number of suns in each region
        for(int colorId = 0; colorId < nbSunsColor; colorId++)
        {
            int count = sunCount[0, colorId];
            for (int i = 1; i < regions.Count; i++)
            {
                if (sunCount[i, colorId] != count)
                {
                    result[0]++;
                }
            }
        }

        foreach(Tuple<int, int> point in points)
        {
            List<Tuple<int, int>> neighbourPillars = panel.GetPathAdjacentPillars(point.First, point.Second);
            foreach(Tuple<int, int> neighbourPillar in neighbourPillars)
            {
                if(grid[neighbourPillar.First, neighbourPillar.Second] != null && grid[neighbourPillar.First, neighbourPillar.Second].Name == "Sun")
                {
                    result[1]++;
                }
            }
        }
        // Check for every region if there only 2 suns and of different colors
        bool existsFalsePair = false;
        foreach (List<Tuple<int, int>> region in regions)
        {
            foreach (Tuple<int, int> sun in region)
            {
                if (grid[sun.First, sun.Second] != null && grid[sun.First, sun.Second].Name == "Sun")
                {
                    int colorId = grid[sun.First, sun.Second].GetColorId();
                    int count = 1;
                    foreach (Tuple<int, int> symbol in region)
                    {
                        if (grid[symbol.First, symbol.Second] != null &&  grid[symbol.First, symbol.Second].Name == "Sun")
                        {
                            if (symbol != sun)
                            {
                                count++;
                            }
                            if (grid[symbol.First, symbol.Second].GetColorId() != colorId)
                            {
                                existsFalsePair = true;
                            }
                        }
                    }
                    if (count != 2)
                    {
                        result[2] = 1;
                    }
                }
            }
        }
        if (!existsFalsePair)
        {
            result[2] = 1;
        }
        return result;
    }
}