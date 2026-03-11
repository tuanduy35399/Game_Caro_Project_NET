using System;
using System.Windows.Forms;
using GameCaro.Models;

namespace GameCaro
{
    public partial class LoginForm : Form
    {
        private readonly MongoService _mongoService;

        public UserModel LoggedInUser { get; private set; }

        public LoginForm(MongoService mongoService)
        {
            _mongoService = mongoService;
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            ToggleUi(false);

            try
            {
                var result = await _mongoService.AuthenticateAsync(txtUsername.Text.Trim(), txtPassword.Text);
                if (!result.Success)
                {
                    lblStatus.Text = result.ErrorMessage;
                    return;
                }

                LoggedInUser = result.User;
                DialogResult = DialogResult.OK;
                Close();
            }
            finally
            {
                ToggleUi(true);
            }
        }

        private void btnOpenRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm(_mongoService))
            {
                registerForm.ShowDialog(this);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ToggleUi(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnOpenRegister.Enabled = enabled;
            btnExit.Enabled = enabled;
        }
    }
}