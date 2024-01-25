using System;
public class Hexagon : PathSymbol
{
    public Hexagon()
    {
        
    }
    public override void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows)
    {
        if (!CheckValidity(index_col, index_row, n_cols, n_rows))
        {
            throw new Exception("Invalid placement");
        }
        
    }
    public override string GetSymbol()
    {
        return "\u2B22";
    }

    public override IPuzzleSymbol Copy()
    {
        return new Hexagon();
    }

    public override int GetColorId()
    {
        return -1;
    }

    public override string Name => "Hexagon";
}