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
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Reminder
{
    public partial class Form1 : Form
    {
        #region Fields

        private const string dataFile = "Reminders.json";
        private const string keyName = "Reminder";

        private BindingList<Reminder> Reminders;
        private System.Timers.Timer _timer;

        #endregion Fields

        #region Constructors

        public Form1()
        {
            InitializeComponent();

            CheckRegistryKey();

            Reminders = new BindingList<Reminder>();
            Reminders.ListChanged += Reminders_ListChanged;
            LoadReminders();

            dgvReminders.DataSource = Reminders;

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            WindowState = FormWindowState.Minimized;
        }

        #endregion Constructors

        #region Methods

        private void Reminders_ListChanged(object sender, ListChangedEventArgs e)
        {
            SaveReminders();
        }

        private void LoadReminders()
        {
            if (File.Exists(dataFile))
            {
                Reminders = JsonConvert.DeserializeObject<BindingList<Reminder>>(File.ReadAllText(dataFile));
            }
        }

        private void SaveReminders()
        {
            dgvReminders.DataSource = Reminders;
            var json = JsonConvert.SerializeObject(Reminders);
            File.WriteAllText(dataFile, json);
        }

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
            SaveReminders();
        }

        private void btnAddReminder_Click(object sender, EventArgs e)
        {
            Reminders.Add(new Reminder { Active = false, ReminderTime = DateTime.Now.AddSeconds(1), Message = "Remind me" });
            SaveReminders();
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

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            if (key.GetValue(keyName) != null)
            {
                key.DeleteValue(keyName, false);
                startWithWindowsToolStripMenuItem.Checked = false;
            }
            else
            {
                key.SetValue(keyName, Application.ExecutablePath.ToString());
                startWithWindowsToolStripMenuItem.Checked = true;
            }
        }

        private void CheckRegistryKey()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            if (key.GetValue(keyName) != null)
            {
                startWithWindowsToolStripMenuItem.Checked = true;
            }
            else
            {
                startWithWindowsToolStripMenuItem.Checked = false;
            }
        }

        private void dgvReminders_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            //if (dgvReminders.IsCurrentCellDirty)
            //{
            //    dgvReminders.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //}
        }

        #endregion Methods
    }
}