using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ExamSP;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // перераховує файли
    private async void SearchButtonClick(object sender, RoutedEventArgs e)
    {
        string directory = dirTextBox.Text;
        string word = wordTextBox.Text;

        if (!Directory.Exists(directory))
        {
            MessageBox.Show("Directory not found");
            return;
        }
        if (word == "")
        {
            MessageBox.Show("Enter word");
            return;
        }

        searchButton.IsEnabled = false;
        saveButton.IsEnabled = false;
        resultTextBox.Clear();
        progress.Value = 0;

        try
        {
            string[] files = await GetFilesAsync(directory);
            progress.Maximum = files.Length;

            if (files.Length == 0)
            {
                resultTextBox.Text = "No txt files in this directory";
            }

            bool found = false;
            foreach (string file in files)
            {
                int count = await CountWordAsync(file, word);
                progress.Value++;

                if (count > 0)
                {
                    found = true;
                    resultTextBox.AppendText($"Назва файлу: {Path.GetFileName(file)}{Environment.NewLine}");
                    resultTextBox.AppendText($"Шлях до файлу: {file}{Environment.NewLine}");
                    resultTextBox.AppendText($"Кількість входження слова: {count}{Environment.NewLine}{Environment.NewLine}");
                }
            }

            if (files.Length > 0 && !found)
            {
                resultTextBox.Text = "Word not found";
            }

            saveButton.IsEnabled = found;
            MessageBox.Show("Complete");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        searchButton.IsEnabled = true;
    }

    // збирає всі тхт файли з директорій і піддиректорій
    private Task<string[]> GetFilesAsync(string directory)
    {
        return Task.Run(() => Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories));
    }

    // читає скільки там слів
    private Task<int> CountWordAsync(string path, string word)
    {
        return Task.Run(() =>
        {
            string text = File.ReadAllText(path);
            char[] separators = new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':', '-', '(', ')', '"' };
            string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (string item in words)
            {
                if (string.Equals(item, word, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }
            return count;
        });
    }

    // зберігає звіт у файл 
    private async void SaveButtonClick(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Text file|*.txt";
        if (dialog.ShowDialog() == true)
        {
            try
            {
                await File.WriteAllTextAsync(dialog.FileName, resultTextBox.Text);
                MessageBox.Show("Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}