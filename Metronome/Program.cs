#region Usings
using System;
using System.Threading;
using Tinkerforge;
#endregion

namespace TinkerForgeProjects.Metronome
{
	public class Program
	{
		#region Fields
		// The host and the port to the master brick
        private static string _Host = "192.168.1.21";
		private static int _Port = 4223;
		// The UID of the Dinstance IR Bricklet
		private static string _BrickletDistanceIrUid = "beC";
		// The UID of the Dual Relay Bricklet
        private static string _BrickletDualRelayUid = "a7E";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			IPConnection ipConnection = new IPConnection();

			BrickletDistanceIR brickletDistanceIr = new BrickletDistanceIR(_BrickletDistanceIrUid, ipConnection);
			BrickletDualRelay brickletDualRelay = new BrickletDualRelay(_BrickletDualRelayUid, ipConnection);

			ipConnection.Connect(_Host, _Port);

			Console.WriteLine("Press any key to exit");

			int index = 0;
			do
			{
				int interval = (int)brickletDistanceIr.GetDistance();
				Thread.Sleep(interval);
				index++;
				if (index % 2 == 0)
				{
					brickletDualRelay.SetState(true, false);
				}
				else
				{
					brickletDualRelay.SetState(false, true);
				}
			}
			while (!Console.KeyAvailable);

			ipConnection.Disconnect();
		}
		#endregion
	}
}
