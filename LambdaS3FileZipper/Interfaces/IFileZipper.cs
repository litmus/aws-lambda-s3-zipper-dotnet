public interface IFileZipper
{
    Task<string> Compress(IEnumerable<string> resources);
}