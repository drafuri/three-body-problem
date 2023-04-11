using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class AddBodyWindow : Window
    {
        double[] result = new double[5];
        public AddBodyWindow()
        {
            InitializeComponent();
        }
        public double[] GetResult()
        {
            return result;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            double[] result = new double[5];
            result[0] = double.Parse(XTextBox.Text);
            result[1] = double.Parse(YTextBox.Text);
            result[2] = double.Parse(VxTextBox.Text);
            result[2] = double.Parse(VyTextBox.Text);
            result[4] = double.Parse(MassTextBox.Text);

            Close();
        }
    }
}
