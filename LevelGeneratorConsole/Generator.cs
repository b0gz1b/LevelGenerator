using System.Runtime.CompilerServices;

static class Generator
{
    public static Path GenerateLevel(int nRows, int nCols, int nWalls, int nHexagon, int nColors, List<int> nSquareByColor, List<int> nSunByColor)
    {
        Panel puzzlePanel = new(nRows, nCols);
        Path randomPath = new(puzzlePanel);
        bool is_valid = false;
        int maxIteration = 10000;
        while(!is_valid && maxIteration-- > 0){
            puzzlePanel = new(nRows, nCols);
            randomPath = new(puzzlePanel);
            // Place random start and end positions
            puzzlePanel.RandomStartEndCOVR();
            // Debug: print the panel
            // puzzlePanel.PrintPanel(printStartEnd : true);
            // Place random walls
            bool walls_valid = false;
            Random random = new();
            while (!walls_valid)
            {
                for (int i = 0; i < nWalls; i++)
                {
                    Wall wall = new();
                    int indexCol;
                    int indexRow;
                    do
                    {
                        int a = random.Next(0, (puzzlePanel.GetGrid().GetLength(0) + 1) / 2);
                        int b = random.Next(0, (puzzlePanel.GetGrid().GetLength(1) - 1) / 2);
                        if (random.Next(2) == 0)
                        {
                            indexRow = b * 2 + 1;
                            indexCol = 2 * a;
                        }
                        else
                        {
                            indexRow = 2 * a;
                            indexCol = b * 2 + 1;
                        }
                    } while (!puzzlePanel.IsPointValid(indexRow, indexCol));
                    puzzlePanel.PlaceSymbol(wall, indexRow, indexCol);
                }
                // Generate a random path inside a try catch
                try
                {
                    randomPath = Path.GenerateRandomPath(puzzlePanel);
                    walls_valid = true;
                }
                catch
                {
                    walls_valid = false;
                    puzzlePanel = new(nRows, nCols);
                }
            }
            List<Tuple<int, int>> points = randomPath.GetPoints();
            List<Tuple<int, int>> points_copy = new(points);
            
            // Debug: print the path
            // randomPath.PrintRegions();

            // Place hexagons randomly on the path
            for(int i = 0; i < nHexagon; i++){
                Hexagon hexagon = new();
                int index = random.Next(0,points_copy.Count);
                puzzlePanel.PlaceSymbol(hexagon, randomPath.GetPoints()[index].Item1, randomPath.GetPoints()[index].Item2);
                points_copy.RemoveAt(index);
            }
            // Debug: print the panel
            // puzzlePanel.PrintPanel();

            List<List<Tuple<int, int>>> regions = puzzlePanel.GetRegions(points);
            int minNumberOfRegions = 0;
            if(nSunByColor.Sum() > 0)
                minNumberOfRegions += 1;
            if(nSquareByColor.Sum() > 0)
                minNumberOfRegions += 1;
            minNumberOfRegions += nRows * nCols / 16;
            if(regions.Count >= nColors && regions.Count >= minNumberOfRegions){
                // Generate the array of region indices
                int[] regionIndices = new int[regions.Count];
                for(int i = 0; i < regionIndices.Length; i++){
                    regionIndices[i] = i;
                }
                // Generate the array of pillars indices in each region
                int[][] pillarsIndices = new int[regions.Count][];
                for(int i = 0; i < regions.Count; i++){
                    pillarsIndices[i] = new int[regions[i].Count];
                    for(int j = 0; j < pillarsIndices[i].Length; j++){
                        pillarsIndices[i][j] = j;
                    }
                }
                // We know have the random orders of exploration of the regions and the pillars in each region
                // We can now try to place the squares and the suns using backtracking
                List<int> nSquareByColor_copy = new(nSquareByColor);
                List<int> nSunByColor_copy = new(nSunByColor);
                Panel puzzlePanelCopy = new(puzzlePanel);
                // init at -1
                int[] regionColor = new int[regions.Count];
                for(int i = 0; i < regionColor.Length; i++){
                    regionColor[i] = -1;
                }
                // init at 0
                int[][] sunCountByRegionByColor = new int[regions.Count][];
                int[] squareCountByRegion = new int[regions.Count];
                for(int i = 0; i < sunCountByRegionByColor.Length; i++){
                    sunCountByRegionByColor[i] = new int[nColors];
                    for(int j = 0; j < sunCountByRegionByColor[i].Length; j++){
                        sunCountByRegionByColor[i][j] = 0;
                    }
                    squareCountByRegion[i] = 0;
                }
                
                // Backtrack loop while we have not placed all the squares and suns
                // Init the array that keeps track of pillars visited at each iteration to a list of empty lists
                List<List<Tuple<bool,Tuple<int, int>>>> pillarsVisited = new(){new()};
                int iteration = 0;
                int maxIterationBacktrack = 5000;
                int mustPlaceInSameRegionAsSun = -1;
                int lastColor = -1;
                while((nSquareByColor_copy.Sum() > 0 || nSunByColor_copy.Sum() > 0) && maxIterationBacktrack-- > 0){
                    // Debug
                    // Console.WriteLine("Iteration " + iteration);
                    
                    // Choose the region to place the square or sun
                    Utils.ShuffleArray(regionIndices);
                    int regionIndice = 0;
                    bool placed = false;
                    while(regionIndice < regionIndices.Length && !placed){
                        int regionIndex = mustPlaceInSameRegionAsSun;

                        // Draw if we place a square or a sun if we have both
                        bool isSquare;
                        if (regionIndex != -1)
                            isSquare = nSunByColor_copy[lastColor] == 0;
                        else if(nSquareByColor_copy.Sum() > 0 && nSunByColor_copy.Sum() > 0)
                            isSquare = random.Next(2) == 0;
                        else
                            isSquare = nSquareByColor_copy.Sum() > 0;
                        
                        // Choose the color of the square or sun in the remaining colors
                        int color = -1;
                        if(regionIndex == -1){
                            if(isSquare){
                                for(int i = 0; i < nSquareByColor_copy.Count; i++){
                                    if(nSquareByColor_copy[i] > 0){
                                        color = i;
                                        break;
                                    }
                                }
                            }
                            else{
                                for(int i = 0; i < nSunByColor_copy.Count; i++){
                                    if(nSunByColor_copy[i] > 0){
                                        color = i;
                                        break;
                                    }
                                }
                            }
                        }
                        else{
                            color = lastColor;
                            if(nSquareByColor_copy[color] == 0 && nSunByColor_copy[color] == 0){
                                break; // We cannot place a square or sun of the same color as the region, hence we will never be able to pair the sun with anything in that region
                            }
                        }
                        if(regionIndex == -1){
                            regionIndex = regionIndices[regionIndice];
                            // Console.WriteLine("\t Trying region " + regionIndex);
                            if(!CanPlaceSquareOrSun(regionIndex, isSquare, color, regionColor, sunCountByRegionByColor, squareCountByRegion, regions[regionIndex].Count, nSunByColor_copy)){
                                regionIndice++;
                                continue;
                            }
                        }
                        else{
                            // Console.WriteLine("\t Trying region " + regionIndex);
                            if(!CanPlaceSquareOrSun(regionIndex, isSquare, color, regionColor, sunCountByRegionByColor, squareCountByRegion, regions[regionIndex].Count, nSunByColor_copy))
                                break;
                        }

                        
                        // Choose the pillar to place the square or sun
                        Utils.ShuffleArray(pillarsIndices[regionIndex]);
                        for(int j = 0; j < pillarsIndices[regionIndex].Length; j++){
                            int pillarIndex = pillarsIndices[regionIndex][j];

                            if(puzzlePanelCopy.GetGrid()[regions[regionIndex][pillarIndex].Item1, regions[regionIndex][pillarIndex].Item2] != null || pillarsVisited[iteration].Contains(new(isSquare, regions[regionIndex][pillarIndex]))){
                                // Debug
                                // Console.WriteLine("\t\tPillar " + regions[regionIndex][pillarIndex] + " already visited or occupied");
                                regionIndice++;
                                continue;
                            }

                            if(isSquare){
                                Square square = new(color);
                                puzzlePanelCopy.PlaceSymbol(square, regions[regionIndex][pillarIndex].Item1, regions[regionIndex][pillarIndex].Item2);
                                squareCountByRegion[regionIndex]++;
                                nSquareByColor_copy[color]--;
                                regionColor[regionIndex] = color;
                                mustPlaceInSameRegionAsSun = -1;
                            }
                            else{
                                Sun sun = new(color);
                                puzzlePanelCopy.PlaceSymbol(sun, regions[regionIndex][pillarIndex].Item1, regions[regionIndex][pillarIndex].Item2);
                                sunCountByRegionByColor[regionIndex][color]++;
                                if(sunCountByRegionByColor[regionIndex][color] == 1 && (squareCountByRegion[regionIndex] == 0 || regionColor[regionIndex] != color))
                                    mustPlaceInSameRegionAsSun = regionIndex;
                                else
                                    mustPlaceInSameRegionAsSun = -1;
                                nSunByColor_copy[color]--;
                            }
                            lastColor = color;
                            pillarsVisited[iteration].Add(new(isSquare, regions[regionIndex][pillarIndex]));
                            placed = true;
                            break;
                        }
                        regionIndice++;
                    }
                    if(!placed){
                        // Debug
                        // Console.WriteLine("\t No pillar found, backtracking");
                        if(iteration == 0)
                            break;
                        else{
                            pillarsVisited.RemoveAt(iteration);
                            iteration--;
                            bool isLastSquare = pillarsVisited[iteration][^1].Item1;
                            Tuple<int, int> lastPillar = pillarsVisited[iteration][^1].Item2;
                            // get the region index of the last pillar and color
                            int lastRegionIndex = -1;
                            int lastColorIt = -1;
                            for(int i = 0; i < regions.Count; i++){
                                if(regions[i].Contains(lastPillar)){
                                    lastRegionIndex = i;
                                    lastColorIt = regionColor[i];
                                    break;
                                }
                            }
                            
                            if(isLastSquare){
                                squareCountByRegion[lastRegionIndex]--;
                                if(squareCountByRegion[lastRegionIndex] == 0)
                                    regionColor[lastRegionIndex] = -1;
                                nSquareByColor_copy[lastColorIt]++;
                                
                            }
                            else{
                                lastColorIt = puzzlePanelCopy.GetGrid()[lastPillar.Item1, lastPillar.Item2].GetColorId();
                                sunCountByRegionByColor[lastRegionIndex][lastColorIt]--;
                                nSunByColor_copy[lastColorIt]++;
                            }
                            // check if we need to pair a sun
                            if(sunCountByRegionByColor[lastRegionIndex][lastColorIt] == 1 && (squareCountByRegion[lastRegionIndex] == 0 || regionColor[lastRegionIndex] != lastColorIt))
                                mustPlaceInSameRegionAsSun = lastRegionIndex;
                            else
                                mustPlaceInSameRegionAsSun = -1;
                            puzzlePanelCopy.RemoveSymbol(lastPillar.Item1, lastPillar.Item2);
                        }
                    }
                    else if(nSquareByColor_copy.Sum() == 0 && nSunByColor_copy.Sum() == 0){
                        // Debug
                        // Console.WriteLine("\t All squares and suns placed");
                        randomPath.SetPanel(puzzlePanelCopy);
                        is_valid = randomPath.isPathValid();
                    }
                    else{
                        iteration++;
                        pillarsVisited.Add(new());
                    }
                    randomPath.SetPanel(puzzlePanelCopy);
                    // Console.WriteLine("Must place sun? " + mustPlaceInSameRegionAsSun);
                    // randomPath.PrintPath();
                    // Console.Write("\t");
                    // for(int i = 0; i < regionColor.Length; i++){
                    //     Console.Write("(" + i + "," + regionColor[i] + ") ");
                    // }
                    // Console.WriteLine();
                    // Pause for 1 second
                    // Thread.Sleep(1);
                    // for(int i = 0; i < nSunByColor_copy.Count; i++){
                    //     Console.Write(i+ "," + nSunByColor_copy[i] + "; ");
                    // }
                    // Console.WriteLine();
                }
                // Console.WriteLine("iterations: " + (5000 - maxIterationBacktrack - 1));

            }
        }
        return randomPath;
    }

    private static bool CanPlaceSquareOrSun(int regionIndex, bool isSquare, int symbolColor, int[] regionColor, int[][] sunCountByRegionByColor, int[] squareCountByRegion, int regionSize, List<int> nSunLeft)
    {
        int suns;
        if (regionColor[regionIndex] == -1 || !isSquare)
        { // If the region has no color or if we want to place a sun, we check the number of suns of the same color as the symbol we want to place
            suns = sunCountByRegionByColor[regionIndex][symbolColor];
        }
        else
        {
            suns = sunCountByRegionByColor[regionIndex][regionColor[regionIndex]]; // If the region has a color and we want to place a square, we check the number of suns of the same color as the region
        }
        // compute the total number of suns in the region
        int totalSuns = sunCountByRegionByColor[regionIndex].Sum();
        // Compute the total number of suns in the region not yey paired with a symbol of the same color
        int totalSunsNotPaired = 0;
        List<int> colorsMissing = new();
        for (int i = 0; i < sunCountByRegionByColor[regionIndex].Length; i++)
            if (i != regionColor[regionIndex] && sunCountByRegionByColor[regionIndex][i] == 1){
                totalSunsNotPaired++;
                colorsMissing.Add(i);
            }

        // compute all free spaces in the region
        int freeSpaces = regionSize - squareCountByRegion[regionIndex] - totalSuns;
        if (freeSpaces == 0)
            return false; // We cannot place a symbol if there is no free space in the region

        if(freeSpaces < totalSunsNotPaired)
            return false; // We cannot place a symbol if there is not enough free space in the region to pair all the suns not yet paired


        if (isSquare)
        {
            if (regionColor[regionIndex] == -1 || regionColor[regionIndex] == symbolColor){ // Check if the region color corresponds to the color of the square if the region has a color
                if (regionColor[regionIndex] == -1 && nSunLeft[symbolColor] == 0 && totalSunsNotPaired > 0)
                    return colorsMissing.Contains(symbolColor);
                
                if (suns == 2)
                {
                    return false; // We cannot place a square if there are already 2 suns of the same color (as the square) in the region
                }
                else if (suns == 1)
                {
                    return squareCountByRegion[regionIndex] == 0; // We can place a square if there is 1 sun of the same color (as the square) in the region and no square
                }
                else if (regionColor[regionIndex] == symbolColor)
                {
                    return freeSpaces - 1 >= totalSunsNotPaired;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        else
        { // Here suns is the number of suns of the same color as the sun we want to place
            if(regionSize == 1)
                return false;
            else if (suns == 2)
                return false; // We cannot place a sun if there are already 2 suns of the same color (as the sun) in the region
            else if (suns == 1)
                return regionColor[regionIndex] == -1 || (regionColor[regionIndex] == symbolColor && squareCountByRegion[regionIndex] == 0); // We can place a sun if there is 1 sun of the same color (as the sun) in the region and no square
            else
                return regionColor[regionIndex] == -1 || (regionColor[regionIndex] == symbolColor && squareCountByRegion[regionIndex] < 2); // We can place a sun if there is no sun and exactly or less than 1 squares of the same color (as the sun) in the region
        }


    }
}