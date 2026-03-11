using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCaro.Models;

namespace GameCaro
{
    public partial class LoginForm : Form
    {
        private readonly MongoService _mongo;

        public UserModel LoggedInUser { get; private set; }

        public LoginForm(MongoService mongo)
        {
            _mongo = mongo ?? throw new ArgumentNullException(nameof(mongo));
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            BtnLogin_Click(sender, e);
        }

        private void btnOpenRegister_Click(object sender, EventArgs e)
        {
            BtnRegister_Click(sender, e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(user))
            {
                lblStatus.Text = "Vui lòng nh?p tên ??ng nh?p.";
                return;
            }

            try
            {
                var authResult = await _mongo.AuthenticateAsync(user, pass);
                if (authResult.Success)
                {
                    LoggedInUser = authResult.User;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = authResult.ErrorMessage ?? "Tên ??ng nh?p ho?c m?t kh?u không ?úng.";
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
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(user))
            {
                lblStatus.Text = "Vui lòng nh?p tên ??ng nh?p.";
                return;
            }

            try
            {
                var result = await _mongo.RegisterAsync(user, pass);
                if (result.Success)
                {
                    // After registration, authenticate to get the user model
                    var authResult = await _mongo.AuthenticateAsync(user, pass);
                    if (authResult.Success)
                    {
                        LoggedInUser = authResult.User;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        lblStatus.Text = authResult.ErrorMessage ?? "??ng nh?p th?t b?i sau khi ??ng ký.";
                    }
                }
                else
                {
                    lblStatus.Text = result.ErrorMessage ?? "??ng ký th?t b?i.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "L?i: " + ex.Message;
            }
        }
    }
}
