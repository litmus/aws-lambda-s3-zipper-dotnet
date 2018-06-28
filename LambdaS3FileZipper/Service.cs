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

	    public async Task<Response> Process(Request request, CancellationToken cancellationToken = default(CancellationToken))
	    {
		    var directory = await fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName, cancellationToken);
		    log.Debug("Retrieved files from {Bucket}:{Resource}", request.OriginBucketName, request.OriginResourceName);

		    var compressedFileName = await fileZipper.Compress(directory);
		    log.Debug("Compressed file to {CompressedFileName}", compressedFileName);

		    var url = await fileUploader.Upload(
			    request.DestinationBucketName, request.DestinationResourceName, compressedFileName, cancellationToken);
		    log.Debug("Uploaded file to {Url}", url);

		    return new Response(url);
	    }
	}
}
