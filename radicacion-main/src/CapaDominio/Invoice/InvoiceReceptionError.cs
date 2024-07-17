namespace CapaDominio.Invoice
{
    public class InvoiceReceptionError : IInvoiceReceptionError
    {
        public int IdEnterprise { get; set; }
        public DateTime DateReception { get; set; }
        public int Code { get; set; }
        public string? Message { get; set; }
        public string? SchemeId { get; set; }
        public string? PartyIdentificationId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? IssueTime { get; set; }
        public int? DocumentType { get; set; }
        public string? DocumentId { get; set; }
        public decimal? MountTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UblVersionId { get; set; }
    }
}
