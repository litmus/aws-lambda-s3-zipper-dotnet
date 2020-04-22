using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
    public class Service : IService
    {
	    private readonly IFileRetriever fileRetriever;
	    private readonly IFileZipper fileZipper;
	    private readonly IFileUploader fileUploader;
	    private readonly ILog log;

	    public Service(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
	    {
		    this.fileRetriever = fileRetriever;
		    this.fileZipper = fileZipper;
		    this.fileUploader = fileUploader;

		    this.log = LogProvider.GetCurrentClassLogger();
	    }

	    public async Task<Response> Process(Request request, CancellationToken cancellationToken = default)
	    {
		    var stopwatch = Stopwatch.StartNew();

		    var files = await fileRetriever.RetrieveToMemory(request.OriginBucketName, request.OriginResourceName, request.OriginResourceExpressionPattern, cancellationToken);
		    log.Debug("Retrieved files from {Bucket}:{Resource} | {Time}ms", request.OriginBucketName, request.OriginResourceName, stopwatch.ElapsedMilliseconds);

			stopwatch.Restart();
		    var compressedFile = await fileZipper.Compress(request.DestinationResourceName, files, cancellationToken);
		    log.Debug("Compressed files to {CompressedFileName} | {Time}ms", compressedFile.ResourceKey, stopwatch.ElapsedMilliseconds);

			stopwatch.Restart();
		    var url = await fileUploader.Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFile, cancellationToken);
		    log.Debug("Uploaded file to {Url} | {Time}ms", url, stopwatch.ElapsedMilliseconds);

		    return new Response(url);
	    }
	}
}
