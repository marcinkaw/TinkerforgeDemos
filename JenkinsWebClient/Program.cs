#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
#endregion

namespace TinkerForgeProjects.JenkinsWebClient
{
	public class Program
	{
		#region Enums
		private enum BuildStates
		{
			Unknown,
			Success,
			Running,
			Failure,
		}
		#endregion

		#region Fields
		// The jenkins xml api request filter
		private static string _XmlApiRequest = "http://localhost:8080/api/xml?tree=jobs[lastBuild[building,result]]";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			Console.WriteLine("Press any key to exit");

			do
			{
				BuildStates buildState = BuildStates.Unknown;
				try
				{
					using (WebClient webClient = new WebClient())
					{
						string downloadedString = webClient.DownloadString(_XmlApiRequest);
						XElement rootElement = XElement.Parse(downloadedString);
						// Example: <hudson><job><name>Proj1</name><lastBuild><building>false</building><result>FAILURE</result></lastBuild></job></hudson>
						IEnumerable<XElement> lastBuildElements = rootElement
							.Elements("job")
							.Select(item => item.Element("lastBuild"))
							.Where(item => (item != null)); ;

						buildState = BuildStates.Success;
						if (lastBuildElements
							.Select(item => item.Element("building"))
							.Where(item => (item != null))
							.Select(item => item.Value)
							.Where(item => (!string.IsNullOrEmpty(item)))
							.Select(item => item.ToUpperInvariant())
							.Where(item => (item == "TRUE")).FirstOrDefault() != null)
						{
							buildState = BuildStates.Running;
						}
						else if (lastBuildElements
								.Select(item => item.Element("result"))
								.Where(item => (item != null))
								.Select(item => item.Value)
								.Where(item => (!string.IsNullOrEmpty(item)))
								.Select(item => item.ToUpperInvariant())
								.Where(item => (item == "FAILURE")).FirstOrDefault() != null)
						{
								buildState = BuildStates.Failure;
						}
					}
				}
				catch
				{
					// Ignore
				}
				Console.WriteLine(buildState.ToString());
				Thread.Sleep(1000);
			}
			while (!Console.KeyAvailable);
		}
		#endregion
	}
}
