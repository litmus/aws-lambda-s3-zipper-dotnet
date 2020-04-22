using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test
{
	public class ServiceFixture
	{
		private Service service;
        private IFileRetriever fileRetriever;
		private IFileZipper fileZipper;
		private IFileUploader fileUploader;

		private Request request;
		private FileResponse[] files;
		private string compressedFileKey;
		private FileResponse compressedFile;
		private string url;
		private CancellationToken cancellationToken;

		[SetUp]
		public void Setup()
		{
			compressedFileKey = "compressed-file";

			request = new Request("origin-bucket", "origin-resource", "destination-bucket", compressedFileKey, flatZipFile: true);

			files = new []
			{
				new FileResponse("file-0", contentStream: new MemoryStream()),
				new FileResponse("file-1", contentStream: new MemoryStream()),
				new FileResponse("file-2", contentStream: new MemoryStream()),
				new FileResponse("file-3", contentStream: new MemoryStream()),
			};

			compressedFile = new FileResponse(compressedFileKey, contentStream: new MemoryStream());

			url = "s3.com/compressed-file";

			cancellationToken = CancellationToken.None;

			fileRetriever = Substitute.For<IFileRetriever>();
			fileRetriever.RetrieveToMemory(request.OriginBucketName, request.OriginResourceName, Arg.Any<string>(), cancellationToken).Returns(files);

			fileZipper = Substitute.For<IFileZipper>();
			fileZipper.Compress(compressedFileKey, files, cancellationToken).Returns(compressedFile);

			fileUploader = Substitute.For<IFileUploader>();
			fileUploader.Upload("destination-bucket", compressedFileKey, compressedFile, cancellationToken).Returns(url);

			service = new Service(fileRetriever, fileZipper, fileUploader);
		}

		[Test]
		public async Task Handle_ShouldHandleRequestAndProvideAResponse()
		{
			var response = await service.Process(request, cancellationToken);

			Assert.That(response.Url, Is.EqualTo(url));
		}

		[Test]
		public async Task Handle_ShouldRetrieveFilesBasedOnRequest()
		{
			await service.Process(request, cancellationToken);

			await fileRetriever.Received().RetrieveToMemory(request.OriginBucketName, request.OriginResourceName, cancellationToken: cancellationToken);
		}

		[Test]
		public async Task Handle_ShouldCompressFilesRetrieved()
		{
			await service.Process(request, cancellationToken);

			await fileZipper.Received().Compress(compressedFileKey, files, cancellationToken);
		}

		[Test]
		public async Task Handle_ShouldUploadCompressedFile()
		{
			await service.Process(request, cancellationToken);

			await fileUploader
				.Received()
				.Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFile, cancellationToken);
		}
	}
}
