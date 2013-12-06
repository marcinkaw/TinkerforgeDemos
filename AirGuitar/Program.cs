#region Usings
using System;
using Tinkerforge;
#endregion

namespace TinkerForgeProjects.AirGuitar
{
	public class Program
	{
		#region Fields
		// The host and the port to the master brick
		private static string _Host = "localhost";
		private static int _Port = 4223;
		// The UID of the Dinstance IR Bricklet
		private static string _BrickletDistanceIrUid = "beC";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			IPConnection ipConnection = new IPConnection();

			BrickletDistanceIR brickletDistanceIr = new BrickletDistanceIR(_BrickletDistanceIrUid, ipConnection);

			ipConnection.Connect(_Host, _Port);

			System.Console.WriteLine("Press any key to exit");

			do
			{
				// Play "sound" with frequency = distance
				Console.Beep((int)brickletDistanceIr.GetDistance(), 200);
			}
			while (!Console.KeyAvailable);

			ipConnection.Disconnect();
		}
		#endregion
	}
}
