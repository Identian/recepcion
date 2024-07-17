using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class CatReceptionExtensibleListedValue
{
    public int Id { get; set; }

    public int IdCodeExtensible { get; set; }

    public string Value { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public int? IdEnterprise { get; set; }

    public bool Active { get; set; }
}
