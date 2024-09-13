# Site Crawler Project

## Overview

The Site Crawler project is a Windows Forms application developed in C#. It automates the process of opening multiple URLs in Chrome browser instances. The application allows users to input a list of URLs and a count to group them. Each group of URLs is opened in separate Chrome tabs, and once a page is completely loaded, the tab is closed automatically. Future enhancements will enable the crawler to find and open URLs from a base domain automatically.


## Gallery

![image](https://github.com/user-attachments/assets/afedc9a1-44aa-4008-9c27-05fbb68a627f)


## Features

- **Group URLs**: Input a list of URLs and a count to group them. Each group is opened in separate Chrome tabs.
- **Automatic Tab Closure**: Tabs are closed automatically once the page is fully loaded.
- **Future Enhancements**:
  - Automatically crawl a base domain to find and retrieve URLs.
  - Group and open the discovered URLs in Chrome tabs.

## Installation

To get started with the Site Crawler project, follow these steps:

1. **Clone the repository**:
    ```bash
    git clone https://github.com/XeroDays/SiteCrawler.git
    cd site-crawler
    ```

2. **Open the project in Visual Studio**:
    Open the `SiteCrawler.sln` file in Visual Studio.

3. **Install dependencies**:
    Ensure you have the necessary NuGet packages installed. The key dependencies include:
    - Selenium.WebDriver
    - Selenium.WebDriver.ChromeDriver
    - Selenium.Support

4. **Install ChromeDriver**:
    Download ChromeDriver from the official site [here](https://sites.google.com/chromium.org/driver/) and ensure it's in your system's PATH or place it in the project directory.

## Usage

To use the Site Crawler, follow these steps:

1. **Prepare your URLs list**:
    Create a text file (`urls.txt`) with one URL per line:
    ```plaintext
    https://example.com/page1
    https://example.com/page2
    https://example.com/page3
    ```

2. **Run the Site Crawler application**:
    Open the project in Visual Studio and run the application. The Windows Forms interface will allow you to:
    - Specify the number of URLs per group.
    - Load the URLs from the text file.

### Steps in the Application:

1. **Open the application**: Launch the application from Visual Studio or the executable file.
2. **Input the number of URLs per group**: Enter the desired number in the provided textbox.
3. **Load the URLs file**: Click the "Load URLs" button and select the `urls.txt` file.
4. **Start the crawling process**: Click the "Start" button to begin opening the URLs in groups in Chrome.

## Contributing

We welcome contributions to enhance the Site Crawler project! To contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes and commit them (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or suggestions, please open an issue in the [GitHub repository](https://github.com/XeroDays/SiteCrawler/issues) or contact the project maintainer.

---

Thank you for using the Site Crawler project! Your feedback and contributions are highly appreciated.
