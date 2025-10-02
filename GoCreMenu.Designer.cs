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
            label2 = new Label();
            SuspendLayout();
            // 
            // checkBoxScenario1
            // 
            checkBoxScenario1.AutoSize = true;
            checkBoxScenario1.Location = new Point(178, 102);
            checkBoxScenario1.Name = "checkBoxScenario1";
            checkBoxScenario1.Size = new Size(223, 24);
            checkBoxScenario1.TabIndex = 0;
            checkBoxScenario1.Text = "合同ができるようにレーンを寄せる";
            checkBoxScenario1.UseVisualStyleBackColor = true;
            // 
            // checkBoxScenario3
            // 
            checkBoxScenario3.AutoSize = true;
            checkBoxScenario3.Location = new Point(178, 277);
            checkBoxScenario3.Name = "checkBoxScenario3";
            checkBoxScenario3.Size = new Size(391, 24);
            checkBoxScenario3.TabIndex = 1;
            checkBoxScenario3.Text = "違う年齢クラスの選手を集めたレースをつくりそこに移動させる。";
            checkBoxScenario3.UseVisualStyleBackColor = true;
            checkBoxScenario3.CheckedChanged += checkBoxScenario3_CheckedChanged;
            // 
            // checkBoxStrategy1
            // 
            checkBoxStrategy1.AutoSize = true;
            checkBoxStrategy1.Location = new Point(214, 136);
            checkBoxStrategy1.Name = "checkBoxStrategy1";
            checkBoxStrategy1.Size = new Size(137, 24);
            checkBoxStrategy1.TabIndex = 2;
            checkBoxStrategy1.Text = "男女も合同にする";
            checkBoxStrategy1.UseVisualStyleBackColor = true;
            // 
            // checkBoxStrategy2
            // 
            checkBoxStrategy2.AutoSize = true;
            checkBoxStrategy2.Location = new Point(214, 173);
            checkBoxStrategy2.Name = "checkBoxStrategy2";
            checkBoxStrategy2.Size = new Size(293, 24);
            checkBoxStrategy2.TabIndex = 3;
            checkBoxStrategy2.Text = "組単位で合同にする(V6でできるようになった)";
            checkBoxStrategy2.UseVisualStyleBackColor = true;
            // 
            // checkBoxScenario2
            // 
            checkBoxScenario2.AutoSize = true;
            checkBoxScenario2.Location = new Point(178, 224);
            checkBoxScenario2.Name = "checkBoxScenario2";
            checkBoxScenario2.Size = new Size(251, 24);
            checkBoxScenario2.TabIndex = 4;
            checkBoxScenario2.Text = "セイコーリザルトで合同のテーブルを作る";
            checkBoxScenario2.UseVisualStyleBackColor = true;
            // 
            // buttonExe
            // 
            buttonExe.Location = new Point(242, 476);
            buttonExe.Name = "buttonExe";
            buttonExe.Size = new Size(71, 41);
            buttonExe.TabIndex = 6;
            buttonExe.Text = "実行";
            buttonExe.UseVisualStyleBackColor = true;
            buttonExe.Click += buttonExe_Click;
            // 
            // buttonQuit
            // 
            buttonQuit.Location = new Point(555, 476);
            buttonQuit.Name = "buttonQuit";
            buttonQuit.Size = new Size(71, 41);
            buttonQuit.TabIndex = 7;
            buttonQuit.Text = "終了";
            buttonQuit.UseVisualStyleBackColor = true;
            buttonQuit.Click += buttonQuit_Click;
            // 
            // labelScenario3
            // 
            labelScenario3.AutoSize = true;
            labelScenario3.Location = new Point(232, 304);
            labelScenario3.Name = "labelScenario3";
            labelScenario3.Size = new Size(334, 20);
            labelScenario3.TabIndex = 8;
            labelScenario3.Text = " こうすることによって電光に距離や種目が表示されます。";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(232, 336);
            label1.Name = "label1";
            label1.Size = new Size(548, 20);
            label1.TabIndex = 9;
            label1.Text = "現在は男女合同と、組単位の合同には対応してません。(競技結果の印刷が未対応なため)";
            // 
            // checkBoxScenario4
            // 
            checkBoxScenario4.AutoSize = true;
            checkBoxScenario4.Location = new Point(178, 381);
            checkBoxScenario4.Name = "checkBoxScenario4";
            checkBoxScenario4.Size = new Size(370, 24);
            checkBoxScenario4.TabIndex = 10;
            checkBoxScenario4.Text = "プログラム作成用に合同にする(実際のレースには使わない)";
            checkBoxScenario4.UseVisualStyleBackColor = true;
            checkBoxScenario4.CheckedChanged += checkBox4Pro_CheckedChanged;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Yu Gothic UI", 30F);
            labelTitle.Location = new Point(232, 9);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(448, 67);
            labelTitle.TabIndex = 11;
            labelTitle.Text = "合同レースクリエーター";
            // 
            // checkBoxScenario5
            // 
            checkBoxScenario5.AutoSize = true;
            checkBoxScenario5.Location = new Point(178, 430);
            checkBoxScenario5.Name = "checkBoxScenario5";
            checkBoxScenario5.Size = new Size(257, 24);
            checkBoxScenario5.TabIndex = 12;
            checkBoxScenario5.Text = "同じレーン順でクラス無差別にしてしまう";
            checkBoxScenario5.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(221, 251);
            label2.Name = "label2";
            label2.Size = new Size(493, 20);
            label2.TabIndex = 13;
            label2.Text = "---------   ここから下は使用不可   ---------   ここから下は使用不可   -----------";
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(931, 563);
            Controls.Add(label2);
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
        private Label label2;
    }
}
