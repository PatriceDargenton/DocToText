
namespace DocToText
{
    partial class FrmConversionSplash
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
            if (disposing)
            {
                components?.Dispose();
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
            lblTitle = new Label();
            progressBarConversion = new ProgressBar();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(24, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(199, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Conversion in progress...";
            // 
            // progressBarConversion
            // 
            progressBarConversion.Location = new Point(24, 57);
            progressBarConversion.MarqueeAnimationSpeed = 30;
            progressBarConversion.Name = "progressBarConversion";
            progressBarConversion.Size = new Size(372, 20);
            progressBarConversion.Style = ProgressBarStyle.Marquee;
            progressBarConversion.TabIndex = 3;
            // 
            // FrmConversionSplash
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 106);
            Controls.Add(progressBarConversion);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmConversionSplash";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DocToText";
            Load += FrmConversionSplash_Load;
            Shown += FrmConversionSplash_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private ProgressBar progressBarConversion;
    }
}