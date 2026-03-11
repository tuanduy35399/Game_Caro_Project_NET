using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GameCaro.Models;

namespace GameCaro
{
    public partial class MatchHistoryForm : Form
    {
        public MatchHistoryForm(List<MatchHistoryModel> matches)
        {
            InitializeComponent();
            BindData(matches ?? new List<MatchHistoryModel>());
        }

        private void BindData(List<MatchHistoryModel> matches)
        {
            var rows = matches.Select((m, index) => new
            {
                No = index + 1,
                PlayerX = m.PlayerX,
                PlayerO = m.PlayerO,
                Winner = string.IsNullOrWhiteSpace(m.Winner) ? "Hòa / không có" : m.Winner,
                Reason = m.Reason,
                Moves = m.MoveCount,
                FinishedAt = m.FinishedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                DurationSeconds = m.DurationSeconds
            }).ToList();

            dgvHistory.DataSource = rows;
        }
    }
}