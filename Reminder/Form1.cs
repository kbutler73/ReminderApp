using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Reminder
{
    public partial class Form1 : Form
    {
        #region Fields

        private BindingList<Reminder> Reminders;
        private System.Timers.Timer _timer;

        #endregion Fields

        #region Constructors

        public Form1()
        {
            InitializeComponent();

            Reminders = new BindingList<Reminder>();
            Reminders.Add(new Reminder { Active = true, ReminderTime = DateTime.Now.AddSeconds(1), Message = "Remind me" });

            dgvReminders.DataSource = Reminders;

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        #endregion Constructors

        #region Methods

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var reminder in Reminders)
            {
                if (!reminder.Active) continue;

                bool trigger = false;
                if (DateTime.Now >= reminder.ReminderTime)
                {
                    if (reminder.Interval > new TimeSpan(0))
                    {
                        if (DateTime.Now > reminder.LastNotificationTime.Add(reminder.Interval))
                        {
                            trigger = true;
                        }
                    }
                    else
                    {
                        trigger = true;
                    }
                }

                if (trigger)
                {
                    notifyIconReminder.BalloonTipText = reminder.Message;
                    notifyIconReminder.ShowBalloonTip(0);
                    if (reminder.Interval == new TimeSpan(0))
                    {
                        reminder.Active = false;
                    }
                    reminder.LastNotificationTime = DateTime.Now;
                }
            }
            dgvReminders.DataSource = Reminders;
        }

        private void btnAddReminder_Click(object sender, EventArgs e)
        {
            Reminders.Add(new Reminder { Active = false, ReminderTime = DateTime.Now.AddSeconds(1), Message = "Remind me" });
            dgvReminders.DataSource = null;
            dgvReminders.DataSource = Reminders;
            dgvReminders.Refresh();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIconReminder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        #endregion Methods
    }
}