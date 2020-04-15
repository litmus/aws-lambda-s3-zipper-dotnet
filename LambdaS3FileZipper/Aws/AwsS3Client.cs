using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaS3FileZipper.Extensions;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Aws
{
	public class AwsS3Client : IAwsS3Client
	{
		private readonly IAmazonS3 client;

		public AwsS3Client(IAmazonS3 client)
		{
			this.client = client;
		}

		public async Task<IEnumerable<string>> List(string bucketName, string resource, CancellationToken cancellationToken)
		{
			var objects = new List<string>();

			var request = new ListObjectsV2Request
			{
				BucketName = bucketName,
				Prefix = resource,
			};

			ListObjectsV2Response response;
			do
			{
				cancellationToken.ThrowIfCancellationRequested();

				response = await client.ListObjectsV2Async(request, cancellationToken);
				objects.AddRange(response.S3Objects.Select(obj => obj.Key));
				request.ContinuationToken = response.ContinuationToken;
			
			} while (response.IsTruncated);

			return objects;
		}

		public async Task<string> Download(string bucketName, string resource, string destinationPath, CancellationToken cancellationToken)
		{
			if (File.Exists(destinationPath))
			{
				throw new IOException($"{destinationPath} already exists");
			}

			var request = new GetObjectRequest
			{
				BucketName = bucketName,
				Key = resource
			};

			if (!Directory.Exists(destinationPath))
			{
				Directory.CreateDirectory(destinationPath);
			}

			var localPath = Path.Combine(destinationPath, resource);
			new FileInfo(localPath).Directory.Create();

			using (var response = await client.GetObjectAsync(request, cancellationToken))
			using (var fileStream = File.OpenWrite(localPath))
			{
				await response.ResponseStream.CopyToAsync(fileStream);
			}

			return localPath;
		}

        public async Task<FileResponse> Download(string bucketName, string resourceKey, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest {BucketName = bucketName, Key = resourceKey};
            using (var response = await client.GetObjectAsync(request, cancellationToken))
            {
                return new FileResponse(resourceKey, await response.ResponseStream.CopyStreamOntoMemory(cancellationToken));
            }
        }

        public async Task Upload(string bucketName, string resourceName, string filePath, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var request = new PutObjectRequest {BucketName = bucketName, Key = resourceName, FilePath = filePath};
			await client.PutObjectAsync(request, cancellationToken);
		}

		public async Task Delete(string bucketName, string resourceName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var request = new DeleteObjectRequest {BucketName = bucketName, Key = resourceName};
			await client.DeleteObjectAsync(request, cancellationToken);
		}
		public string GenerateUrl(string bucketName, string resourceName)
		{
			var request = new GetPreSignedUrlRequest
			{
				BucketName = bucketName,
				Key = resourceName,
				Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(12))
			};

			return client.GetPreSignedURL(request);
		}
	}
}