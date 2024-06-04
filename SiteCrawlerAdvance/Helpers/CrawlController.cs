using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerAdvance.Helpers
{
    public class CrawlController
    {

        public async Task StartCrawling(string url, int NumberOfTabsPerSession)
        {

            Console.WriteLine("Starting Chrome Service...!");

            string currentDirectory = Directory.GetCurrentDirectory();

            // Path to the urls.txt file in the current directory
            string filePath = Path.Combine(currentDirectory, "urls.txt");
            List<string> urls = new List<string>();
            //urls.Add("https://beta.cbd.ae");
            //urls.Add("https://beta.cbd.ae/islami");

            if (File.Exists(filePath))
            {
                // Read all lines from the file and store them in a list
                urls = new List<string>(File.ReadAllLines(filePath));
            }

            List<Helper> pending = new List<Helper>();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            int dividetheListintoParts = NumberOfTabsPerSession;
            //divide the list into parts and add them into another list of lists
            List<List<string>> dividedList = new List<List<string>>();
            for (int i = 0; i < urls.Count; i += dividetheListintoParts)
            {
                dividedList.Add(urls.GetRange(i, Math.Min(dividetheListintoParts, urls.Count - i)));
            }


            foreach (List<string> item in dividedList)
            {
                var browserAutomation4 = new Helper();
                pending.Add(browserAutomation4);
                var task1 = browserAutomation4.OpenUrlsAsync(item);
                await Task.WhenAll(task1);
                pending.Remove(browserAutomation4);
            }

            async void OnProcessExit(object? sender, EventArgs e)
            {
                foreach (var item in pending)
                {
                    await item.CloseBrowser();
                }
                Console.WriteLine("Process is exiting");
            }
        }

    }
}
