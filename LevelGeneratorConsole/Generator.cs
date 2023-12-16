static class Generator
{
    public static Path GenerateLevel(int nRows, int nCols, int nXa , int nSquare, int nSun, int nHexagon)
    {
        Panel puzzlePanel = new(nRows, nCols);
        // Place random start and end positions
        puzzlePanel.SetStart(puzzlePanel.RandomStartEnd());
        puzzlePanel.SetEnd(puzzlePanel.RandomStartEnd());
        // Generate a random path
        Path randomPath = Path.GenerateRandomPath(puzzlePanel);
        // Backtrack algorithm to place squares and suns
        return randomPath;
    }
}