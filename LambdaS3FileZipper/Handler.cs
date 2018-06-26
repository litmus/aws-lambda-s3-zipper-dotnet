using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
	public class Handler : IHandler
	{
		private readonly IFileRetriever fileRetriever;
		private readonly IFileZipper fileZipper;
		private readonly IFileUploader fileUploader;
		private readonly ILog log;

		public Handler(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
		{
			this.fileRetriever = fileRetriever;
			this.fileZipper = fileZipper;
			this.fileUploader = fileUploader;
			this.log = LogProvider.GetCurrentClassLogger();
		}

		public async Task<Response> Handle(Request request, ILambdaContext lambdaContext)
		{
            var sw = Stopwatch.StartNew();
            log.Trace("Received zip request @{Request} - {AwsRequestId}", request, lambdaContext.AwsRequestId);

			var directory = await fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName);
			log.Debug("Retrieved files from {Bucket}:{Resource}", request.OriginBucketName, request.OriginResourceName);

			var compressedFileName = await fileZipper.Compress(directory);
			log.Debug("Compressed file to {CompressedFileName}", compressedFileName);
			
      var url = await fileUploader.Upload(
				request.DestinationBucketName, request.DestinationResourceName, compressedFileName, CancellationToken.None);

            log.Debug("Uploaded file to {Url}", url);

            log.Trace("Completed zip request {AwsRequestId} in {ElapsedMilliseconds} ms", lambdaContext.AwsRequestId, sw.ElapsedMilliseconds);
			return new Response(url);
		}
	}
}
