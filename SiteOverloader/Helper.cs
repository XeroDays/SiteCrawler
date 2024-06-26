﻿using HtmlAgilityPack;
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
        public async Task OpenUrlsAsync(List<string> urls)
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
                await Console.Out.WriteLineAsync((DataController.sno++) + ". " + url);
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
