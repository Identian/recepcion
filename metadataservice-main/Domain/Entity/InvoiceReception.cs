using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class InvoiceReception
{
    /// <summary>
    /// Comentario para  Primary Key invoice_reception
    /// </summary>
    public int Id { get; set; }

    public DateTime DateReception { get; set; }

    public int AccountId { get; set; }

    public string? PersonFirstname { get; set; }

    public string? PersonMiddlename { get; set; }

    public string? PersonFamilyname { get; set; }

    public string? RegistrationName { get; set; }

    public string? PartyName { get; set; }

    public string SchemeId { get; set; } = null!;

    public string? PartyIdentificationId { get; set; }

    public DateOnly IssueDate { get; set; }

    public TimeOnly? IssueTime { get; set; }

    public string DocumentType { get; set; } = null!;

    public string DocumentId { get; set; } = null!;

    public string? Uuid { get; set; }

    public int Accuse { get; set; }

    public int PaymentStatusId { get; set; }

    public int Status { get; set; }

    public int Active { get; set; }

    public int ReceptionType { get; set; }

    public int IdEnterpriseIssues { get; set; }

    public int IdEnterpriseSupplier { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CaseNumber { get; set; }

    public decimal? MountTotal { get; set; }

    public int? EnterpriseConsecutive { get; set; }

    public int? LastConstraintStatus { get; set; }

    public bool? WasSendReceive { get; set; }

    public bool? WasSendResponse { get; set; }

    public int? StatusDian { get; set; }

    public string? StatusDianMessage { get; set; }

    public string? StatusDianValidation { get; set; }

    public DateTime? StatusDianDatetime { get; set; }

    public string? TechProviderPartyIdentificationId { get; set; }

    public string? DeliveryNote { get; set; }

    public string? TechProviderSoftwareId { get; set; }

    public string UblVersionId { get; set; } = null!;

    public DateTime IssueDatetime { get; set; }

    public decimal? LineExtensionAmount { get; set; }

    public decimal? TaxExclusiveAmount { get; set; }

    public decimal? TaxInclusiveAmount { get; set; }

    public decimal? AllowanceTotalAmount { get; set; }

    public decimal? ChargeTotalAmount { get; set; }

    public decimal? PrePaidAmount { get; set; }

    public string? Currency { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal? CalculationRate { get; set; }

    public bool? InfodatosRequiredNotSet { get; set; }

    public bool? HasWarnings { get; set; }

    public virtual ICollection<InvoiceReceptionExtensible> InvoiceReceptionExtensibles { get; set; } = new List<InvoiceReceptionExtensible>();
}
