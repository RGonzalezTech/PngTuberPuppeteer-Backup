using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PngTuber.Pupper
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool isEnabled = true;
        private readonly Action<object> action;

        public DelegateCommand(Action<object> action)
        {
            this.action = action;
        }

        public DelegateCommand(Action action)
        {
            this.action = (_) => action.Invoke();
        }

        public DelegateCommand(Func<Task> action)
        {
            this.action = (_) => action.Invoke();
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.IsEnabled;
        }

        public void Execute(object parameter)
        {
            this.action?.Invoke(parameter);
        }
    }
}
