#region Usings
using System.Windows;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public partial class MainWindow : Window
	{
		#region Variables
		private TrafficLightViewModel _TrafficLightViewModel;
		#endregion

		#region Constructors
		public MainWindow()
		{
			InitializeComponent();

			_TrafficLightViewModel = new TrafficLightViewModel();

			this.Closed += (sender, e) =>
			{
				if (_TrafficLightViewModel != null) _TrafficLightViewModel.Dispose();
			};

			this.DataContext = _TrafficLightViewModel;
		}
		#endregion
	}
}
