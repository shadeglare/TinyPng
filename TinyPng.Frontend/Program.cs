using System;
using System.Windows.Forms;
using System.Drawing;

using TinyPng.Tools;

namespace TinyPng.Frontend
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new AppView());
        }
    }
}
