namespace CapaDominio.Invoice
{
    public interface IInvoiceReceptionError
    {
        int Code { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime DateReception { get; set; }
        string DocumentId { get; set; }
        int? DocumentType { get; set; }
        int IdEnterprise { get; set; }
        DateTime? IssueDate { get; set; }
        DateTime? IssueTime { get; set; }
        string Message { get; set; }
        decimal? MountTotal { get; set; }
        string PartyIdentificationId { get; set; }
        string SchemeId { get; set; }
        string UblVersionId { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}