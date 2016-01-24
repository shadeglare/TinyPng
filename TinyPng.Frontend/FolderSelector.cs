using System;
using System.Windows.Forms;
using System.Drawing;

namespace TinyPng.Frontend
{
    sealed class FolderSelector : TableLayoutPanel
    {
        private Label Title { get; set; }

        private TextBox Path { get; set; }

        private Button Choose { get; set; }

        public FolderSelector()
        {
            this.Title = new Label();
            this.Title.Dock = DockStyle.Fill;
            this.Title.AutoSize = true;
            this.Title.TextAlign = ContentAlignment.MiddleLeft;
            this.Title.AutoSize = true;

            this.Path = new TextBox();
            this.Path.Dock = DockStyle.Fill;
            this.Path.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.Path.ReadOnly = true;

            this.Choose = new Button();
            this.Choose.Dock = DockStyle.Fill;
            this.Choose.AutoSize = true;
            this.Choose.Text = "Select";

            this.Choose.Click += (o, e) =>
            {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Path.Text = dialog.SelectedPath;
                    this.FolderChanged(this, dialog.SelectedPath);
                }
            };

            this.ColumnCount = 3;
            this.RowCount = 1;
            this.Dock = DockStyle.Fill;
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

            this.Controls.Add(this.Title, 0, 0);
            this.Controls.Add(this.Path, 1, 0);
            this.Controls.Add(this.Choose, 2, 0);
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Title.Text = value;
            }
        }

        public event Action<Object, String> FolderChanged = (o, e) => {};
    }
}
