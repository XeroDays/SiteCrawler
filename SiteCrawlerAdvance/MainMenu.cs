using SiteCrawlerAdvance.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiteCrawlerAdvance
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            numbericCount.Value = 5;
        }

        List<string> urlsFound = new List<string>();
        List<string> urlSucceed = new List<string>();
        List<string> urlsFailed = new List<string>();

        private void btnStart_Click(object sender, EventArgs e)
        {
            CrawlController helper = new CrawlController();
            helper.UrlCrawledSuccess += (url) =>
            {
                url = Uri.UnescapeDataString(url);
                urlSucceed.Add(url);
                urlSucceed = urlSucceed.Distinct().ToList();
                urlSucceed.Sort();
                lblTotalSuccess.Text = urlSucceed.Count.ToString();
                txtSuccess.Text = string.Join(Environment.NewLine, urlSucceed);
            };

            helper.UrlCrawledFailed += (url) =>
            {
                url = Uri.UnescapeDataString(url);
                urlsFailed.Add(url);
                urlsFailed = urlsFailed.Distinct().ToList();
                urlsFailed.Sort();
                lblTotalFailed.Text = urlsFailed.Count.ToString();
                txtFailed.Text = string.Join(Environment.NewLine, urlsFailed);
            };

            helper.OnNewUrlFound += (url) =>
            {
                url = Uri.UnescapeDataString(url);
                urlsFound.Add(url);
                urlsFound = urlsFound.Distinct().ToList();
                urlsFound.Sort();
                lblTotalUrlFound.Text = urlsFound.Count.ToString();
                txtUrls.Text = string.Join(Environment.NewLine, urlsFound);
                updateLogFile();
            };

            //take all  the urls from txturls and remove duplicates and again set to txturls


            List<string> urls = txtBaseUrl.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            helper.StartCrawling(urls, Convert.ToInt32(numbericCount.Value));
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
