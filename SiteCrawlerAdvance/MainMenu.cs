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

        private async void btnStart_Click(object sender, EventArgs e)
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
            };

            //take all  the urls from txturls and remove duplicates and again set to txturls



            await helper.StartCrawling(textBox1.Text, Convert.ToInt32(numbericCount.Value));
        }
    }
}
