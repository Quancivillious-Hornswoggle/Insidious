namespace Insidious_GUI
{
    partial class Deauth
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Deauth));
            deauthNetworkButton = new Button();
            deauthSingleButton = new Button();
            stopDeauthButton = new Button();
            panel1 = new Panel();
            scanAPButton = new Button();
            apListBox = new ListBox();
            panel2 = new Panel();
            checkedListBox1 = new CheckedListBox();
            button1 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // deauthNetworkButton
            // 
            deauthNetworkButton.BackColor = Color.LightGray;
            deauthNetworkButton.FlatAppearance.BorderColor = Color.Maroon;
            deauthNetworkButton.FlatAppearance.BorderSize = 3;
            deauthNetworkButton.FlatStyle = FlatStyle.Flat;
            deauthNetworkButton.Location = new Point(312, 52);
            deauthNetworkButton.Name = "deauthNetworkButton";
            deauthNetworkButton.Size = new Size(225, 85);
            deauthNetworkButton.TabIndex = 0;
            deauthNetworkButton.Text = "Deauth All Users On Selected Network";
            deauthNetworkButton.UseVisualStyleBackColor = false;
            // 
            // deauthSingleButton
            // 
            deauthSingleButton.BackColor = Color.LightGray;
            deauthSingleButton.FlatAppearance.BorderColor = Color.Maroon;
            deauthSingleButton.FlatAppearance.BorderSize = 3;
            deauthSingleButton.FlatStyle = FlatStyle.Flat;
            deauthSingleButton.Location = new Point(312, 308);
            deauthSingleButton.Name = "deauthSingleButton";
            deauthSingleButton.Size = new Size(225, 102);
            deauthSingleButton.TabIndex = 1;
            deauthSingleButton.Text = "Deauth Selected User/Users On Selected Network";
            deauthSingleButton.UseVisualStyleBackColor = false;
            // 
            // stopDeauthButton
            // 
            stopDeauthButton.BackColor = Color.LightGray;
            stopDeauthButton.FlatAppearance.BorderColor = Color.Maroon;
            stopDeauthButton.FlatAppearance.BorderSize = 3;
            stopDeauthButton.FlatStyle = FlatStyle.Flat;
            stopDeauthButton.Location = new Point(312, 594);
            stopDeauthButton.Name = "stopDeauthButton";
            stopDeauthButton.Size = new Size(225, 85);
            stopDeauthButton.TabIndex = 3;
            stopDeauthButton.Text = "Stop";
            stopDeauthButton.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Controls.Add(scanAPButton);
            panel1.Controls.Add(apListBox);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(255, 723);
            panel1.TabIndex = 4;
            // 
            // scanAPButton
            // 
            scanAPButton.BackColor = Color.LightGray;
            scanAPButton.FlatAppearance.BorderColor = Color.Maroon;
            scanAPButton.FlatAppearance.BorderSize = 3;
            scanAPButton.FlatStyle = FlatStyle.Flat;
            scanAPButton.Location = new Point(3, 3);
            scanAPButton.Name = "scanAPButton";
            scanAPButton.Size = new Size(250, 85);
            scanAPButton.TabIndex = 5;
            scanAPButton.Text = "Scan AP's";
            scanAPButton.UseVisualStyleBackColor = false;
            scanAPButton.Click += scanAPButton_Click;
            // 
            // apListBox
            // 
            apListBox.FormattingEnabled = true;
            apListBox.ItemHeight = 25;
            apListBox.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" });
            apListBox.Location = new Point(0, 94);
            apListBox.Name = "apListBox";
            apListBox.Size = new Size(253, 629);
            apListBox.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.DimGray;
            panel2.Controls.Add(checkedListBox1);
            panel2.Controls.Add(button1);
            panel2.Location = new Point(582, 10);
            panel2.Name = "panel2";
            panel2.Size = new Size(255, 723);
            panel2.TabIndex = 6;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(0, 96);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(253, 620);
            checkedListBox1.TabIndex = 7;
            // 
            // button1
            // 
            button1.BackColor = Color.LightGray;
            button1.FlatAppearance.BorderColor = Color.Maroon;
            button1.FlatAppearance.BorderSize = 3;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(250, 85);
            button1.TabIndex = 5;
            button1.Text = "Scan For Devices Connected To Selected AP";
            button1.UseVisualStyleBackColor = false;
            // 
            // Deauth
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(849, 745);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(stopDeauthButton);
            Controls.Add(deauthSingleButton);
            Controls.Add(deauthNetworkButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Deauth";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Deauth";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button deauthNetworkButton;
        private Button deauthSingleButton;
        private Button stopDeauthButton;
        private Panel panel1;
        private Button scanAPButton;
        private ListBox apListBox;
        private Panel panel2;
        private CheckedListBox checkedListBox1;
        private Button button1;
    }
}