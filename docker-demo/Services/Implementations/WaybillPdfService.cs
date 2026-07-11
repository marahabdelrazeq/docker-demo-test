using docker_demo.DTOs.WaybillDto;
using docker_demo.Services.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace docker_demo.Services.Implementations
{
    public class WaybillPdfService : IWaybillPdfService, IDisposable
    {
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private IBrowser? _browser;

        public async Task<byte[]> GeneratePdfAsync(WaybillPdfRequest request)
        {
            var browser = await GetBrowserAsync();
            var html = WaybillHtmlBuilder.Build(request);

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
                    await new BrowserFetcher().DownloadAsync();
                    _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true,
                        Args = ["--no-sandbox"],
                    });
                }
            }
            finally
            {
                _initLock.Release();
            }

            return _browser;
        }

        public void Dispose()
        {
            _browser?.CloseAsync().GetAwaiter().GetResult();
            _initLock.Dispose();
        }
    }
}
