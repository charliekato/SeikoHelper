namespace SeikoHelper
{
    partial class PaperSokuho
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtBoxTemplateFilePath = new TextBox();
            lblTemplateFilePath = new Label();
            btnTemplatePath = new Button();
            lblPrgNo = new Label();
            txtBoxPrgNo = new TextBox();
            btnGo = new Button();
            SuspendLayout();
            // 
            // txtBoxTemplateFilePath
            // 
            txtBoxTemplateFilePath.Location = new Point(132, 76);
            txtBoxTemplateFilePath.Name = "txtBoxTemplateFilePath";
            txtBoxTemplateFilePath.Size = new Size(570, 31);
            txtBoxTemplateFilePath.TabIndex = 0;
            // 
            // lblTemplateFilePath
            // 
            lblTemplateFilePath.AutoSize = true;
            lblTemplateFilePath.Location = new Point(26, 79);
            lblTemplateFilePath.Name = "lblTemplateFilePath";
            lblTemplateFilePath.Size = new Size(89, 25);
            lblTemplateFilePath.TabIndex = 1;
            lblTemplateFilePath.Text = "テンプレート";
            // 
            // btnTemplatePath
            // 
            btnTemplatePath.Location = new Point(770, 70);
            btnTemplatePath.Name = "btnTemplatePath";
            btnTemplatePath.Size = new Size(73, 42);
            btnTemplatePath.TabIndex = 2;
            btnTemplatePath.Text = "選択";
            btnTemplatePath.UseVisualStyleBackColor = true;
            btnTemplatePath.Click += btnTemplatePath_Click;
            // 
            // lblPrgNo
            // 
            lblPrgNo.AutoSize = true;
            lblPrgNo.Location = new Point(264, 157);
            lblPrgNo.Name = "lblPrgNo";
            lblPrgNo.Size = new Size(84, 25);
            lblPrgNo.TabIndex = 3;
            lblPrgNo.Text = "競技番号";
            // 
            // txtBoxPrgNo
            // 
            txtBoxPrgNo.Location = new Point(370, 153);
            txtBoxPrgNo.Name = "txtBoxPrgNo";
            txtBoxPrgNo.Size = new Size(80, 31);
            txtBoxPrgNo.TabIndex = 4;
            // 
            // btnGo
            // 
            btnGo.Location = new Point(530, 149);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(63, 34);
            btnGo.TabIndex = 7;
            btnGo.Text = "作成";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // PaperSokuho
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 251);
            Controls.Add(btnGo);
            Controls.Add(txtBoxPrgNo);
            Controls.Add(lblPrgNo);
            Controls.Add(btnTemplatePath);
            Controls.Add(lblTemplateFilePath);
            Controls.Add(txtBoxTemplateFilePath);
            Name = "PaperSokuho";
            Text = "PaperSokuho";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtBoxTemplateFilePath;
        private Label lblTemplateFilePath;
        private Button btnTemplatePath;
        private Label lblPrgNo;
        private TextBox txtBoxPrgNo;
        private Button btnGo;
    }
}