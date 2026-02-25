namespace SeikoHelper
{
    partial class MainMenu
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
            checkBoxScenario1 = new CheckBox();
            checkBoxScenario3 = new CheckBox();
            checkBoxStrategy1 = new CheckBox();
            checkBoxStrategy2 = new CheckBox();
            checkBoxScenario2 = new CheckBox();
            buttonExe = new Button();
            buttonQuit = new Button();
            labelScenario3 = new Label();
            label1 = new Label();
            checkBoxScenario4 = new CheckBox();
            labelTitle = new Label();
            checkBoxScenario5 = new CheckBox();
            SuspendLayout();
            // 
            // checkBoxScenario1
            // 
            checkBoxScenario1.AutoSize = true;
            checkBoxScenario1.Location = new Point(222, 128);
            checkBoxScenario1.Margin = new Padding(4, 4, 4, 4);
            checkBoxScenario1.Name = "checkBoxScenario1";
            checkBoxScenario1.Size = new Size(270, 29);
            checkBoxScenario1.TabIndex = 0;
            checkBoxScenario1.Text = "合同ができるようにレーンを寄せる";
            checkBoxScenario1.UseVisualStyleBackColor = true;
            // 
            // checkBoxScenario3
            // 
            checkBoxScenario3.AutoSize = true;
            checkBoxScenario3.Location = new Point(222, 346);
            checkBoxScenario3.Margin = new Padding(4, 4, 4, 4);
            checkBoxScenario3.Name = "checkBoxScenario3";
            checkBoxScenario3.Size = new Size(474, 29);
            checkBoxScenario3.TabIndex = 1;
            checkBoxScenario3.Text = "違う年齢クラスの選手を集めたレースをつくりそこに移動させる。";
            checkBoxScenario3.UseVisualStyleBackColor = true;
            checkBoxScenario3.CheckedChanged += checkBoxScenario3_CheckedChanged;
            // 
            // checkBoxStrategy1
            // 
            checkBoxStrategy1.AutoSize = true;
            checkBoxStrategy1.Location = new Point(268, 170);
            checkBoxStrategy1.Margin = new Padding(4, 4, 4, 4);
            checkBoxStrategy1.Name = "checkBoxStrategy1";
            checkBoxStrategy1.Size = new Size(167, 29);
            checkBoxStrategy1.TabIndex = 2;
            checkBoxStrategy1.Text = "男女も合同にする";
            checkBoxStrategy1.UseVisualStyleBackColor = true;
            // 
            // checkBoxStrategy2
            // 
            checkBoxStrategy2.AutoSize = true;
            checkBoxStrategy2.Location = new Point(268, 216);
            checkBoxStrategy2.Margin = new Padding(4, 4, 4, 4);
            checkBoxStrategy2.Name = "checkBoxStrategy2";
            checkBoxStrategy2.Size = new Size(352, 29);
            checkBoxStrategy2.TabIndex = 3;
            checkBoxStrategy2.Text = "組単位で合同にする(V6でできるようになった)";
            checkBoxStrategy2.UseVisualStyleBackColor = true;
            // 
            // checkBoxScenario2
            // 
            checkBoxScenario2.AutoSize = true;
            checkBoxScenario2.Location = new Point(222, 280);
            checkBoxScenario2.Margin = new Padding(4, 4, 4, 4);
            checkBoxScenario2.Name = "checkBoxScenario2";
            checkBoxScenario2.Size = new Size(308, 29);
            checkBoxScenario2.TabIndex = 4;
            checkBoxScenario2.Text = "セイコーリザルトで合同のテーブルを作る";
            checkBoxScenario2.UseVisualStyleBackColor = true;
            // 
            // buttonExe
            // 
            buttonExe.Location = new Point(302, 595);
            buttonExe.Margin = new Padding(4, 4, 4, 4);
            buttonExe.Name = "buttonExe";
            buttonExe.Size = new Size(89, 51);
            buttonExe.TabIndex = 6;
            buttonExe.Text = "実行";
            buttonExe.UseVisualStyleBackColor = true;
            buttonExe.Click += buttonExe_Click;
            // 
            // buttonQuit
            // 
            buttonQuit.Location = new Point(694, 595);
            buttonQuit.Margin = new Padding(4, 4, 4, 4);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Size = new Size(89, 51);
            buttonQuit.TabIndex = 7;
            buttonQuit.Text = "終了";
            buttonQuit.UseVisualStyleBackColor = true;
            buttonQuit.Click += buttonQuit_Click;
            // 
            // labelScenario3
            // 
            labelScenario3.AutoSize = true;
            labelScenario3.Location = new Point(290, 380);
            labelScenario3.Margin = new Padding(4, 0, 4, 0);
            labelScenario3.Name = "labelScenario3";
            labelScenario3.Size = new Size(403, 25);
            labelScenario3.TabIndex = 8;
            labelScenario3.Text = " こうすることによって電光に距離や種目が表示されます。";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(290, 420);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(657, 25);
            label1.TabIndex = 9;
            label1.Text = "現在は男女合同と、組単位の合同には対応してません。(競技結果の印刷が未対応なため)";
            // 
            // checkBoxScenario4
            // 
            checkBoxScenario4.AutoSize = true;
            checkBoxScenario4.Location = new Point(222, 476);
            checkBoxScenario4.Margin = new Padding(4, 4, 4, 4);
            checkBoxScenario4.Name = "checkBoxScenario4";
            checkBoxScenario4.Size = new Size(442, 29);
            checkBoxScenario4.TabIndex = 10;
            checkBoxScenario4.Text = "プログラム作成用に合同にする(実際のレースには使わない)";
            checkBoxScenario4.UseVisualStyleBackColor = true;
            checkBoxScenario4.CheckedChanged += checkBox4Pro_CheckedChanged;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Yu Gothic UI", 30F);
            labelTitle.Location = new Point(290, 11);
            labelTitle.Margin = new Padding(4, 0, 4, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(538, 81);
            labelTitle.TabIndex = 11;
            labelTitle.Text = "合同レースクリエーター";
            // 
            // checkBoxScenario5
            // 
            checkBoxScenario5.AutoSize = true;
            checkBoxScenario5.Location = new Point(222, 538);
            checkBoxScenario5.Margin = new Padding(4, 4, 4, 4);
            checkBoxScenario5.Name = "checkBoxScenario5";
            checkBoxScenario5.Size = new Size(312, 29);
            checkBoxScenario5.TabIndex = 12;
            checkBoxScenario5.Text = "同じレーン順でクラス無差別にしてしまう";
            checkBoxScenario5.UseVisualStyleBackColor = true;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1164, 704);
            Controls.Add(checkBoxScenario5);
            Controls.Add(labelTitle);
            Controls.Add(checkBoxScenario4);
            Controls.Add(label1);
            Controls.Add(labelScenario3);
            Controls.Add(buttonQuit);
            Controls.Add(buttonExe);
            Controls.Add(checkBoxScenario2);
            Controls.Add(checkBoxStrategy2);
            Controls.Add(checkBoxStrategy1);
            Controls.Add(checkBoxScenario3);
            Controls.Add(checkBoxScenario1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "MainMenu";
            Text = "MainMenu";
            Load += MainMenu_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBoxScenario1;
        private CheckBox checkBoxScenario3;
        private CheckBox checkBoxStrategy1;
        private CheckBox checkBoxStrategy2;
        private CheckBox checkBoxScenario2;
        private CheckBox checkBoxScenario4;
        private Button buttonExe;
        private Button buttonQuit;
        private Label labelScenario3;
        private Label label1;
        private Label labelTitle;
        private CheckBox checkBoxScenario5;
    }
}
