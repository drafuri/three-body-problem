using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1;
using System.Windows.Media.Media3D;


namespace ThreeBodySimulation
{
    public partial class MainWindow : Window
    {
        private Ellipse[] bodies;
        private double[] x;
        private double[] y;
        private double[] vx;
        private double[] vy;
        private double[] mass;
        private DispatcherTimer animationTimer;
        private DispatcherTimer beginningTimer;
        private double G = 6.67430 * Math.Pow(10, -11);
        private double Scale = 1;
        private double S = 1;
        private double[] start_x;
        private double[] start_y;
        


        public MainWindow()
        {
            InitializeComponent();
            InitializeBodies();
        }

        private void InitializeBodies()
        {
            bodies = new Ellipse[] { Body1, Body2, Body3 };
            x = new double[] { 0, 0, 0 };
            y = new double[] { 0, 0, 0 };
            start_x = new double[] { 0, 0, 0 };
            start_y = new double[] { 0, 0, 0 };
            vx = new double[] { 0, 0.0, 0.0 };
            vy = new double[] { 0.0, 0.0, 0.0 };
            mass = new double[] { 0, 0, 0 };//{ Math.Pow(10, 22), Math.Pow(10, 22), Math.Pow(10, 22) };
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(10);
            animationTimer.Tick += AnimationTimer_Tick;

            beginningTimer= new DispatcherTimer();
            beginningTimer.Interval = TimeSpan.FromMilliseconds(100);
            beginningTimer.Tick += beginningTimer_Tick;
            beginningTimer.Start();

            body1Trail = new Polyline();
            body1Trail.Stroke = Brushes.Red;
            body1Trail.StrokeThickness = 2.0 / Scale;
            canvas.Children.Add(body1Trail);

            body2Trail = new Polyline();
            body2Trail.Stroke = Brushes.Blue;
            body2Trail.StrokeThickness = 2.0 / Scale;
            canvas.Children.Add(body2Trail);

            body3Trail = new Polyline();
            body3Trail.Stroke = Brushes.Green;
            body3Trail.StrokeThickness = 2.0 / Scale;
            canvas.Children.Add(body3Trail);


        }

        private void beginningTimer_Tick(object sender, EventArgs e)
        {
            Scale = 1 / GetText(scale);

            Canvas.SetLeft(Body1, GetText(x1));
            Canvas.SetTop(Body1, GetText(y1));
            Canvas.SetLeft(Body2, GetText(x2));
            Canvas.SetTop(Body2, GetText(y2));
            Canvas.SetLeft(Body3, GetText(x3));
            Canvas.SetTop(Body3, GetText(y3));

            if (Button1.IsChecked == true || Button2.IsChecked == true || Button3.IsChecked == true)
            {
                CameraMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            beginningTimer.Stop();

            Scale = 1 / GetText(scale);
            x[0] = GetText(x1) / Scale;
            x[1] = GetText(x2) / Scale;
            x[2] = GetText(x3) / Scale;
            y[0] = GetText(y1) / Scale;
            y[1] = GetText(y2) / Scale;
            y[2] = GetText(y3) / Scale;
            vx[0] = GetText(vx1);
            vx[1] = GetText(vx2);
            vx[2] = GetText(vx3);
            vy[0] = GetText(vy1);
            vy[1] = GetText(vy2);
            vy[2] = GetText(vy3);
            mass[0] = GetText(m1);
            mass[1] = GetText(m2);
            mass[2] = GetText(m3);
            S = GetText(DeltaT);

            body1Trail.StrokeThickness = 2.0;
            body2Trail.StrokeThickness = 2.0;
            body2Trail.StrokeThickness = 2.0;

            for(int i = 0; i < 3; i++)
            {
                start_x[i] = x[i];
                start_y[i] = y[i];
            }

            Masa.Visibility= Visibility.Collapsed;
            B1.Visibility = Visibility.Collapsed;
            B2.Visibility = Visibility.Visible;
            B3.Visibility = Visibility.Visible;

            mm1.Text = "С * ШагВремени";
            m1.Text = "0";

            DeltaX.Text = "DeltaX";
            DeltaY.Text = "DeltaY";

            animationTimer.Start();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            if (B2.Content.ToString() == "Пауза") 
            {
                animationTimer.Stop();
                B2.Content = "Возобновить";
            }
            else
            {
                animationTimer.Start();
                B2.Content = "Пауза";
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            var newWindow = new MainWindow();
            newWindow.Show();
            Close();
        }



        public void SetText(TextBox textbox, double text)
        {
            textbox.Text = text.ToString();
        }

        private double GetText(TextBox textBox)
        {
            try
            {
                if (textBox.Text.Length == 0)
                {
                    if (new List<string>() { "vx1", "vx2", "vx3", "vy1", "vy2", "vy3" }.Any(X => X == textBox.Name))
                    {
                        return 0;
                    }
                    if(textBox.Name == "Scale")
                    {
                        return 1;
                    }
                }
                string[] a = textBox.Text.Split(' ');
                if (a.Length == 2)
                    return double.Parse(a[0]) * Math.Pow(10, double.Parse(a[1]));
                else
                    return double.Parse(a[0]);
            }
            catch(System.FormatException) { return 0; }
        }

        private void UpdateBodies(double deltaT)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    double dx = x[j] - x[i];
                    double dy = y[j] - y[i];
                    double d = Math.Sqrt(dx * dx + dy * dy);
                    double f = G * mass[i] * mass[j] / (d * d);
                    double fx = f * dx / d;
                    double fy = f * dy / d;
                    vx[i] += fx / mass[i] * deltaT;
                    vy[i] += fy / mass[i] * deltaT;
                    vx[j] -= fx / mass[j] * deltaT;
                    vy[j] -= fy / mass[j] * deltaT;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                x[i] += vx[i] * deltaT;
                y[i] += vy[i] * deltaT;
                Canvas.SetLeft(bodies[i], x[i] * Scale);
                Canvas.SetTop(bodies[i], y[i] * Scale);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            UpdateBodies(S);

            body1Trail.Points.Add(new Point((double)Body1.GetValue(Canvas.LeftProperty) + Body1.Width / 2.0, (double)Body1.GetValue(Canvas.TopProperty) + Body1.Height / 2.0));
            body2Trail.Points.Add(new Point((double)Body2.GetValue(Canvas.LeftProperty) + Body2.Width / 2.0, (double)Body2.GetValue(Canvas.TopProperty) + Body2.Height / 2.0));
            body3Trail.Points.Add(new Point((double)Body3.GetValue(Canvas.LeftProperty) + Body3.Width / 2.0, (double)Body3.GetValue(Canvas.TopProperty) + Body3.Height / 2.0));

            SetText(x1, x[0] * Scale);
            SetText(y1, y[0] * Scale);
            SetText(x2, x[1] * Scale);
            SetText(y2, y[1] * Scale);
            SetText(x3, x[2] * Scale);
            SetText(y3, y[2] * Scale);
            SetText(vx1, (x[0] - start_x[0]) * Scale);
            SetText(vy1, (y[0] - start_y[0]) * Scale);
            SetText(vx2, (x[1] - start_x[1]) * Scale);
            SetText(vy2, (y[1] - start_y[1]) * Scale);
            SetText(vx3, (x[2] - start_x[2]) * Scale);
            SetText(vy3, (y[2] - start_y[2]) * Scale);
            SetText(m1, int.Parse(m1.Text) + 1);

            if (Button1.IsChecked == true || Button2.IsChecked == true || Button3.IsChecked == true)
            {
                CameraMove();
            }
        }

        public double[] NewCoord(Ellipse body)
        {
            return new double[] { canvas.ActualWidth / 2 - (double)body.GetValue(Canvas.LeftProperty) ,
                                  canvas.ActualHeight / 2 - (double)body.GetValue(Canvas.TopProperty) };
        }

        public void CameraMove()
        {
            double[] Coord1 = NewCoord(Body1);
            double[] Coord2 = NewCoord(Body2);
            double[] Coord3 = NewCoord(Body3);

            List<double[]> doubles = new List<double[]>();
            if (Button1.IsChecked == true)
                doubles.Add(Coord1);
            if (Button2.IsChecked == true)
                doubles.Add(Coord2);
            if (Button3.IsChecked == true)
                doubles.Add(Coord3);

            double Top = 0;
            double Left = 0;
            foreach (var item in doubles)
            {
                Top += item[0];
                Left += item[1];
            }
            int count = doubles.Count();
            canvas.RenderTransform = null;
            canvas.RenderTransform = new TranslateTransform(Top / count, Left / count);

        }
    }
}