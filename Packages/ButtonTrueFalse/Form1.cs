using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonTrueFalse
{
    public partial class Form1 : Form
    {
        public bool state = false, changed = false, changeDisplay = false;
        public DateTime lastChange;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PackageHost.RegisterStateObjectLinks(this);
            PackageHost.RegisterMessageCallbacks(this);
            PackageHost.DeclarePackageDescriptor();
            button1.Text = "Push Me";
            button1.BackColor = Color.FromKnownColor(KnownColor.Red);
            label1.Text = "Off";
            PackageHost.PushStateObject("isOn", false);
            PackageHost.PushStateObject("changeState", false);

            Task.Factory.StartNew(() =>
            {
                var lastTime = DateTime.MinValue;
                while (PackageHost.IsRunning)
                {
                    if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                    {
                        if (changed)
                        {
                            changed = false;
                            changeDisplay = true;
                            lastChange = DateTime.Now;
                            PackageHost.PushStateObject("changeState", true);
                        }
                        if (DateTime.Now.Subtract(lastChange).TotalMilliseconds > 500 && changeDisplay)
                        {
                            changeDisplay = false;
                            PackageHost.PushStateObject("changeState", false);

                        }

                        lastTime = DateTime.Now;
                    }
                    Thread.Sleep(1);
                }

            });

            this.Text = string.Format("IsRunning: {0} - IsConnected: {1} - IsStandAlone: {2}", PackageHost.IsRunning, PackageHost.IsConnected, PackageHost.IsStandAlone);
            PackageHost.WriteInfo("I'm running !");
        }




        private void button1_Click(object sender, EventArgs e)
        {
            if (state)
            {
                state = false;
                button1.BackColor = Color.FromKnownColor(KnownColor.Red);
                label1.Text = "Off";
                PackageHost.PushStateObject("isOn", state);
                changed = true;

            }
            else
            {
                state = true;
                button1.BackColor = Color.FromKnownColor(KnownColor.Green);
                label1.Text = "On";
                PackageHost.PushStateObject("isOn", state);
                changed = true;
            }
            PackageHost.PushStateObject("isOn", state);
        }

        [MessageCallback(Key = "set")]
        void set(bool set)
        {
            if (state)
            {
                if (!set)
                {
                    state = false;
                    button1.BackColor = Color.FromKnownColor(KnownColor.Red);
                    PackageHost.PushStateObject("isOn", state);
                    label1.Text = "Off";
                    changed = true;
                }

            }
            else
            {
                if (set)
                {
                    state = true;
                    button1.BackColor = Color.FromKnownColor(KnownColor.Green);
                    PackageHost.PushStateObject("isOn", state);
                    label1.Text = "On";
                    changed = true;
                }
            }

        }
    }
}

