using System;
public class Square : PillarSymbol
{
    int color_id;
    public Square(int color_id)
    {
        this.color_id = color_id;
        
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
        // support for two colors only
        if (color_id == 0)
            return "\u25A0";
        else    
            return "\u25A1";
    }

    public override IPuzzleSymbol Copy()
    {
        return new Square(color_id);
    }

    public override int GetColorId()
    {
        return color_id;
    }

    public override string Name => "Square";
}