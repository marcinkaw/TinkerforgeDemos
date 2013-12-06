#region Usings
using System;
using System.Windows.Input;
#endregion

namespace TinkerForgeProjects.TrafficLight
{
	public class DelegateCommand : ICommand
	{
		#region Variables
		private readonly Predicate<object> _CanExecute;
		private readonly Action<object> _Execute;
		#endregion

		#region Events
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		#endregion

		#region Constructors
		public DelegateCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
		{
			_Execute = execute;
			_CanExecute = canExecute;
		}
		#endregion

		#region Methods
		public bool CanExecute(object parameter)
		{
			if (_CanExecute == null)
			{
				return true;
			}

			return _CanExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_Execute(parameter);
		}
		#endregion
	}
}
