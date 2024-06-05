﻿using System;
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

        List<string> UrlsToComplete = new List<string>();
        List<string> UrlsDone = new List<string>();
        List<Crawler> pending = new List<Crawler>();

        private int NumberOfTabsPerSession;



        public void StartCrawling(string url, int numberOfTabsPerSession)
        {
            NumberOfTabsPerSession = numberOfTabsPerSession;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            UrlsToComplete.Add(url);
             
            InitiateBunch(new List<string> { url});
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
                UrlCrawledSuccess?.Invoke(url);
            };

            browserCrawler.UrlCrawledFailed += (url) =>
            {
                UrlCrawledFailed?.Invoke(url);
            };

            browserCrawler.OnNewUrlFound += (url) =>
            {
                UrlsToComplete.Add(url);
                OnNewUrlFound?.Invoke(url);
            };

            pending.Add(browserCrawler);
            var task = browserCrawler.OpenUrlsAsync(urls);
            await Task.WhenAll(task);
            await browserCrawler.CloseBrowser();
            pending.Remove(browserCrawler);
            ReTrigger();
        }


        private void ReTrigger()
        {
            if (UrlsToComplete.Count > 0)
            {
                List<string> urlsCheck = new List<string>();
                 
                //remove duplicates from urlstocomeplte
                UrlsToComplete = UrlsToComplete.Distinct().ToList();

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
