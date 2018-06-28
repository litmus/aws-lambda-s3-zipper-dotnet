using System;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	public class EnvironmentVariableS3TestEnvironment : IS3TestEnvironment
	{
		private const string TestBucketEnvironmentVariable = "S3TestBucket";
		private const string TestObjectEnvironmentVariable = "S3TestObject";
		private const string OriginTestBucketEnvironmentVariable = "OriginS3TestBucket";
		private const string OriginTestResourceNameEnvironmentVariable = "OriginS3TestResourceName";
		private const string DestinationTestBucketEnvironmentVariable = "DestinationS3TestBucket";

		private string integrationTestBucket;

		public string IntegrationTestBucket
		{
			get
			{
				if (integrationTestBucket == null)
				{
					integrationTestBucket = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariable);

					if (integrationTestBucket == null)
					{
						throw new Exception($"missing required environment variable {TestBucketEnvironmentVariable}");
					}
				}

				return integrationTestBucket;
			}
		}

		private string integrationTestResourceName;

		public string IntegrationTestResourceName
		{
			get
			{
				if (integrationTestResourceName == null)
				{
					integrationTestResourceName = Environment.GetEnvironmentVariable(TestObjectEnvironmentVariable);

					if (integrationTestResourceName == null)
					{
						throw new Exception($"missing required environment variable {TestObjectEnvironmentVariable}");
					}
				}

				return integrationTestResourceName;
			}
		}

		private string originTestBucket;
		public string OriginTestBucket
		{
			get
			{
				if (originTestBucket == null)
				{
					originTestBucket = Environment.GetEnvironmentVariable(OriginTestBucketEnvironmentVariable);

					if (originTestBucket == null)
					{
						throw new Exception($"missing required environment variable {OriginTestBucketEnvironmentVariable}");
					}
				}

				return originTestBucket;
			}
		}

		private string originTestResourceName;
		public string OriginTestResourceName
		{
			get
			{
				if (originTestResourceName == null)
				{
					originTestResourceName = Environment.GetEnvironmentVariable(OriginTestResourceNameEnvironmentVariable);

					if (originTestResourceName == null)
					{
						throw new Exception($"missing required environment variable {OriginTestResourceNameEnvironmentVariable}");
					}
				}

				return originTestResourceName;
			}
		}

		private string destinationTestBucket;
		public string DestinationTestBucket
		{
			get
			{
				if (destinationTestBucket == null)
				{
					destinationTestBucket = Environment.GetEnvironmentVariable(DestinationTestBucketEnvironmentVariable);

					if (destinationTestBucket == null)
					{
						throw new Exception($"missing required environment variable {DestinationTestBucketEnvironmentVariable}");
					}
				}

				return destinationTestBucket;
			}
		}
	}
}