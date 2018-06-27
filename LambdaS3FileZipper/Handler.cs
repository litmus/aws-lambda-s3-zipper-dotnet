using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
	public class Handler : IHandler
	{
		private readonly ILog log = LogProvider.GetCurrentClassLogger();

		private readonly IService service;

		public Handler()
		{
			service = ServiceFactory.BuildDefault();
		}

		public Handler(IService service)
		{
			this.service = service;
		}

		public async Task<Response> Handle(Request request, ILambdaContext context)
		{
			var stopwatch = Stopwatch.StartNew();
			log.Trace("Received zip request {AwsRequestId}, @{Request}", context.AwsRequestId, request);

			var response = await service.Process(request);

			log.Trace("Completed zip request {AwsRequestId} in {ElapsedMilliseconds}ms", 
				context.AwsRequestId, stopwatch.ElapsedMilliseconds);
			return response;
		}
	}
}
