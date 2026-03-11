using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
        public partial class RegisterForm : Form
        {
            private readonly MongoService _mongoService;

            public RegisterForm(MongoService mongoService)
            {
                _mongoService = mongoService;
                InitializeComponent();
            }

            private async void btnRegister_Click(object sender, EventArgs e)
            {
                lblStatus.Text = string.Empty;

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    lblStatus.Text = "Mật khẩu xác nhận không khớp.";
                    return;
                }

                ToggleUi(false);
                try
                {
                    var result = await _mongoService.RegisterAsync(txtUsername.Text.Trim(), txtPassword.Text);
                    if (!result.Success)
                    {
                        lblStatus.Text = result.ErrorMessage;
                        return;
                    }

                    MessageBox.Show(this, "Đăng ký thành công. Bạn có thể đăng nhập ngay bây giờ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                finally
                {
                    ToggleUi(true);
                }
            }

            private void btnCancel_Click(object sender, EventArgs e)
            {
                Close();
            }

            private void ToggleUi(bool enabled)
            {
                txtUsername.Enabled = enabled;
                txtPassword.Enabled = enabled;
                txtConfirmPassword.Enabled = enabled;
                btnRegister.Enabled = enabled;
                btnCancel.Enabled = enabled;
            }
        }
    }


