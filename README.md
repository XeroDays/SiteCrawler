# Site Crawler

## Overview

**Site Crawler** is a Windows desktop application for working with large URL lists. It uses your locally installed **Google Chrome** to open pages in controlled batches, track which URLs load successfully, surface failures, and optionally discover additional links on the same website.

**Site Crawler Advance** is a Windows Forms app for interactive crawling, discovery, and live results—helping developers and site owners explore site structure, warm or exercise many pages, and maintain awareness of which URLs are reachable during a session.

## Gallery

![image](https://github.com/user-attachments/assets/e93c96ef-0c17-41d5-b3f3-4695ac3ce62e)

![image](https://github.com/user-attachments/assets/afedc9a1-44aa-4008-9c27-05fbb68a627f)

## Application

### Site Crawler Advance (Windows Forms)

Enter one or more seed URLs, configure how pages are grouped and how many crawl rounds should discover new links, then start the process. Chrome opens URLs in batches while the UI updates in real time.

## Features

- **Seed URL input** — Start from one or more base URLs entered in the application.
- **Grouped batch opening** — Control how many URLs are opened together in each Chrome session (Group Set).
- **Configurable link discovery** — Choose how many initial batches should crawl pages and collect new same-site links (Crawl Pages).
- **Same-host link discovery** — Automatically finds internal links that belong to the same domain as the seed URL.
- **Live result panels** — Separate views for successful loads, failures, and newly discovered URLs with running totals.
- **Queue-based crawling** — Discovered URLs are added to the work queue and processed until the queue is exhausted.
- **Persistent URL log** — Discovered URLs are saved to `urls.txt` in the application directory as crawling progresses.
- **Failure visibility** — Navigation errors, timeouts, and other load problems are recorded in the failed URLs panel.
- **Local Chrome integration** — Uses your installed Google Chrome browser (no separate remote service).
- **Graceful shutdown** — Open browser sessions are closed when the application exits.

### Planned enhancements

- Export inspection or crawl results to CSV and Excel.
- Scheduled automated runs.
- Email notifications for broken or failed URLs.
- Advanced filtering and reporting.

## Workflow

### Interactive crawl

1. Launch **Site Crawler Advance**.
2. Enter seed URL(s) in the base URL field (one per line).
3. Set **Group Set** — how many URLs open per Chrome batch.
4. Set **Crawl Pages** — how many batches should discover new links from loaded pages.
5. Click **Initiate** to start.
6. Watch the three result areas update: successful loads, failures, and discovered URLs.
7. Review or reuse the generated `urls.txt` for discovered links.

## Requirements

- Windows
- [.NET 8](https://dotnet.microsoft.com/download)
- [Google Chrome](https://www.google.com/chrome/) installed locally
- Visual Studio (recommended for building and running from source)

## Getting started

1. Clone this repository.
2. Open `Siteoverloader.sln` in Visual Studio.
3. Restore NuGet packages and build the solution.
4. Run **Site Crawler Advance** from Visual Studio.

Release builds can be produced via GitHub Releases (automated build workflow).

## Contributing

Contributions are welcome.

1. Fork the repository.
2. Create a feature branch.
3. Make your changes and commit them.
4. Push to your branch.
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## Contact

For questions, suggestions, or bug reports, open an issue in the [GitHub repository](https://github.com/XeroDays/SiteCrawler/issues) or contact the project maintainer.

---

Thank you for using **Site Crawler**. Feedback and contributions are appreciated.
