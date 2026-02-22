namespace SeikoHelper
{
    partial class NewRecordManager
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
            label3 = new Label();
            lblCompXls2DB = new Label();
            btnCompXls2DB = new Button();
            label2 = new Label();
            btnTxtGen = new Button();
            lblTxtGen = new Label();
            btnGR2XLS = new Button();
            label4 = new Label();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            label3.Location = new Point(102, 181);
            label3.Name = "label3";
            label3.Size = new Size(179, 32);
            label3.TabIndex = 12;
            label3.Text = "競技後にやること";
            // 
            // lblCompXls2DB
            // 
            lblCompXls2DB.AutoSize = true;
            lblCompXls2DB.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblCompXls2DB.Location = new Point(124, 97);
            lblCompXls2DB.Name = "lblCompXls2DB";
            lblCompXls2DB.Size = new Size(279, 30);
            lblCompXls2DB.TabIndex = 15;
            lblCompXls2DB.Text = "大会記録 excel と DB を比較";
            // 
            // btnCompXls2DB
            // 
            btnCompXls2DB.Font = new Font("Yu Gothic UI", 11F);
            btnCompXls2DB.Location = new Point(444, 92);
            btnCompXls2DB.Name = "btnCompXls2DB";
            btnCompXls2DB.Size = new Size(73, 41);
            btnCompXls2DB.TabIndex = 14;
            btnCompXls2DB.Text = "実行";
            btnCompXls2DB.UseVisualStyleBackColor = true;
            btnCompXls2DB.Click += btnCompXls2DB_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            label2.Location = new Point(102, 34);
            label2.Name = "label2";
            label2.Size = new Size(179, 32);
            label2.TabIndex = 16;
            label2.Text = "競技前にやること";
            // 
            // btnTxtGen
            // 
            btnTxtGen.Font = new Font("Yu Gothic UI", 11F);
            btnTxtGen.Location = new Point(444, 289);
            btnTxtGen.Name = "btnTxtGen";
            btnTxtGen.Size = new Size(73, 41);
            btnTxtGen.TabIndex = 18;
            btnTxtGen.Text = "実行";
            btnTxtGen.UseVisualStyleBackColor = true;
            btnTxtGen.Click += btnTxtGen_Click;
            // 
            // lblTxtGen
            // 
            lblTxtGen.AutoSize = true;
            lblTxtGen.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblTxtGen.Location = new Point(124, 294);
            lblTxtGen.Name = "lblTxtGen";
            lblTxtGen.Size = new Size(267, 30);
            lblTxtGen.TabIndex = 17;
            lblTxtGen.Text = "更新した「新記録.txt」を作成";
            // 
            // btnGR2XLS
            // 
            btnGR2XLS.Font = new Font("Yu Gothic UI", 11F);
            btnGR2XLS.Location = new Point(444, 227);
            btnGR2XLS.Name = "btnGR2XLS";
            btnGR2XLS.Size = new Size(73, 41);
            btnGR2XLS.TabIndex = 20;
            btnGR2XLS.Text = "実行";
            btnGR2XLS.UseVisualStyleBackColor = true;
            btnGR2XLS.Click += btnGR2XLS_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label4.Location = new Point(124, 232);
            label4.Name = "label4";
            label4.Size = new Size(167, 30);
            label4.TabIndex = 19;
            label4.Text = "発生新記録作成";
            // 
            // NewRecordManager
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnGR2XLS);
            Controls.Add(label4);
            Controls.Add(btnTxtGen);
            Controls.Add(lblTxtGen);
            Controls.Add(label2);
            Controls.Add(lblCompXls2DB);
            Controls.Add(btnCompXls2DB);
            Controls.Add(label3);
            Name = "NewRecordManager";
            Text = "新記録Manager";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblCompare;
        private TextBox tbxExcelFile1;
        private Label lblExcelFileName;
        private Button btnRunCompare;
        private Button btnSelect2;
        private TextBox tbxExcelFile2;
        private Label label3;
        private Label lblCompXls2DB;
        private Button btnCompXls2DB;
        private Label label2;
        private Button btnTxtGen;
        private Label lblTxtGen;
        private Button btnGR2XLS;
        private Label label4;
    }
}