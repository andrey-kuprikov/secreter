using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Secreter
{
    public partial class AddPassword : Form
    {
        const string txtNameCaption = "Record name";
        const string txtLoginCaption = "Login";
        const string txtPasswordCaption = "Password";

        public AddPassword()
        {
            InitializeComponent();
        }

        private void SetupCaption(TextBox control, string caption)
        {
            if (control.Text == string.Empty)
            {
                control.Text = caption;
            }
        }
        private void RemoveCaption(TextBox control, string caption)
        {
            if (control.Text == caption)
            {
                control.Text = string.Empty;
            }
        }

        private void txtName_Enter(object sender, EventArgs e)
        {
            RemoveCaption(txtName, txtNameCaption);
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            SetupCaption(txtName, txtNameCaption);
        }

        private void txtLogin_Enter(object sender, EventArgs e)
        {
            RemoveCaption(txtLogin, txtLoginCaption);
        }

        private void txtLogin_Leave(object sender, EventArgs e)
        {
            SetupCaption(txtLogin, txtLoginCaption);
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            RemoveCaption(txtPassword, txtPasswordCaption);
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            SetupCaption(txtPassword, txtPasswordCaption);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Global.mgr.Add(txtName.Text, txtLogin.Text, txtPassword.Text);
        }

        private void AddPassword_Load(object sender, EventArgs e)
        {
            SetupCaption(txtName, txtNameCaption);
            SetupCaption(txtLogin, txtLoginCaption);
            SetupCaption(txtPassword, txtPasswordCaption);
        }
    }
}
