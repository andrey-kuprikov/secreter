using System;
using System.IO;
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
        KeyboardHook hook;
        NotifyIcon appIcon;
        ContextMenu trayMenu;

        public Secreter()
        {
            this.components = new System.ComponentModel.Container();

            InitManager();
            RegisterHook();

            InitializeComponent();
        }

        private void InitTrayMenu()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Manage passwords", OnManage);
            trayMenu.MenuItems.Add("Settings", OnSettings);
            trayMenu.MenuItems.Add("Exit", OnExit);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnSettings(object sender, EventArgs e)
        {
        }

        private void OnManage(object sender, EventArgs e)
        {
            var form = new AddPassword();
            form.Show();
        }

        private void InitAppIcon()
        {
            InitTrayMenu();

            appIcon = new NotifyIcon(this.components);
            appIcon.ContextMenu = trayMenu;
            appIcon.Visible = true;
            appIcon.Icon = new Icon(Path.Combine(Application.StartupPath, "Resources/lock.ico"));
        }

        private void InitManager()
        {
            Global.mgr = new Manager(Settings.Default.DataFileLocation);
            Global.mgr.secretKey = "1234";
        }

        private void RegisterHook()
        {
            hook = new KeyboardHook();

            // register the event that is fired after the key press.
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey((uint)MyModifierKeys.Control | (uint)MyModifierKeys.Shift, Keys.P);
        }

        private void ShowApp()
        {
            if (!this.Visible)
            {
                this.Location = new Point()
                {
                    X = Cursor.Position.X - 120,
                    Y = Cursor.Position.Y - 4
                };

                FillPasswordList();
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }
        
        private void HideApp()
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
            }
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            ShowApp();
        }

        private void FillPasswordList()
        {
            lstPasswords.Items.Clear();
            lstPasswords.Items.AddRange(Global.mgr.GetNames());
        }

        private void Secreter_Load(object sender, EventArgs e)
        {
            HideApp();
            ShowInTaskbar = true;
            InitAppIcon();
        }

        private void ShowNotify(string title, string message)
        {
            appIcon.ShowBalloonTip(1000, title, message, ToolTipIcon.Info);
        }

        private PasswordRecord GetPasswordObjectByMouseXY(int x, int y)
        {
            int index = lstPasswords.IndexFromPoint(x, y);
            if (index < 0) return null;

            var name = lstPasswords.Items[index].ToString();
            return Global.mgr.Get(name);
        }

        private void lstPasswords_MouseDown(object sender, MouseEventArgs e)
        {
            var pass = GetPasswordObjectByMouseXY(e.X, e.Y);
            var message = string.Empty;

            if (pass != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    message = "Логин скопирован в буфер обмена";
                    Clipboard.SetText(pass.login);
                }
                if (e.Button == MouseButtons.Right)
                {
                    message = "Пароль скопирован в буфер обмена";
                    Clipboard.SetText(pass.password);
                }
            }
            
            ShowNotify("Успешно", message);
            HideApp();
        }
    }
}
