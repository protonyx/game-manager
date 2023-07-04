namespace GameManager.Application;

public static class ListExtensions
{
    public static void MoveItemByIndex<T>(this IList<T> list, int oldIndex, int newIndex)
    {
        if (newIndex < 0 || newIndex > list.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(newIndex));
        }
        if (oldIndex < 0 || oldIndex > list.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(oldIndex));
        }
        if (oldIndex == newIndex)
        {
            return;
        }
        
        var target = list[oldIndex];
        var delta = newIndex < oldIndex ? -1 : 1;

        for (int i = oldIndex; i != newIndex; i += delta)
        {
            list[i] = list[i + delta];
        }

        list[newIndex] = target;
    }
}