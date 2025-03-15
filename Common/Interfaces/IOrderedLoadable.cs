namespace AerovelenceMod.Common.Interfaces
{
    interface IOrderedLoadable
    {
        void Load();
        void Unload();
        float Priority { get; }
    }
}
