using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace TaskHW;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void AnalyzeButtonClick(object sender, RoutedEventArgs e)
    {
        string text = inputTextBox.Text;
        if (text == "")
        {
            MessageBox.Show("Enter text");
            return;
        }

        analyzeButton.IsEnabled = false;
        string report = await AnalyzeAsync(text);

        if (fileRadioButton.IsChecked == true)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file|*.txt";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await File.WriteAllTextAsync(dialog.FileName, report);
                    MessageBox.Show("Saved");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        else
        {
            resultTextBlock.Text = report;
        }

        analyzeButton.IsEnabled = true;
    }

    private Task<string> AnalyzeAsync(string text)
    {
        return Task.Run(() =>
        {
            int characters = text.Length;

            int words = 0;
            string[] parts = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                if (part.Any(char.IsLetterOrDigit))
                {
                    words++;
                }
            }

            int sentences = 0;
            int questions = 0;
            int exclamations = 0;
            bool hasText = false;
            bool hasQuestion = false;
            bool hasExclamation = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '.' || c == '!' || c == '?')
                {
                    if (c == '?')
                    {
                        hasQuestion = true;
                    }
                    if (c == '!')
                    {
                        hasExclamation = true;
                    }

                    bool last = i == text.Length - 1 || (text[i + 1] != '.' && text[i + 1] != '!' && text[i + 1] != '?');
                    if (last)
                    {
                        if (hasText)
                        {
                            sentences++;
                            if (hasQuestion)
                            {
                                questions++;
                            }
                            else if (hasExclamation)
                            {
                                exclamations++;
                            }
                        }
                        hasText = false;
                        hasQuestion = false;
                        hasExclamation = false;
                    }
                }
                else if (char.IsLetterOrDigit(c))
                {
                    hasText = true;
                }
            }

            if (hasText)
            {
                sentences++;
            }

            return $"Sentences :: {sentences}{Environment.NewLine}"
                + $"Characters :: {characters}{Environment.NewLine}"
                + $"Words :: {words}{Environment.NewLine}"
                + $"Question sentences :: {questions}{Environment.NewLine}"
                + $"Exclamatory sentences :: {exclamations}";
        });
    }
}