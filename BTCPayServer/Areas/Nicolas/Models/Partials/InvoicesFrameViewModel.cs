using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Models.InvoicingModels;
using BTCPayServer.Services.Invoices;

namespace BTCPayServer.Areas.Nicolas.Models.Partials
{
    public class InvoicesFrameViewModel
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public string SearchTerm { get; set; }
        public int? TimezoneOffset { get; set; }

        public List<InvoiceListItem> FrameInvoices { get; set; } = new List<InvoiceListItem>();
        public string StatusMessage { get; set; }
    }

    public class InvoiceListItem
    {
        public DateTimeOffset Date { get; set; }

        public string OrderId { get; set; }
        public string RedirectUrl { get; set; }
        public string InvoiceId { get; set; }

        public InvoiceStatus Status { get; set; }
        public string StatusString { get; set; }
        public bool CanMarkComplete { get; set; }
        public bool CanMarkInvalid { get; set; }
        public bool CanMarkStatus => CanMarkComplete || CanMarkInvalid;
        public bool ShowCheckout { get; set; }
        public string ExceptionStatus { get; set; }
        public string AmountCurrency { get; set; }
        public string StatusMessage { get; set; }

        public InvoiceDetailsModel Details { get; set; }
    }
}
