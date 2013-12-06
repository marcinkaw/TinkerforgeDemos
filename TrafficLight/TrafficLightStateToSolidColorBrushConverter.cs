#region Usings
using System;
using System.Windows.Data;
using System.Windows.Media;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public class TrafficLightStateToSolidColorBrushConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			if ((value != null) && (value.GetType() == typeof(TrafficLightStates)))
			{
				TrafficLightStates trafficLightStates = (TrafficLightStates)value;

				switch (trafficLightStates)
				{
					case TrafficLightStates.Red:
						solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
						break;
					case TrafficLightStates.Yellow:
						solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
						break;
					case TrafficLightStates.Green:
						solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
						break;
				}
			}
			return solidColorBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
