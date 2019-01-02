using Amazon.S3;
using LambdaS3FileZipper.Aws;

namespace LambdaS3FileZipper
{
    public class ServiceFactory
    {
        public static Service BuildDefault() => Build(forcePathStyleOnAwsS3: true);

        public static Service Build(bool forcePathStyleOnAwsS3)
        {
            var amazonS3Config = new AmazonS3Config {ForcePathStyle = forcePathStyleOnAwsS3};
            var amazonS3Client = new AmazonS3Client(amazonS3Config);
            var awsS3Client = new AwsS3Client(amazonS3Client);

            var retriever = new S3FileRetriever(awsS3Client);
            var zipper = new FileZipper();
            var uploader = new S3FileUploader(awsS3Client);

            var service = new Service(retriever, zipper, uploader);
            return service;
        }
    }
}
