using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineProcessor.CommandLine
{
	/// <summary>
	/// Command line attribute for property
	/// </summary>
	public class CommandLineArgumentAttribute : Attribute {

		/// <summary>
		/// Argument name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Argument is required
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Number of parameters which follow
		/// </summary>
		public int NumberOfParameters { get; set; }

		/// <summary>
		/// Default value
		/// </summary>
		public object Default { get; set; }
	}
}
