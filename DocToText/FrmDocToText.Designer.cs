namespace DocToText
{
    partial class FrmDocToText
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
            lblTitle = new Label();
            lblDescription = new Label();
            btnAddContextMenus = new Button();
            btnRemoveContextMenus = new Button();
            btnRunAsAdmin = new Button();
            lblAdminState = new Label();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(24, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(147, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "DocToText Tool";
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(24, 58);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(639, 15);
            lblDescription.TabIndex = 1;
            lblDescription.Text = "Add or remove Explorer context menu commands for .doc and .docx files: Convert to plain text / Convert to Markdown.";
            // 
            // btnAddContextMenus
            // 
            btnAddContextMenus.Location = new Point(24, 105);
            btnAddContextMenus.Name = "btnAddContextMenus";
            btnAddContextMenus.Size = new Size(260, 35);
            btnAddContextMenus.TabIndex = 2;
            btnAddContextMenus.Text = "Add context menus";
            btnAddContextMenus.UseVisualStyleBackColor = true;
            btnAddContextMenus.Click += BtnAddContextMenus_Click;
            // 
            // btnRemoveContextMenus
            // 
            btnRemoveContextMenus.Location = new Point(304, 105);
            btnRemoveContextMenus.Name = "btnRemoveContextMenus";
            btnRemoveContextMenus.Size = new Size(260, 35);
            btnRemoveContextMenus.TabIndex = 3;
            btnRemoveContextMenus.Text = "Remove context menus";
            btnRemoveContextMenus.UseVisualStyleBackColor = true;
            btnRemoveContextMenus.Click += BtnRemoveContextMenus_Click;
            // 
            // btnRunAsAdmin
            // 
            btnRunAsAdmin.Location = new Point(584, 105);
            btnRunAsAdmin.Name = "btnRunAsAdmin";
            btnRunAsAdmin.Size = new Size(152, 35);
            btnRunAsAdmin.TabIndex = 4;
            btnRunAsAdmin.Text = "Restart as admin";
            btnRunAsAdmin.UseVisualStyleBackColor = true;
            btnRunAsAdmin.Click += BtnRunAsAdmin_Click;
            // 
            // lblAdminState
            // 
            lblAdminState.AutoSize = true;
            lblAdminState.Location = new Point(24, 160);
            lblAdminState.Name = "lblAdminState";
            lblAdminState.Size = new Size(74, 15);
            lblAdminState.TabIndex = 5;
            lblAdminState.Text = "Admin state:";
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Location = new Point(24, 188);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(712, 78);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Ready.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FrmDocToText
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 290);
            Controls.Add(lblStatus);
            Controls.Add(lblAdminState);
            Controls.Add(btnRunAsAdmin);
            Controls.Add(btnRemoveContextMenus);
            Controls.Add(btnAddContextMenus);
            Controls.Add(lblDescription);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmDocToText";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DocToText";
            Load += FrmDocToText_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblDescription;
        private Button btnAddContextMenus;
        private Button btnRemoveContextMenus;
        private Button btnRunAsAdmin;
        private Label lblAdminState;
        private Label lblStatus;
    }
}
