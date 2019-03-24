using System.Diagnostics;

namespace pWindowJax
{
    partial class InfoBox
    {
        private System.Windows.Forms.Label pWindowJaxLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label forklabel;
        private System.Windows.Forms.LinkLabel ForkLink;
        private System.Windows.Forms.Label madebylabel;
        private System.Windows.Forms.LinkLabel madebyLink;
        private System.Windows.Forms.LinkLabel RepositoryLink;
        private System.Windows.Forms.Label repositorylabel;
        private System.Windows.Forms.Label licenseLabel;
        private System.Windows.Forms.Label licenseContentLabel;
        private System.Windows.Forms.Label versionLabel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoBox));
            this.pWindowJaxLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.forklabel = new System.Windows.Forms.Label();
            this.ForkLink = new System.Windows.Forms.LinkLabel();
            this.madebylabel = new System.Windows.Forms.Label();
            this.madebyLink = new System.Windows.Forms.LinkLabel();
            this.RepositoryLink = new System.Windows.Forms.LinkLabel();
            this.repositorylabel = new System.Windows.Forms.Label();
            this.licenseLabel = new System.Windows.Forms.Label();
            this.licenseContentLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pWindowJaxLabel
            // 
            this.pWindowJaxLabel.AutoSize = true;
            this.pWindowJaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pWindowJaxLabel.Location = new System.Drawing.Point(14, 13);
            this.pWindowJaxLabel.Name = "pWindowJaxLabel";
            this.pWindowJaxLabel.Size = new System.Drawing.Size(134, 25);
            this.pWindowJaxLabel.TabIndex = 0;
            this.pWindowJaxLabel.Text = "pWindowJax";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Alternate way of resizing and moving windows.";
            // 
            // forklabel
            // 
            this.forklabel.AutoSize = true;
            this.forklabel.Location = new System.Drawing.Point(12, 116);
            this.forklabel.Name = "forklabel";
            this.forklabel.Size = new System.Drawing.Size(66, 13);
            this.forklabel.TabIndex = 2;
            this.forklabel.Text = "Forked from:";
            // 
            // ForkLink
            // 
            this.ForkLink.AutoSize = true;
            this.ForkLink.Location = new System.Drawing.Point(113, 116);
            this.ForkLink.Name = "ForkLink";
            this.ForkLink.Size = new System.Drawing.Size(195, 13);
            this.ForkLink.TabIndex = 3;
            this.ForkLink.TabStop = true;
            this.ForkLink.Text = "https://github.com/peppy/pWindowJax";
            this.ForkLink.VisitedLinkColor = System.Drawing.Color.Blue;
            this.ForkLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // madebylabel
            // 
            this.madebylabel.AutoSize = true;
            this.madebylabel.Location = new System.Drawing.Point(12, 69);
            this.madebylabel.Name = "madebylabel";
            this.madebylabel.Size = new System.Drawing.Size(51, 13);
            this.madebylabel.TabIndex = 4;
            this.madebylabel.Text = "Made by:";
            // 
            // madebyLink
            // 
            this.madebyLink.AutoSize = true;
            this.madebyLink.Location = new System.Drawing.Point(113, 69);
            this.madebyLink.Name = "madebyLink";
            this.madebyLink.Size = new System.Drawing.Size(53, 13);
            this.madebyLink.TabIndex = 5;
            this.madebyLink.TabStop = true;
            this.madebyLink.Text = "Miterosan";
            this.madebyLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // RepositoryLink
            // 
            this.RepositoryLink.AutoSize = true;
            this.RepositoryLink.Location = new System.Drawing.Point(113, 93);
            this.RepositoryLink.Name = "RepositoryLink";
            this.RepositoryLink.Size = new System.Drawing.Size(211, 13);
            this.RepositoryLink.TabIndex = 6;
            this.RepositoryLink.TabStop = true;
            this.RepositoryLink.Text = "https://github.com/miterosan/pWindowJax";
            this.RepositoryLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // repositorylabel
            // 
            this.repositorylabel.AutoSize = true;
            this.repositorylabel.Location = new System.Drawing.Point(12, 93);
            this.repositorylabel.Name = "repositorylabel";
            this.repositorylabel.Size = new System.Drawing.Size(60, 13);
            this.repositorylabel.TabIndex = 7;
            this.repositorylabel.Text = "Repository:";
            // 
            // licenseLabel
            // 
            this.licenseLabel.AutoSize = true;
            this.licenseLabel.Location = new System.Drawing.Point(12, 137);
            this.licenseLabel.Name = "licenseLabel";
            this.licenseLabel.Size = new System.Drawing.Size(47, 13);
            this.licenseLabel.TabIndex = 8;
            this.licenseLabel.Text = "License:";
            // 
            // licenseContentLabel
            // 
            this.licenseContentLabel.AutoSize = true;
            this.licenseContentLabel.Location = new System.Drawing.Point(113, 137);
            this.licenseContentLabel.Name = "licenseContentLabel";
            this.licenseContentLabel.Size = new System.Drawing.Size(26, 13);
            this.licenseContentLabel.TabIndex = 9;
            this.licenseContentLabel.Text = "MIT";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(144, 22);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(37, 13);
            this.versionLabel.TabIndex = 10;
            this.versionLabel.Text = "v0.0.0";
            // 
            // InfoBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 162);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.licenseContentLabel);
            this.Controls.Add(this.licenseLabel);
            this.Controls.Add(this.repositorylabel);
            this.Controls.Add(this.RepositoryLink);
            this.Controls.Add(this.madebyLink);
            this.Controls.Add(this.madebylabel);
            this.Controls.Add(this.ForkLink);
            this.Controls.Add(this.forklabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pWindowJaxLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoBox";
            this.Text = "Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void linkLabel2_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/miterosan");
        }

        private void linkLabel3_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/miterosan/pWindowJax");
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/peppy/pWindowJax");
        }
    }
}