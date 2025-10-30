namespace Insidious_GUI.ModuleWindows
{
    partial class Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            startServerButton = new Button();
            stopServerButton = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            SuspendLayout();
            // 
            // startServerButton
            // 
            startServerButton.BackColor = Color.LightGray;
            startServerButton.FlatAppearance.BorderColor = Color.Maroon;
            startServerButton.FlatAppearance.BorderSize = 3;
            startServerButton.FlatStyle = FlatStyle.Flat;
            startServerButton.Location = new Point(12, 12);
            startServerButton.Name = "startServerButton";
            startServerButton.Size = new Size(153, 74);
            startServerButton.TabIndex = 6;
            startServerButton.Text = "Start Web Server";
            startServerButton.UseVisualStyleBackColor = false;
            startServerButton.Click += startServerButton_Click;
            // 
            // stopServerButton
            // 
            stopServerButton.BackColor = Color.LightGray;
            stopServerButton.FlatAppearance.BorderColor = Color.Maroon;
            stopServerButton.FlatAppearance.BorderSize = 3;
            stopServerButton.FlatStyle = FlatStyle.Flat;
            stopServerButton.Location = new Point(177, 12);
            stopServerButton.Name = "stopServerButton";
            stopServerButton.Size = new Size(153, 74);
            stopServerButton.TabIndex = 7;
            stopServerButton.Text = "Stop Web Server";
            stopServerButton.UseVisualStyleBackColor = false;
            stopServerButton.Click += stopServerButton_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(342, 96);
            Controls.Add(stopServerButton);
            Controls.Add(startServerButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Server";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Web Server Hoster";
            FormClosing += Server_FormClosing;
            Load += Server_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button startServerButton;
        private Button stopServerButton;
        private FolderBrowserDialog folderBrowserDialog;
    }
}