using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Exceptions;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

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

		public async Task<string> RetrieveToLocalDirectory(
		    string bucket,
		    string resource,
            string resourceExpressionPattern = default,
		    CancellationToken cancellationToken = default)
		{
		    var files = await FindFilesMatchingExpression(bucket, resource, resourceExpressionPattern, cancellationToken);

			var downloadPath = Path.Combine(Path.GetTempPath(), bucket);
			if (Directory.Exists(downloadPath))
			{
				Directory.Delete(downloadPath, recursive: true);
			}

			await RetrieveConcurrently(files, retrieveEachFile: file => s3Client.Download(bucket, file, downloadPath, cancellationToken), cancellationToken);
			return downloadPath;
		}

		public async Task<FileResponse[]> RetrieveToMemory(
			string bucket,
			string fileKey,
			string resourceExpressionPattern = default,
			CancellationToken cancellationToken = default)
		{
			var fileKeys = await FindFilesMatchingExpression(bucket, fileKey, resourceExpressionPattern, cancellationToken);

			var fileResponses = new ConcurrentBag<FileResponse>();
			await RetrieveConcurrently(
				fileKeys,
				retrieveEachFile: async file => fileResponses.Add(await s3Client.Download(bucket, file, cancellationToken)),
				cancellationToken);
			return fileResponses.ToArray();
		}

		private async Task<string[]> FindFilesMatchingExpression(
			string bucket,
			string fileKey,
			string resourceExpressionPattern = default,
			CancellationToken cancellationToken = default)
		{
			resourceExpressionPattern ??= DefaultResourceExpressionPattern;
			log.Debug("Using resource name expression pattern {resourceExpressionPattern}", resourceExpressionPattern);

			var regex = new Regex(resourceExpressionPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var files = (await s3Client.List(bucket, fileKey, cancellationToken)).Where(file => regex.IsMatch(file)).ToArray();
			if (files.Any() == false)
			{
				log.Warn("No files under {bucket}/{resource} matching {resourceExpressionPattern}", bucket, fileKey, resourceExpressionPattern);
				throw new ResourceNotFoundException(bucket, fileKey, resourceExpressionPattern);
			}

			return files;
		}

		private async Task RetrieveConcurrently(string[] fileKeys, Func<string, Task> retrieveEachFile, CancellationToken cancellationToken)
		{
			using var throttler = new SemaphoreSlim(MaxConcurrentDownloads);

			var tasks = new List<Task>();
			var total = fileKeys.Length;
			var current = 0;

			foreach (var fileKey in fileKeys)
			{
				await throttler.WaitAsync(cancellationToken);

				tasks.Add(Task.Run(async () =>
				{
					try
					{
						var stopwatch = Stopwatch.StartNew();

						await retrieveEachFile(fileKey);

						Interlocked.Increment(ref current);
						log.Trace("Retrieved file {File} ({Current}/{Total}) | {Time}ms", fileKey, current, total, stopwatch.ElapsedMilliseconds);
					}
					finally
					{
						throttler.Release();
					}
				}, cancellationToken));
			}

			await Task.WhenAll(tasks.ToArray());
		}
	}
}