namespace LambdaS3FileZipper
{
    using LambdaS3FileZipper.Logging;

    public class HelloWorld
    {
        internal ILog Logger { get; set; }
        public string HelloText { get; set; } = "Hello";
    }
}