using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace CommandLineProcessor.Parameters
{
		/// <summary>
	/// Methods used to set parameters on fields
	/// </summary>
	public static class ConfigParameterSetter {

		/// <summary>
		/// Set properties o object with ConfigParameter to their default values.
		/// </summary>
		/// <param name="o">Object on which to set parameters</param>
		public static void SetParameters(this object o) {
			SetParameters(o, Enumerable.Empty<Tuple<string, string>>());
		}

		/// <summary>
		/// Set properties on object with the ConfigParameter attribute from a set of
		/// command line parameters of the form: [ParameterName]=[ParameterValue]
		/// </summary>
		/// <param name="o">Object on which to set parameters</param>
		/// <param name="args">Command line args of parameter name / parameter value pairs</param>
		public static void SetParameters(this object o, string[] args) {
			try {
				// series of name=value command line parameters
				var commandLineParameters = (from string arg in args
																		 select arg.Split(new char[] { '=' })).Select(a => new Tuple<string, string>(a[0], a[1]));
				SetParameters(o, commandLineParameters);
			}
			catch (Exception ex) {
				throw new ArgumentException("Invalid command line parameter syntax", ex);
			}
		}

		/// <summary>
		/// Set properties on object with the ConfigParameter attribute from a set of
		/// XML element.  Element name is parameter name, element contents is parameter value.
		/// </summary>
		/// <param name="o">Object on which to set parameters</param>
		/// <param name="rootElement">XML root element with parameter elements</param>
		public static void SetParameters(this object o, XElement rootElement) {
			IEnumerable<Tuple<string, string>> parameters = from XElement e in rootElement.Elements()
											 select new Tuple<string, string>(e.Name.LocalName, e.Value);
			SetParameters(o, parameters);
		}

		/// <summary>
		/// Set properties on object with the ConfigParameter attribute from a set of
		/// parameter name / parameter value pairs.
		/// </summary>
		/// <param name="o">Object on which to set parameters</param>
		/// <param name="parameters">Set of parameter name / parameter value pairs</param>
		public static void SetParameters(this object o, IEnumerable<Tuple<string, string>> parameters) {
			Dictionary<string, string> parameterDict = parameters.ToDictionary(parmeter => parmeter.Item1, parmeter => parmeter.Item2);
			SetParameters(o, parameterDict);
		}

		/// <summary>
		/// Set properties on object with the ConfigParameter attribute from a dictionary
		/// of parameter values keyed on parameter name.
		/// </summary>
		/// <param name="o">Object on which to set parameters</param>
		/// <param name="parameters">Dictionary of parameter name / parameter value pairs</param>
		public static void SetParameters(this object o, Dictionary<string, string> parameters) {

			PropertyInfo[] configProperties = GetConfigProperties(o.GetType()).ToArray();

			foreach (PropertyInfo info in configProperties) {
				string name = GetConfigParameterName(info);
				string stringValue = parameters.ContainsKey(name) ? parameters[name] : GetConfigParameterDefault(info);
				if (stringValue != null) {
					object value = GetParameterValue(info, stringValue);
					if (value != null) {
						// if value found, set the property
						info.SetValue(o, value, null);
					}
				}
			}
		}

		/// <summary>
		/// Get a parameter's value
		/// </summary>
		/// <param name="propertyInfo">Property information</param>
		/// <param name="stringValue">Parameter value as string</param>
		/// <returns>Parameter object value</returns>
		private static object GetParameterValue(PropertyInfo propertyInfo, string stringValue) {
			Type propertyType = propertyInfo.PropertyType;

			object value = null;

			if (propertyType.IsEnum) {
				// parse enum
				try {
					value = Enum.Parse(propertyType, stringValue);
				}
				catch (ArgumentException ex) {
					// catch exception because TryParse() doesn't work on enum
				}
			}
			else if (propertyType == typeof(TimeSpan)) {
				// parse timespan
				TimeSpan timeSpan;
				bool valid = TimeSpan.TryParse(stringValue, out timeSpan);
				if (valid) {
					value = timeSpan;
				}
			}
			else {
				// value type...
				switch (Type.GetTypeCode(propertyType)) {
					case TypeCode.String:
						value = stringValue;
						break;

					case TypeCode.Int32:
						value = int.Parse(stringValue);
						break;

					case TypeCode.Double:
						value = double.Parse(stringValue);
						break;

					case TypeCode.Single:
						value = float.Parse(stringValue);
						break;

					case TypeCode.Boolean:
						value = bool.Parse(stringValue);
						break;
				}
			}
			return value;
		}

		/// <summary>
		/// Get information for properties which have the ConfigurationParameter
		/// attribute.
		/// </summary>
		/// <param name="type">Type to get ConfigurationParameter attributes for</param>
		/// <returns>List of type information</returns>
		private static IEnumerable<PropertyInfo> GetConfigProperties(Type type) {
			return type.GetProperties().Where(p => GetConfigParameterAttribute(p) != null);
		}

		/// <summary>
		/// Get the ConfigurationParameter attribute for a member
		/// </summary>
		/// <param name="info">Member info</param>
		/// <returns>
		/// ConfigurationParameter attribute for property
		/// </returns>
		private static ConfigParameterAttribute GetConfigParameterAttribute(PropertyInfo info) {
			return (ConfigParameterAttribute)info.GetCustomAttributes(
					typeof(ConfigParameterAttribute), false).FirstOrDefault();
		}

		/// <summary>
		/// Get configuartion parameter name from MemberInfo
		/// </summary>
		/// <param name="info">Member info</param>
		/// <returns>Name of configuration parameter</returns>
		private static string GetConfigParameterName(MemberInfo info) {
			ConfigParameterAttribute attr = (ConfigParameterAttribute)info.GetCustomAttributes(
					typeof(ConfigParameterAttribute), false).First();

			return attr.Name ?? info.Name;
		}

		/// <summary>
		/// Get configuartion parameter default value from MemberInfo
		/// </summary>
		/// <param name="info">Member info</param>
		/// <returns>Name of configuration parameter</returns>
		private static string GetConfigParameterDefault(MemberInfo info) {
			ConfigParameterAttribute attr = (ConfigParameterAttribute)info.GetCustomAttributes(
					typeof(ConfigParameterAttribute), false).First();

			return attr.DefaultValue;
		}

	}
}
