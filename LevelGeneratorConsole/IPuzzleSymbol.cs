public interface IPuzzleSymbol
{
    public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows);
    public string GetSymbol();
    public int GetColorId();
    public IPuzzleSymbol Copy();
    public string Name { get; }
}