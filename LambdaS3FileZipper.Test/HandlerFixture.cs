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

		[SetUp]
        public void Setup()
		{
			fileRetriever = Substitute.For<IFileRetriever>();

			fileZipper = Substitute.For<IFileZipper>();

			fileUploader = Substitute.For<IFileUploader>();

			handler = new Handler(fileRetriever, fileZipper, fileUploader);
		}

        [Test]
        public void Handle_ShouldHandleRequestAndProvideAResponse()
        {
			var request = new Request();

	        var response = handler.Handle(request);

			Assert.That(response, Is.Not.Null);
        }
    }
}