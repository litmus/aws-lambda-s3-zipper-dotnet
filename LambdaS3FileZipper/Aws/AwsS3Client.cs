using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace LambdaS3FileZipper.Aws
{
	public class AwsS3Client : IAwsS3Client
	{
		private readonly IAmazonS3 client;

		public RegionEndpoint RegionEndpoint { get; }

		public AwsS3Client(RegionEndpoint regionEndpoint)
		{
			RegionEndpoint = regionEndpoint;
			client = new AmazonS3Client(regionEndpoint);
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
				response = await client.ListObjectsV2Async(request, cancellationToken);
				objects.AddRange(response.S3Objects.Select(obj => obj.Key));
			} while (response.IsTruncated);

			return objects;
		}

		public Task Upload(string bucketName, string resourceName, string localFile,
			CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}