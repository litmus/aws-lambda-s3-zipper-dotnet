using System.Collections.Generic;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileZipper
	{
		Task<string> Compress(string localDirectory);
	}
}