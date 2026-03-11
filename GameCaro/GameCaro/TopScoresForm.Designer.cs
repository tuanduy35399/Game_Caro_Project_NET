namespace GameCaro
{
    public class TopScoresForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvTopScores;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvTopScores = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopScores)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTopScores
            // 
            this.dgvTopScores.AllowUserToAddRows = false;
            this.dgvTopScores.AllowUserToDeleteRows = false;
            this.dgvTopScores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTopScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopScores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTopScores.Location = new System.Drawing.Point(0, 0);
            this.dgvTopScores.MultiSelect = false;
            this.dgvTopScores.Name = "dgvTopScores";
            this.dgvTopScores.ReadOnly = true;
            this.dgvTopScores.RowHeadersVisible = false;
            this.dgvTopScores.RowHeadersWidth = 51;
            this.dgvTopScores.RowTemplate.Height = 24;
            this.dgvTopScores.Size = new System.Drawing.Size(645, 353);
            this.dgvTopScores.TabIndex = 0;
            // 
            // TopScoresForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 353);
            this.Controls.Add(this.dgvTopScores);
            this.Name = "TopScoresForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Top điểm cao";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopScores)).EndInit();
            this.ResumeLayout(false);
        }
}
