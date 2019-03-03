using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    public class Reminder
    {
        #region Properties

        public bool Active { get; set; }
        public DateTime ReminderTime { get; set; }
        public TimeSpan Interval { get; set; }

        [Browsable(false)]
        public DateTime LastNotificationTime { get; set; }

        public string Message { get; set; }

        #endregion Properties
    }
}