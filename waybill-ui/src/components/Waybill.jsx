import "./Waybill.css";

const documentInfo = [
  [
    { label: "اسم الشركة", value: "شركة التخليص المتحدة" },
    { label: "رقم الوثيقة", value: "31867" },
  ],
  [
    { label: "رقم الفاتورة", value: "85858" },
    { label: "نوع الحمولة", value: "حاويات" },
  ],
];

const goodsInfo = [
  [
    { label: "رقم الحاوية الأول", value: "7174171717" },
    { label: "رقم الحاوية الثانية", value: "7174171717" },
  ],
  [{ label: "قيمة البضاعة", value: "25" }],
  [{ label: "وصف البضاعة", value: "يتم كتابة وصف البضاعة هنا..", full: true }],
];

const loadingInfo = [
  [{ label: "بلد التحميل", value: "سلطنة عمان" }],
  [{ label: "محافظة التحميل", value: "مسقط" }],
  [{ label: "مصدر التحميل", value: "داخل المدينة" }],
  [{ label: "موقع التحميل", value: "رسيل" }],
  [{ label: "تاريخ التحميل", value: "06/05/2026" }],
  [{ label: "وقت التحميل", value: "05:05 AM" }],
  [{ label: "عنوان التحميل", value: "ساحة المناولة" }],
];

const unloadingInfo = [
  [
    { label: "بلد التفريغ", value: "سلطنة عمان" },
    { label: "محافظة التفريغ", value: "صحار" },
  ],
  [
    { label: "مدينة التفريغ", value: "صحار" },
    { label: "موقع التفريغ", value: "إلى الميناء" },
  ],
  [
    { label: "تاريخ التفريغ", value: "06/05/2026" },
    { label: "وقت التفريغ", value: "05:05 AM" },
  ],
  [{ label: "عنوان التفريغ", value: "-" }],
];

const tractorInfo = [
  [{ label: "شركة النقل", value: "شركة الهدى للنقل" }],
  [{ label: "رقم اللوحة", value: "5558" }],
  [{ label: "ترميز اللوحة", value: "أ ج د" }],
  [{ label: "صفة الاستعمال", value: "سطحة" }],
];

const trailerInfo = [
  [{ label: "رقم اللوحة", value: "5558" }],
  [{ label: "ترميز اللوحة", value: "أ ج د" }],
  [{ label: "صفة الاستعمال", value: "سطحة" }],
];

const carrierInsurance = [
  [{ label: "نوع التأمين", value: "تأمين" }],
  [{ label: "شركة التأمين", value: "شركة النور للتأمين" }],
  [{ label: "رقم عقد التأمين", value: "933214478525868" }],
  [{ label: "مبلغ التأمين (بالريال العماني)", value: "500" }],
  [{ label: "تاريخ إنتهاء التأمين", value: "06/05/2027" }],
];

const clearanceInsurance = carrierInsurance;

const driverInfo = [
  [
    { label: "رقم وثيقة السائق", value: "85858239" },
    { label: "اسم السائق", value: "محمد نجيب الفاروقي" },
  ],
  [
    { label: "جنسية السائق", value: "باكستاني" },
    { label: "رقم الهاتف", value: "0553902062" },
  ],
];

const wagesInfo = [
  [
    { label: "السعر (بالريال العماني)", value: "500" },
    { label: "طريقة الدفع", value: "كاش" },
  ],
  [{ label: "الدفعة المقدمة (بالريال العماني)", value: "250" }],
];

const delayInfo = [
  [
    { label: "مبلغ التأخير لشركة النقل", value: "250" },
    { label: "مبلغ التأخير لشركة التخليص", value: "500" },
  ],
];

function Section({ title, rows, columns = 2 }) {
  return (
    <div className="section">
      <div className="section__header">{title}</div>
      <div className="section__body">
        {rows.map((row, i) => (
          <div
            className="section__row"
            key={i}
            style={{ gridTemplateColumns: `repeat(${row[0]?.full ? 1 : columns}, 1fr)` }}
          >
            {row.map((cell, j) => (
              <div className="section__cell" key={j}>
                <span className="section__label">{cell.label}</span>
                <span className="section__value">{cell.value}</span>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}

export default function Waybill() {
  return (
    <div className="waybill-page">
      <div className="waybill">
        <header className="waybill__header">
          <div className="waybill__logo">
            <div className="waybill__logo-mark" />
            <span>نافذ</span>
          </div>
          <h1 className="waybill__title">طباعة وثيقة نقل</h1>
          <div className="waybill__seal">
            <div className="waybill__seal-mark" />
            <span>وزارة النقل والاتصالات وتقنية المعلومات</span>
          </div>
        </header>

        <div className="waybill__meta">
          <div className="waybill__meta-cell">
            <span className="section__label">تاريخ الطباعة</span>
            <span className="section__value">2026-06-11</span>
          </div>
          <div className="waybill__meta-cell">
            <span className="section__label">بواسطة</span>
            <span className="section__value">شركة التخليص المتحدة</span>
          </div>
          <div className="waybill__meta-cell">
            <span className="section__label">تاريخ إنشاء الوثيقة</span>
            <span className="section__value">2026-06-08</span>
          </div>
        </div>

        <Section title="معلومات الوثيقة" rows={documentInfo} columns={2} />

        <div className="waybill__pair">
          <Section title="معلومات البضاعة" rows={goodsInfo} columns={2} />
          <Section title="معلومات التحميل" rows={loadingInfo} columns={1} />
        </div>

        <Section title="معلومات التفريغ" rows={unloadingInfo} columns={2} />

        <div className="waybill__pair">
          <Section title="معلومات المقطورة" rows={trailerInfo} columns={1} />
          <Section title="معلومات القاطرة والناقل" rows={tractorInfo} columns={1} />
        </div>

        <div className="waybill__pair">
          <Section title="معلومات التأمين شركة التخليص" rows={clearanceInsurance} columns={1} />
          <Section title="معلومات التأمين شركة النقل" rows={carrierInsurance} columns={1} />
        </div>

        <Section title="معلومات السائق" rows={driverInfo} columns={2} />
        <Section title="معلومات الأجور" rows={wagesInfo} columns={2} />
        <Section title="معلومات بدلات التأخير" rows={delayInfo} columns={2} />

        <footer className="waybill__footer">
          <div className="waybill__signature-block">
            <span className="section__label">التوقيع الإلكتروني</span>
            <div className="waybill__barcode" />
          </div>
          <div className="waybill__signature-line">
            <span className="section__label">اسم الموظف وتوقيعه</span>
            <div className="waybill__line" />
          </div>
        </footer>
      </div>
    </div>
  );
}
