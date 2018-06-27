using System;
using System.Collections.Generic;
using System.Text;
using CommandLineProcessor.CommandLine;

namespace CommandLineProcessor.ConsoleApp
{
	public enum Discraft
	{
		Buzzz,
		Meteor,
		Mantis
	}

	class Settings
	{
		//TODO Can also do enums


		public Settings(string[] commandLineArgs)
		{
			this.ParseCommandLineArgs(commandLineArgs);
			//TODO check for invalid arguments
		}

		[CommandLineArgument(Name = "-foo", Default = "foo_default")]
		public string Foo { get; set; }

		//TODO integer arguments
		//[CommandLineArgument(Name = "-bar", Default = -1)]
		//public int Bar { get; set; }

		[CommandLineArgument(Name = "-foobar", Default = false)]
		public bool FooBar { get; set; }

		[CommandLineArgument(Name = "-barfoo", Required = true)]
		public Discraft BarFoo { get; set; }
	}
}
