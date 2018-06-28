using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test
{
	[TestFixture]
    public class HandlerFixture
	{
		private Handler handler;

		private IService service;

		private Request request;
		private ILambdaContext context;

		[SetUp]
	    public void SetUp()
	    {
		    request = new Request("origin-bucket", "origin-resource", "destination-bucket", "destination-resource");

		    context = Substitute.For<ILambdaContext>();

			service = Substitute.For<IService>();

		    handler = new Handler(service);
	    }

		[Test]
		public async Task Handle_ShouldProcessRequest()
		{
			await handler.Handle(request, context);

			await service.Received().Process(request, Arg.Any<CancellationToken>());
		}
    }
}
