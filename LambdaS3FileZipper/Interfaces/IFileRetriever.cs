using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFileRetriever
{
    Task<IEnumerable<string>> Retrieve(string bucket, string resource);
}