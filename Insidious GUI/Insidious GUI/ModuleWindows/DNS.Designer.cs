namespace Insidious_GUI.ModuleWindows
{
    partial class DNS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DNS));
            addressCheckedListBox = new CheckedListBox();
            panel1 = new Panel();
            forwardCheckedButton = new Button();
            forwardAllButton = new Button();
            getLocalIPButon = new Button();
            addIpButton = new Button();
            label1 = new Label();
            forwardAddressTextBox = new TextBox();
            spoofAddressTextBox = new TextBox();
            stopButton = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // addressCheckedListBox
            // 
            addressCheckedListBox.FormattingEnabled = true;
            addressCheckedListBox.Location = new Point(4, 66);
            addressCheckedListBox.Name = "addressCheckedListBox";
            addressCheckedListBox.Size = new Size(180, 368);
            addressCheckedListBox.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(stopButton);
            panel1.Controls.Add(forwardCheckedButton);
            panel1.Controls.Add(forwardAllButton);
            panel1.Controls.Add(getLocalIPButon);
            panel1.Controls.Add(addIpButton);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(forwardAddressTextBox);
            panel1.Controls.Add(spoofAddressTextBox);
            panel1.Controls.Add(addressCheckedListBox);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(427, 565);
            panel1.TabIndex = 2;
            // 
            // forwardCheckedButton
            // 
            forwardCheckedButton.BackColor = Color.LightGray;
            forwardCheckedButton.FlatAppearance.BorderColor = Color.Maroon;
            forwardCheckedButton.FlatAppearance.BorderSize = 3;
            forwardCheckedButton.FlatStyle = FlatStyle.Flat;
            forwardCheckedButton.Location = new Point(233, 192);
            forwardCheckedButton.Name = "forwardCheckedButton";
            forwardCheckedButton.Size = new Size(180, 83);
            forwardCheckedButton.TabIndex = 6;
            forwardCheckedButton.Text = "Forward Checked Queries to IP";
            forwardCheckedButton.UseVisualStyleBackColor = false;
            forwardCheckedButton.Click += forwardCheckedButton_Click;
            // 
            // forwardAllButton
            // 
            forwardAllButton.BackColor = Color.LightGray;
            forwardAllButton.FlatAppearance.BorderColor = Color.Maroon;
            forwardAllButton.FlatAppearance.BorderSize = 3;
            forwardAllButton.FlatStyle = FlatStyle.Flat;
            forwardAllButton.Location = new Point(233, 66);
            forwardAllButton.Name = "forwardAllButton";
            forwardAllButton.Size = new Size(180, 83);
            forwardAllButton.TabIndex = 3;
            forwardAllButton.Text = "Forward ANY Queries to IP";
            forwardAllButton.UseVisualStyleBackColor = false;
            forwardAllButton.Click += forwardAllButton_Click;
            // 
            // getLocalIPButon
            // 
            getLocalIPButon.BackColor = Color.LightGray;
            getLocalIPButon.FlatAppearance.BorderColor = Color.Maroon;
            getLocalIPButon.FlatAppearance.BorderSize = 3;
            getLocalIPButon.FlatStyle = FlatStyle.Flat;
            getLocalIPButon.Location = new Point(233, 369);
            getLocalIPButon.Name = "getLocalIPButon";
            getLocalIPButon.Size = new Size(180, 65);
            getLocalIPButon.TabIndex = 5;
            getLocalIPButon.Text = "Get Local IP";
            getLocalIPButon.UseVisualStyleBackColor = false;
            getLocalIPButon.Click += getLocalIPButon_Click;
            // 
            // addIpButton
            // 
            addIpButton.BackColor = Color.LightGray;
            addIpButton.FlatAppearance.BorderColor = Color.Maroon;
            addIpButton.FlatAppearance.BorderSize = 3;
            addIpButton.FlatStyle = FlatStyle.Flat;
            addIpButton.Location = new Point(4, 497);
            addIpButton.Name = "addIpButton";
            addIpButton.Size = new Size(180, 65);
            addIpButton.TabIndex = 4;
            addIpButton.Text = "Add IP";
            addIpButton.UseVisualStyleBackColor = false;
            addIpButton.Click += addIpButton_Click;
            // 
            // label1
            // 
            label1.ForeColor = Color.WhiteSmoke;
            label1.Location = new Point(0, -3);
            label1.Name = "label1";
            label1.Size = new Size(184, 50);
            label1.TabIndex = 1;
            label1.Text = "Address(s) to Spoof";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // forwardAddressTextBox
            // 
            forwardAddressTextBox.Location = new Point(233, 155);
            forwardAddressTextBox.Name = "forwardAddressTextBox";
            forwardAddressTextBox.Size = new Size(180, 31);
            forwardAddressTextBox.TabIndex = 3;
            forwardAddressTextBox.TextChanged += forwardIPTextBoxTextChanged;
            // 
            // spoofAddressTextBox
            // 
            spoofAddressTextBox.Location = new Point(4, 459);
            spoofAddressTextBox.Name = "spoofAddressTextBox";
            spoofAddressTextBox.Size = new Size(180, 31);
            spoofAddressTextBox.TabIndex = 2;
            // 
            // stopButton
            // 
            stopButton.BackColor = Color.LightGray;
            stopButton.FlatAppearance.BorderColor = Color.Maroon;
            stopButton.FlatAppearance.BorderSize = 3;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.Location = new Point(233, 459);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(180, 65);
            stopButton.TabIndex = 7;
            stopButton.Text = "Stop Spoofing";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // DNS
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(451, 589);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DNS";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DNS";
            Load += DNS_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private CheckedListBox addressCheckedListBox;
        private Panel panel1;
        private Label label1;
        private Button forwardAllButton;
        private Button getLocalIPButon;
        private Button addIpButton;
        private TextBox forwardAddressTextBox;
        private TextBox spoofAddressTextBox;
        private Button forwardCheckedButton;
        private Button stopButton;
    }
}