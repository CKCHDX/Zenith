using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Zenith
{
    public partial class Form1 : Form
    {
        private bool isFullScreen = false;
        private bool isTextOnlyMode = false;

        private Timer visibilityTimer;
        private Timer mouseHoverTimer;
        private Control[] controlsToToggle;

        private List<string> history = new List<string>();

        private Button downloadButton;

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();

            // Initialize the timers
            InitializeTimers();

            // Initialize the UI components
            InitializeUIComponents();

            // Ensure there are no paddings or margins
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
            this.webView.Margin = new Padding(0);
            this.webView.Padding = new Padding(0);

            // Set the DockStyle of webView to Fill
            this.webView.Dock = DockStyle.Fill;
        }



        private void InitializeTimers()
        {
            visibilityTimer = new Timer();
            visibilityTimer.Interval = 2000; // 2 seconds for hiding buttons
            visibilityTimer.Tick += VisibilityTimer_Tick;

            mouseHoverTimer = new Timer();
            mouseHoverTimer.Interval = 50; // Check mouse position frequently
            mouseHoverTimer.Tick += MouseHoverTimer_Tick;
            mouseHoverTimer.Start();
        }

        private void InitializeUIComponents()
        {
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Top
            };
            searchTextBox.KeyDown += SearchTextBox_KeyDown; // Handle Enter key press
            Controls.Add(searchTextBox);

            fullScreenButton = new Button
            {
                Text = "Full Screen",
                Dock = DockStyle.Top
            };
            fullScreenButton.Click += FullScreenButton_Click;
            Controls.Add(fullScreenButton);


            // Adding Download Button
            downloadButton = new Button
            {
                Text = "Download",
                Dock = DockStyle.Top
            };
            downloadButton.Click += DownloadButton_Click;
            Controls.Add(downloadButton);

            // Controls to toggle, including the bookmark and other buttons
            controlsToToggle = new Control[] { downloadButton, backButton, forwardButton, fullScreenButton };
        }


        private void MouseHoverTimer_Tick(object sender, EventArgs e)
        {
            Point mousePosition = PointToClient(Cursor.Position);
            if (mousePosition.Y < 5) // If the mouse is near the top
            {
                ShowButtons();
                visibilityTimer.Stop();
                visibilityTimer.Start();
            }
            if (mousePosition.X < 5)
            {
                ShowBookMark();
                visibilityTimer.Stop();
                visibilityTimer.Start();
            }
            if (mousePosition.X > this.ClientSize.Width - 5)
            {
                ShowListBoxes();
                visibilityTimer.Stop();
                visibilityTimer.Start();
            }
        }

        private void ShowListBoxes()
        {
            listBoxHistory.Visible = true;
        }

        private void HideListBoxes()
        {
            listBoxHistory.Visible = false;
        }

        private void ShowBookMark()
        {
            listBoxBookmarks.Visible = true;
        }

        private void HideBookMark()
        {
            listBoxBookmarks.Visible = false;
        }

        private void VisibilityTimer_Tick(object sender, EventArgs e)
        {
            HideButtons();
            visibilityTimer.Stop();
        }

        private void ShowButtons()
        {
            foreach (var control in controlsToToggle)
            {
                control.Visible = true;
            }
        }

        private void HideButtons()
        {
            foreach (var control in controlsToToggle)
            {
                control.Visible = false;
            }
            HideListBoxes();
            HideBookMark();
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                // Enter full-screen mode
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.fullScreenButton.Text = "Exit Full Screen";

                HideButtons();
            }
            else
            {
                // Exit full-screen mode
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                this.fullScreenButton.Text = "Full Screen";

                ShowButtons();
            }

            isFullScreen = !isFullScreen; // Toggle the state
        }

        private void FullScreenButton_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }



        private void DarkModeButton_Click(object sender, EventArgs e)
        {
            ToggleDarkMode();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            DownloadCurrentPage();
        }

        private void ToggleDarkMode()
        {


        }

        private void DownloadCurrentPage()
        {
            var downloadDialog = new SaveFileDialog
            {
                FileName = "downloaded_page.html",
                Filter = "HTML Files (*.html)|*.html|All Files (*.*)|*.*"
            };

            if (downloadDialog.ShowDialog() == DialogResult.OK)
            {
                webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML").ContinueWith(async htmlTask =>
                {
                    var htmlContent = await htmlTask;
                    System.IO.File.WriteAllText(downloadDialog.FileName, htmlContent);
                    MessageBox.Show("Page downloaded successfully!", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
        }





        private void BackButton_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2.CanGoBack)
                webView.CoreWebView2.GoBack();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2.CanGoForward)
                webView.CoreWebView2.GoForward();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            string url = searchTextBox.Text.Trim();

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                webView.CoreWebView2.Navigate(url);

                // Add to history
                if (!history.Contains(url))
                {
                    history.Add(url);
                    listBoxHistory.Items.Add(url); // Assuming you have a ListBox named listBoxHistory
                }
            }
            else
            {
                MessageBox.Show("The URL is not valid. Please enter a valid URL.", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                webView.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize WebView2: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            var downloadOperation = e.DownloadOperation;
            e.Handled = true;

            // Create and show a form to display download progress
            var progressDialog = new Form
            {
                Text = "Download Progress",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.Manual,
                Location = new Point(this.ClientSize.Width - 400, this.ClientSize.Height - 200),
                Size = new Size(400, 200)
            };

            var progressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100
            };
            progressDialog.Controls.Add(progressBar);

            var label = new Label
            {
                Dock = DockStyle.Top,
                Text = $"Downloading: {downloadOperation.Uri}"
            };
            progressDialog.Controls.Add(label);

            downloadOperation.BytesReceivedChanged += (s, args) =>
            {
                if (downloadOperation.TotalBytesToReceive.HasValue && downloadOperation.TotalBytesToReceive.Value > 0)
                {
                    progressBar.Value = (int)((downloadOperation.BytesReceived * 100L) / (long)downloadOperation.TotalBytesToReceive.Value);
                }
            };

            downloadOperation.StateChanged += (s, args) =>
            {
                if (downloadOperation.State == CoreWebView2DownloadState.Completed)
                {
                    progressDialog.Close();
                    MessageBox.Show("Download completed!", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (downloadOperation.State == CoreWebView2DownloadState.Interrupted)
                {
                    progressDialog.Close();
                    MessageBox.Show("Download interrupted!", "Download", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            progressDialog.Show();
        }
    }
}