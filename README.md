# Site Crawler

## Overview

**Site Crawler** is a Windows desktop toolkit for working with large URL lists. It uses your locally installed **Google Chrome** to open pages in controlled batches, track which URLs load successfully, surface failures, and optionally discover additional links on the same website.

The repository includes two complementary applications:

- **Site Crawler Advance** — a Windows Forms app for interactive crawling, discovery, and live results.
- **Site Overloader** — a console companion for opening many URLs from a prepared list file.

Together they help developers and site owners explore site structure, warm or exercise many pages, and maintain awareness of which URLs are reachable during a session.

## Gallery

![image](https://github.com/user-attachments/assets/e93c96ef-0c17-41d5-b3f3-4695ac3ce62e)

![image](https://github.com/user-attachments/assets/afedc9a1-44aa-4008-9c27-05fbb68a627f)

## Applications

### Site Crawler Advance (Windows Forms)

The main graphical application. Enter one or more seed URLs, configure how pages are grouped and how many crawl rounds should discover new links, then start the process. Chrome opens URLs in batches while the UI updates in real time.

### Site Overloader (Console)

A lightweight command-line tool that reads a `urls.txt` file from the working folder, splits the list into fixed-size groups, and opens each group in Chrome sequentially. Useful when you already have a URL list and only need batch loading without the full UI.

## Features

### Site Crawler Advance

- **Seed URL input** — Start from one or more base URLs entered in the application.
- **Grouped batch opening** — Control how many URLs are opened together in each Chrome session (Group Set).
- **Configurable link discovery** — Choose how many initial batches should crawl pages and collect new same-site links (Crawl Pages).
- **Same-host link discovery** — Automatically finds internal links that belong to the same domain as the seed URL.
- **Live result panels** — Separate views for successful loads, failures, and newly discovered URLs with running totals.
- **Queue-based crawling** — Discovered URLs are added to the work queue and processed until the queue is exhausted.
- **Persistent URL log** — Discovered URLs are saved to `urls.txt` in the application directory as crawling progresses.
- **Failure visibility** — Navigation errors, timeouts, and other load problems are recorded in the failed URLs panel.

### Site Overloader

- **File-driven URL lists** — Loads URLs from `urls.txt` (one URL per line).
- **Automatic chunking** — Splits long lists into manageable batches for sequential processing.
- **Batch page opening** — Opens each batch in Chrome before moving to the next group.

### Shared

- **Local Chrome integration** — Uses your installed Google Chrome browser (no separate remote service).
- **Graceful shutdown** — Open browser sessions are closed when the application exits.

### Planned enhancements

- Export inspection or crawl results to CSV and Excel.
- Scheduled automated runs.
- Email notifications for broken or failed URLs.
- Advanced filtering and reporting.

## Workflows

### Interactive crawl (Site Crawler Advance)

1. Launch **Site Crawler Advance**.
2. Enter seed URL(s) in the base URL field (one per line).
3. Set **Group Set** — how many URLs open per Chrome batch.
4. Set **Crawl Pages** — how many batches should discover new links from loaded pages.
5. Click **Initiate** to start.
6. Watch the three result areas update: successful loads, failures, and discovered URLs.
7. Review or reuse the generated `urls.txt` for discovered links.

### Batch URL loading (Site Overloader)

1. Create or update `urls.txt` with one URL per line in the application folder.
2. Run the **Site Overloader** console application.
3. The tool reads the file, processes URLs in fixed-size chunks, and opens each chunk in Chrome until the full list is handled.

## Requirements

- Windows
- [.NET](https://dotnet.microsoft.com/download) (7.0 for Site Crawler Advance; 8.0 for Site Overloader)
- [Google Chrome](https://www.google.com/chrome/) installed locally
- Visual Studio (recommended for building and running from source)

## Getting started

1. Clone this repository.
2. Open `Siteoverloader.sln` in Visual Studio (builds Site Crawler Advance).
3. Restore NuGet packages and build the solution.
4. Run **Site Crawler Advance** from Visual Studio, or build and run **Site Overloader** separately from its project folder with a prepared `urls.txt`.

Release builds of Site Crawler Advance can be produced via GitHub Releases (automated build workflow).

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
