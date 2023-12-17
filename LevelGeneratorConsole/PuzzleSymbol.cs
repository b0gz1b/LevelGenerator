public interface IPuzzleSymbol
{
    public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows);
    public string GetSymbol();
    public int GetColorId();
    public IPuzzleSymbol Copy();
    public string Name { get; }
}

public abstract class PathSymbol : IPuzzleSymbol
{
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows)
    {
        // Check for the validity of the placement, i.e. if the placement is on a node of the panel
        // node <-> col % 2 == 0 && row % 2 == 0
        // edge <-> col % 2 != row % 2
        return index_col % 2 == 0 && index_row % 2 == 0 || index_col % 2 != index_row % 2;
    }

    abstract public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    abstract public string GetSymbol();
    abstract public IPuzzleSymbol Copy();
    public abstract int GetColorId();
    public abstract string Name { get; }
}
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

public abstract class EdgeSymbol : PathSymbol
{
    new public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows)
    {
        // Check for the validity of the placement, i.e. if the placement is on an edge of the panel
        // node <-> col % 2 == 0 && row % 2 == 0
        // edge <-> col % 2 != row % 2
        return index_col % 2 != index_row % 2;
    }

    abstract override public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    abstract override public string GetSymbol();
}

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

public class Sun : PillarSymbol
{
    int color_id;
    public Sun(int color_id)
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
            return "\u2BCD";
        else    
            return "\u2BCF";
    }

    public override IPuzzleSymbol Copy()
    {
        return new Sun(color_id);
    }

    public override int GetColorId()
    {
        return color_id;
    }

    public override string Name => "Sun";
}

public class Wall : EdgeSymbol
{
    public Wall()
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
        return "@";
    }

    public override IPuzzleSymbol Copy()
    {
        return new Wall();
    }

    public override int GetColorId()
    {
        return -1;
    }

    public override string Name => "Wall";
}