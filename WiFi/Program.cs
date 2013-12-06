using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiFi
{
	class Program
	{
		private static string HOST = "192.168.1.9"; // WiFi Port
		private static int PORT = 4223;
		private static string distanceIrUID = "bd9"; // Your bricklet UID
		private static string dualRelayUID = "a4y"; // Your bricklet UID

		static void Main(string[] args)
		{
			Tinkerforge.IPConnection ipcon = new Tinkerforge.IPConnection(); // Create IP connection
			Tinkerforge.BrickletDistanceIR distanceIr = new Tinkerforge.BrickletDistanceIR(distanceIrUID, ipcon);
			Tinkerforge.BrickletDualRelay dr = new Tinkerforge.BrickletDualRelay(dualRelayUID, ipcon); // Create device object

			ipcon.Connect(HOST, PORT); // Connect to brickd
			// Don't use device before ipcon is connected

			System.Console.WriteLine("Press key to exit");
			int i = 0;
			do
			{
				int interval = (int)distanceIr.GetDistance();
				System.Threading.Thread.Sleep(interval);
				i++;
				if (i % 2 == 0)
				{
					dr.SetState(true, false);
				}
				else
				{
					dr.SetState(false, true);
				}
			}
			while (!Console.KeyAvailable);

			dr.SetState(false, false);

			//// Turn relays alternating on/off for 10 times with 1 second delay
			//for (int i = 0; i < 10; i++)
			//{
			//    System.Threading.Thread.Sleep(1000);
			//    if (i % 2 == 0)
			//    {
			//        dr.SetState(true, false);
			//    }
			//    else
			//    {
			//        dr.SetState(false, true);
			//    }
			//}

			//System.Console.WriteLine("Press key to exit");
			//System.Console.ReadKey();

			ipcon.Disconnect();
		}
	}
}
