namespace Insidious_GUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            consolePanel = new Panel();
            consoleTextBox = new TextBox();
            consoleLabel = new Label();
            wifiBruteButton = new Button();
            panel1 = new Panel();
            label1 = new Label();
            panel2 = new Panel();
            label2 = new Label();
            wifiDeauthButton = new Button();
            panel3 = new Panel();
            label3 = new Label();
            dnsSpoofButton = new Button();
            panel4 = new Panel();
            label4 = new Label();
            serverHostButton = new Button();
            panel5 = new Panel();
            label5 = new Label();
            mitmButton = new Button();
            panel6 = new Panel();
            label6 = new Label();
            packetCaptureButton = new Button();
            consolePanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // consolePanel
            // 
            consolePanel.BackColor = Color.Black;
            consolePanel.Controls.Add(consoleTextBox);
            consolePanel.Controls.Add(consoleLabel);
            consolePanel.Location = new Point(12, 12);
            consolePanel.Name = "consolePanel";
            consolePanel.Size = new Size(300, 643);
            consolePanel.TabIndex = 1;
            // 
            // consoleTextBox
            // 
            consoleTextBox.BackColor = Color.DimGray;
            consoleTextBox.ForeColor = Color.WhiteSmoke;
            consoleTextBox.Location = new Point(3, 69);
            consoleTextBox.Multiline = true;
            consoleTextBox.Name = "consoleTextBox";
            consoleTextBox.Size = new Size(297, 571);
            consoleTextBox.TabIndex = 2;
            // 
            // consoleLabel
            // 
            consoleLabel.BackColor = Color.Transparent;
            consoleLabel.Font = new Font("Georgia", 15F);
            consoleLabel.ForeColor = Color.WhiteSmoke;
            consoleLabel.Location = new Point(3, 0);
            consoleLabel.Name = "consoleLabel";
            consoleLabel.Size = new Size(294, 66);
            consoleLabel.TabIndex = 1;
            consoleLabel.Text = "Console";
            consoleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // wifiBruteButton
            // 
            wifiBruteButton.BackColor = Color.DarkGray;
            wifiBruteButton.FlatAppearance.BorderColor = Color.Maroon;
            wifiBruteButton.FlatAppearance.BorderSize = 3;
            wifiBruteButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            wifiBruteButton.FlatStyle = FlatStyle.Flat;
            wifiBruteButton.Font = new Font("Verdana", 9F);
            wifiBruteButton.Location = new Point(25, 61);
            wifiBruteButton.Name = "wifiBruteButton";
            wifiBruteButton.Size = new Size(241, 81);
            wifiBruteButton.TabIndex = 3;
            wifiBruteButton.Text = "Open Window";
            wifiBruteButton.UseVisualStyleBackColor = false;
            wifiBruteButton.Click += wifiBruteButton_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(wifiBruteButton);
            panel1.Location = new Point(344, 37);
            panel1.Name = "panel1";
            panel1.Size = new Size(291, 179);
            panel1.TabIndex = 8;
            // 
            // label1
            // 
            label1.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(288, 55);
            label1.TabIndex = 4;
            label1.Text = "Wi-Fi Cracking";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.DimGray;
            panel2.Controls.Add(label2);
            panel2.Controls.Add(wifiDeauthButton);
            panel2.Location = new Point(666, 37);
            panel2.Name = "panel2";
            panel2.Size = new Size(291, 179);
            panel2.TabIndex = 9;
            // 
            // label2
            // 
            label2.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(288, 55);
            label2.TabIndex = 4;
            label2.Text = "Wi-Fi Deauth";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // wifiDeauthButton
            // 
            wifiDeauthButton.BackColor = Color.DarkGray;
            wifiDeauthButton.FlatAppearance.BorderColor = Color.Maroon;
            wifiDeauthButton.FlatAppearance.BorderSize = 3;
            wifiDeauthButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            wifiDeauthButton.FlatStyle = FlatStyle.Flat;
            wifiDeauthButton.Font = new Font("Verdana", 9F);
            wifiDeauthButton.Location = new Point(25, 61);
            wifiDeauthButton.Name = "wifiDeauthButton";
            wifiDeauthButton.Size = new Size(241, 81);
            wifiDeauthButton.TabIndex = 3;
            wifiDeauthButton.Text = "Open Window";
            wifiDeauthButton.UseVisualStyleBackColor = false;
            wifiDeauthButton.Click += wifiDeauthButton_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.DimGray;
            panel3.Controls.Add(label3);
            panel3.Controls.Add(dnsSpoofButton);
            panel3.Location = new Point(344, 248);
            panel3.Name = "panel3";
            panel3.Size = new Size(291, 179);
            panel3.TabIndex = 10;
            // 
            // label3
            // 
            label3.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(288, 55);
            label3.TabIndex = 4;
            label3.Text = "DNS Spoof";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dnsSpoofButton
            // 
            dnsSpoofButton.BackColor = Color.DarkGray;
            dnsSpoofButton.FlatAppearance.BorderColor = Color.Maroon;
            dnsSpoofButton.FlatAppearance.BorderSize = 3;
            dnsSpoofButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            dnsSpoofButton.FlatStyle = FlatStyle.Flat;
            dnsSpoofButton.Font = new Font("Verdana", 9F);
            dnsSpoofButton.Location = new Point(25, 61);
            dnsSpoofButton.Name = "dnsSpoofButton";
            dnsSpoofButton.Size = new Size(241, 81);
            dnsSpoofButton.TabIndex = 3;
            dnsSpoofButton.Text = "Open Window";
            dnsSpoofButton.UseVisualStyleBackColor = false;
            dnsSpoofButton.Click += dnsSpoofButton_Click;
            // 
            // panel4
            // 
            panel4.BackColor = Color.DimGray;
            panel4.Controls.Add(label4);
            panel4.Controls.Add(serverHostButton);
            panel4.Location = new Point(666, 248);
            panel4.Name = "panel4";
            panel4.Size = new Size(291, 179);
            panel4.TabIndex = 11;
            // 
            // label4
            // 
            label4.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 0);
            label4.Name = "label4";
            label4.Size = new Size(288, 55);
            label4.TabIndex = 4;
            label4.Text = "Web Server Hoster";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // serverHostButton
            // 
            serverHostButton.BackColor = Color.DarkGray;
            serverHostButton.FlatAppearance.BorderColor = Color.Maroon;
            serverHostButton.FlatAppearance.BorderSize = 3;
            serverHostButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            serverHostButton.FlatStyle = FlatStyle.Flat;
            serverHostButton.Font = new Font("Verdana", 9F);
            serverHostButton.Location = new Point(25, 61);
            serverHostButton.Name = "serverHostButton";
            serverHostButton.Size = new Size(241, 81);
            serverHostButton.TabIndex = 3;
            serverHostButton.Text = "Open Window";
            serverHostButton.UseVisualStyleBackColor = false;
            serverHostButton.Click += serverHostButton_Click;
            // 
            // panel5
            // 
            panel5.BackColor = Color.DimGray;
            panel5.Controls.Add(label5);
            panel5.Controls.Add(mitmButton);
            panel5.Location = new Point(347, 455);
            panel5.Name = "panel5";
            panel5.Size = new Size(291, 179);
            panel5.TabIndex = 11;
            // 
            // label5
            // 
            label5.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(288, 55);
            label5.TabIndex = 4;
            label5.Text = "MITM";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // mitmButton
            // 
            mitmButton.BackColor = Color.DarkGray;
            mitmButton.FlatAppearance.BorderColor = Color.Maroon;
            mitmButton.FlatAppearance.BorderSize = 3;
            mitmButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            mitmButton.FlatStyle = FlatStyle.Flat;
            mitmButton.Font = new Font("Verdana", 9F);
            mitmButton.Location = new Point(25, 61);
            mitmButton.Name = "mitmButton";
            mitmButton.Size = new Size(241, 81);
            mitmButton.TabIndex = 3;
            mitmButton.Text = "Open Window";
            mitmButton.UseVisualStyleBackColor = false;
            mitmButton.Click += mitmButton_Click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.DimGray;
            panel6.Controls.Add(label6);
            panel6.Controls.Add(packetCaptureButton);
            panel6.Location = new Point(669, 455);
            panel6.Name = "panel6";
            panel6.Size = new Size(291, 179);
            panel6.TabIndex = 12;
            // 
            // label6
            // 
            label6.Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(3, 0);
            label6.Name = "label6";
            label6.Size = new Size(288, 55);
            label6.TabIndex = 4;
            label6.Text = "Packet Capture";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // packetCaptureButton
            // 
            packetCaptureButton.BackColor = Color.DarkGray;
            packetCaptureButton.FlatAppearance.BorderColor = Color.Maroon;
            packetCaptureButton.FlatAppearance.BorderSize = 3;
            packetCaptureButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            packetCaptureButton.FlatStyle = FlatStyle.Flat;
            packetCaptureButton.Font = new Font("Verdana", 9F);
            packetCaptureButton.Location = new Point(25, 61);
            packetCaptureButton.Name = "packetCaptureButton";
            packetCaptureButton.Size = new Size(241, 81);
            packetCaptureButton.TabIndex = 3;
            packetCaptureButton.Text = "Open Window";
            packetCaptureButton.UseVisualStyleBackColor = false;
            packetCaptureButton.Click += packetCaptureButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1000, 667);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(consolePanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Insidious";
            Load += Form1_Load;
            consolePanel.ResumeLayout(false);
            consolePanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel consolePanel;
        private Label consoleLabel;
        private TextBox consoleTextBox;
        private Button wifiBruteButton;
        private Panel panel1;
        private Label label1;
        private Panel panel2;
        private Label label2;
        private Button wifiDeauthButton;
        private Panel panel3;
        private Label label3;
        private Button dnsSpoofButton;
        private Panel panel4;
        private Label label4;
        private Button serverHostButton;
        private Panel panel5;
        private Label label5;
        private Button mitmButton;
        private Panel panel6;
        private Label label6;
        private Button packetCaptureButton;
    }
}
