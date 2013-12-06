#region Usings
using System;
using System.Windows.Data;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public class TrafficLightStateToIsOnConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((value != null) && (value.GetType() == typeof(TrafficLightStates)) && ((TrafficLightStates)value != TrafficLightStates.Off));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
