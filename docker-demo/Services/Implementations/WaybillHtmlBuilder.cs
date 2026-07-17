using System.Net;
using System.Text;
using docker_demo.DTOs.WaybillDto;

namespace docker_demo.Services.Implementations
{
    public static class WaybillHtmlBuilder
    {
        private static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

        private static string Cell(string label, string? value) =>
            $"<div class=\"cell\"><span class=\"label\">{E(label)}</span><span class=\"value\">{E(value)}</span></div>";

        private static string Row(int columns, params string[] cells) =>
            $"<div class=\"row\" style=\"grid-template-columns:repeat({columns},1fr)\">{string.Concat(cells)}</div>";

        private static string Section(string title, string rowsHtml) =>
            $"<div class=\"section\"><div class=\"section-header\">{E(title)}</div><div class=\"section-body\">{rowsHtml}</div></div>";

        public static string Build(WaybillPdfRequest r, string? primaryLogoDataUri = null, string? secondaryLogoDataUri = null)
        {
            var primaryLogo = primaryLogoDataUri is null
                ? "<div class=\"logo\">نافذ</div>"
                : $"<img class=\"logo-img\" src=\"{E(primaryLogoDataUri)}\" alt=\"نافذ\" />";

            var secondaryLogo = secondaryLogoDataUri is null
                ? "<div class=\"seal\">وزارة النقل والاتصالات وتقنية المعلومات</div>"
                : $"<img class=\"seal-img\" src=\"{E(secondaryLogoDataUri)}\" alt=\"MTCIT\" />";

            var sb = new StringBuilder();

            sb.Append(Section("معلومات الوثيقة", string.Concat(
                Row(2, Cell("اسم الشركة", r.Document.CompanyName), Cell("رقم الوثيقة", r.Document.DocumentNumber)),
                Row(2, Cell("رقم الفاتورة", r.Document.InvoiceNumber), Cell("نوع الحمولة", r.Document.CargoType))
            )));

            var goods = Section("معلومات البضاعة", string.Concat(
                Row(2, Cell("رقم الحاوية الأول", r.Goods.FirstContainerNumber), Cell("رقم الحاوية الثانية", r.Goods.SecondContainerNumber)),
                Row(1, Cell("قيمة البضاعة", r.Goods.GoodsValue)),
                Row(1, Cell("وصف البضاعة", r.Goods.GoodsDescription))
            ));

            var loading = Section("معلومات التحميل", string.Concat(
                Row(1, Cell("بلد التحميل", r.Loading.Country)),
                Row(1, Cell("محافظة التحميل", r.Loading.Governorate)),
                Row(1, Cell("مصدر التحميل", r.Loading.Source)),
                Row(1, Cell("موقع التحميل", r.Loading.Location)),
                Row(1, Cell("تاريخ التحميل", r.Loading.Date)),
                Row(1, Cell("وقت التحميل", r.Loading.Time)),
                Row(1, Cell("عنوان التحميل", r.Loading.Address))
            ));

            sb.Append($"<div class=\"pair\">{goods}{loading}</div>");

            sb.Append(Section("معلومات التفريغ", string.Concat(
                Row(2, Cell("بلد التفريغ", r.Unloading.Country), Cell("محافظة التفريغ", r.Unloading.Governorate)),
                Row(2, Cell("مدينة التفريغ", r.Unloading.City), Cell("موقع التفريغ", r.Unloading.Location)),
                Row(2, Cell("تاريخ التفريغ", r.Unloading.Date), Cell("وقت التفريغ", r.Unloading.Time)),
                Row(1, Cell("عنوان التفريغ", r.Unloading.Address))
            )));

            var trailer = Section("معلومات المقطورة", string.Concat(
                Row(1, Cell("رقم اللوحة", r.Trailer.PlateNumber)),
                Row(1, Cell("ترميز اللوحة", r.Trailer.PlateCode)),
                Row(1, Cell("صفة الاستعمال", r.Trailer.UsageType))
            ));

            var tractor = Section("معلومات القاطرة والناقل", string.Concat(
                Row(1, Cell("شركة النقل", r.Tractor.CarrierCompany)),
                Row(1, Cell("رقم اللوحة", r.Tractor.PlateNumber)),
                Row(1, Cell("ترميز اللوحة", r.Tractor.PlateCode)),
                Row(1, Cell("صفة الاستعمال", r.Tractor.UsageType))
            ));

            sb.Append($"<div class=\"pair\">{trailer}{tractor}</div>");

            static string InsuranceRows(InsuranceInfo i) => string.Concat(
                Row(1, Cell("نوع التأمين", i.InsuranceType)),
                Row(1, Cell("شركة التأمين", i.InsuranceCompany)),
                Row(1, Cell("رقم عقد التأمين", i.PolicyNumber)),
                Row(1, Cell("مبلغ التأمين (بالريال العماني)", i.AmountOmr)),
                Row(1, Cell("تاريخ إنتهاء التأمين", i.ExpiryDate))
            );

            var clearanceInsurance = Section("معلومات التأمين شركة التخليص", InsuranceRows(r.ClearanceInsurance));
            var carrierInsurance = Section("معلومات التأمين شركة النقل", InsuranceRows(r.CarrierInsurance));

            sb.Append($"<div class=\"pair\">{clearanceInsurance}{carrierInsurance}</div>");

            sb.Append(Section("معلومات السائق", string.Concat(
                Row(2, Cell("رقم وثيقة السائق", r.Driver.LicenseNumber), Cell("اسم السائق", r.Driver.Name)),
                Row(2, Cell("جنسية السائق", r.Driver.Nationality), Cell("رقم الهاتف", r.Driver.Phone))
            )));

            sb.Append(Section("معلومات الأجور", string.Concat(
                Row(2, Cell("السعر (بالريال العماني)", r.Wages.PriceOmr), Cell("طريقة الدفع", r.Wages.PaymentMethod)),
                Row(1, Cell("الدفعة المقدمة (بالريال العماني)", r.Wages.AdvancePaymentOmr))
            )));

            sb.Append(Section("معلومات بدلات التأخير", Row(2,
                Cell("مبلغ التأخير لشركة النقل", r.Delay.CarrierDelayAmount),
                Cell("مبلغ التأخير لشركة التخليص", r.Delay.ClearanceDelayAmount)
            )));

            return $$"""
                <!doctype html>
                <html lang="ar" dir="rtl">
                <head>
                <meta charset="UTF-8" />
                <style>{{Css}}</style>
                </head>
                <body>
                <div class="waybill">
                    <header class="header">
                    {{primaryLogo}}
                    <h1 class="title">طباعة وثيقة نقل</h1>
                    {{secondaryLogo}}
                    </header>
                    <div class="meta">
                    <div class="meta-cell">{{Cell("تاريخ الطباعة", r.Meta.PrintDate)}}</div>
                    <div class="meta-cell">{{Cell("بواسطة", r.Meta.Via)}}</div>
                    <div class="meta-cell">{{Cell("تاريخ إنشاء الوثيقة", r.Meta.CreatedDate)}}</div>
                    </div>
                    {{sb}}
                    <footer class="footer">
                    <div class="sig-block">
                        <span class="label">التوقيع الإلكتروني</span>
                        <div class="barcode"></div>
                    </div>
                    <div class="sig-block">
                        <span class="label">اسم الموظف وتوقيعه</span>
                        <div class="sig-line"></div>
                    </div>
                    </footer>
                </div>
                </body>
                </html>
                """;
        }

        private const string Css = """
            * { box-sizing: border-box; }
            body { margin: 0; font-family: 'Cairo', sans-serif; color: #0d2b47; background: #fff; }
            .waybill { direction: rtl; padding: 24px 28px 32px; }
            .header { display: flex; align-items: flex-start; justify-content: space-between; gap: 16px; padding-bottom: 16px; }
            .logo { font-weight: 800; font-size: 14px; color: #1e4181; }
            .title { margin: 0; font-size: 22px; font-weight: 800; color: #0d2b47; text-align: center; flex: 1; }
            .seal { max-width: 160px; font-size: 9px; color: #1f2937; text-align: left; }
            .meta { display: grid; grid-template-columns: repeat(3, 1fr); border: 1px solid #e3e3e5; border-radius: 6px; margin-bottom: 18px; overflow: hidden; }
            .meta-cell { padding: 10px 14px; border-inline-start: 1px solid #e3e3e5; }
            .meta-cell:first-child { border-inline-start: none; }
            .section { border: 1px solid #e3e3e5; border-radius: 6px; margin-bottom: 14px; overflow: hidden; break-inside: avoid; }
            .section-header { background: #f6f6f6; color: #87722e; font-weight: 700; font-size: 14px; padding: 8px 14px; text-align: center; }
            .row { display: grid; border-top: 1px solid #e3e3e5; }
            .row:first-child { border-top: none; }
            .cell { display: flex; align-items: center; gap: 6px; padding: 9px 14px; border-inline-start: 1px solid #e3e3e5; font-size: 13px; }
            .cell:first-child { border-inline-start: none; }
            .label { color: #0d2b47; font-weight: 700; white-space: nowrap; }
            .value { color: #0d2b47; font-weight: 400; white-space: nowrap; }
            .pair { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; margin-bottom: 14px; }
            .pair .section { margin-bottom: 0; }
            .footer { display: flex; align-items: center; justify-content: space-between; margin-top: 24px; gap: 24px; }
            .sig-block { display: flex; flex-direction: column; gap: 8px; }
            .barcode { width: 140px; height: 32px; background: repeating-linear-gradient(90deg, #111827 0, #111827 2px, transparent 2px, transparent 4px); }
            .sig-line { width: 180px; height: 1px; border-bottom: 1px solid #9ca3af; }
            """;
    }
}
