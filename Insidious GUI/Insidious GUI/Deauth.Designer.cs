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
            deauthAllButton = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            deauthSingleButton = new Button();
            deauthIPInput = new TextBox();
            stopDeauthButton = new Button();
            SuspendLayout();
            // 
            // deauthAllButton
            // 
            deauthAllButton.BackColor = Color.LightGray;
            deauthAllButton.FlatAppearance.BorderColor = Color.Maroon;
            deauthAllButton.FlatAppearance.BorderSize = 3;
            deauthAllButton.FlatStyle = FlatStyle.Flat;
            deauthAllButton.Location = new Point(12, 12);
            deauthAllButton.Name = "deauthAllButton";
            deauthAllButton.Size = new Size(225, 85);
            deauthAllButton.TabIndex = 0;
            deauthAllButton.Text = "Deauth All";
            deauthAllButton.UseVisualStyleBackColor = false;
            // 
            // deauthSingleButton
            // 
            deauthSingleButton.BackColor = Color.LightGray;
            deauthSingleButton.FlatAppearance.BorderColor = Color.Maroon;
            deauthSingleButton.FlatAppearance.BorderSize = 3;
            deauthSingleButton.FlatStyle = FlatStyle.Flat;
            deauthSingleButton.Location = new Point(13, 142);
            deauthSingleButton.Name = "deauthSingleButton";
            deauthSingleButton.Size = new Size(225, 85);
            deauthSingleButton.TabIndex = 1;
            deauthSingleButton.Text = "Deauth Single";
            deauthSingleButton.UseVisualStyleBackColor = false;
            // 
            // deauthIPInput
            // 
            deauthIPInput.Location = new Point(13, 233);
            deauthIPInput.Name = "deauthIPInput";
            deauthIPInput.Size = new Size(225, 31);
            deauthIPInput.TabIndex = 2;
            // 
            // stopDeauthButton
            // 
            stopDeauthButton.BackColor = Color.LightGray;
            stopDeauthButton.FlatAppearance.BorderColor = Color.Maroon;
            stopDeauthButton.FlatAppearance.BorderSize = 3;
            stopDeauthButton.FlatStyle = FlatStyle.Flat;
            stopDeauthButton.Location = new Point(13, 308);
            stopDeauthButton.Name = "stopDeauthButton";
            stopDeauthButton.Size = new Size(225, 85);
            stopDeauthButton.TabIndex = 3;
            stopDeauthButton.Text = "Stop";
            stopDeauthButton.UseVisualStyleBackColor = false;
            // 
            // Deauth
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(250, 408);
            Controls.Add(stopDeauthButton);
            Controls.Add(deauthIPInput);
            Controls.Add(deauthSingleButton);
            Controls.Add(deauthAllButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Deauth";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Deauth";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button deauthAllButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button deauthSingleButton;
        private TextBox deauthIPInput;
        private Button stopDeauthButton;
    }
}