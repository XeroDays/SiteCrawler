using PuppeteerSharp;
using System.Diagnostics;

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


        private IBrowser? browser;
        private string? userDataDir;
        private Process? browserProcess;

        public async Task OpenUrlsAsync(List<string> urls,bool canCrawl)
        {
            if (urls.Count == 0)
                return;

            string chromePath = GetChromePath();
            userDataDir = Path.Combine(Path.GetTempPath(), "SiteCrawler_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(userDataDir);

            browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                ExecutablePath = chromePath,
                UserDataDir = userDataDir,
                DefaultViewport = null,
                IgnoredDefaultArgs = new[] { "--enable-automation" },
                Args = new[]
                {
                    "--start-maximized",
                    "--disable-blink-features=AutomationControlled",
                    "--no-first-run",
                    "--no-default-browser-check",
                    "--disable-infobars"
                }
            });

            browserProcess = browser.Process;

            var startupPage = (await browser.PagesAsync()).FirstOrDefault();
            var tasks = new List<Task>();

            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];
                string log = (DataController.sno++) + ". " + url;
                await Console.Out.WriteLineAsync(log);
                UrlCrawledStarted(log);
                tasks.Add(OpenPageAsync(url, canCrawl, i == 0 ? startupPage : null));
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

        private static string NormalizeHost(string host)
        {
            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                return host.Substring(4);
            return host;
        }

        private static bool IsSameHost(string pageUrl, string discoveredUrl)
        {
            return NormalizeHost(new Uri(pageUrl).Host) == NormalizeHost(new Uri(discoveredUrl).Host);
        }

        private async Task OpenPageAsync(string url, bool canCrawl, IPage? reusePage)
        {
            if (browser == null)
                return;

            url = NormalizeUrl(url);
            var page = reusePage ?? await browser.NewPageAsync();
            var closePageWhenDone = reusePage == null;

            try
            {
                var response = await page.GoToAsync(url, new NavigationOptions
                {
                    Timeout = 60000,
                    WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
                });

                if (response == null || (int)response.Status >= 400)
                {
                    var status = response != null ? (int)response.Status : 0;
                    UrlCrawledFailed?.Invoke($"{url}");
                    return;
                }

                if (page.MainFrame == null)
                {
                    Console.WriteLine("Page is null");
                    return;
                }

               if(canCrawl)
                {
                    try
                    {
                        await page.WaitForFunctionAsync(
                            @"() => {
                                const navbarPlaceholder = document.getElementById('navbar-placeholder');
                                const footerPlaceholder = document.getElementById('footer-placeholder');
                                if (navbarPlaceholder || footerPlaceholder) {
                                    const dropdownLinks = document.querySelectorAll(
                                        'ul.dropdown-menu li a[href], ul li a.dropdown-item[href]'
                                    ).length;
                                    const placeholderNavLinks = navbarPlaceholder
                                        ? document.querySelectorAll('#navbar-placeholder a[href]').length
                                        : 0;
                                    const placeholderFooterLinks = footerPlaceholder
                                        ? document.querySelectorAll('#footer-placeholder a[href]').length
                                        : 0;
                                    if (navbarPlaceholder && (dropdownLinks > 0 || placeholderNavLinks > 5)) return true;
                                    if (footerPlaceholder && placeholderFooterLinks > 0) return true;
                                }

                                if (!window.__scrapeLinkWatch) {
                                    window.__scrapeLinkWatch = { last: 0, stableMs: 0, lastTs: Date.now() };
                                }

                                const count = document.querySelectorAll('a[href]').length;
                                const now = Date.now();
                                const watch = window.__scrapeLinkWatch;

                                if (count !== watch.last) {
                                    watch.last = count;
                                    watch.stableMs = 0;
                                    watch.lastTs = now;
                                    return false;
                                }

                                watch.stableMs += now - watch.lastTs;
                                watch.lastTs = now;
                                return count > 0 && watch.stableMs >= 800;
                            }",
                            new WaitForFunctionOptions { Timeout = 15000, PollingInterval = 200 });
                    }
                    catch (WaitTaskTimeoutException)
                    {
                        // Proceed with whatever links exist (static pages still work)
                    }

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
            finally
            {
                if (closePageWhenDone)
                {
                    try
                    {
                        await page.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to close page: {ex.Message}");
                    }
                }
            }
        }


        private async Task getUrlsFromPage(IPage page,string url)
        {
            // Extract all URLs from the page
            var newurls = await page.EvaluateExpressionAsync<string[]>(@"
                (() => {
                    const base = window.location.href;
                    const skip = h => !h || /^(mailto:|tel:|javascript:|data:|#)$/i.test(h.trim());
                    const seen = new Set();
                    const out = [];

                    const addHref = (href) => {
                        href = (href || '').trim();
                        if (skip(href)) return;
                        try {
                            const abs = new URL(href, base).href;
                            if (/^https?:\/\//i.test(abs) && !seen.has(abs)) {
                                seen.add(abs);
                                out.push(abs);
                            }
                        } catch (_) {}
                    };

                    const collectFromRoot = (root) => {
                        root.querySelectorAll('a[href]').forEach(a => addHref(a.getAttribute('href')));
                        root.querySelectorAll('*').forEach(el => {
                            if (el.shadowRoot) collectFromRoot(el.shadowRoot);
                        });
                    };

                    collectFromRoot(document);

                    document.querySelectorAll('nav [data-href], header [data-href], [role=""navigation""] [data-href], ul [data-href], nav [data-url], header [data-url], [role=""navigation""] [data-url], ul [data-url]').forEach(el => {
                        if (el.closest('a[href]')) return;
                        addHref(el.getAttribute('data-href') || el.getAttribute('data-url'));
                    });

                    return out;
                })()
            ");

            // Print the extracted URLs
            foreach (string urll in newurls)
            {
                if (urll.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string clean1 = urll.Split('?').ToList().First();
                clean1 = Uri.UnescapeDataString(clean1);
                clean1 = clean1.Split("/#").ToList().First();
                clean1 = clean1.Split("#").ToList().First();
                //remove last slash
                if (clean1.Last() == '/')
                {
                    clean1 = clean1.Substring(0, clean1.Length - 1);
                }

                if (clean1.Contains("mailto:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }


                //check if the url neding is pdf
                if (clean1.EndsWith(".pdf"))
                {
                    continue;
                }

                if (!Uri.TryCreate(clean1, UriKind.Absolute, out var parsedUrl)
                    || (parsedUrl.Scheme != Uri.UriSchemeHttp && parsedUrl.Scheme != Uri.UriSchemeHttps))
                {
                    continue;
                }

                if (IsSameHost(url, clean1))
                {
                    OnNewUrlFound?.Invoke(clean1);
                }
            }
        }


        public async Task CloseBrowser()
        {
            var processToKill = browserProcess;

            try
            {
                if (browser != null && browser.IsConnected)
                    await browser.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing browser: {ex.Message}");
            }
            finally
            {
                TerminateBrowserProcess(processToKill);
                browser = null;
                browserProcess = null;
                CleanupUserDataDir();
            }
        }

        private void TerminateBrowserProcess(Process? processToKill)
        {
            if (processToKill == null)
                return;

            try
            {
                if (!processToKill.HasExited)
                    processToKill.Kill(entireProcessTree: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to terminate browser process: {ex.Message}");

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = $"/PID {processToKill.Id} /T /F",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    })?.WaitForExit();
                }
                catch (Exception taskKillEx)
                {
                    Console.WriteLine($"Failed to taskkill browser process: {taskKillEx.Message}");
                }
            }
        }

        private void CleanupUserDataDir()
        {
            if (userDataDir == null || !Directory.Exists(userDataDir))
                return;

            try
            {
                Directory.Delete(userDataDir, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete browser profile: {ex.Message}");
            }
            finally
            {
                userDataDir = null;
            }
        }
    }
}
