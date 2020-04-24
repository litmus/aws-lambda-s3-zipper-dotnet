using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileZipper
	{
		Task<string> Compress(string localDirectory, bool flat);

		Task<FileResponse> Compress(
			string zipFileKey,
			IEnumerable<FileResponse> filesResponses,
			bool flat = false,
			CancellationToken cancellationToken = default);
	}
}