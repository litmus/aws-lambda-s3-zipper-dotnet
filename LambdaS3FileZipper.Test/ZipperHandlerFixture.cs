using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test
{
    public class ZipperHandlerFixture
    {
	    private ZipperHandler zipperHandler;

	    private IFileRetriever fileRetriever;
	    private IFileZipper fileZipper;
	    private IFileUploader fileUploader;

		[SetUp]
        public void Setup()
		{
			fileRetriever = Substitute.For<IFileRetriever>();

			fileZipper = Substitute.For<IFileZipper>();

			fileUploader = Substitute.For<IFileUploader>();

			zipperHandler = new ZipperHandler(fileRetriever, fileZipper, fileUploader);
		}

        [Test]
        public void Handle_ShouldHandleRequestAndProvideAResponse()
        {
			var request = new Request();

	        var response = zipperHandler.Handle(request);

			Assert.That(response, Is.Not.Null);
        }
    }
}