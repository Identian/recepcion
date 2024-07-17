using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class EnterpriseCatReceptionExtensible
{
    public int Id { get; set; }

    public int IdEnterprise { get; set; }

    public byte IdCatReceptionExtensible { get; set; }

    public bool IsToSupplier { get; set; }

    public bool IsRequired { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Label { get; set; }

    public bool Active { get; set; }

    public virtual CatReceptionExtensible IdCatReceptionExtensibleNavigation { get; set; } = null!;

    public virtual ICollection<InvoiceReceptionExtensible> InvoiceReceptionExtensibles { get; set; } = new List<InvoiceReceptionExtensible>();
}
