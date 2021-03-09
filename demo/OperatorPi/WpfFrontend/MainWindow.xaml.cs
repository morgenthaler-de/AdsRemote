using AdsRemote;
using System;
using System.Windows;
using System.Windows.Media;

namespace OperatorPi
{
    public partial class MainWindow : Window
    {
        public const string PLC_AMSNETID = "192.168.13.154.1.1";
        ViewModel model;
        PLC plc;

        public MainWindow()
        {
            InitializeComponent();

            plc = new PLC(PLC_AMSNETID);

            model = plc.Class<ViewModel>();
            model.Temp.ValueChanged += Temp_ValueChanged;
            DataContext = model;
        }

        double score = 0.0;
        private void Temp_ValueChanged(object sender, AdsRemote.Common.Var e)
        {
            double bh = blueEllipse.ActualHeight * 0.2;
            double rh = redEllipse.ActualHeight - redEllipse.StrokeThickness * 2.0;

            double scale = model.Temp / 1000;
            tempEllipse.RenderTransform = new ScaleTransform(scale, scale);

            double gh = tempEllipse.ActualHeight * scale;
             
            if (gh > bh && gh < rh)
                score = score + 0.1;

            if (gh < bh)
                score = score - .01;

            if (gh > rh)
            {
                score = score - 0.1;
                tempEllipse.StrokeThickness = ((rh - bh) / 2) / scale / 2;
            }
            else
                tempEllipse.StrokeThickness = (gh < bh ? 2 : (gh - bh) / 2) / scale / 2;

            score = Math.Max(0, score);

            scoreLabel.Content = $"SCORE: {(int)score}";

            if (model.Reset)
                model.Reset.RemoteValue = false;
        }

        private void resetLabel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            score = 0;
            model.Reset.RemoteValue = true;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.F11)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;
                    Topmost = false;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    Topmost = true;
                    WindowState = WindowState.Maximized;
                }
            }
        }
    } // class
}
