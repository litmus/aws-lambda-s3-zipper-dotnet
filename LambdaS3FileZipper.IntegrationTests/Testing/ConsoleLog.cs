using Serilog;

namespace LambdaS3FileZipper.IntegrationTests.Testing
{
	public static class ConsoleLog
	{
		/// <summary>
		/// Enables Serilog logging to the Console
		/// </summary>
		public static void EnableSerilog()
		{
			Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console().CreateLogger();
		}
	}
}
