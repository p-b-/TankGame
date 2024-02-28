namespace TankGame
{
    partial class TankGameForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TankGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(1200, 600);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "TankGameForm";
            this.Text = "Tank Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TankGameForm_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TankGameForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TankGameForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TankGameForm_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TankGameForm_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion
    }
}