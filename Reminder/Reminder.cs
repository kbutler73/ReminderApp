using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    public class Reminder : INotifyPropertyChanged
    {
        #region Fields

        private bool _active;
        private DateTime _reminderTime;
        private TimeSpan _interval;
        private string _message;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public bool Active
        {
            get => _active;
            set
            {
                _active = value
                    ;
                OnPropertyChanged(nameof(Active));
            }
        }

        public DateTime ReminderTime
        {
            get => _reminderTime;
            set
            {
                _reminderTime = value;
                OnPropertyChanged(nameof(ReminderTime));
            }
        }

        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }

        [Browsable(false)]
        public DateTime LastNotificationTime { get; set; }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        #endregion Properties

        #region Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }
}