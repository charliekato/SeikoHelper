namespace SeikoHelper
{

    partial class SelectServer
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
            btnOK = new Button();
            tbxServerName = new TextBox();
            btnQuit = new Button();
            lblServerName = new Label();
            lblPassword = new Label();
            tbxPassword = new TextBox();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Location = new Point(169, 183);
            btnOK.Margin = new Padding(2, 2, 2, 2);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(98, 62);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // tbxServerName
            // 
            tbxServerName.Location = new Point(228, 54);
            tbxServerName.Margin = new Padding(2, 2, 2, 2);
            tbxServerName.Name = "tbxServerName";
            tbxServerName.Size = new Size(245, 31);
            tbxServerName.TabIndex = 2;
            tbxServerName.KeyDown += tbxServerName_KeyDown;
            // 
            // btnQuit
            // 
            btnQuit.Location = new Point(358, 183);
            btnQuit.Margin = new Padding(2, 2, 2, 2);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new Size(98, 62);
            btnQuit.TabIndex = 3;
            btnQuit.Text = "終了";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // lblServerName
            // 
            lblServerName.AutoSize = true;
            lblServerName.Location = new Point(85, 58);
            lblServerName.Name = "lblServerName";
            lblServerName.Size = new Size(84, 25);
            lblServerName.TabIndex = 4;
            lblServerName.Text = "Server 名";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(85, 117);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(88, 25);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Password";
            // 
            // tbxPassword
            // 
            tbxPassword.Location = new Point(228, 113);
            tbxPassword.Margin = new Padding(2);
            tbxPassword.Name = "tbxPassword";
            tbxPassword.Size = new Size(245, 31);
            tbxPassword.TabIndex = 5;
            // 
            // SelectServer
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 312);
            Controls.Add(lblPassword);
            Controls.Add(tbxPassword);
            Controls.Add(lblServerName);
            Controls.Add(btnQuit);
            Controls.Add(tbxServerName);
            Controls.Add(btnOK);
            Margin = new Padding(2, 2, 2, 2);
            Name = "SelectServer";
            Text = "サーバーの選択";
            Load += SelectServer_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOK;
        private TextBox tbxServerName;
        private Button btnQuit;
        private Label lblServerName;
        private Label lblPassword;
        private TextBox tbxPassword;
    }
}
