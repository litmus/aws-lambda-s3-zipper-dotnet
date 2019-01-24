using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Exceptions;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;

namespace LambdaS3FileZipper.Aws
{
	public class S3FileRetriever : IFileRetriever
	{
		private const int MaxConcurrentDownloads = 10;
	    private const string DefaultResourceExpressionPattern = ".*";

        private readonly IAwsS3Client s3Client;
		private readonly ILog log;

		public S3FileRetriever(IAwsS3Client s3Client)
		{
			this.s3Client = s3Client;

			this.log = LogProvider.GetCurrentClassLogger();
		}

		public async Task<string> Retrieve(
		    string bucket,
		    string resource,
            string resourceExpressionPattern = default,
		    CancellationToken cancellationToken = default)
		{
		    if (string.IsNullOrWhiteSpace(resourceExpressionPattern))
		    {
		        resourceExpressionPattern = DefaultResourceExpressionPattern;
            }

            log.Debug("Using resource name expression pattern {resourceExpressionPattern}", resourceExpressionPattern);
		    var regex = new Regex(resourceExpressionPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var files = (await s3Client.List(bucket, resource, cancellationToken)).Where(file => regex.IsMatch(file)).ToArray();
		    if (files.Any() == false)
			{
				log.Warn("There are no files listed under {bucket}/{resource} that match expression {resourceExpressionPattern}; nothing to ZIP",
				    bucket, resource, resourceExpressionPattern);
				throw new ResourceNotFoundException(bucket, resource, resourceExpressionPattern);
			}

			var downloadPath = Path.Combine(Path.GetTempPath(), bucket);
			if (Directory.Exists(downloadPath))
			{
				Directory.Delete(downloadPath, recursive: true);
			}

			using (var throttler = new SemaphoreSlim(MaxConcurrentDownloads))
			{
				var tasks = new List<Task>();

				foreach (var file in files)
				{
					await throttler.WaitAsync(cancellationToken);

					tasks.Add(Task.Run(async () =>
					{
						try
						{
							await s3Client.Download(bucket, file, downloadPath, cancellationToken);
						}
						finally
						{
							throttler.Release();
						}
					}, cancellationToken));
				}

				await Task.WhenAll(tasks.ToArray());
			}
			
			return downloadPath;
		}
	}
}