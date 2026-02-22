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
            lblSCLASS = new Label();
            rdbYokoMochi = new RadioButton();
            rdbTateMochi = new RadioButton();
            labelWinnerList = new Label();
            lblExtractNewRecord = new Label();
            SuspendLayout();
            // 
            // labelMenu
            // 
            labelMenu.AutoSize = true;
            labelMenu.Font = new Font("Yu Gothic UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 128);
            labelMenu.Location = new Point(435, 11);
            labelMenu.Margin = new Padding(4, 0, 4, 0);
            labelMenu.Name = "labelMenu";
            labelMenu.Size = new Size(107, 45);
            labelMenu.TabIndex = 0;
            labelMenu.Text = "Menu";
            // 
            // labelEntryList
            // 
            labelEntryList.AutoSize = true;
            labelEntryList.Font = new Font("Yu Gothic UI", 12F);
            labelEntryList.Location = new Point(102, 190);
            labelEntryList.Margin = new Padding(4, 0, 4, 0);
            labelEntryList.Name = "labelEntryList";
            labelEntryList.Size = new Size(199, 32);
            labelEntryList.TabIndex = 4;
            labelEntryList.Text = "エントリーリスト作成";
            labelEntryList.Click += labelEntryList_Click;
            // 
            // lblGoDoCreater
            // 
            lblGoDoCreater.AutoSize = true;
            lblGoDoCreater.Font = new Font("Yu Gothic UI", 12F);
            lblGoDoCreater.Location = new Point(102, 95);
            lblGoDoCreater.Margin = new Padding(4, 0, 4, 0);
            lblGoDoCreater.Name = "lblGoDoCreater";
            lblGoDoCreater.Size = new Size(163, 32);
            lblGoDoCreater.TabIndex = 6;
            lblGoDoCreater.Text = "合同クリエーター";
            lblGoDoCreater.Click += GodoCreaterClick;
            // 
            // lblSCLASS
            // 
            lblSCLASS.AutoSize = true;
            lblSCLASS.Font = new Font("Yu Gothic UI", 12F);
            lblSCLASS.Location = new Point(102, 555);
            lblSCLASS.Margin = new Padding(4, 0, 4, 0);
            lblSCLASS.Name = "lblSCLASS";
            lblSCLASS.Size = new Size(259, 32);
            lblSCLASS.TabIndex = 8;
            lblSCLASS.Text = "標準記録判定クラス追加";
            lblSCLASS.Click += lblSCLASS_Click;
            // 
            // rdbYokoMochi
            // 
            rdbYokoMochi.AutoSize = true;
            rdbYokoMochi.Location = new Point(210, 239);
            rdbYokoMochi.Margin = new Padding(4);
            rdbYokoMochi.Name = "rdbYokoMochi";
            rdbYokoMochi.Size = new Size(86, 29);
            rdbYokoMochi.TabIndex = 9;
            rdbYokoMochi.TabStop = true;
            rdbYokoMochi.Text = "横持ち";
            rdbYokoMochi.UseVisualStyleBackColor = true;
            // 
            // rdbTateMochi
            // 
            rdbTateMochi.AutoSize = true;
            rdbTateMochi.Location = new Point(374, 238);
            rdbTateMochi.Margin = new Padding(4);
            rdbTateMochi.Name = "rdbTateMochi";
            rdbTateMochi.Size = new Size(86, 29);
            rdbTateMochi.TabIndex = 10;
            rdbTateMochi.TabStop = true;
            rdbTateMochi.Text = "縦持ち";
            rdbTateMochi.UseVisualStyleBackColor = true;
            // 
            // labelWinnerList
            // 
            labelWinnerList.AutoSize = true;
            labelWinnerList.Font = new Font("Yu Gothic UI", 12F);
            labelWinnerList.Location = new Point(102, 311);
            labelWinnerList.Margin = new Padding(4, 0, 4, 0);
            labelWinnerList.Name = "labelWinnerList";
            labelWinnerList.Size = new Size(186, 32);
            labelWinnerList.TabIndex = 11;
            labelWinnerList.Text = "優勝者リスト作成";
            labelWinnerList.Click += labelWinnerList_Click;
            // 
            // lblExtractNewRecord
            // 
            lblExtractNewRecord.AutoSize = true;
            lblExtractNewRecord.Font = new Font("Yu Gothic UI", 12F);
            lblExtractNewRecord.Location = new Point(102, 393);
            lblExtractNewRecord.Margin = new Padding(4, 0, 4, 0);
            lblExtractNewRecord.Name = "lblExtractNewRecord";
            lblExtractNewRecord.Size = new Size(134, 32);
            lblExtractNewRecord.TabIndex = 7;
            lblExtractNewRecord.Text = "新記録管理";
            lblExtractNewRecord.Click += lblExtractNewRecord_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1015, 660);
            Controls.Add(labelWinnerList);
            Controls.Add(rdbTateMochi);
            Controls.Add(rdbYokoMochi);
            Controls.Add(lblSCLASS);
            Controls.Add(lblExtractNewRecord);
            Controls.Add(lblGoDoCreater);
            Controls.Add(labelEntryList);
            Controls.Add(labelMenu);
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "MainMenu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelMenu;
        private Label labelEntryList;
        private Label lblGoDoCreater;
        private Label lblSCLASS;
        private RadioButton rdbYokoMochi;
        private RadioButton rdbTateMochi;
        private Label labelWinnerList;
        private Label lblExtractNewRecord;
    }
}
