
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
                Headless = false,
                ExecutablePath = chromePath,
                DefaultViewport = null,
                Args = new[] { "--start-maximized", "--disable-blink-features=AutomationControlled" }
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
        }

        private static string NormalizeUrl(string url)
        {
            url = url.Trim();
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }
            return url;
        }

        private async Task OpenPageAsync(IBrowser browser, string url,bool canCrawl)
        {
            url = NormalizeUrl(url);
            using var page = await browser.NewPageAsync();
            try
            {
                await page.GoToAsync(url, new NavigationOptions
                {
                    Timeout = 60000,
                    WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
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
                UrlCrawledFailed?.Invoke($"Navigation: {url} ({ex.Message})");
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
            if (browser != null && browser.IsConnected)
            {
                await browser.CloseAsync();
                browser = null!;
            }
        }
    }
}
