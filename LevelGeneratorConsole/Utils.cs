using System.Collections.Generic;

static class Utils
{
    public static void ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            // Generate a random index within the remaining unshuffled part of the array
            int randomIndex = random.Next(0, i + 1);

            // Swap the elements at randomIndex and i
            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
            
        }
    }

    public static int Sum(List<int> numbers)
    {
        int sum = 0;
        foreach (int number in numbers)
        {
            sum += number;
        }
        return sum;
    }

    public static int Sum(int[] numbers)
    {
        int sum = 0;
        foreach (int number in numbers)
        {
            sum += number;
        }
        return sum;
    }

    public static T Last<T>(List<T> list)
    {
        T last = list[0];
        foreach (T element in list)
        {
            last = element;
        }
        return last;
    }

    public static bool TupleListContains(List<Tuple<int, int>> list, Tuple<int, int> tuple)
    {
        foreach (Tuple<int, int> element in list)
        {
            if (element.First == tuple.First && element.Second == tuple.Second)
            {
                return true;
            }
        }
        return false;
    }

    public static bool TupleListRemove(List<Tuple<int, int>> list, Tuple<int, int> tuple)
    {
        foreach (Tuple<int, int> element in list)
        {
            if (element.First == tuple.First && element.Second == tuple.Second)
            {
                list.Remove(element);
                return true;
            }
        }
        return false;
    }

    public static List<Tuple<int, int>> InvertTupleList(List<Tuple<int, int>> list)
    {
        List<Tuple<int, int>> invertedList = new List<Tuple<int, int>>();
        foreach (Tuple<int, int> element in list)
        {
            invertedList.Add(element.Invert());
        }
        return invertedList;
    }
}

