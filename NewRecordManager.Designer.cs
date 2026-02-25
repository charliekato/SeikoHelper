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
            lblUpdateXlsx = new Label();
            tbxThisYear = new TextBox();
            lblThisYear = new Label();
            lblExcel2DB = new Label();
            btnExcel2DB = new Button();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            label3.Location = new Point(102, 241);
            label3.Name = "label3";
            label3.Size = new Size(179, 32);
            label3.TabIndex = 12;
            label3.Text = "競技後にやること";
            // 
            // lblCompXls2DB
            // 
            lblCompXls2DB.AutoSize = true;
            lblCompXls2DB.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblCompXls2DB.Location = new Point(124, 157);
            lblCompXls2DB.Name = "lblCompXls2DB";
            lblCompXls2DB.Size = new Size(279, 30);
            lblCompXls2DB.TabIndex = 15;
            lblCompXls2DB.Text = "大会記録 excel と DB を比較";
            // 
            // btnCompXls2DB
            // 
            btnCompXls2DB.Font = new Font("Yu Gothic UI", 11F);
            btnCompXls2DB.Location = new Point(444, 152);
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
            btnTxtGen.Location = new Point(444, 360);
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
            lblTxtGen.Location = new Point(124, 365);
            lblTxtGen.Name = "lblTxtGen";
            lblTxtGen.Size = new Size(267, 30);
            lblTxtGen.TabIndex = 17;
            lblTxtGen.Text = "更新した「新記録.txt」を作成";
            // 
            // btnGR2XLS
            // 
            btnGR2XLS.Font = new Font("Yu Gothic UI", 11F);
            btnGR2XLS.Location = new Point(444, 287);
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
            label4.Location = new Point(124, 292);
            label4.Name = "label4";
            label4.Size = new Size(167, 30);
            label4.TabIndex = 19;
            label4.Text = "発生新記録作成";
            // 
            // lblUpdateXlsx
            // 
            lblUpdateXlsx.AutoSize = true;
            lblUpdateXlsx.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblUpdateXlsx.Location = new Point(169, 395);
            lblUpdateXlsx.Name = "lblUpdateXlsx";
            lblUpdateXlsx.Size = new Size(233, 30);
            lblUpdateXlsx.TabIndex = 21;
            lblUpdateXlsx.Text = "大会記録エクセルの更新";
            // 
            // tbxThisYear
            // 
            tbxThisYear.Location = new Point(421, 482);
            tbxThisYear.Name = "tbxThisYear";
            tbxThisYear.Size = new Size(53, 31);
            tbxThisYear.TabIndex = 23;
            tbxThisYear.Text = "26";
            // 
            // lblThisYear
            // 
            lblThisYear.AutoSize = true;
            lblThisYear.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblThisYear.Location = new Point(211, 481);
            lblThisYear.Name = "lblThisYear";
            lblThisYear.Size = new Size(191, 30);
            lblThisYear.TabIndex = 24;
            lblThisYear.Text = "年、または、回 ==>";
            // 
            // lblExcel2DB
            // 
            lblExcel2DB.AutoSize = true;
            lblExcel2DB.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblExcel2DB.Location = new Point(124, 97);
            lblExcel2DB.Name = "lblExcel2DB";
            lblExcel2DB.Size = new Size(274, 30);
            lblExcel2DB.TabIndex = 26;
            lblExcel2DB.Text = "大会記録 excel をDB に反映";
            // 
            // btnExcel2DB
            // 
            btnExcel2DB.Font = new Font("Yu Gothic UI", 11F);
            btnExcel2DB.Location = new Point(444, 92);
            btnExcel2DB.Name = "btnExcel2DB";
            btnExcel2DB.Size = new Size(73, 41);
            btnExcel2DB.TabIndex = 25;
            btnExcel2DB.Text = "実行";
            btnExcel2DB.UseVisualStyleBackColor = true;
            btnExcel2DB.Click += btnExcel2DB_Click;
            // 
            // NewRecordManager
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(775, 627);
            Controls.Add(lblExcel2DB);
            Controls.Add(btnExcel2DB);
            Controls.Add(lblThisYear);
            Controls.Add(tbxThisYear);
            Controls.Add(lblUpdateXlsx);
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

        private Label label3;
        private Label lblCompXls2DB;
        private Button btnCompXls2DB;
        private Label label2;
        private Button btnTxtGen;
        private Label lblTxtGen;
        private Button btnGR2XLS;
        private Label label4;
        private Label lblUpdateXlsx;
        private TextBox tbxThisYear;
        private Label lblThisYear;
        private Label lblExcel2DB;
        private Button btnExcel2DB;
    }
}
