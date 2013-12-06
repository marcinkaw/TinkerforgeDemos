#region Usings
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Tinkerforge;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public class TrafficLightViewModel : INotifyPropertyChanged, IDisposable
	{
		#region Constants
		// The host and the port to the master brick
        private const string CnstHost = "192.168.1.21";
		private const int CnstPort = 4223;
		// The UID of the Dual Relay Bricklet
		private const string CnstBrickletDualRelayUID = "a7E";
		#endregion

		#region Variables
		private readonly DelegateCommand _ConnectCommand;
		private readonly DelegateCommand _DisconnectCommand;

		private readonly IPConnection _IpConnection;
		private readonly BrickletDualRelay _BrickletDualRelay;

		private TrafficLightStates _TrafficLightState = TrafficLightStates.Off;
		#endregion

		#region INotifyPropertyChanged Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Event Wrappers
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		#region Constructors
		public TrafficLightViewModel()
		{
			_ConnectCommand = new DelegateCommand(MyExecuteConnectCommand, MyCanExecuteConnectCommand);
			_DisconnectCommand = new DelegateCommand(MyExecuteDisconnectCommand, MyCanExecuteDisconnectCommand);

			_IpConnection = new IPConnection();
			_BrickletDualRelay = new BrickletDualRelay(CnstBrickletDualRelayUID, _IpConnection);

			_IpConnection.Connected += new IPConnection.ConnectedEventHandler(IpConnection_Connected);
			_IpConnection.Disconnected += new IPConnection.DisconnectedEventHandler(IpConnection_Disconnected);
		}
		#endregion

		#region IDisposable Methods
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				MyExecuteDisconnectCommand(null);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion


		#region Event Handlers
		private void IpConnection_Connected(IPConnection sender, short connectReason)
		{
			try
			{
				bool relay1 = false;
				bool relay2 = false;
				_BrickletDualRelay.GetState(out relay1, out relay2);
				this.TrafficLightState = MyGetTrafficLightStateFromRelayStates(relay1, relay2);
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occured while requesting the state of the dual relay bricklet. " + ex.Message);
				MyExecuteDisconnectCommand(null);
			}
		}

		private void IpConnection_Disconnected(IPConnection sender, short disconnectReason)
		{
			this.TrafficLightState = TrafficLightStates.Off;
		}
		#endregion

		#region Methods
		public void Connect()
		{
			MyExecuteConnectCommand(null);
		}

		public void Disconnect()
		{
			MyExecuteDisconnectCommand(null);
		}
		#endregion

		#region Properties
		public ICommand ConnectCommand { get { return (ICommand)_ConnectCommand; } }
		public ICommand DisconnectCommand { get { return (ICommand)_DisconnectCommand; } }

		public TrafficLightStates TrafficLightState
		{
			get { return _TrafficLightState; }
			set 
			{
				if (_TrafficLightState != value)
				{
					_TrafficLightState = value;
					this.OnPropertyChanged("TrafficLightState");

					if ((_TrafficLightState != TrafficLightStates.Off) && (_IpConnection != null) && (_IpConnection.GetConnectionState() == 1))
					{
						bool relay1 = false;
						bool relay2 = false;
						MyGetRelayStatesFromTrafficLightState(_TrafficLightState, out relay1, out relay2);
						_BrickletDualRelay.SetState(relay1, relay2);
					}
				}
			}
		}
		#endregion

		#region My Methods
		private bool MyCanExecuteConnectCommand(object parameter)
		{
			// Connection state:
			// 0 = No connection is established
			// 1 = A connection is established
			// 2 = Is trying to connect
			return ((_IpConnection != null) && (_IpConnection.GetConnectionState() == 0));
		}

		private void MyExecuteConnectCommand(object parameter)
		{
			if (MyCanExecuteConnectCommand(parameter))
			{
				try
				{
					_IpConnection.Connect(CnstHost, CnstPort);
				}
				catch (Exception ex)
				{
					MessageBox.Show("A connection could not be established. " + ex.Message);
				}
			}
		}

		private bool MyCanExecuteDisconnectCommand(object parameter)
		{
			// Connection state:
			// 0 = No connection is established
			// 1 = A connection is established
			// 2 = Is trying to connect
			return ((_IpConnection != null) && (_IpConnection.GetConnectionState() != 0));
		}

		private void MyExecuteDisconnectCommand(object parameter)
		{
			if (MyCanExecuteDisconnectCommand(parameter))
			{
				while (_IpConnection.GetConnectionState() == 2)
				{
				}

				if (_IpConnection.GetConnectionState() != 0) _IpConnection.Disconnect();
			}
		}

		// relay1 = false > B1
		// relay1 = true  > A1

		// relay2 = false > B2
		// relay2 = true  > A2

		// B1, B2 = Yellow
		// A1, B2 = Red
		// A1, A2 = Green
		// B1, A2 = Green

		private TrafficLightStates MyGetTrafficLightStateFromRelayStates(bool relay1, bool relay2)
		{
			TrafficLightStates trafficLightState = TrafficLightStates.Green;
			if (!relay1 && !relay2)
			{
				trafficLightState = TrafficLightStates.Yellow;
			}
			else if (relay1 && !relay2)
			{
				trafficLightState = TrafficLightStates.Red;
			}
			return trafficLightState;
		}

		private void MyGetRelayStatesFromTrafficLightState(TrafficLightStates trafficLightState, out bool relay1, out bool relay2)
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
