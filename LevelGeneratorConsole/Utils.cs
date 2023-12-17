public static class Utils
{
    public static void ShuffleArray<T>(T[] array)
    {
        Random random = new Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            // Generate a random index within the remaining unshuffled part of the array
            int randomIndex = random.Next(0, i + 1);

            // Swap the elements at randomIndex and i
            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }
    }

    
}
