using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteOverloader
{
    internal class Helper
    {
        //public async Task LoadAndScrapePages(List<string> urls)
        //{


        //    //List<string> clean = new List<string>();

        //    //foreach (var item in urls)
        //    //{ 
        //    // clean.Add(item.Split('#').First());  
        //    //}

        //    //clean = clean.Distinct().ToList();
        //    //foreach (var item in clean)
        //    //{
        //    //    await Console.Out.WriteLineAsync(item);
        //    //}

        //    //return;


        //    // Path to local Chrome installation
        //    var chromePath = @"C:\Users\Sayed\AppData\Local\Google\Chrome\Application\chrome.exe"; // Update this path accordingly

        //    // Launch the browser using the local Chrome installation
        //    var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //    {
        //        Headless = false, // Set to true if you don't want to see the browser window
        //        ExecutablePath = chromePath,
        //        DefaultViewport = null, // Ensure the default viewport is null
        //        Args = new[] { "--start-maximized" }
        //    });

        //    var pages = new List<Page>();

        //    // Open a new tab for each URL



        //    foreach (var url in urls)
        //    {
        //        await Console.Out.WriteLineAsync(url);
        //        try
        //        {
        //            var page = await browser.NewPageAsync();
        //            if (page != null)
        //            {
        //                await Task.Delay(200);
        //                pages.Add((Page)page);
        //                page.GoToAsync(url);
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            await Console.Out.WriteLineAsync("Error loading: " + url);

        //        }

        //    }

        //    // Give some time for pages to load
        //    //await Task.Delay(5000);
        //    //List<string> firstlink = new List<string>();
        //    //foreach (var page in pages)
        //    //{
        //    //    var content = await page.GetContentAsync();

        //    //    // Load the content into HtmlAgilityPack for parsing
        //    //    var htmlDocument = new HtmlDocument();
        //    //    htmlDocument.LoadHtml(content);

        //    //    // Find all URLs in the page (all href attributes in <a> tags)
        //    //    var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]")
        //    //        ?.Select(node => node.GetAttributeValue("href", string.Empty))
        //    //        .Where(href => !string.IsNullOrEmpty(href))
        //    //        .ToList();

        //    //    // Display found URLs in the console
        //    //    Console.WriteLine($"URLs found in {page.Url}:");
        //    //    if (links != null && links.Any())
        //    //    {
        //    //        foreach (var link in links)
        //    //        {
        //    //            firstlink.Add(link);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        Console.WriteLine("No URLs found.");
        //    //    }

        //    //    Console.WriteLine();
        //    //}

        //    Console.WriteLine("Scraping complete.  ");
        //    Console.WriteLine("--------------------------------------------------------");


        //    //List<string> cleann = new List<string>();

        //    //foreach (var item in firstlink)
        //    //{
        //    //    if(item.StartsWith("/"))
        //    //    {
        //    //        cleann.Add("https://beta.cbd.ae"+item);
        //    //    }  
        //    //}



        //    ////list all the clena urls
        //    //foreach (var item in cleann)
        //    //{
        //    //    Console.WriteLine(item);
        //    //}

        //    Console.ReadKey();
        //}


        private IBrowser browser;
        public async Task OpenUrlsAsync(List<string> urls)
        {
            // Download the Chromium revision if it does not already exist
            var chromePath = @"C:\Users\Sayed\AppData\Local\Google\Chrome\Application\chrome.exe";

            // Launch a new browser instance
              browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false, // Set to true if you don't want to see the browser window
                ExecutablePath = chromePath,
                DefaultViewport = null, // Ensure the default viewport is null
                Args = new[] { "--start-maximized" }
            });

            var tasks = new List<Task>();
            int i = 1;
            foreach (var url in urls)
            {
                await Console.Out.WriteLineAsync((i++) + ". " + url);
                tasks.Add(OpenPageAsync(browser, url));
            }

            await Task.WhenAll(tasks);  
            await browser.CloseAsync();
            //await Task.Delay(90000);
        }

        private async Task OpenPageAsync(IBrowser browser, string url)
        {
            using var page = await browser.NewPageAsync();
            try
            {
                // Increase the timeout to 60 seconds (60000 milliseconds)
                await page.GoToAsync(url, new NavigationOptions { Timeout = 60000 });
            }
            catch (PuppeteerSharp.NavigationException ex)
            {
                Console.WriteLine($"Failed to navigate to {url}: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout when navigating to {url}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when navigating to {url}: {ex.Message}");
            }
        }

        public async Task CloseBrowser()
        {
             
            await browser.CloseAsync();
        }
    }
}
