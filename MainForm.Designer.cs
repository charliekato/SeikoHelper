namespace SeikoHelper
{
    partial class MainForm
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
            labelMenu = new Label();
            labelEntryList = new Label();
            lblGoDoCreater = new Label();
            lblExtractNewRecord = new Label();
            lblSCLASS = new Label();
            rdbYokoMochi = new RadioButton();
            rdbTateMochi = new RadioButton();
            SuspendLayout();
            // 
            // labelMenu
            // 
            labelMenu.AutoSize = true;
            labelMenu.Font = new Font("Yu Gothic UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 128);
            labelMenu.Location = new Point(348, 9);
            labelMenu.Name = "labelMenu";
            labelMenu.Size = new Size(90, 38);
            labelMenu.TabIndex = 0;
            labelMenu.Text = "Menu";
            // 
            // labelEntryList
            // 
            labelEntryList.AutoSize = true;
            labelEntryList.Font = new Font("Yu Gothic UI", 12F);
            labelEntryList.Location = new Point(82, 152);
            labelEntryList.Name = "labelEntryList";
            labelEntryList.Size = new Size(166, 28);
            labelEntryList.TabIndex = 4;
            labelEntryList.Text = "エントリーリスト作成";
            labelEntryList.Click += labelEntryList_Click;
            // 
            // lblGoDoCreater
            // 
            lblGoDoCreater.AutoSize = true;
            lblGoDoCreater.Font = new Font("Yu Gothic UI", 12F);
            lblGoDoCreater.Location = new Point(82, 76);
            lblGoDoCreater.Name = "lblGoDoCreater";
            lblGoDoCreater.Size = new Size(137, 28);
            lblGoDoCreater.TabIndex = 6;
            lblGoDoCreater.Text = "合同クリエーター";
            lblGoDoCreater.Click += GodoCreaterClick;
            // 
            // lblExtractNewRecord
            // 
            lblExtractNewRecord.AutoSize = true;
            lblExtractNewRecord.Font = new Font("Yu Gothic UI", 12F);
            lblExtractNewRecord.Location = new Point(82, 236);
            lblExtractNewRecord.Name = "lblExtractNewRecord";
            lblExtractNewRecord.Size = new Size(112, 28);
            lblExtractNewRecord.TabIndex = 7;
            lblExtractNewRecord.Text = "新記録抽出";
            lblExtractNewRecord.Click += lblExtractNewRecord_Click;
            // 
            // lblSCLASS
            // 
            lblSCLASS.AutoSize = true;
            lblSCLASS.Font = new Font("Yu Gothic UI", 12F);
            lblSCLASS.Location = new Point(82, 310);
            lblSCLASS.Name = "lblSCLASS";
            lblSCLASS.Size = new Size(216, 28);
            lblSCLASS.TabIndex = 8;
            lblSCLASS.Text = "標準記録判定クラス追加";
            lblSCLASS.Click += lblSCLASS_Click;
            // 
            // rdbYokoMochi
            // 
            rdbYokoMochi.AutoSize = true;
            rdbYokoMochi.Location = new Point(168, 191);
            rdbYokoMochi.Name = "rdbYokoMochi";
            rdbYokoMochi.Size = new Size(71, 24);
            rdbYokoMochi.TabIndex = 9;
            rdbYokoMochi.TabStop = true;
            rdbYokoMochi.Text = "横持ち";
            rdbYokoMochi.UseVisualStyleBackColor = true;
            // 
            // rdbTateMochi
            // 
            rdbTateMochi.AutoSize = true;
            rdbTateMochi.Location = new Point(299, 190);
            rdbTateMochi.Name = "rdbTateMochi";
            rdbTateMochi.Size = new Size(71, 24);
            rdbTateMochi.TabIndex = 10;
            rdbTateMochi.TabStop = true;
            rdbTateMochi.Text = "縦持ち";
            rdbTateMochi.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(rdbTateMochi);
            Controls.Add(rdbYokoMochi);
            Controls.Add(lblSCLASS);
            Controls.Add(lblExtractNewRecord);
            Controls.Add(lblGoDoCreater);
            Controls.Add(labelEntryList);
            Controls.Add(labelMenu);
            Name = "MainForm";
            Text = "MainMenu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelMenu;
        private Label labelEntryList;
        private Label lblGoDoCreater;
        private Label lblExtractNewRecord;
        private Label lblSCLASS;
        private RadioButton rdbYokoMochi;
        private RadioButton rdbTateMochi;
    }
}
