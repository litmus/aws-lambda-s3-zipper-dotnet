using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<Stream> CopyStreamOntoMemory(this Stream stream, CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, 4096, cancellationToken).ConfigureAwait(false);
            return memoryStream.Reset();
        }

        public static Stream Reset(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }
    }
}
