 

  # Site URL Inspection

## Overview

**Site URL Inspection** is a Windows Forms application developed in C# that helps inspect and validate website URLs. The application crawls websites, discovers internal URLs, and verifies the accessibility of each page. It is designed to identify broken, dead, invalid, or inaccessible URLs, helping developers and website administrators maintain healthy website structures and improve user experience.

The tool can process large sets of URLs, validate response statuses, and provide insights into broken links across a website.

 

## Gallery
 ![image](https://github.com/user-attachments/assets/e93c96ef-0c17-41d5-b3f3-4695ac3ce62e)

![image](https://github.com/user-attachments/assets/afedc9a1-44aa-4008-9c27-05fbb68a627f)

## Features

* **Website Crawling**: Crawl websites and discover internal URLs automatically.
* **URL Validation**: Check every discovered URL for accessibility and response status.
* **Dead Link Detection**: Identify broken, dead, redirected, or invalid URLs.
* **Batch Processing**: Process and validate multiple URLs efficiently.
* **Website Health Monitoring**: Ensure all pages and links within a website are functioning correctly.
* **Detailed Inspection Results**: Review URL validation results and identify pages that require attention.
* **Future Enhancements**:

  * Export inspection reports to CSV and Excel formats.
  * Scheduled automated website inspections.
  * Email notifications for broken links.
  * Advanced filtering and reporting capabilities.

## Installation

To get started with the Site URL Inspection project, follow these steps:

1. **Clone the repository**:

   ```bash
   git clone https://github.com/XeroDays/SiteCrawler.git
   cd site-crawler
   ```

2. **Open the project in Visual Studio**:

   Open the `SiteCrawler.sln` file in Visual Studio.

3. **Install dependencies**:

   Ensure you have the necessary NuGet packages installed. The key dependencies include:

   * Selenium.WebDriver
   * Selenium.WebDriver.ChromeDriver
   * Selenium.Support

4. **Install ChromeDriver**:

   Download ChromeDriver from the official site and ensure it is available in your system's PATH or placed within the project directory.

## Usage

### Inspect a Website

1. Launch the application.
2. Enter the website URL you want to inspect.
3. Configure crawling and inspection settings as needed.
4. Start the inspection process.
5. Review the results to identify:

   * Broken URLs (404 errors)
   * Unreachable pages
   * Redirected URLs
   * Invalid links
   * Other HTTP response issues

### Inspection Workflow

1. **Enter Website URL**: Provide the starting URL or domain.
2. **Start Crawling**: The application crawls the website and discovers internal links.
3. **Validate URLs**: Each discovered URL is checked for availability and response status.
4. **Analyze Results**: Review detected issues and fix broken or problematic links.

## Contributing

We welcome contributions to enhance the Site URL Inspection project.

1. Fork the repository.

2. Create a feature branch:

   ```bash
   git checkout -b feature-branch
   ```

3. Commit your changes:

   ```bash
   git commit -m "Add new feature"
   ```

4. Push to your branch:

   ```bash
   git push origin feature-branch
   ```

5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Contact

For any questions, suggestions, or bug reports, please open an issue in the GitHub repository or contact the project maintainer.

---

Thank you for using **Site URL Inspection**. Your feedback and contributions help improve the project and make website maintenance easier for everyone.
