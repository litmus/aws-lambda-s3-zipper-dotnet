using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

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
				Prefix = resource
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

		public async Task Download(string bucketName, string resource, string destinationPath, CancellationToken cancellationToken)
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

			using (var response = await client.GetObjectAsync(request, cancellationToken))
			using (var fileStream = File.OpenWrite(destinationPath))
			{
				await response.ResponseStream.CopyToAsync(fileStream);
			}	
		}

		public async Task Upload(string bucketName, string resourceName, string filePath, CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			var request = new PutObjectRequest {BucketName = bucketName, Key = resourceName, FilePath = filePath};
			await client.PutObjectAsync(request, token);
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