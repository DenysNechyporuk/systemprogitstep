using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace async1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void CalculateButtonClick(object sender, RoutedEventArgs e)
    {
        int number;
        if (!int.TryParse(numberTextBox.Text, out number) || number < 0)
        {
            MessageBox.Show("Enter positive number");
            return;
        }

        BigInteger result = await FactorialAsync(number);
        list.Items.Add($"{number}! = {result}");
    }

    private Task<BigInteger> FactorialAsync(int x)
    {
        return Task.Run(() =>
        {
            Thread.Sleep(2000);
            BigInteger result = 1;
            for (int i = 1; i <= x; i++)
            {
                result *= i;
            }
            return result;
        });
    }
}