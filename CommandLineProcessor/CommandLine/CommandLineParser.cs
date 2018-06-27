using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLineProcessor.CommandLine
{

	/// <summary>
	/// Command line parser
	/// </summary>
	public static class CommandLineParser
	{
		/// <summary>
		/// Parse command line arguments for a target object
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="args">Command line arguments</param>
		public static void ParseCommandLineArgs(this object target, string[] args)
		{

			foreach (PropertyInfo property in CommandArgumentProperties(target))
			{
				CommandLineArgumentAttribute arg = GetCommandLineArgumentAttribute(property);
				int argIndex = GetIndexOfArg(args, arg.Name);
				if (argIndex < 0)
				{
					if (arg.Required)
					{
						throw new ArgumentException("Required argument missing", arg.Name);
					}

					property.SetValue(target, arg.Default, null);
				}
				else
				{
					switch (Type.GetTypeCode(property.PropertyType))
					{
						case TypeCode.Boolean:
							// boolean
							property.SetValue(target, argIndex >= 0, null);
							break;

						case TypeCode.String:
							// string
							string argValue = GetStringFromArgs(args, ref argIndex);
							property.SetValue(target, argValue, null);
							break;

						default:
							if (property.PropertyType.IsEnum)
							{
								// enum
								string enumValueStr = GetStringFromArgs(args, ref argIndex);
								object enumValue = Enum.Parse(property.PropertyType, enumValueStr);
								property.SetValue(target, enumValue, null);
							}
							else
							{
								throw new ArgumentException("Unsupported option type.");
							}

							break;
					}
				}
			}
		}

		/// <summary>
		/// Get the index of a command line argument using case-insensitive matching
		/// </summary>
		/// <param name="args">Command line arguments</param>
		/// <param name="argument">Argument to search</param>
		/// <returns>Index of argument (-1 if not found)</returns>
		private static int GetIndexOfArg(string[] args, string argument)
		{
			int index = -1;
			for (int n = 0; n < args.Length; n++)
			{
				if (String.Equals(argument, args[n], StringComparison.OrdinalIgnoreCase))
				{
					index = n;
					break;
				}
			}

			return index;
		}

		/// <summary>
		/// Get string after active argument
		/// </summary>
		/// <param name="args">Arguments</param>
		/// <param name="n">Current index offset into arguments</param>
		/// <returns>Name following argument</returns>
		private static string GetStringFromArgs(string[] args, ref int n)
		{
			string argType = args[n];
			string name = null;
			if (++n < args.Length)
			{
				name = args[n];
			}
			else
			{
				throw new ArgumentException(string.Format("{0} has no argument", argType));
			}

			return name;
		}

		/// <summary>
		/// Get properties with CommandLineArgument attribute
		/// </summary>
		/// <param name="o">Object to check</param>
		/// <returns>List of properties with CommandLineArgument attribute</returns>
		private static IEnumerable<PropertyInfo> CommandArgumentProperties(object o)
		{
			return o.GetType().GetProperties().Where(p => GetCommandLineArgumentAttribute(p) != null);
		}

		private static CommandLineArgumentAttribute GetCommandLineArgumentAttribute(PropertyInfo info)
		{
			return (CommandLineArgumentAttribute)info.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false).FirstOrDefault();
		}
	}
}
