using System;
using System.IO;

namespace LambdaS3FileZipper.Models
{
    public class FileResponse : IDisposable
    {
        public FileResponse(string resourceKey, Stream contentStream)
        {
            ResourceKey = resourceKey;
            ContentStream = contentStream;
        }

        public string ResourceKey { get; set; }
        public Stream ContentStream { get; set; }

        public void Dispose()
        {
            ContentStream?.Dispose();
        }
    }
}
