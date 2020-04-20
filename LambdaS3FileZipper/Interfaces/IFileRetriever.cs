using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileRetriever
	{
		/// <summary>
		/// Retrieves files in given <c><see cref="bucket"/></c> matching name key <c><see cref="resource"/></c> 
		/// into a local directory named after <c><see cref="bucket"/></c>'s value.
		///
		/// <para>Optional: downloaded files can be filtered based on regular expression defined
		/// by <c><see cref="resourceExpressionPattern"/></c></para>
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="resource"></param>
		/// <param name="resourceExpressionPattern">Optional. Should be a valid regular expression</param>
		/// <param name="cancellationToken">Optional</param>
		/// <returns>Local directory path</returns>
		Task<string> RetrieveToLocalDirectory(
			string bucket, string resource, string resourceExpressionPattern = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieves files in given {<see cref="bucket"/>} matching file key {<see cref="fileKey"/>}
		/// into memory as a collection of <see cref="FileResponse"/>.
		///
		/// <para>Optional: downloaded files can be filtered based on regular expression defined
		/// by <c><see cref="resourceExpressionPattern"/></c></para>
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="fileKey"></param>
		/// <param name="resourceExpressionPattern">Optional. Should be a valid regular expression</param>
		/// <param name="cancellationToken">Optional</param>
		/// <returns>Collection of files with their content</returns>
		Task<FileResponse[]> RetrieveToMemory(
			string bucket, string fileKey, string resourceExpressionPattern = default, CancellationToken cancellationToken = default);
	}
}