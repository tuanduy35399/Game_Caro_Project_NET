using GameCaro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class TopScoresForm : Form
    {
        public TopScoresForm(List<UserModel> users)
        {
            InitializeComponent();
            BindData(users ?? new List<UserModel>());
        }

        private void BindData(List<UserModel> users)
        {
            var rows = users.Select((u, index) => new
            {
                Rank = index + 1,
                Username = u.Username,
                Score = u.Score,
                TotalGames = u.TotalGames,
                CreatedAt = u.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvTopScores.DataSource = rows;
        }
    }
}