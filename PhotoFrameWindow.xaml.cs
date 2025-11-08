using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PhotoFrame
{
    public partial class PhotoFrameWindow : Window
    {
        string path;

        public PhotoFrameWindow(string imagePath)
        {
            InitializeComponent();
            path = imagePath;
            LoadImage();
            // allow mouse resize/drag - we still set click-through later
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2) // double-click to close
                    this.Close();
                else
                    this.DragMove();
            };
        }

        void LoadImage()
        {
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                PhotoImage.Source = bmp;
            }
            catch
            {
                // ignore for brevity
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            // place window on the desktop layer (above wallpaper but below normal windows)
            IntPtr progman = NativeMethods.FindWindow("Progman", null);
            NativeMethods.SetWindowPos(hwnd, progman, 100, 100, (int)Width, (int)Height,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);

            // make transparent background click-through
            var exStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            exStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_TOOLWINDOW;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // initial visual tweaks
            UpdateBorders();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBorders();
        }

        void UpdateBorders()
        {
            // Ambient border thickness scales slightly with size
            double t = Math.Max(6, Math.Min(40, Math.Min(ActualWidth, ActualHeight) * 0.06));
            AmbientBorder.Margin = new Thickness(t / 2);
            FrameBorder.Margin = new Thickness(t / 4);
            AmbientBorder.CornerRadius = new CornerRadius(Math.Max(4, t / 3));
            FrameBorder.CornerRadius = new CornerRadius(Math.Max(2, t / 4));
        }
    }
}
