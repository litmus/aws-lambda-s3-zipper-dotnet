using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

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

		/// <summary>
		/// Uploads in-memory file <c><see cref="fileResponse"/></c> of type <see cref="FileResponse"/>
		/// to the given <c><see cref="bucket"/></c> under the name <c><see cref="fileKey"/></c>.
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="fileKey"></param>
		/// <param name="fileResponse"></param>
		/// <param name="cancellationToken">Optional</param>
		/// <returns><c>URL</c> of the file uploaded</returns>
		Task<string> Upload(string bucket, string fileKey, FileResponse fileResponse, CancellationToken cancellationToken = default);
	}
}