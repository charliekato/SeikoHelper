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
            labelEntryList2 = new Label();
            textBoxEntryList = new TextBox();
            labelEntryList = new Label();
            buttonEntryList = new Button();
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
            // labelEntryList2
            // 
            labelEntryList2.AutoSize = true;
            labelEntryList2.Font = new Font("Yu Gothic UI", 12F);
            labelEntryList2.Location = new Point(79, 91);
            labelEntryList2.Name = "labelEntryList2";
            labelEntryList2.Size = new Size(182, 28);
            labelEntryList2.TabIndex = 2;
            labelEntryList2.Text = "出力 Excel ファイル名";
            // 
            // textBoxEntryList
            // 
            textBoxEntryList.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            textBoxEntryList.Location = new Point(290, 88);
            textBoxEntryList.Name = "textBoxEntryList";
            textBoxEntryList.Size = new Size(206, 34);
            textBoxEntryList.TabIndex = 3;
            textBoxEntryList.Text = "EntryList.xlsx";
            // 
            // labelEntryList
            // 
            labelEntryList.AutoSize = true;
            labelEntryList.Font = new Font("Yu Gothic UI", 12F);
            labelEntryList.Location = new Point(61, 52);
            labelEntryList.Name = "labelEntryList";
            labelEntryList.Size = new Size(166, 28);
            labelEntryList.TabIndex = 4;
            labelEntryList.Text = "エントリーリスト作成";
            // 
            // buttonEntryList
            // 
            buttonEntryList.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            buttonEntryList.Location = new Point(569, 76);
            buttonEntryList.Name = "buttonEntryList";
            buttonEntryList.Size = new Size(77, 46);
            buttonEntryList.TabIndex = 5;
            buttonEntryList.Text = "実行";
            buttonEntryList.UseVisualStyleBackColor = true;
            buttonEntryList.Click += buttonEntryList_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonEntryList);
            Controls.Add(labelEntryList);
            Controls.Add(textBoxEntryList);
            Controls.Add(labelEntryList2);
            Controls.Add(labelMenu);
            Name = "MainForm";
            Text = "MainMenu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelMenu;
        private Label labelEntryList2;
        private TextBox textBoxEntryList;
        private Label labelEntryList;
        private Button buttonEntryList;
    }
}
