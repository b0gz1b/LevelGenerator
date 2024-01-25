using System;

public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
{
    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    public T1 Item1 { get { return First; } }
    public T2 Item2 { get { return Second; } }
    internal Tuple(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }

    public bool Equals(Tuple<T1, T2> other)
    {
        if (other == null) return false;
        if (First == null)
        {
            if (other.First != null) return false;
        }
        else if (!First.Equals(other.First)) return false;
        if (Second == null)
        {
            if (other.Second != null) return false;
        }
        else if (!Second.Equals(other.Second)) return false;
        return true;
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}