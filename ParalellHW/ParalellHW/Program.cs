using System.IO;
using System.Threading.Tasks;

namespace ParalellHW;

internal class Program
{
    private static void Main(string[] args)
    {
        int start;
        int end;

        Console.Write("Enter start :: ");
        if (!int.TryParse(Console.ReadLine(), out start))
        {
            Console.WriteLine("Wrong number");
            return;
        }

        Console.Write("Enter end :: ");
        if (!int.TryParse(Console.ReadLine(), out end))
        {
            Console.WriteLine("Wrong number");
            return;
        }

        if (start > end)
        {
            int temp = start;
            start = end;
            end = temp;
        }

        string path = "table.txt";
        WriteTable(start, end, path);

        Console.WriteLine("Table saved to :: " + Path.GetFullPath(path));
        Console.ReadLine();
    }

    static void WriteTable(int start, int end, string path)
    {
        string[] blocks = new string[end - start + 1];

        Parallel.For(start, end + 1, i =>
        {
            string[] lines = new string[10];
            for (int j = 1; j <= 10; j++)
            {
                lines[j - 1] = $"{i} * {j} = {i * j}";
            }
            blocks[i - start] = string.Join(Environment.NewLine, lines);
        });

        File.WriteAllLines(path, blocks);
    }
}