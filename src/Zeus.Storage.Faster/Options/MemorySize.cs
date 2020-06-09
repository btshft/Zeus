namespace Zeus.Storage.Faster.Options
{
    public enum MemorySize
    {
        Mb4 = 22,
        Mb8 = Mb4 + 1,
        Mb16 = Mb8 + 1,
        Mb32 = Mb16 + 1,
        Mb64 = Mb32 + 1,
        Mb128 = Mb64 + 1,
        Mb256 = Mb128 + 1,
        Mb512 = Mb256 + 1,
        Mb1024 = Mb512 + 1,
        Gb1 = Mb1024,
        Gb2 = Gb1 + 1
    }
}