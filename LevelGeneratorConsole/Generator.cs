static class Generator
{
    public static Path GenerateLevel(int nRows, int nCols, int nWalls , int nSquare, int nSun, int nHexagon)
    {
        Panel puzzlePanel = new(nRows, nCols);
        bool is_valid = false;
        Path randomPath = new(puzzlePanel);
        while(!is_valid){
            // Place random start and end positions
            puzzlePanel.RandomStartEndCOVR();
            // Place random walls
            for(int i = 0; i < nWalls; i++){
                Wall wall = new();
                int indexRow = 0;
                int indexCol = 0;
                Random random = new();
                do{
                    int a = random.Next(0, (puzzlePanel.GetGrid().GetLength(0) + 1) / 2);
                    int b = random.Next(0, (puzzlePanel.GetGrid().GetLength(1) - 1) / 2);
                    if(random.Next(2) == 0){
                        indexRow = b * 2 + 1;
                        indexCol = 2 * a;
                    }
                    else{
                        indexRow = 2 * a;
                        indexCol = b * 2 + 1;
                    }
                }while(!puzzlePanel.IsPointValid(indexRow, indexCol) );
                puzzlePanel.PlaceSymbol(wall, indexRow, indexCol);
            }
            // Generate a random path
            randomPath = Path.GenerateRandomPath(puzzlePanel);
            // Backtrack algorithm to place squares and suns

        }
        return randomPath;
    }
}