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
            checkedListBox1 = new CheckedListBox();
            checkedListBox2 = new CheckedListBox();
            panel1 = new Panel();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "*" });
            checkedListBox1.Location = new Point(0, 62);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(180, 368);
            checkedListBox1.TabIndex = 0;
            // 
            // checkedListBox2
            // 
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Location = new Point(420, 50);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(180, 368);
            checkedListBox2.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(checkedListBox1);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(300, 602);
            panel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.ForeColor = Color.WhiteSmoke;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(207, 40);
            label1.TabIndex = 1;
            label1.Text = "Address to Spoof";
            // 
            // DNS
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(800, 718);
            Controls.Add(panel1);
            Controls.Add(checkedListBox2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DNS";
            Text = "DNS";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CheckedListBox checkedListBox1;
        private CheckedListBox checkedListBox2;
        private Panel panel1;
        private Label label1;
    }
}