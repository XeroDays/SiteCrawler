namespace SiteCrawlerAdvance
{
    partial class MainMenu
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
            txtBaseUrl = new TextBox();
            btnStart = new Button();
            panel1 = new Panel();
            label1 = new Label();
            numericGroupPages = new NumericUpDown();
            txtSuccess = new TextBox();
            txtFailed = new TextBox();
            txtUrls = new TextBox();
            panel2 = new Panel();
            lblTotalFailed = new Label();
            label6 = new Label();
            lblTotalSuccess = new Label();
            label5 = new Label();
            lblTotalUrlFound = new Label();
            label3 = new Label();
            label2 = new Label();
            label4 = new Label();
            numericCrawlPages = new NumericUpDown();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericGroupPages).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericCrawlPages).BeginInit();
            SuspendLayout();
            // 
            // txtBaseUrl
            // 
            txtBaseUrl.BorderStyle = BorderStyle.FixedSingle;
            txtBaseUrl.Location = new Point(14, 157);
            txtBaseUrl.Margin = new Padding(3, 4, 3, 4);
            txtBaseUrl.Multiline = true;
            txtBaseUrl.Name = "txtBaseUrl";
            txtBaseUrl.Size = new Size(162, 223);
            txtBaseUrl.TabIndex = 0;
            txtBaseUrl.Text = "https://beta.cbd.ae";
            txtBaseUrl.TextChanged += txtBaseUrl_TextChanged;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Lime;
            btnStart.FlatAppearance.BorderColor = Color.Green;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.ForeColor = Color.FromArgb(64, 64, 64);
            btnStart.Location = new Point(10, 389);
            btnStart.Margin = new Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(162, 31);
            btnStart.TabIndex = 1;
            btnStart.Text = "Intiate";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label4);
            panel1.Controls.Add(numericCrawlPages);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(numericGroupPages);
            panel1.ForeColor = Color.FromArgb(64, 64, 64);
            panel1.Location = new Point(14, 16);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(162, 133);
            panel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 11);
            label1.Name = "label1";
            label1.Size = new Size(75, 20);
            label1.TabIndex = 1;
            label1.Text = "Group Set";
            // 
            // numbericCount
            // 
            numericGroupPages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericGroupPages.Location = new Point(85, 8);
            numericGroupPages.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericGroupPages.Name = "numbericCount";
            numericGroupPages.Size = new Size(73, 27);
            numericGroupPages.TabIndex = 0;
            numericGroupPages.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // txtSuccess
            // 
            txtSuccess.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtSuccess.Font = new Font("Segoe UI", 7.8F);
            txtSuccess.Location = new Point(182, 13);
            txtSuccess.Multiline = true;
            txtSuccess.Name = "txtSuccess";
            txtSuccess.Size = new Size(351, 623);
            txtSuccess.TabIndex = 3;
            // 
            // txtFailed
            // 
            txtFailed.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtFailed.Font = new Font("Segoe UI", 7.8F);
            txtFailed.ForeColor = Color.Red;
            txtFailed.Location = new Point(541, 13);
            txtFailed.Multiline = true;
            txtFailed.Name = "txtFailed";
            txtFailed.Size = new Size(313, 623);
            txtFailed.TabIndex = 4;
            // 
            // txtUrls
            // 
            txtUrls.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtUrls.Font = new Font("Segoe UI", 7.8F);
            txtUrls.ForeColor = Color.FromArgb(0, 192, 0);
            txtUrls.Location = new Point(861, 13);
            txtUrls.Multiline = true;
            txtUrls.Name = "txtUrls";
            txtUrls.Size = new Size(503, 623);
            txtUrls.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(lblTotalFailed);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(lblTotalSuccess);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(lblTotalUrlFound);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(11, 427);
            panel2.Name = "panel2";
            panel2.Size = new Size(164, 121);
            panel2.TabIndex = 6;
            // 
            // lblTotalFailed
            // 
            lblTotalFailed.AutoSize = true;
            lblTotalFailed.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalFailed.Location = new Point(91, 83);
            lblTotalFailed.Name = "lblTotalFailed";
            lblTotalFailed.Size = new Size(18, 20);
            lblTotalFailed.TabIndex = 8;
            lblTotalFailed.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(1, 83);
            label6.Name = "label6";
            label6.Size = new Size(91, 20);
            label6.TabIndex = 7;
            label6.Text = "Total failed :";
            // 
            // lblTotalSuccess
            // 
            lblTotalSuccess.AutoSize = true;
            lblTotalSuccess.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalSuccess.Location = new Point(103, 59);
            lblTotalSuccess.Name = "lblTotalSuccess";
            lblTotalSuccess.Size = new Size(18, 20);
            lblTotalSuccess.TabIndex = 6;
            lblTotalSuccess.Text = "0";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(1, 59);
            label5.Name = "label5";
            label5.Size = new Size(103, 20);
            label5.TabIndex = 5;
            label5.Text = "Total Success :";
            // 
            // lblTotalUrlFound
            // 
            lblTotalUrlFound.AutoSize = true;
            lblTotalUrlFound.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalUrlFound.Location = new Point(118, 36);
            lblTotalUrlFound.Name = "lblTotalUrlFound";
            lblTotalUrlFound.Size = new Size(18, 20);
            lblTotalUrlFound.TabIndex = 4;
            lblTotalUrlFound.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(-1, 36);
            label3.Name = "label3";
            label3.Size = new Size(122, 20);
            label3.TabIndex = 3;
            label3.Text = "Total URL found :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(2, 3);
            label2.Name = "label2";
            label2.Size = new Size(53, 20);
            label2.TabIndex = 2;
            label2.Text = "Status";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 44);
            label4.Name = "label4";
            label4.Size = new Size(88, 20);
            label4.TabIndex = 3;
            label4.Text = "Crawl Pages";
            // 
            // numericCrawlImages
            // 
            numericCrawlPages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericCrawlPages.Location = new Point(97, 41);
            numericCrawlPages.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericCrawlPages.Name = "numericCrawlImages";
            numericCrawlPages.Size = new Size(61, 27);
            numericCrawlPages.TabIndex = 2;
            numericCrawlPages.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1370, 645);
            Controls.Add(panel2);
            Controls.Add(txtUrls);
            Controls.Add(txtFailed);
            Controls.Add(txtSuccess);
            Controls.Add(panel1);
            Controls.Add(btnStart);
            Controls.Add(txtBaseUrl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainMenu";
            Text = "MainMenu";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericGroupPages).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericCrawlPages).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtBaseUrl;
        private Button btnStart;
        private Panel panel1;
        private Label label1;
        private NumericUpDown numericGroupPages;
        private TextBox txtSuccess;
        private TextBox txtFailed;
        private TextBox txtUrls;
        private Panel panel2;
        private Label lblTotalUrlFound;
        private Label label3;
        private Label label2;
        private Label lblTotalSuccess;
        private Label label5;
        private Label lblTotalFailed;
        private Label label6;
        private Label label4;
        private NumericUpDown numericCrawlPages;
    }
}