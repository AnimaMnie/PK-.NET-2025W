using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        string[] urls =
        {
            "https://www.gutenberg.org/files/84/84-0.txt",//Frankenstein
            "https://www.gutenberg.org/files/11/11-0.txt",//Alicja w Krainie Czarów
            "https://www.gutenberg.org/files/1661/1661-0.txt",//Przygody Sherlocka Holmesa
            "https://www.gutenberg.org/files/2701/2701-0.txt" //Moby Dick 
        };

        HttpClient client = new HttpClient();
        var wordCounts = new ConcurrentDictionary<string, int>();

        Stopwatch downloadWatch = Stopwatch.StartNew();
        
        var downloadTasks = urls.Select(url => client.GetStringAsync(url)).ToArray();
        string[] texts = await Task.WhenAll(downloadTasks);

        downloadWatch.Stop();

        Stopwatch processingWatch = Stopwatch.StartNew();
        
        Parallel.ForEach(texts, text =>
        {
            var words = Regex.Split(text.ToLowerInvariant(), @"\W+")
                             .Where(w => !string.IsNullOrWhiteSpace(w));

            foreach (var word in words)
            {
                wordCounts.AddOrUpdate(word, 1, (_, count) => count + 1);
            }
        });

        processingWatch.Stop();
        
        var topWords = wordCounts
            .OrderByDescending(kvp => kvp.Value)
            .Take(10)
            .ToList();

        Console.WriteLine("Najczęstsze słowa:");
        int index = 1;
        foreach (var word in topWords)
        {
            Console.WriteLine($"{index}. {word.Key}: {word.Value}");
            index++;
        }

        Console.WriteLine();
        Console.WriteLine($"Czas pobierania: {downloadWatch.Elapsed.TotalSeconds:F2} sekundy");
        Console.WriteLine($"Czas przetwarzania: {processingWatch.Elapsed.TotalSeconds:F2} sekundy");
    }
}
