using System;

namespace CommandLineProcessor.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var settings = new Settings(args);
				Console.WriteLine("Foo: {0}", settings.Foo);
				//Console.WriteLine("Bar: {0}", settings.Bar);
				Console.WriteLine("FooBar: {0}", settings.FooBar);
				Console.WriteLine("BarFoo: {0}", settings.BarFoo);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				Console.WriteLine("<CR> to exit...");
				Console.ReadLine();
			}

		}
	}
}
