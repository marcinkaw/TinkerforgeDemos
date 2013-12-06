#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Tinkerforge;
#endregion

namespace TinkerForgeProjects.CiSignalizer
{
	public class Program
	{
		#region Enums
		private enum TrafficLightStates
		{
			Off,
			Red,
			Yellow,
			Green,
		}
		#endregion

		#region Fields
		// The host and the port to the master brick
		private static string _Host = "localhost";
		private static int _Port = 4223;
		// The UID of the Dual Relay Bricklet
		private static string _BrickletDualRelayUid = "a7E";
		private static string _BrickletIO4Uid = "aej";
		// The jenkins xml api request filter
		//private static string _XmlApiRequest = "http://ci1:8181/api/xml?tree=jobs[lastBuild[building,result]]";
		private static string _XmlApiRequest = "http://ci1:8181/view/Test/api/xml?tree=jobs[lastBuild[building,result]]";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			IPConnection ipConnection = new IPConnection();

			BrickletDualRelay brickletDualRelay = new BrickletDualRelay(_BrickletDualRelayUid, ipConnection);

			BrickletIO4 brickletIO4 = new BrickletIO4(_BrickletIO4Uid, ipConnection);

			ipConnection.Connect(_Host, _Port);

			Console.WriteLine("Press any key to exit");

			do
			{
				TrafficLightStates trafficLightState = TrafficLightStates.Off;
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

						// Build server state: Success
						trafficLightState = TrafficLightStates.Green;
						if (lastBuildElements
							.Select(item => item.Element("building"))
							.Where(item => (item != null))
							.Select(item => item.Value)
							.Where(item => (!string.IsNullOrEmpty(item)))
							.Select(item => item.ToUpperInvariant())
							.Where(item => (item == "TRUE")).FirstOrDefault() != null)
						{
							// Build server state: Running
							trafficLightState = TrafficLightStates.Yellow;
						}
						else if (lastBuildElements
								.Select(item => item.Element("result"))
								.Where(item => (item != null))
								.Select(item => item.Value)
								.Where(item => (!string.IsNullOrEmpty(item)))
								.Select(item => item.ToUpperInvariant())
								.Where(item => (item == "FAILURE")).FirstOrDefault() != null)
						{
							// Build server state: Failure
							trafficLightState = TrafficLightStates.Red;
						}
					}
				}
				catch
				{
					// Ignore
				}

				System.Console.WriteLine(trafficLightState.ToString());
				if ((trafficLightState != TrafficLightStates.Off) && (ipConnection.GetConnectionState() == 1))
				{
					bool relay1 = false;
					bool relay2 = false;
					// Set dual realy bricklet
					MyGetRelayStatesFromTrafficLightState(trafficLightState, out relay1, out relay2);
					brickletDualRelay.SetState(relay1, relay2);
					// Set IO4 bricklet
					MySetBrickletIO4State(trafficLightState, brickletIO4);
				}

				

				Thread.Sleep(1000);
			}
			while (!Console.KeyAvailable);

			ipConnection.Disconnect();
		}

		#endregion

		#region My Methods
		private static void MySetBrickletIO4State(TrafficLightStates trafficLightState, BrickletIO4 brickletIO4)
		{
			switch (trafficLightState)
			{
				case TrafficLightStates.Off:
					brickletIO4.SetConfiguration(1 << 0, 'o', false);
					brickletIO4.SetConfiguration(1 << 1, 'o', false);
					brickletIO4.SetConfiguration(1 << 2, 'o', false);
					brickletIO4.SetConfiguration(1 << 3, 'o', false);
					break;
				case TrafficLightStates.Red:
					brickletIO4.SetConfiguration(1 << 0, 'o', true);
					brickletIO4.SetConfiguration(1 << 1, 'o', true);
					brickletIO4.SetConfiguration(1 << 2, 'o', false);
					brickletIO4.SetConfiguration(1 << 3, 'o', false);
					break;
				case TrafficLightStates.Yellow:
					brickletIO4.SetConfiguration(1 << 0, 'o', false);
					brickletIO4.SetConfiguration(1 << 1, 'o', false);
					brickletIO4.SetConfiguration(1 << 2, 'o', true);
					brickletIO4.SetConfiguration(1 << 3, 'o', false);
					break;
				case TrafficLightStates.Green:
					brickletIO4.SetConfiguration(1 << 0, 'o', false);
					brickletIO4.SetConfiguration(1 << 1, 'o', false);
					brickletIO4.SetConfiguration(1 << 2, 'o', false);
					brickletIO4.SetConfiguration(1 << 3, 'o', true);
					break;
				default:
					break;
			}
		}

		private static void MyGetRelayStatesFromTrafficLightState(TrafficLightStates trafficLightState, out bool relay1, out bool relay2)
		{
			// Yellow
			relay1 = false;
			relay2 = false;
			switch (trafficLightState)
			{
				case TrafficLightStates.Red:
					relay1 = true;
					relay2 = false;
					break;
				case TrafficLightStates.Green:
					relay1 = true;
					relay2 = true;
					break;
			}
		}
		#endregion
	}
}
