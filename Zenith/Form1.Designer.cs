
using Microsoft.Web.WebView2.WinForms;
using System.Windows.Forms;

namespace Zenith
{
    partial class Form1
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

        private WebView2 webView;

        private Button backButton;
        private Button forwardButton;
        private Button fullScreenButton;
        private ListBox listBoxHistory;
        private ListBox listBoxBookmarks;
        private TextBox searchTextBox;
        private void InitializeComponent()
        {
            this.webView = new WebView2();

            this.backButton = new Button();
            this.forwardButton = new Button();
            this.fullScreenButton = new Button();
            this.listBoxHistory = new ListBox();
            this.listBoxBookmarks = new ListBox();
            this.searchTextBox = new TextBox();

            // Configure properties and event handlers
            // Example for webView
            this.webView.Dock = DockStyle.Fill;
            this.Controls.Add(this.webView);


            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }
    }
}

