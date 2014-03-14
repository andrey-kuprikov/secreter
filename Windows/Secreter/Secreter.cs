using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Secreter.Properties;

namespace Secreter
{
    public partial class Secreter : Form
    {
        KeyboardHook hook = new KeyboardHook();

        public Secreter()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            InitializeComponent();

            // register the event that is fired after the key press.
            hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey((uint)MyModifierKeys.Control | (uint)MyModifierKeys.Shift,
                /*Secreter.MyModifierKeys.Control | Secreter.MyModifierKeys.Alt,*/
                Keys.P);
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            // show the keys pressed in a label.
            lblKey.Text = e.Modifier.ToString() + " + " + e.Key.ToString();
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            var mgr = new Manager(Settings.Default.DataFileLocation);
            mgr.secretKey = "1234";

            var names = mgr.GetNames();
            mgr.Add("test name", "test_login", "test_pass");
            var p = mgr.Get("test name");
        }
    }
}
