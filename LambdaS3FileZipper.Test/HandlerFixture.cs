using System.Collections.Generic;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test
{
    public class HandlerFixture
    {
	    private Handler handler;

	    private IFileRetriever fileRetriever;
	    private IFileZipper fileZipper;
	    private IFileUploader fileUploader;

	    private Request request;
	    private IEnumerable<string> files;
	    private string compressedFile;
		private string url;

		[SetUp]
        public void Setup()
		{
			request = new Request("origin-bucket", "origin-resource", "destination-bucket", "destination-resource");

			files = new[] {"file-0", "file-1", "file-2", "file-3"};

			compressedFile = "compressed-file";

			url = "s3.com/compressed-file";

			fileRetriever = Substitute.For<IFileRetriever>();
			fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName).Returns(files);

			fileZipper = Substitute.For<IFileZipper>();
			fileZipper.Compress(files).Returns(compressedFile);

			fileUploader = Substitute.For<IFileUploader>();
			fileUploader.Upload(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(url);

			handler = new Handler(fileRetriever, fileZipper, fileUploader);
		}

        [Test]
        public async Task Handle_ShouldHandleRequestAndProvideAResponse()
        {
	        var response = await handler.Handle(request);

			Assert.That(response.Url, Is.EqualTo(url));
        }

	    [Test]
	    public async Task Handle_ShouldRetrieveFilesBasedOnRequest()
	    {
		    await handler.Handle(request);

		    await fileRetriever.Received().Retrieve(request.OriginBucketName, request.OriginResourceName);
	    }

	    [Test]
	    public async Task Handle_ShouldCompressFilesRetrieved()
	    {
		    await handler.Handle(request);

		    await fileZipper.Received().Compress(files);
	    }

	    [Test]
	    public async Task Handle_ShouldUploadCompressedFile()
	    {
		    await handler.Handle(request);

		    await fileUploader.Received().Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFile);
	    }
	}
}