public interface IPuzzleSymbol
{
    public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows);
    public string GetSymbol();
}

public abstract class PathSymbol : IPuzzleSymbol
{
    public bool CheckValidity(int index_col, int index_row, int n_cols, int n_rows)
    {
        // Check for the validity of the placement, i.e. if the placement is on an edge or a node of the panel
        // node <-> col % 2 == 0 && row % 2 == 0
        // edge <-> col % 2 != row % 2
        return (index_col % 2 == 0 && index_row % 2 == 0) || (index_col % 2 != index_row % 2);
    }

    abstract public void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows);
    abstract public string GetSymbol();
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
}

public class Hexagon : PathSymbol
{
    public Hexagon()
    {
        Console.WriteLine("Hexagon created");
    }
    public override void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows)
    {
        if (!CheckValidity(index_col, index_row, n_cols, n_rows))
        {
            throw new Exception("Invalid placement");
        }
        Console.WriteLine("Place Hexagon at ({0}, {1})", index_col, index_row);
    }
    public override string GetSymbol()
    {
        return "\u2B22";
    }
}

public class Square : PillarSymbol
{
    int color_id;
    public Square(int color_id)
    {
        this.color_id = color_id;
        Console.WriteLine("Square created");
    }
    public override void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows)
    {
        if (!CheckValidity(index_col, index_row, n_cols, n_rows))
        {
            throw new Exception("Invalid placement");
        }
        Console.WriteLine("Place Square at ({0}, {1})", index_col, index_row);
    }
    public override string GetSymbol()
    {
        return "\u25A0";
    }
}

public class Sun : PillarSymbol
{
    int color_id;
    public Sun(int color_id)
    {
        this.color_id = color_id;
        Console.WriteLine("Sun created");
    }
    public override void PlaceSymbol(int index_col, int index_row, int n_cols, int n_rows)
    {
        if (!CheckValidity(index_col, index_row, n_cols, n_rows))
        {
            throw new Exception("Invalid placement");
        }
        Console.WriteLine("Place Sun at ({0}, {1})", index_col, index_row);
    }
    public override string GetSymbol()
    {
        return "\u2BCD";
    }
}