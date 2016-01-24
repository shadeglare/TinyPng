﻿using System;
using System.Drawing;
using System.Windows.Forms;

using TinyPng.Tools;

namespace TinyPng.Frontend
{
    sealed class AppView : Form
    {
        private AppViewModel ViewModel { get; set; }
        private ImageCompressor Compressor { get; set; }

        private TableLayoutPanel RootLayout { get; set; }
        private FolderSelector SourceSelector { get; set; }
        private FolderSelector TargetSelector { get; set; }
        private Button Convert { get; set; }

        public AppView()
            : base()
        {
            this.ViewModel = new AppViewModel();
            this.ViewModel.CanStartChanged += (o, e) => 
                {
                    this.Convert.Enabled = e;
                    if (e)
                    {
                        this.Compressor = new ImageCompressor(new ImageCompressorSettings(
                            this.ViewModel.SourcePath,
                            this.ViewModel.TargetPath,
                            2, 3));
                        this.Compressor.Started += this.Compressor_Started;
                        this.Compressor.Stopped += this.Compressor_Stopped;
                    }
                };

            this.SourceSelector = new FolderSelector();
            this.SourceSelector.Text = "Source path:";
            this.SourceSelector.FolderChanged += (o, e) => this.ViewModel.SourcePath = e;

            this.TargetSelector = new FolderSelector();
            this.TargetSelector.Text = "Target path:";
            this.TargetSelector.FolderChanged += (o, e) => this.ViewModel.TargetPath = e;

            this.Convert = new Button();
            this.Convert.Text = "Convert";
            this.Convert.Enabled = false;
            this.Convert.Dock = DockStyle.Fill;
            this.Convert.Click += (sender, e) => this.Compressor.Start();

            this.RootLayout = new TableLayoutPanel();
            this.RootLayout.ColumnCount = 1;
            this.RootLayout.RowCount = 4;
            this.RootLayout.Dock = DockStyle.Fill;
            this.RootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            this.RootLayout.Controls.Add(this.SourceSelector, 0, 0);
            this.RootLayout.Controls.Add(this.TargetSelector, 0, 1);
            this.RootLayout.Controls.Add(this.Convert, 0, 3);

            this.MinimumSize = new Size(640, 480);
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(5);
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.Text = "Tinypng Compressor";
            this.Controls.Add(this.RootLayout);
        }

        private void Compressor_Stopped(Object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() => this.ViewModel.CanStart = true));
        }

        private void Compressor_Started(Object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() => this.ViewModel.CanStart = false));
        }
    }
}

