using System.Collections.Generic;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileRetriever
	{
		Task<IEnumerable<string>> Retrieve(string bucket, string resource);
	}
}