namespace Insidious_GUI
{
    partial class Mitm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mitm));
            listBox1 = new ListBox();
            deauthAllButton = new Button();
            button1 = new Button();
            button4 = new Button();
            button2 = new Button();
            button3 = new Button();
            button5 = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.DarkGray;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 25;
            listBox1.Location = new Point(12, 92);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(180, 329);
            listBox1.TabIndex = 2;
            // 
            // deauthAllButton
            // 
            deauthAllButton.BackColor = Color.LightGray;
            deauthAllButton.FlatAppearance.BorderColor = Color.Maroon;
            deauthAllButton.FlatAppearance.BorderSize = 3;
            deauthAllButton.FlatStyle = FlatStyle.Flat;
            deauthAllButton.Location = new Point(12, 12);
            deauthAllButton.Name = "deauthAllButton";
            deauthAllButton.Size = new Size(181, 74);
            deauthAllButton.TabIndex = 4;
            deauthAllButton.Text = "Scan For Addresses";
            deauthAllButton.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.BackColor = Color.LightGray;
            button1.FlatAppearance.BorderColor = Color.Maroon;
            button1.FlatAppearance.BorderSize = 3;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(228, 18);
            button1.Name = "button1";
            button1.Size = new Size(153, 74);
            button1.TabIndex = 5;
            button1.Text = "Poison All";
            button1.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.BackColor = Color.LightGray;
            button4.FlatAppearance.BorderColor = Color.Maroon;
            button4.FlatAppearance.BorderSize = 3;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(228, 98);
            button4.Name = "button4";
            button4.Size = new Size(153, 74);
            button4.TabIndex = 6;
            button4.Text = "Poison Selected IP";
            button4.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.LightGray;
            button2.FlatAppearance.BorderColor = Color.Maroon;
            button2.FlatAppearance.BorderSize = 3;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(228, 338);
            button2.Name = "button2";
            button2.Size = new Size(153, 74);
            button2.TabIndex = 7;
            button2.Text = "Restore";
            button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.LightGray;
            button3.FlatAppearance.BorderColor = Color.Maroon;
            button3.FlatAppearance.BorderSize = 3;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(228, 178);
            button3.Name = "button3";
            button3.Size = new Size(153, 74);
            button3.TabIndex = 8;
            button3.Text = "DDOS";
            button3.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            button5.BackColor = Color.LightGray;
            button5.FlatAppearance.BorderColor = Color.Maroon;
            button5.FlatAppearance.BorderSize = 3;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(228, 258);
            button5.Name = "button5";
            button5.Size = new Size(153, 74);
            button5.TabIndex = 9;
            button5.Text = "Packet Passthrough";
            button5.UseVisualStyleBackColor = false;
            // 
            // Mitm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(410, 433);
            Controls.Add(button5);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button4);
            Controls.Add(button1);
            Controls.Add(deauthAllButton);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Mitm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mitm";
            ResumeLayout(false);
        }

        #endregion
        private ListBox listBox1;
        private Button deauthAllButton;
        private Button button1;
        private Button button4;
        private Button button2;
        private Button button3;
        private Button button5;
    }
}