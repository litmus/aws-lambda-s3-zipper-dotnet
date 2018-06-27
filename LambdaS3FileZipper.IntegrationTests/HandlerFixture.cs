using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
    public class HandlerFixture
	{
		private readonly Handler handler;
		private readonly IS3TestEnvironment testEnvironment;

		public HandlerFixture()
		{
			var s3Client = new AwsS3Client(new AmazonS3Client(RegionEndpoint.USEast1));
			var service = new Service(
				new S3FileRetriever(s3Client),
				new FileZipper(), 
				new S3FileUploader(s3Client));

			handler = new Handler(service);

			testEnvironment = new EnvironmentVariableS3TestEnvironment();
		}

		[Test]
		public async Task Handle_ShouldCreateZipFil()
		{
			var request = new Request
			{
				OriginBucketName = testEnvironment.OriginTestBucket,
				OriginResourceName = "",
				DestinationBucketName = testEnvironment.DestinationTestBucket,
				DestinationResourceName = "integration-test.zip"
			};

			var context = Substitute.For<ILambdaContext>();
			context.AwsRequestId.Returns("integration-test");

			await handler.Handle(request, context);
		}
	}
}
