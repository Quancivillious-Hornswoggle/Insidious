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
            scanDevicesButton = new Button();
            poisonAllButton = new Button();
            poisonSelectedButton = new Button();
            restoreButtn = new Button();
            doSButton = new Button();
            packetPassthroughButton = new Button();
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
            // scanDevicesButton
            // 
            scanDevicesButton.BackColor = Color.LightGray;
            scanDevicesButton.FlatAppearance.BorderColor = Color.Maroon;
            scanDevicesButton.FlatAppearance.BorderSize = 3;
            scanDevicesButton.FlatStyle = FlatStyle.Flat;
            scanDevicesButton.Location = new Point(12, 12);
            scanDevicesButton.Name = "scanDevicesButton";
            scanDevicesButton.Size = new Size(181, 74);
            scanDevicesButton.TabIndex = 4;
            scanDevicesButton.Text = "Scan For Addresses";
            scanDevicesButton.UseVisualStyleBackColor = false;
            scanDevicesButton.Click += scanDevicesButton_Click;
            // 
            // poisonAllButton
            // 
            poisonAllButton.BackColor = Color.LightGray;
            poisonAllButton.FlatAppearance.BorderColor = Color.Maroon;
            poisonAllButton.FlatAppearance.BorderSize = 3;
            poisonAllButton.FlatStyle = FlatStyle.Flat;
            poisonAllButton.Location = new Point(228, 18);
            poisonAllButton.Name = "poisonAllButton";
            poisonAllButton.Size = new Size(153, 74);
            poisonAllButton.TabIndex = 5;
            poisonAllButton.Text = "Poison All";
            poisonAllButton.UseVisualStyleBackColor = false;
            // 
            // poisonSelectedButton
            // 
            poisonSelectedButton.BackColor = Color.LightGray;
            poisonSelectedButton.FlatAppearance.BorderColor = Color.Maroon;
            poisonSelectedButton.FlatAppearance.BorderSize = 3;
            poisonSelectedButton.FlatStyle = FlatStyle.Flat;
            poisonSelectedButton.Location = new Point(228, 98);
            poisonSelectedButton.Name = "poisonSelectedButton";
            poisonSelectedButton.Size = new Size(153, 74);
            poisonSelectedButton.TabIndex = 6;
            poisonSelectedButton.Text = "Poison Selected IP";
            poisonSelectedButton.UseVisualStyleBackColor = false;
            // 
            // restoreButtn
            // 
            restoreButtn.BackColor = Color.LightGray;
            restoreButtn.FlatAppearance.BorderColor = Color.Maroon;
            restoreButtn.FlatAppearance.BorderSize = 3;
            restoreButtn.FlatStyle = FlatStyle.Flat;
            restoreButtn.Location = new Point(228, 338);
            restoreButtn.Name = "restoreButtn";
            restoreButtn.Size = new Size(153, 74);
            restoreButtn.TabIndex = 7;
            restoreButtn.Text = "Restore";
            restoreButtn.UseVisualStyleBackColor = false;
            // 
            // doSButton
            // 
            doSButton.BackColor = Color.LightGray;
            doSButton.FlatAppearance.BorderColor = Color.Maroon;
            doSButton.FlatAppearance.BorderSize = 3;
            doSButton.FlatStyle = FlatStyle.Flat;
            doSButton.Location = new Point(228, 178);
            doSButton.Name = "doSButton";
            doSButton.Size = new Size(153, 74);
            doSButton.TabIndex = 8;
            doSButton.Text = "DDOS";
            doSButton.UseVisualStyleBackColor = false;
            // 
            // packetPassthroughButton
            // 
            packetPassthroughButton.BackColor = Color.LightGray;
            packetPassthroughButton.FlatAppearance.BorderColor = Color.Maroon;
            packetPassthroughButton.FlatAppearance.BorderSize = 3;
            packetPassthroughButton.FlatStyle = FlatStyle.Flat;
            packetPassthroughButton.Location = new Point(228, 258);
            packetPassthroughButton.Name = "packetPassthroughButton";
            packetPassthroughButton.Size = new Size(153, 74);
            packetPassthroughButton.TabIndex = 9;
            packetPassthroughButton.Text = "Packet Passthrough";
            packetPassthroughButton.UseVisualStyleBackColor = false;
            // 
            // Mitm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(410, 433);
            Controls.Add(packetPassthroughButton);
            Controls.Add(doSButton);
            Controls.Add(restoreButtn);
            Controls.Add(poisonSelectedButton);
            Controls.Add(poisonAllButton);
            Controls.Add(scanDevicesButton);
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
        private Button scanDevicesButton;
        private Button poisonAllButton;
        private Button poisonSelectedButton;
        private Button restoreButtn;
        private Button doSButton;
        private Button packetPassthroughButton;
    }
}