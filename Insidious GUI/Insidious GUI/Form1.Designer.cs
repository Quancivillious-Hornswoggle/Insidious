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
            consoleLabel = new Label();
            consoleTextBox = new TextBox();
            button1 = new Button();
            consolePanel.SuspendLayout();
            SuspendLayout();
            // 
            // consolePanel
            // 
            consolePanel.BackColor = Color.Black;
            consolePanel.Controls.Add(consoleTextBox);
            consolePanel.Controls.Add(consoleLabel);
            consolePanel.Location = new Point(12, 12);
            consolePanel.Name = "consolePanel";
            consolePanel.Size = new Size(300, 720);
            consolePanel.TabIndex = 1;
            // 
            // consoleLabel
            // 
            consoleLabel.BackColor = Color.Transparent;
            consoleLabel.Font = new Font("Segoe UI", 15F);
            consoleLabel.ForeColor = Color.WhiteSmoke;
            consoleLabel.Location = new Point(3, 0);
            consoleLabel.Name = "consoleLabel";
            consoleLabel.Size = new Size(294, 66);
            consoleLabel.TabIndex = 1;
            consoleLabel.Text = "Console";
            consoleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // consoleTextBox
            // 
            consoleTextBox.BackColor = Color.DimGray;
            consoleTextBox.ForeColor = Color.WhiteSmoke;
            consoleTextBox.Location = new Point(3, 69);
            consoleTextBox.Multiline = true;
            consoleTextBox.Name = "consoleTextBox";
            consoleTextBox.Size = new Size(297, 648);
            consoleTextBox.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(593, 219);
            button1.Name = "button1";
            button1.Size = new Size(241, 169);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1228, 744);
            Controls.Add(button1);
            Controls.Add(consolePanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Insidious";
            Load += Form1_Load;
            consolePanel.ResumeLayout(false);
            consolePanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel consolePanel;
        private Label consoleLabel;
        private TextBox consoleTextBox;
        private Button button1;
    }
}
