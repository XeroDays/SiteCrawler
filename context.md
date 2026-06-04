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
| Purpose | Windows desktop tools that open URL batches in Google Chrome via browser automation; optional same-host link discovery and queue-based crawling |
| Architecture | Two standalone .NET apps; UI/orchestration → controller → Puppeteer browser helper; event-driven callbacks to UI |
| Primary app | `SiteCrawlerAdvance` — WinForms (`net7.0-windows7.0`) |
| Secondary app | `SiteOverloader` — console (`net8.0`); not in `Siteoverloader.sln` |
| Languages | C# |
| UI | Windows Forms (`MainMenu`) |
| Database | None |
| Persistence | `urls.txt` written at runtime (discovered URLs, numbered lines) |
| External services | Google Chrome (local executable); PuppeteerSharp drives installed Chrome |
| NuGet | PuppeteerSharp 18.0.0; HtmlAgilityPack 1.11.61 (referenced, unused in code) |
| Deployment | Local WinExe/console; GitHub Actions zips `SiteCrawlerAdvance` Release build on `release: created` |
| Solution | `Siteoverloader.sln` — only `SiteCrawlerAdvance` project |

---

## Architecture Rules

- **Entry:** `Program.cs` → `Application.Run(new MainMenu())` for WinForms app.
- **UI layer** (`MainMenu`) wires events, holds URL lists, updates labels/text boxes; does not launch browsers directly.
- **Orchestration** (`CrawlController`) manages queue, batch size, crawl-depth budget, and re-triggers next batch.
- **Browser automation** (`Crawler` / `Helper`) owns Puppeteer launch, tabs, navigation, optional link extraction.
- **No repository/service/API layers** — flat helper classes, static counters in `DataController`.
- **Cross-cutting:** delegates/events bubble URL state (started, success, failed, discovered) upward.
- **Chrome path:** resolved locally via standard Windows install paths; throws if missing.
- **Same-host crawl:** discovered links filtered by host match, query/hash stripped, `.pdf` skipped.
- **Scripts blocked** (advance crawler only): request interception aborts `ResourceType.Script`.
- **Process exit:** `AppDomain.ProcessExit` closes pending browser instances.

---

## Feature Registry

### Advanced URL Crawler (WinForms)

**Purpose:** User enters seed URLs; opens them in grouped Chrome sessions; optionally discovers same-host links for N initial batches; shows success/fail/found lists; persists discoveries to `urls.txt`.

**Entry Points:**
- `SiteCrawlerAdvance/Program.cs`
- `SiteCrawlerAdvance/MainMenu.cs` — `btnStart_Click`

**Primary Files:**
- `SiteCrawlerAdvance/MainMenu.cs`
- `SiteCrawlerAdvance/MainMenu.Designer.cs`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/Helpers/Crawler.cs`

**Related Files:**
- `SiteCrawlerAdvance/Helpers/DataController.cs` — serial counter `sno`
- `SiteCrawlerAdvance/SiteCrawlerAdvance.csproj`
- `urls.txt` (output under `Environment.CurrentDirectory`)

**Dependencies:**
- PuppeteerSharp → local Chrome
- `CrawlController` → `Crawler`

**Workflow:**
Start → parse `txtBaseUrl` lines → `CrawlController.StartCrawling` → batch dequeue (`numericGroupPages`) → `Crawler.OpenUrlsAsync` → navigate / optional `getUrlsFromPage` → events → UI lists + `updateLogFile` → `ReTrigger` until queue empty

---

### URL Batch Loader (Console)

**Purpose:** Read `urls.txt` from working directory; split into fixed-size chunks; open each chunk sequentially in one Chrome session per chunk.

**Entry Points:**
- `SiteOverloader/Program.cs`

**Primary Files:**
- `SiteOverloader/Program.cs`
- `SiteOverloader/Helper.cs`
- `SiteOverloader/DataController.cs`

**Related Files:**
- `SiteOverloader/urls.txt` (sample/input; copy to output)
- `SiteOverloader/SiteOverloader.csproj`

**Dependencies:**
- PuppeteerSharp → local Chrome
- `DataController.NumberOfTabsPerSession` (constant 20)

**Workflow:**
Start → load `urls.txt` → divide by 20 → foreach chunk: `Helper.OpenUrlsAsync` → parallel tabs per URL → close browser → next chunk

---

### Release Build & Publish

**Purpose:** On GitHub release, build WinForms app and attach zip artifact.

**Entry Points:**
- `.github/workflows/build.yml` — `on: release: types: [created]`

**Primary Files:**
- `.github/workflows/build.yml`

**Workflow:**
Release created → checkout → .NET 7 → `dotnet restore/build` solution → zip `SiteCrawlerAdvance/bin/Release/net7.0-windows7.0` → upload artifact → publish to GitHub release (`RELEASE_TOKEN`)

---

## Workflow Registry

### Start Advanced Crawl

**Trigger:** User clicks **Intiate** (`btnStart`).

**Flow:**
`MainMenu.btnStart_Click` → new `CrawlController` + event handlers → `StartCrawling(urls, groupSize, crawlPageCount)` → `ReTrigger` → `InitiateBunch` (≤ groupSize URLs) → `Crawler.OpenUrlsAsync` → per URL `OpenPageAsync` → success/fail events → discovered URLs enqueue → `ReTrigger` until `UrlsToComplete` empty

**Files:**
- `SiteCrawlerAdvance/MainMenu.cs`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/Helpers/Crawler.cs`

---

### Discover Same-Host Links

**Trigger:** `CrawlPagesCount > 0` for a batch (`canCrawl` true; decremented per batch).

**Flow:**
Page load → `getUrlsFromPage` (JS `querySelectorAll('a')`) → normalize URL → skip PDF → same host → `OnNewUrlFound` → `CrawlController` adds to `UrlsToComplete` → UI + log file

**Files:**
- `SiteCrawlerAdvance/Helpers/Crawler.cs` — `getUrlsFromPage`
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteCrawlerAdvance/MainMenu.cs` — `updateLogFile`

---

### Persist Discovered URLs

**Trigger:** Each new URL found (`OnNewUrlFound` in `MainMenu`).

**Flow:**
`updateLogFile` → `Environment.CurrentDirectory/urls.txt` → create if missing → write numbered lines from `urlsFound`

**Files:**
- `SiteCrawlerAdvance/MainMenu.cs` — `updateLogFile`

---

### Console Batch URL Open

**Trigger:** Run `SiteOverloader` executable with `urls.txt` present.

**Flow:**
`Program.cs` → read lines → chunk by `NumberOfTabsPerSession` → `Helper.OpenUrlsAsync` per chunk → `OpenPageAsync` (60s timeout) → close browser → next chunk

**Files:**
- `SiteOverloader/Program.cs`
- `SiteOverloader/Helper.cs`
- `SiteOverloader/DataController.cs`

---

### Graceful Browser Shutdown

**Trigger:** Process exit.

**Flow:**
`ProcessExit` handler → `CloseBrowser` on all `pending` crawlers/helpers

**Files:**
- `SiteCrawlerAdvance/Helpers/CrawlController.cs`
- `SiteOverloader/Program.cs`

---

## File Responsibility Map

| Responsibility | File |
|----------------|------|
| WinForms entry | `SiteCrawlerAdvance/Program.cs` |
| Main UI & URL lists / log | `SiteCrawlerAdvance/MainMenu.cs` |
| UI layout & controls | `SiteCrawlerAdvance/MainMenu.Designer.cs` |
| Crawl queue & batching | `SiteCrawlerAdvance/Helpers/CrawlController.cs` |
| Puppeteer crawl + link extract | `SiteCrawlerAdvance/Helpers/Crawler.cs` |
| Serial log counter (advance) | `SiteCrawlerAdvance/Helpers/DataController.cs` |
| Console entry | `SiteOverloader/Program.cs` |
| Puppeteer batch open | `SiteOverloader/Helper.cs` |
| Tab batch size constant | `SiteOverloader/DataController.cs` |
| Sample URL list | `SiteOverloader/urls.txt` |
| CI release build | `.github/workflows/build.yml` |
| Solution definition | `Siteoverloader.sln` |
| User-facing overview | `README.md` |

---

## Data Flow Map

**Advanced crawler:**
```
MainMenu (seed URLs, settings)
  → CrawlController (queue, dedupe, batch, crawl budget)
    → Crawler (Chrome/Puppeteer, tabs, navigation, link scrape)
      → Events (started / success / failed / found)
        → MainMenu (UI text boxes, counters)
        → urls.txt (numbered discovered URLs)
```

**Console overloader:**
```
urls.txt (working directory)
  → Program (chunk lists)
    → Helper (Chrome session per chunk, parallel page tasks)
      → Console log (serial + URL)
```

**No server, DB, or file DB layer** — only flat text `urls.txt`.

---

## Integration Registry

### Google Chrome (local)

| Field | Detail |
|-------|--------|
| Purpose | Browser engine for navigation and DOM link extraction |
| Auth | None — local `chrome.exe` |
| Detection | `GetChromePath()` — UserProfile / ProgramFiles paths |
| Entry points | `SiteCrawlerAdvance/Helpers/Crawler.cs`, `SiteOverloader/Helper.cs` |
| Library | PuppeteerSharp `LaunchAsync` with `ExecutablePath`, `Headless = false` |

### PuppeteerSharp

| Field | Detail |
|-------|--------|
| Purpose | .NET wrapper to control Chrome; tabs, navigation, request interception, JS evaluate |
| Files | `Crawler.cs`, `Helper.cs`; package refs in both `.csproj` |
| Auth | N/A |

### GitHub Actions / Releases

| Field | Detail |
|-------|--------|
| Purpose | Build Release WinForms output and attach zip to GitHub Release |
| Files | `.github/workflows/build.yml` |
| Auth | `secrets.RELEASE_TOKEN` |

### HtmlAgilityPack

| Field | Detail |
|-------|--------|
| Purpose | Listed in projects; **not used** in current code (import only in `Helper.cs`) |
| Files | `.csproj` references |

---

## Dependency Impact Map

| Module | Changing These Files | May Impact |
|--------|----------------------|------------|
| Main UI | `MainMenu.cs`, `MainMenu.Designer.cs` | Start trigger, displayed URLs, counters, log path |
| Crawl orchestration | `CrawlController.cs` | Batch size, queue order, crawl depth, re-trigger logic |
| Browser crawl | `Crawler.cs` | Navigation, timeouts, script blocking, link discovery rules |
| Console batch | `SiteOverloader/Program.cs`, `Helper.cs` | Chunking, tab concurrency, timeouts |
| Chrome resolution | `GetChromePath()` in either helper | Both apps fail to launch if paths change |
| Constants | `DataController.cs` (either project) | Log numbering; overloader tabs-per-session (20) |
| Build/release | `build.yml`, `SiteCrawlerAdvance.csproj` | CI artifact path, target framework |
| URL persistence | `MainMenu.updateLogFile` | Format/location of runtime `urls.txt` |

---

## Known Conventions

- **Namespaces:** `SiteCrawlerAdvance`, `SiteCrawlerAdvance.Helpers`, `SiteOverloader`
- **Partial WinForms:** logic in `MainMenu.cs`, designer in `MainMenu.Designer.cs`
- **Event pattern:** `delegate` + `event` for URL lifecycle (`UrlCrawledStarted`, `UrlCrawledSuccess`, `UrlCrawledFailed`, `OnNewUrlFound`)
- **URL normalization:** `Uri.UnescapeDataString`, strip query/`#`, trim trailing `/`, distinct + sort in UI lists
- **Batch naming:** UI label "Group Set" → `numericGroupPages`; "Crawl Pages" → `numericCrawlPages` (how many batches perform link discovery)
- **Default UI values:** `numericGroupPages` = 5 in ctor; designer defaults for crawl pages = 1
- **Failure prefixes:** `"Navigation: "`, `"Timeout: "`, `"Error: "` prepended in failed URL strings (advance)
- **Static serial:** `DataController.sno` incremented per URL logged to console
- **urls.txt:** copied to output via `.csproj` `CopyToOutputDirectory`; runtime write uses numbered prefix in advance app
- **Solution naming:** `Siteoverloader.sln` builds only advance project; overloader maintained separately in repo
- **README drift:** README mentions Selenium; codebase uses **PuppeteerSharp**

---

## Project Layout

```
SiteCrawler/
├── context.md                 ← this file
├── README.md
├── Siteoverloader.sln
├── SiteCrawlerAdvance/        ← primary WinForms app (in solution)
│   ├── Program.cs
│   ├── MainMenu.cs / .Designer.cs / .resx
│   └── Helpers/
│       ├── CrawlController.cs
│       ├── Crawler.cs
│       └── DataController.cs
├── SiteOverloader/            ← console app (not in solution)
│   ├── Program.cs
│   ├── Helper.cs
│   ├── DataController.cs
│   └── urls.txt
└── .github/workflows/build.yml
```

---

## Maintenance Rules (Summary)

Update `context.md` when any of the above changes. Keep sections synchronized with repo reality; remove stale entries (e.g. Selenium, unused integrations). Preserve top-of-file rules and "read without context" note.
