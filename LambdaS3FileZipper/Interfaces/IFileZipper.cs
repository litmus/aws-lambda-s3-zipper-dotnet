using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFileZipper
{
    Task<string> Compress(IEnumerable<string> resources);
}