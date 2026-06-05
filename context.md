# AI Context Index — SiteCrawler

> **If this file is read without other context:** the user wants project context only. Use this index to navigate; do not re-scan the repo unless sections are stale.

## How to Use and Maintain This File

**Purpose:** AI context index for Cursor/LLM sessions. Not user documentation.

**When to update:** feature add/remove, workflow change, business logic change, file moves, architecture change, new integrations, DB/API contract changes.

**Format rules:**
- Short, information-dense sections
- No code snippets, install steps, or deep implementation detail
- Prefer file paths and workflow chains
- Concise, high signal, easy to search

**Do NOT include:** user docs, large explanations, implementation internals.

**Focus:** navigation, architecture, workflows, dependencies, change impact.

---

## Project Summary

| Item | Detail |
|------|--------|
| Purpose | Windows desktop app: open URL batches in local Google Chrome; optional same-host link discovery and queue-based crawling |
| Architecture | Single WinForms app; UI → orchestration → Puppeteer helper; delegate events bubble URL state upward |
| App | `SiteCrawlerAdvance` — WinForms (`net8.0-windows8.0`) |
| Languages | C# |
| UI | Windows Forms (`MainMenu`) |
| Database | None |
| Persistence | Runtime `urls.txt` in `Environment.CurrentDirectory` (discovered URLs, numbered lines) |
| External services | Local Google Chrome only; PuppeteerSharp drives installed `chrome.exe` |
| NuGet | PuppeteerSharp 20.2.1; HtmlAgilityPack 1.11.61 (referenced, unused) |
| Deployment | Local WinExe; GitHub Actions zips Release build on `release: published` |
| Solution | `Siteoverloader.sln` — `SiteCrawlerAdvance` only |
| License | MIT (`LICENSE.txt`) |

---

## Architecture Rules

- **Entry:** `SiteCrawlerAdvance/Program.cs` → `Application.Run(new MainMenu())`.
- **UI** (`MainMenu`) wires events, holds URL lists, thread-safe updates via `RunOnUiThread`; does not launch browsers directly.
- **Orchestration** (`CrawlController`) owns queue, dedupe at enqueue (`EnqueueUrl` vs `UrlsToComplete` + `UrlsDone`, normalized URL), batch dequeue, crawl-depth budget (`CrawlPagesCount`), sequential batches via `ReTrigger` → `InitiateBunch`.
- **Browser** (`Crawler` in `Helpers/` folder, `SiteCrawlerAdvance` namespace) owns Puppeteer launch, tabs, navigation, optional link extraction.
- **No service/repository/API layers** — flat helper classes; static `DataController.sno` for console serial logging.
- **Events:** `UrlCrawledStarted`, `UrlCrawledSuccess`, `UrlCrawledFailed`, `OnNewUrlFound`, `CrawlCompleted` — UI subscribes to all except `UrlCrawledStarted`.
- **Chrome path:** `GetChromePath()` probes standard Windows install paths; throws if missing.
- **Same-host crawl:** host match after normalize; query/hash stripped; non-HTML resource extensions skipped (pdf, images, docs, archives, media, fonts, css/js, etc.); non-http(s) skipped (`mailto:`, `tel:`, `javascript:`, `data:`); bare seed hosts get `https://` prefix via `NormalizeUrl` only at navigation — relative hrefs resolved to absolute in JS during discovery.
- **Dynamic DOM:** before link scrape, `WaitForFunctionAsync` (15s, 200ms poll, non-fatal timeout) waits for multi-region anchor stabilization (total `a[href]` plus header/nav/menu, nested menu, footer widgets when present on page) stable ≥800ms each, or async `#navbar-placeholder` / `#footer-placeholder` fast-path; on timeout, scrape whatever exists.
- **Same-host match:** compares hosts with leading `www.` stripped.
- **Process exit:** `AppDomain.ProcessExit` closes pending browser instances in `CrawlController`.
- **Headless:** `false` (visible Chrome).

---

## Feature Registry

### Advanced URL Crawler (WinForms)

**Purpose:** Seed URLs → grouped Chrome sessions → optional same-host discovery for N initial batches → live success/fail/found panels → `urls.txt` log.

**Entry Points:**
- `SiteCrawlerAdvance/Program.cs`
- `SiteCrawlerAdvance/MainMenu.cs` — `btnStart_Click`

**Primary Files:**
- `SiteCrawlerAdvance/MainMenu.cs`
- `SiteCrawlerAdvance/MainMenu.Designer.cs`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/Helpers/Crawler.cs`

**Related Files:**
- `SiteCrawlerAdvance/Helpers/DataController.cs` — static `sno` (namespace `SiteCrawlerAdvance`)
- `SiteCrawlerAdvance/MainMenu.resx`
- `SiteCrawlerAdvance/SiteCrawlerAdvance.csproj`
- `urls.txt` (runtime under working directory)

**Dependencies:**
- PuppeteerSharp → local Chrome
- `CrawlController` → `Crawler`

**Workflow:**
Initiate → parse `txtBaseUrl` → `StartCrawling(urls, groupSize, crawlPageCount)` → `ReTrigger` → batch (≤ `numericGroupPages`) → `OpenUrlsAsync` → navigate / optional `getUrlsFromPage` → events → UI + `updateLogFile` → next batch until queue empty → `CrawlCompleted` re-enables button

---

### Release Build & Publish

**Purpose:** On GitHub release publish, MSBuild Release and attach zip to release.

**Entry Points:**
- `.github/workflows/build.yml` — `on: release: types: [published]`

**Primary Files:**
- `.github/workflows/build.yml`

**Workflow:**
Release published → checkout → MSBuild restore/build `Siteoverloader.sln` Release → zip `SiteCrawlerAdvance/bin/Release` → upload artifact → `softprops/action-gh-release` (`secrets.RELEASE_TOKEN`)

---

### Planned (README only — not implemented)

CSV/Excel export, scheduled runs, email alerts for failures, advanced filtering/reporting.

---

## Workflow Registry

### Start Advanced Crawl

**Trigger:** User clicks **Intiate** (`btnStart`); button disabled until complete.

**Flow:**
`btnStart_Click` → `CrawlController` + handlers → `StartCrawling` → `ReTrigger` → dedupe/except done → dequeue batch → `InitiateBunch` → `OpenUrlsAsync` → per URL `OpenPageAsync` → events → `CloseBrowser` → `ReTrigger` until queue empty and `pending` empty → `CrawlCompleted`

**Files:**
- `SiteCrawlerAdvance/MainMenu.cs`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/Helpers/Crawler.cs`

---

### Discover Same-Host Links

**Trigger:** `CrawlPagesCount > 0` for batch (`canCrawl` true; decremented once per batch).

**Flow:**
`DOMContentLoaded` → optional `WaitForFunctionAsync` (multi-region anchor stabilization incl. `[class*="nav"]` / placeholder fast-path) → `getUrlsFromPage` (JS `a[href]` + `area[href]` + shadow DOM walk + explicit header/nav/footer/menu/nav-class region pass + same-origin iframes + all `[data-href]`/`[data-url]`, dedupe, skip non-navigable schemes, relative→absolute via page URL) → C# clean (query/hash, resource extension skip, http(s) only, mailto guard, www-normalized same host) → `EnqueueUrl` (distinct vs queue + done) → `OnNewUrlFound` → UI + log

**Files:**
- `SiteCrawlerAdvance/Helpers/Crawler.cs`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/MainMenu.cs`

---

### Persist Discovered URLs

**Trigger:** Each `OnNewUrlFound`.

**Flow:**
`updateLogFile` → `Environment.CurrentDirectory/urls.txt` → create if missing → overwrite numbered lines from `urlsFound`

**Files:**
- `SiteCrawlerAdvance/MainMenu.cs`

---

### Graceful Browser Shutdown

**Trigger:** Process exit.

**Flow:**
`ProcessExit` → `CloseBrowser` on all entries in `pending` list

**Files:**
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`

---

## File Responsibility Map

| Responsibility | File |
|----------------|------|
| WinForms entry | `SiteCrawlerAdvance/Program.cs` |
| Main UI, URL lists, log, UI thread marshaling | `SiteCrawlerAdvance/MainMenu.cs` |
| UI layout & controls | `SiteCrawlerAdvance/MainMenu.Designer.cs` |
| Crawl queue, batching, crawl budget, completion | `SiteCrawlerAdvance/Helpers/CrawlController.cs` |
| Puppeteer crawl, normalize, dynamic nav wait, link extract | `SiteCrawlerAdvance/Helpers/Crawler.cs` |
| Console serial counter | `SiteCrawlerAdvance/Helpers/DataController.cs` |
| CI release build | `.github/workflows/build.yml` |
| Solution | `Siteoverloader.sln` |
| User-facing overview | `README.md` |

---

## Data Flow Map

```
MainMenu (seed URLs, numericGroupPages, numericCrawlPages)
  → CrawlController (queue, dedupe, Except UrlsDone, batch size)
    → Crawler (Chrome/Puppeteer, tabs, DOMContentLoaded nav, anchor stabilization wait, shadow-DOM link scrape)
      → Events → MainMenu (txtSuccess / txtFailed / txtUrls, labels)
      → urls.txt (numbered discovered URLs)
```

**No server, DB, or ORM** — flat text `urls.txt` only.

---

## Integration Registry

### Google Chrome (local)

| Field | Detail |
|-------|--------|
| Purpose | Browser for navigation and DOM link extraction |
| Auth | None — local `chrome.exe` |
| Detection | `GetChromePath()` — UserProfile / LocalApplicationData / ProgramFiles paths |
| Entry point | `SiteCrawlerAdvance/Helpers/Crawler.cs` |
| Launch | `Puppeteer.LaunchAsync`, `Headless = false`, `ExecutablePath`, `--disable-blink-features=AutomationControlled` |

### PuppeteerSharp

| Field | Detail |
|-------|--------|
| Purpose | Control Chrome: tabs, navigation, JS evaluate, `WaitForFunctionAsync` for dynamic DOM |
| Files | `Crawler.cs`, `SiteCrawlerAdvance.csproj` |
| Auth | N/A |

### GitHub Actions / Releases

| Field | Detail |
|-------|--------|
| Purpose | MSBuild Release + zip artifact on GitHub Release |
| Files | `.github/workflows/build.yml` |
| Auth | `secrets.RELEASE_TOKEN` |

### HtmlAgilityPack

| Field | Detail |
|-------|--------|
| Purpose | Package reference only; no usage in code |
| Files | `SiteCrawlerAdvance.csproj` |

---

## Dependency Impact Map

| Module | Changing These Files | May Impact |
|--------|----------------------|------------|
| Main UI | `MainMenu.cs`, `MainMenu.Designer.cs` | Start/stop UX, panels, counters, log format, control defaults |
| Crawl orchestration | `CrawlController.cs` | Queue order, batch size, crawl depth, completion signal |
| Browser crawl | `Crawler.cs` | Navigation, timeouts, dynamic nav wait, URL normalize (navigation), discovery filters, relative URL resolution, Chrome args |
| Chrome resolution | `GetChromePath()` in `Crawler.cs` | App fails launch if paths wrong |
| Constants | `DataController.cs` | Console log numbering |
| Build/release | `build.yml`, `SiteCrawlerAdvance.csproj` | CI artifact path, TFM, package versions |
| URL persistence | `MainMenu.updateLogFile` | Runtime `urls.txt` location and format |

---

## Known Conventions

- **Namespaces:** `SiteCrawlerAdvance`, `SiteCrawlerAdvance.Helpers` (`CrawlController` only); `Crawler` and `DataController` live under `Helpers/` but use `SiteCrawlerAdvance` namespace
- **Partial WinForms:** logic `MainMenu.cs`, designer `MainMenu.Designer.cs`, resources `MainMenu.resx`
- **Event pattern:** `delegate` + `event` for URL lifecycle; `CrawlCompleted` uses `EventHandler`
- **UI thread:** `RunOnUiThread` / `BeginInvoke` for event handlers updating controls
- **URL normalization (UI lists):** `Uri.UnescapeDataString`, distinct, sort
- **URL normalization (navigation):** `NormalizeUrl` in `OpenPageAsync` — trim, prepend `https://` if no scheme; does not resolve relative paths
- **URL normalization (discovery):** JS resolves relative hrefs to absolute; C# strips query/`#`, trailing `/`, skips non-HTML resource extensions, http(s) only, same host (www-normalized)
- **Skipped link schemes:** `mailto:`, `tel:`, `javascript:`, `data:`, bare `#` (JS + C# mailto guard)
- **Link extraction:** all `a[href]` and `area[href]` (includes nested `ul > li` menus), recursive shadow DOM, explicit pass over header/nav/footer/`#links-list`/menu-content/`[class*="nav"]` roots, same-origin iframe traversal, all `[data-href]`/`[data-url]` elements
- **UI labels:** Group Set → `numericGroupPages` (default 5 in ctor); Crawl Pages → `numericCrawlPages` (designer default 1)
- **Button text:** `Intiate` (typo in designer)
- **Failure prefixes:** `HTTP {status}:`, `Navigation:`, `Timeout:`, `Error:` on failed URL strings
- **Navigation success:** main document HTTP status must be &lt; 400 (`GoToAsync` response); 404/5xx → `UrlCrawledFailed` with `HTTP {status}:` prefix
- **Navigation timeout:** 60s; waits `DOMContentLoaded`; link scrape may wait up to 15s for multi-region anchor stabilization
- **Static serial:** `DataController.sno` per console log line
- **urls.txt:** overwrites full discovered list on each new URL
- **README:** user docs; code uses PuppeteerSharp (not Selenium)

---

## Project Layout

```
SiteCrawler/
├── context.md
├── README.md
├── LICENSE.txt
├── Siteoverloader.sln
├── SiteCrawlerAdvance/
│   ├── Program.cs
│   ├── MainMenu.cs / .Designer.cs / .resx
│   └── Helpers/
│       ├── CrawlController.cs
│       ├── Crawler.cs
│       └── DataController.cs
└── .github/workflows/build.yml
```

---

## Maintenance Rules (Summary)

Update `context.md` when any of the above changes. Remove stale entries; keep top-of-file rules and the “read without context” note. Sync TFMs, package versions, and workflows with `.csproj` and `build.yml`.
