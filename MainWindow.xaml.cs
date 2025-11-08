using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PhotoFrame
{
    public partial class MainWindow : Window
    {
        ObservableCollection<FrameInfo> frames = new ObservableCollection<FrameInfo>();
        int nextId = 1;

        public MainWindow()
        {
            InitializeComponent();
            FramesList.ItemsSource = frames;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (frames.Count >= 3)
            {
                MessageBox.Show("Maximum of 3 frames allowed.", "PhotoFrame");
                return;
            }

            var dlg = new OpenFileDialog();
            dlg.Title = "Select image";
            dlg.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files|*.*";
            if (dlg.ShowDialog(this) == true)
            {
                var info = new FrameInfo
                {
                    Id = nextId++,
                    Path = dlg.FileName,
                    DisplayName = System.IO.Path.GetFileName(dlg.FileName)
                };
                frames.Add(info);
                CreateAndShowFrame(info);
            }
        }

        private void CreateAndShowFrame(FrameInfo info)
        {
            var w = new PhotoFrameWindow(info.Path);
            info.Window = w;
            w.Closed += (s, e) => { /* keep list in sync */ };
            w.Show();
        }

        private void CloseAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var f in frames.ToArray())
            {
                f.Window?.Close();
                frames.Remove(f);
            }
        }

        private void ShowFrame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int id)
            {
                var info = frames.FirstOrDefault(x => x.Id == id);
                if (info != null)
                {
                    if (info.Window == null || !info.Window.IsVisible)
                    {
                        CreateAndShowFrame(info);
                    }
                    else
                    {
                        info.Window.Activate();
                    }
                }
            }
        }

        private void CloseFrame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int id)
            {
                var info = frames.FirstOrDefault(x => x.Id == id);
                if (info != null)
                {
                    info.Window?.Close();
                    frames.Remove(info);
                }
            }
        }
    }

    public class FrameInfo
    {
        public int Id { get; set; }
        public string Path { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public PhotoFrameWindow? Window { get; set; }
    }
}
