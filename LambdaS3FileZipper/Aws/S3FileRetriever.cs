using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;

namespace LambdaS3FileZipper.Aws
{
	public class S3FileRetriever : IFileRetriever
	{
		private const int MaxConcurrentDownloads = 10;

	    private readonly IAwsS3Client s3Client;

	    public S3FileRetriever(IAwsS3Client s3Client)
	    {
		    this.s3Client = s3Client;
	    }

	    public async Task<string> Retrieve(string bucket, string resource, CancellationToken cancellationToken)
	    {
		    var files = await s3Client.List(bucket, resource, cancellationToken);

		    var downloadPath = Path.Combine(Path.GetTempPath(), bucket);

		    if (Directory.Exists(downloadPath))
		    {
				Directory.Delete(downloadPath, recursive: true);
		    }

		    using (var semaphore = new SemaphoreSlim(MaxConcurrentDownloads))
		    {
			    var tasks = new List<Task>();
			    foreach (var file in files)
			    {
				    await semaphore.WaitAsync(cancellationToken);

				    var task = Task.Factory.StartNew(async () =>
				    {
					    try
					    {
						    await s3Client.Download(bucket, file, downloadPath, cancellationToken);
					    }
					    finally
					    {
						    semaphore.Release();
					    }
				    }, cancellationToken);

					tasks.Add(task);
				}

			    await Task.WhenAll(tasks.ToArray());
		    }

		    return downloadPath;
	    }
    }
}
