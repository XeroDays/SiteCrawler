﻿
using PuppeteerSharp;

namespace SiteCrawlerAdvance
{

    internal class Crawler
    {
        public delegate void SiteCrawledEventHandler(string url);
        public event SiteCrawledEventHandler UrlCrawledStarted;
        public event SiteCrawledEventHandler UrlCrawledSuccess;
        public event SiteCrawledEventHandler UrlCrawledFailed;
        public event SiteCrawledEventHandler OnNewUrlFound;


        static string GetChromePath()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            List<string> potentialPaths = new List<string>
            {
                Path.Combine(userProfile, "AppData", "Local", "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe")
            };

            foreach (string path in potentialPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            throw new FileNotFoundException("Chrome executable not found in standard locations.");
        }


        private IBrowser browser;
        public async Task OpenUrlsAsync(List<string> urls,bool canCrawl)
        {
            // Download the Chromium revision if it does not already exist
            //var chromePath = @"C:\Users\Sayed\AppData\Local\Google\Chrome\Application\chrome.exe";
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string chromePath = GetChromePath();

            // Launch a new browser instance
            browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false, // Set to true if you don't want to see the browser window
                ExecutablePath = chromePath,
                DefaultViewport = null, // Ensure the default viewport is null
                Args = new[] { "--start-maximized" }
            });

            var tasks = new List<Task>();

            foreach (var url in urls)
            {
                string log = (DataController.sno++) + ". " + url;
                await Console.Out.WriteLineAsync(log);
                UrlCrawledStarted(log);
                tasks.Add(OpenPageAsync(browser, url, canCrawl));
            }

            await Task.WhenAll(tasks);
            await browser.CloseAsync(); 
        }

        private async Task OpenPageAsync(IBrowser browser, string url,bool canCrawl)
        {
            using var page = await browser.NewPageAsync();
            try
            {
                await page.SetRequestInterceptionAsync(true);

                // Intercept requests and block scripts
                page.Request += async (sender, e) =>
                {
                    if (e.Request.ResourceType == ResourceType.Script)
                    {
                        await e.Request.AbortAsync();
                    }
                    else
                    {
                        await e.Request.ContinueAsync();
                    }
                };


                await page.GoToAsync(url,new NavigationOptions { 
                    Timeout = 30000 
                });

                if (page.MainFrame == null)
                {
                    Console.WriteLine("Page is null");
                    return;
                }

               if(canCrawl)
                {
                    await getUrlsFromPage(page, url); 
                }
                  
                  

              
                UrlCrawledSuccess?.Invoke(url);


            }
            catch (PuppeteerSharp.NavigationException ex)
            {
                Console.WriteLine($"Failed to navigate to {url}: {ex.Message}");
                UrlCrawledFailed?.Invoke("Navigation: " + url);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout when navigating to {url}: {ex.Message}");
                UrlCrawledFailed?.Invoke("Timeout: " + url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when navigating to {url}: {ex.Message}");
                UrlCrawledFailed?.Invoke("Error: " + url);
            }
        }


        private async Task getUrlsFromPage(IPage page,string url)
        {
            // Extract all URLs from the page
            var newurls = await page.EvaluateExpressionAsync<string[]>(@"
                Array.from(document.querySelectorAll('a'))
                    .map(anchor => anchor.href)
                    .filter(href => href)
            ");

            // Print the extracted URLs
            foreach (string urll in newurls)
            {
                string clean1 = urll.Split('?').ToList().First();
                clean1 = Uri.UnescapeDataString(clean1);
                clean1 = clean1.Split("/#").ToList().First();
                clean1 = clean1.Split("#").ToList().First();
                //remove last slash
                if (clean1.Last() == '/')
                {
                    clean1 = clean1.Substring(0, clean1.Length - 1);
                }


                //check if the url neding is pdf
                if (clean1.EndsWith(".pdf"))
                {
                    continue;
                }

                if (new Uri(clean1).Host == new Uri(url).Host)
                {
                    OnNewUrlFound?.Invoke(clean1);
                }
            }
        }


        public async Task CloseBrowser()
        {
            await browser.CloseAsync();
        }
    }
}
