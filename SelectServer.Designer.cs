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
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Location = new Point(220, 234);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(128, 80);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // tbxServerName
            // 
            tbxServerName.Location = new Point(217, 69);
            tbxServerName.Name = "tbxServerName";
            tbxServerName.Size = new Size(397, 39);
            tbxServerName.TabIndex = 2;
            tbxServerName.KeyDown += tbxServerName_KeyDown;
            // 
            // btnQuit
            // 
            btnQuit.Location = new Point(466, 234);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new Size(128, 80);
            btnQuit.TabIndex = 3;
            btnQuit.Text = "終了";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // SelectServer
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 399);
            Controls.Add(btnQuit);
            Controls.Add(tbxServerName);
            Controls.Add(btnOK);
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
    }
}
