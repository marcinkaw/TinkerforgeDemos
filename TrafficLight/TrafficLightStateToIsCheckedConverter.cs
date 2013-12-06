#region Usings
using System;
using System.Windows.Data;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public class TrafficLightStateToIsCheckedConverter : IValueConverter
	{
		#region Constants
		private const string CnstParameterValueRed = "Red";
		private const string CnstParameterValueYellow = "Yellow";
		private const string CnstParameterValueGreen = "Green";
		#endregion

		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool? isChecked = null;
			if ((value != null) && (value.GetType() == typeof(TrafficLightStates)) && (parameter != null) && (parameter.GetType() == typeof(string)))
			{
				TrafficLightStates trafficLightStates = (TrafficLightStates)value;
				string parameterValue = (string)parameter;

				isChecked = false;
				switch (trafficLightStates)
				{
					case TrafficLightStates.Off:
						isChecked = null;
						break;
					case TrafficLightStates.Red:
						isChecked = (parameterValue == CnstParameterValueRed);
						break;
					case TrafficLightStates.Yellow:
						isChecked = (parameterValue == CnstParameterValueYellow);
						break;
					case TrafficLightStates.Green:
						isChecked = (parameterValue == CnstParameterValueGreen);
						break;
				}
			}
			return isChecked;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TrafficLightStates trafficLightState = TrafficLightStates.Off;
			if ((value != null) && (value.GetType() == typeof(bool)) && (parameter != null) && (parameter.GetType() == typeof(string)))
			{
				bool isChecked = (bool)value;
				string parameterValue = (string)parameter;

				if (parameterValue == CnstParameterValueRed)
				{
					trafficLightState = TrafficLightStates.Red;
				}
				else if (parameterValue == CnstParameterValueYellow)
				{
					trafficLightState = TrafficLightStates.Yellow;
				}
				else if (parameterValue == CnstParameterValueGreen)
				{
					trafficLightState = TrafficLightStates.Green;
				}
			}
			return trafficLightState;
		}
		#endregion
	}
}
