using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineProcessor.Parameters
{
	/// <summary>
	/// Attribute to be placed on property that is set by a parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigParameterAttribute : Attribute {
		/// <summary>
		/// Constructor with parameter name.
		/// </summary>
		public ConfigParameterAttribute() {
		}

		/// <summary>
		/// Parameter name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Default parameter value
		/// </summary>
		public string DefaultValue { get; set; }
	}

}
