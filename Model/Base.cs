namespace Model
{
    /// <summary>
    /// Base class of all model classes.
    /// </summary>
    public abstract class Base
    {
        // All model objects have an associated unique id.
        public int Id { get; set; }
    }
}
