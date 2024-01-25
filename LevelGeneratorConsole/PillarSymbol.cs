public abstract class PillarSymbol : IPuzzleSymbol
{
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows)
    {
        // Check for the validity of the placement, i.e. if the placement is on a pillar
        // pillar <-> col % 2 == 1 && row % 2 == 1
        return index_col % 2 == 1 && index_row % 2 == 1;
    }

    abstract public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    abstract public string GetSymbol();
    abstract public IPuzzleSymbol Copy();
    public abstract int GetColorId();
    public abstract string Name { get; }
}