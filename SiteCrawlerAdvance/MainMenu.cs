using SiteCrawlerAdvance.Helpers;
using System.Data;

namespace SiteCrawlerAdvance
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            numericGroupPages.Value = 5;
        }

        List<string> urlsFound = new List<string>();
        List<string> urlSucceed = new List<string>();
        List<string> urlsFailed = new List<string>();

        private void RunOnUiThread(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        private void SetStartButtonEnabled(bool enabled)
        {
            RunOnUiThread(() => btnStart.Enabled = enabled);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled)
                return;

            List<string> urls = txtBaseUrl.Text
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (urls.Count == 0)
                return;

            btnStart.Enabled = false;

            try
            {
                CrawlController helper = new CrawlController();
                helper.UrlCrawledSuccess += (url) =>
                {
                    url = Uri.UnescapeDataString(url);
                    RunOnUiThread(() =>
                    {
                        urlSucceed.Add(url);
                        urlSucceed = urlSucceed.Distinct().ToList();
                        urlSucceed.Sort();
                        lblTotalSuccess.Text = urlSucceed.Count.ToString();
                        txtSuccess.Text = string.Join(Environment.NewLine, urlSucceed);
                    });
                };

                helper.UrlCrawledFailed += (url) =>
                {
                    url = Uri.UnescapeDataString(url);
                    RunOnUiThread(() =>
                    {
                        urlsFailed.Add(url);
                        urlsFailed = urlsFailed.Distinct().ToList();
                        urlsFailed.Sort();
                        lblTotalFailed.Text = urlsFailed.Count.ToString();
                        txtFailed.Text = string.Join(Environment.NewLine, urlsFailed);
                    });
                };

                helper.OnNewUrlFound += (url) =>
                {
                    url = Uri.UnescapeDataString(url);
                    RunOnUiThread(() =>
                    {
                        urlsFound.Add(url);
                        urlsFound = urlsFound.Distinct().ToList();
                        urlsFound.Sort();
                        lblTotalUrlFound.Text = urlsFound.Count.ToString();
                        txtUrls.Text = string.Join(Environment.NewLine, urlsFound);
                        updateLogFile();
                    });
                };

                helper.CrawlCompleted += (_, _) => SetStartButtonEnabled(true);

                helper.StartCrawling(urls, Convert.ToInt32(numericGroupPages.Value), Convert.ToInt32(numericCrawlPages.Value));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start crawl: {ex.Message}");
                SetStartButtonEnabled(true);
            }
        }

        void updateLogFile()
        {
            //take path from current directory and append log.txt
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "urls.txt");

            //if file doesn exist create one
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path).Close();
            }

            //add all the urls with serial number to the file
            System.IO.File.WriteAllLines(path, urlsFound.Select((url, index) => (index + 1) + ". " + url));


        }

        private void txtBaseUrl_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
