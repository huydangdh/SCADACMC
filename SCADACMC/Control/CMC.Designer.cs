namespace SCADACMC.Control
{
    partial class CMCControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NamePanel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NamePanel
            // 
            this.NamePanel.BackColor = System.Drawing.Color.Red;
            this.NamePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NamePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.NamePanel.Location = new System.Drawing.Point(0, 0);
            this.NamePanel.Name = "NamePanel";
            this.NamePanel.Size = new System.Drawing.Size(150, 25);
            this.NamePanel.TabIndex = 5;
            this.NamePanel.Text = "B41C1AH1";
            this.NamePanel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CMC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NamePanel);
            this.Name = "CMC";
            this.Size = new System.Drawing.Size(150, 25);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label NamePanel;
    }
}
