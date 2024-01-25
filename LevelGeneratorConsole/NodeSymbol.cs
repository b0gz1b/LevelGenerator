public abstract class NodeSymbol : PathSymbol
{
    new public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows)
    {
        // Check for the validity of the placement, i.e. if the placement is on a node of the panel
        // node <-> col % 2 == 0 && row % 2 == 0
        // edge <-> col % 2 != row % 2
        return index_col % 2 == 0 && index_row % 2 == 0;
    }

    abstract override public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    abstract override public string GetSymbol();
}