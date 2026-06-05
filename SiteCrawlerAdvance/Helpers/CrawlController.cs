using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerAdvance.Helpers
{
    public class CrawlController
    {

        //create event and delegate when url is crawled
        public delegate void UrlCrawledEventHandler(string url);
        public event UrlCrawledEventHandler UrlCrawledStarted;
        public event UrlCrawledEventHandler UrlCrawledSuccess;
        public event UrlCrawledEventHandler UrlCrawledFailed;
        public event UrlCrawledEventHandler OnNewUrlFound;
        public event EventHandler? CrawlCompleted;

        List<string> UrlsToComplete = new List<string>();
        List<string> UrlsDone = new List<string>();
        List<Crawler> pending = new List<Crawler>();

        private int NumberOfTabsPerSession;

        private int CrawlPagesCount;


        private static string NormalizeQueueUrl(string url)
        {
            url = Uri.UnescapeDataString(url.Trim());
            if (url.EndsWith('/'))
                url = url.TrimEnd('/');
            return url;
        }

        private bool EnqueueUrl(string url)
        {
            url = NormalizeQueueUrl(url);
            if (UrlsDone.Contains(url) || UrlsToComplete.Contains(url))
                return false;

            UrlsToComplete.Add(url);
            return true;
        }


        public void StartCrawling(List<string> urls, int numberOfTabsPerSession,int crawlPagesCount)
        {
            NumberOfTabsPerSession = numberOfTabsPerSession;
            CrawlPagesCount = crawlPagesCount;
            
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            foreach (var u in urls)
                EnqueueUrl(u);

            ReTrigger();
        }



        private async void InitiateBunch(List<string> urls)
        {
            Crawler browserCrawler = new Crawler();
            browserCrawler.UrlCrawledStarted += (url) =>
            {
                UrlCrawledStarted?.Invoke(url);
            };

            browserCrawler.UrlCrawledSuccess += (url) =>
            {
                url = NormalizeQueueUrl(url);
                if (!UrlsDone.Contains(url))
                    UrlsDone.Add(url);
                UrlCrawledSuccess?.Invoke(url);
            };

            browserCrawler.UrlCrawledFailed += (url) =>
            {
                UrlCrawledFailed?.Invoke(url);
            };

            browserCrawler.OnNewUrlFound += (url) =>
            {
                url = NormalizeQueueUrl(url);
                if (EnqueueUrl(url))
                    OnNewUrlFound?.Invoke(url);
            };

            pending.Add(browserCrawler);

            bool canCrawl = false;
            if (CrawlPagesCount > 0)
            {
                canCrawl = true;
                CrawlPagesCount--;
            }

            try
            {
                await browserCrawler.OpenUrlsAsync(urls, canCrawl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Crawl batch failed: {ex.Message}");
            }
            finally
            {
                try
                {
                    await browserCrawler.CloseBrowser();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to close browser: {ex.Message}");
                }

                pending.Remove(browserCrawler);
                ReTrigger();
            }
        }


        private void ReTrigger()
        {
            if (UrlsToComplete.Count > 0)
            {
                List<string> urlsCheck = new List<string>();
                 
                //remove duplicates from urlstocomeplte
                UrlsToComplete = UrlsToComplete.Distinct().ToList();

                //remove the urls which are already done
                UrlsToComplete = UrlsToComplete.Except(UrlsDone).ToList();

                //take only 25 items from the urlsToComplete and remove them from the list
                for (int i = 0; i < NumberOfTabsPerSession; i++)
                {
                    if (UrlsToComplete.Count > 0)
                    {
                        urlsCheck.Add(UrlsToComplete[0]);
                        UrlsToComplete.RemoveAt(0);
                    }
                }
                 
                InitiateBunch(urlsCheck);
            }
            else if (pending.Count == 0)
            {
                CrawlCompleted?.Invoke(this, EventArgs.Empty);
            }
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
