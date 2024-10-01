public sealed class UniqueIdGenerator
{
    private static ulong _curId = 0;

    public static ulong Get()
    {
        return _curId++;
    }
}