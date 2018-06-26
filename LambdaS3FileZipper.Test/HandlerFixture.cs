using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test
{
	public class HandlerFixture
	{
		private Handler handler;
        private ILambdaContext lambdaContext;
        private IFileRetriever fileRetriever;
		private IFileZipper fileZipper;
		private IFileUploader fileUploader;

		private Request request;
		private string directory;
		private string compressedFile;
		private string url;

		[SetUp]
		public void Setup()
		{
			request = new Request("origin-bucket", "origin-resource", "destination-bucket", "destination-resource");

			directory = @"/tmp/downloads";

			compressedFile = "compressed-file";

			url = "s3.com/compressed-file";

			fileRetriever = Substitute.For<IFileRetriever>();
			fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName).Returns(directory);

			fileZipper = Substitute.For<IFileZipper>();
			fileZipper.Compress(directory).Returns(compressedFile);

			fileUploader = Substitute.For<IFileUploader>();
			fileUploader.Upload(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(url);

			handler = new Handler(fileRetriever, fileZipper, fileUploader);

            lambdaContext = Substitute.For<ILambdaContext>();
		}

		[Test]
		public async Task Handle_ShouldHandleRequestAndProvideAResponse()
		{
			var response = await handler.Handle(request, lambdaContext);

			Assert.That(response.Url, Is.EqualTo(url));
		}

		[Test]
		public async Task Handle_ShouldRetrieveFilesBasedOnRequest()
		{
			await handler.Handle(request, lambdaContext);

			await fileRetriever.Received().Retrieve(request.OriginBucketName, request.OriginResourceName);
		}

		[Test]
		public async Task Handle_ShouldCompressFilesRetrieved()
		{
			await handler.Handle(request, lambdaContext);

			await fileZipper.Received().Compress(directory);
		}

		[Test]
		public async Task Handle_ShouldUploadCompressedFile()
		{
			await handler.Handle(request, lambdaContext);

			await fileUploader
				.Received()
				.Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFile, CancellationToken.None);
		}
	}
}
