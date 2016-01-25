using System;
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

        private DataGridView OperationLogsGrid { get; set; }

        private Label OperationLabel { get; set; }

        private Button CompressButton { get; set; }

        public AppView()
            : base()
        {
            this.ViewModel = new AppViewModel();
            this.ViewModel.CanStartChanged += (o, e) =>
            {
                this.CompressButton.Enabled = e;
                if (e)
                {
                    this.Compressor = new ImageCompressor(new ImageCompressorSettings(
                            this.ViewModel.SourcePath,
                            this.ViewModel.TargetPath,
                            2, 5));
                    this.Compressor.Started += this.Compressor_Started;
                    this.Compressor.Stopped += this.Compressor_Stopped;
                    this.Compressor.ImageCompressCompleted += this.Compressor_ImageCompressCompleted;
                }
            };
            this.ViewModel.CompressStarted += (o) =>
            {
                this.SourceSelector.Enabled = false;
                this.TargetSelector.Enabled = false;
                this.OperationLogsGrid.Rows.Clear();
                this.CompressButton.Text = "Compressing...";
            };
            this.ViewModel.CompressStopped += (o) =>
            {
                this.SourceSelector.Enabled = true;
                this.TargetSelector.Enabled = true;
                this.CompressButton.Text = "Finished!";
                
                var timer = new Timer();
                timer.Interval = 2000;
                timer.Tick += (t, e) =>
                {
                    timer.Stop();
                    this.ViewModel.CompressorState = CompressorState.Idle;
                };
                timer.Start();
            };
            this.ViewModel.CompressIdled += (o) =>
            {
                this.CompressButton.Text = "Compress";
            };

            this.SourceSelector = new FolderSelector();
            this.SourceSelector.Text = "Source path:";
            this.SourceSelector.FolderChanged += (o, e) => this.ViewModel.SourcePath = e;

            this.TargetSelector = new FolderSelector();
            this.TargetSelector.Text = "Target path:";
            this.TargetSelector.FolderChanged += (o, e) => this.ViewModel.TargetPath = e;

            this.OperationLogsGrid = new DataGridView();
            this.OperationLogsGrid.Dock = DockStyle.Fill;
            this.OperationLogsGrid.ReadOnly = true;
            this.OperationLogsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.OperationLogsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.OperationLogsGrid.Columns.Add("FileSourcePath", "Source File");
            this.OperationLogsGrid.Columns.Add("FileTargetPath", "Destination File");

            this.OperationLabel = new Label();
            this.OperationLabel.Text = "Compression results:";
            this.OperationLabel.Dock = DockStyle.Fill;
            this.OperationLabel.TextAlign = ContentAlignment.MiddleLeft;

            this.CompressButton = new Button();
            this.CompressButton.Text = "Compress";
            this.CompressButton.Enabled = false;
            this.CompressButton.Dock = DockStyle.Fill;
            this.CompressButton.Click += (sender, e) =>
            {
                this.CompressButton.Enabled = false;
                this.Compressor.Start();
            };

            this.RootLayout = new TableLayoutPanel();
            this.RootLayout.ColumnCount = 1;
            this.RootLayout.RowCount = 4;
            this.RootLayout.Dock = DockStyle.Fill;
            this.RootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.RootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            this.RootLayout.Controls.Add(this.SourceSelector, 0, 0);
            this.RootLayout.Controls.Add(this.TargetSelector, 0, 1);
            this.RootLayout.Controls.Add(this.OperationLabel, 0, 2);
            this.RootLayout.Controls.Add(this.OperationLogsGrid, 0, 3);
            this.RootLayout.Controls.Add(this.CompressButton, 0, 4);

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
            this.Invoke(new MethodInvoker(() => this.ViewModel.CompressorState = CompressorState.Stopped));
        }

        private void Compressor_Started(Object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() => this.ViewModel.CompressorState = CompressorState.Working));
        }

        private void Compressor_ImageCompressCompleted(Object sender, CompressCompletedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => this.OperationLogsGrid.Rows.Add(
                new []{ e.Endpoints.Source.FilePath, e.Endpoints.Target.FilePath })));
        }
    }
}

