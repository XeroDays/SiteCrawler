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

        private async void btnStart_Click(object sender, EventArgs e)
        {
            CrawlController helper = new CrawlController();
            helper.UrlCrawledStarted += (url) =>
            {
                txtSuccess.Text += url + Environment.NewLine;
            };

            helper.UrlCrawledFailed+= (url) =>
            {
                ErrorLog.Text += url + Environment.NewLine;
            };
            await helper.StartCrawling(textBox1.Text, Convert.ToInt32(numbericCount.Value)); 
        }
    }
}
