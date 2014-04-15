#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Tinkerforge;
using System.Text;
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
		private static string _Host = MySettings.Default.TinkerForgeIp;
		//private static string _Host = "localhost";
		private static int _Port = 4223;
		// The UID of the Dual Relay Bricklet
		private static string _BrickletDualRelayUid = "a7E";
		private static string _BrickleLedUid = "eAy";
		// The jenkins xml api request filter
		//private static string _XmlApiRequest = "http://ci1:8181/view/Test/api/xml?tree=jobs[name,lastBuild[building,result]]";
		private static string _XmlApiRequest = MySettings.Default.JenkinsURL;
		#endregion

		#region Main
		static void Main(string[] args)
		{
			bool WasSignaled = false;
			IPConnection ipConnection = new IPConnection();

			BrickletDualRelay brickletDualRelay = new BrickletDualRelay(_BrickletDualRelayUid, ipConnection);
			BrickletLCD20x4 brickledLed = new BrickletLCD20x4(_BrickleLedUid, ipConnection);

			try
			{
				ipConnection.Connect(_Host, _Port);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.ReadKey();
				return;
			}

			Console.WriteLine("Press any key to exit");

			do
			{
				TrafficLightStates trafficLightState = TrafficLightStates.Off;
				string[] ledTexts = new string[4];

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
							.Where(item => (item != null));

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

						int AllCount = lastBuildElements.Count();
						int IsBuldingCount = lastBuildElements
							.Select(item => item.Element("building"))
							.Where(item => (item != null))
							.Select(item => item.Value)
							.Where(item => (!string.IsNullOrEmpty(item)))
							.Select(item => item.ToUpperInvariant())
							.Where(item => (item == "TRUE")).Count();
						int FailuteCount = lastBuildElements
							.Select(item => item.Element("result"))
							.Where(item => (item != null))
							.Select(item => item.Value)
							.Where(item => (!string.IsNullOrEmpty(item)))
							.Select(item => item.ToUpperInvariant())
							.Where(item => (item == "FAILURE")).Count();
						int SuccessCount = lastBuildElements
							.Select(item => item.Element("result"))
							.Where(item => (item != null))
							.Select(item => item.Value)
							.Where(item => (!string.IsNullOrEmpty(item)))
							.Select(item => item.ToUpperInvariant())
							.Where(item => (item == "SUCCESS")).Count();

						ledTexts[0] = "Jobs: " + AllCount.ToString();
						ledTexts[1] = "Failures: " + FailuteCount.ToString();
						ledTexts[2] = "Succeeded: " + SuccessCount.ToString();
						ledTexts[3] = "Building: " + IsBuldingCount.ToString();

						
					}
				}
				catch
				{
					continue;
					// Ignore
				}

				System.Console.WriteLine(trafficLightState.ToString());


				if ((trafficLightState != TrafficLightStates.Off) && (ipConnection.GetConnectionState() == 1))
				{
					//bool relay1 = false;
					//bool relay2 = false;
					//MyGetRelayStatesFromTrafficLightState(trafficLightState, out relay1, out relay2);
					//brickletDualRelay.SetState(relay1, relay2);


					if (trafficLightState == TrafficLightStates.Red)
					{
						
						if (!WasSignaled)
						{
							Console.WriteLine("turnOn");
							brickletDualRelay.SetState(true, true);
							
							Thread.Sleep(10000);
							Console.WriteLine("turnOff");
							brickletDualRelay.SetState(false, false);
						}
						WasSignaled = true;
					}
					else
					{
						WasSignaled = false;
						Console.WriteLine("turnOff");
						brickletDualRelay.SetState(false, false);
					}
				}

				if (ipConnection.GetConnectionState() == 1 && ledTexts != null)
				{
						brickledLed.ClearDisplay();
						brickledLed.WriteLine(0, 0, ledTexts[0]);
						brickledLed.WriteLine(1, 0, ledTexts[1]);
						brickledLed.WriteLine(2, 0, ledTexts[2]);
						brickledLed.WriteLine(3, 0, ledTexts[3]);
						if (trafficLightState == TrafficLightStates.Red)
						{
							if (brickledLed.IsBacklightOn())
							{
								brickledLed.BacklightOff();
							}
							else
							{
								brickledLed.BacklightOn();
							}
						}
						else
						{
							brickledLed.BacklightOff();
						}
				}

				Thread.Sleep(1000);
			}
			while (!Console.KeyAvailable);

			ipConnection.Disconnect();
		}
		#endregion

		#region My Methods
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
