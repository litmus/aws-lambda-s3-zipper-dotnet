using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileUploader
	{
		/// <summary>
		/// Uploads file referenced by <c><see cref="localFileName"/></c> to the given <c><see cref="bucket"/></c>
		/// under the name <c><see cref="fileKey"/></c>.
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="fileKey"></param>
		/// <param name="localFileName"></param>
		/// <param name="cancellationToken">Optional</param>
		/// <returns><c>URL</c> of the file uploaded</returns>
		Task<string> Upload(string bucket, string fileKey, string localFileName, CancellationToken cancellationToken = default);
	}
}