using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
    public class HandlerFixture : S3Fixture
	{
		private readonly Handler handler;

		public HandlerFixture()
		{
			var service = new Service(
				new S3FileRetriever(Client),
				new FileZipper(), 
				new S3FileUploader(Client));

			handler = new Handler(service);
		}

		[Test]
		public async Task Handle_ShouldCreateZipFil()
		{
			var request = new Request
			{
				OriginBucketName = TestEnvironment.OriginTestBucket,
				OriginResourceName = "",
				DestinationBucketName = TestEnvironment.DestinationTestBucket,
				DestinationResourceName = "integration-test.zip"
			};

			var context = Substitute.For<ILambdaContext>();
			context.AwsRequestId.Returns("integration-test");

			try
			{
				await handler.Handle(request, context);

				var objects = await Client.List(request.DestinationBucketName, request.DestinationResourceName, CancellationToken.None);
				Assert.True(objects.Contains(request.DestinationResourceName));
			}
			finally
			{
				await DeleteTempS3Object(request.DestinationBucketName, request.DestinationResourceName);
			}
		}
	}
}
