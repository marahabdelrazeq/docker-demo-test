using docker_demo.DTOs.WaybillDto;
using docker_demo.Services.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace docker_demo.Services.Implementations
{
    public class WaybillPdfService : IWaybillPdfService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private readonly SemaphoreSlim _logoLock = new(1, 1);
        private IBrowser? _browser;
        private bool _logosLoaded;
        private string? _primaryLogoDataUri;
        private string? _secondaryLogoDataUri;

        public WaybillPdfService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> GeneratePdfAsync(WaybillPdfRequest request)
        {
            var browser = await GetBrowserAsync();
            await EnsureLogosLoadedAsync();
            var html = WaybillHtmlBuilder.Build(request, _primaryLogoDataUri, _secondaryLogoDataUri);

            await using var page = await browser.NewPageAsync();
            await page.SetJavaScriptEnabledAsync(false);
            await page.SetContentAsync(html);

            return await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "16px",
                    Bottom = "16px",
                    Left = "16px",
                    Right = "16px",
                },
            });
        }

        private async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser is not null)
            {
                return _browser;
            }

            await _initLock.WaitAsync();
            try
            {
                if (_browser is null)
                {
                    var cacheDir = Environment.GetEnvironmentVariable("PUPPETEER_CACHE_DIR");
                    var fetcherOptions = new BrowserFetcherOptions
                    {
                        Browser = SupportedBrowser.ChromeHeadlessShell,
                        Path = string.IsNullOrEmpty(cacheDir) ? null : cacheDir,
                    };

                    var installedBrowser = await new BrowserFetcher(fetcherOptions).DownloadAsync();
                    _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true,
                        Browser = SupportedBrowser.ChromeHeadlessShell,
                        ExecutablePath = installedBrowser.GetExecutablePath(),
                        Args = ["--no-sandbox", "--disable-dev-shm-usage", "--disable-gpu", "--disable-extensions"],
                    });
                }
            }
            finally
            {
                _initLock.Release();
            }

            return _browser;
        }

        private async Task EnsureLogosLoadedAsync()
        {
            if (_logosLoaded)
            {
                return;
            }

            await _logoLock.WaitAsync();
            try
            {
                if (!_logosLoaded)
                {
                    _primaryLogoDataUri = await TryLoadLogoAsync(_configuration["PdfDocSettings:PrimaryLogoPath"]);
                    _secondaryLogoDataUri = await TryLoadLogoAsync(_configuration["PdfDocSettings:SecondaryLogoPath"]);
                    _logosLoaded = true;
                }
            }
            finally
            {
                _logoLock.Release();
            }
        }

        private static async Task<string?> TryLoadLogoAsync(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            var mimeType = Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                _ => "image/png",
            };

            var bytes = await File.ReadAllBytesAsync(path);
            return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
        }

        public void Dispose()
        {
            _browser?.CloseAsync().GetAwaiter().GetResult();
            _initLock.Dispose();
            _logoLock.Dispose();
        }
    }
}
