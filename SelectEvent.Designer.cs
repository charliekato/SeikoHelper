namespace SeikoHelper
{
    partial class SelectEvent
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
            listEvent = new ListBox();
            btnSelect = new Button();
            btnQuit = new Button();
            SuspendLayout();
            // 
            // listEvent
            // 
            listEvent.FormattingEnabled = true;
            listEvent.ItemHeight = 25;
            listEvent.Location = new Point(27, 16);
            listEvent.Margin = new Padding(2);
            listEvent.Name = "listEvent";
            listEvent.Size = new Size(1582, 679);
            listEvent.TabIndex = 0;
            // 
            // btnSelect
            // 
            btnSelect.Location = new Point(456, 751);
            btnSelect.Margin = new Padding(2);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(84, 57);
            btnSelect.TabIndex = 1;
            btnSelect.Text = "選択";
            btnSelect.UseVisualStyleBackColor = true;
            btnSelect.Click += btnSelect_Click;
            // 
            // btnQuit
            // 
            btnQuit.Location = new Point(1023, 742);
            btnQuit.Margin = new Padding(2);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new Size(89, 57);
            btnQuit.TabIndex = 2;
            btnQuit.Text = "終了";
            btnQuit.UseVisualStyleBackColor = true;
            btnQuit.Click += btnQuit_Click;
            // 
            // SelectEvent
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1633, 846);
            Controls.Add(btnQuit);
            Controls.Add(btnSelect);
            Controls.Add(listEvent);
            Margin = new Padding(2);
            Name = "SelectEvent";
            Text = "競技選択  　　　一般社団法人 大津市水泳協会";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listEvent;
        private Button btnSelect;
        private Button btnQuit;
    }

}