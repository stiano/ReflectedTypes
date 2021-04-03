namespace ReflectedTypes
{
    /// <summary>
    /// General abstraction for getting hold of the actual type instance, if any. Will return null for static classes etc.
    /// </summary>
    internal interface IReflectedInstance
    {
        object? Instance { get; }
    }
}