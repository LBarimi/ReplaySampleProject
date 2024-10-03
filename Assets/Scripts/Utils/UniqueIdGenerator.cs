public sealed class UniqueIdGenerator
{
    private static int _curId = 1;

    public static int Get()
    {
        return _curId++;
    }

    public static void Clear()
    {
        _curId = 1;
    }
}