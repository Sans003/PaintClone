using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using SystemColors = System.Windows.SystemColors;

namespace WPFContextMenuControl
{
    /// <summary> 
    /// Interaction logic for MainWindow.xaml 
    /// </summary> 

    public partial class MainWindow : Window
    {
        public Color color = Color.Black;
        public double size = 5;
        Point position = new Point();
        public string filetype = "png";
        BitmapEncoder Encoder;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                position = e.GetPosition(canvas);
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && canvas.IsMouseOver)
            {
                Rectangle rect = new Rectangle();
                Canvas.SetLeft(rect, position.X);
                Canvas.SetTop(rect, position.Y);
                rect.StrokeThickness = size;
                rect.Stroke = SystemColors.WindowFrameBrush;
                position = e.GetPosition(canvas);

                canvas.Children.Add(rect);
            }
        }
        private void canvas_ExportImage(object sender, RoutedEventArgs e)
        {
            FileDialog fileSaver = new SaveFileDialog();
            fileSaver.Filter = "Image Files|*.jpeg;*.png;*.gif";
            fileSaver.ShowDialog();
            string filename = fileSaver.FileName;
            string filetype = Path.GetExtension(filename);
            Rect bounds = VisualTreeHelper.GetContentBounds(canvas); 
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            switch (filetype)
            {
                case ".png":
                    Encoder = new PngBitmapEncoder();
                    break;
                case ".gif":
                    Encoder = new GifBitmapEncoder();
                    break;
                case ".jpeg":
                    Encoder = new JpegBitmapEncoder();
                    break;
            }
            Encoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                Encoder.Save(ms);
                ms.Close();

                System.IO.File.WriteAllBytes(filename, ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                //up
                size += 2;
            }
            else
            {
                //down lol what else
                size -= 2;
            }
        }
    }
}