using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    // LoginForm placed in its own file (must be the first class in this file if designer is used)
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblStatus;
        private readonly MongoService _mongo;

        public string LoggedInUsername { get; private set; }

        public LoginForm(MongoService mongo)
        {
            _mongo = mongo ?? throw new ArgumentNullException(nameof(mongo));
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Login - GameCaro";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(320, 180);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblUser = new Label { Text = "Username:", Location = new Point(12, 15), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(90, 12), Width = 200 };

            var lblPass = new Label { Text = "Password:", Location = new Point(12, 50), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(90, 47), Width = 200, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Login", Location = new Point(90, 85), Width = 95 };
            btnRegister = new Button { Text = "Register", Location = new Point(195, 85), Width = 95 };

            lblStatus = new Label { Text = "", Location = new Point(12, 125), ForeColor = Color.Red, AutoSize = true };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(lblStatus);

            this.AcceptButton = btnLogin;
        }

        // Use async event handlers and run blocking DB calls off the UI thread to avoid freezes.
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(user))
            {
                lblStatus.Text = "Vui lòng nh?p tên ??ng nh?p.";
                return;
            }

            try
            {
                // Offload Authenticate to background thread to avoid blocking UI
                var authResult = await Task.Run(() => _mongo.Authenticate(user, pass));
                if (authResult)
                {
                    LoggedInUsername = user;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = "Tên ??ng nh?p ho?c m?t kh?u không ?úng, ho?c không th? k?t n?i t?i c? s? d? li?u.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "L?i: " + ex.Message;
            }
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(user))
            {
                lblStatus.Text = "Vui lòng nh?p tên ??ng nh?p.";
                return;
            }

            try
            {
                // Offload CreateUser (which is synchronous) to background thread and capture error
                var result = await Task.Run(() =>
                {
                    string error;
                    var ok = _mongo.CreateUser(user, pass, out error);
                    return Tuple.Create(ok, error);
                });

                if (result.Item1)
                {
                    LoggedInUsername = user;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = result.Item2 ?? "??ng ký th?t b?i ho?c không th? k?t n?i t?i c? s? d? li?u.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "L?i: " + ex.Message;
            }
        }
    }
}