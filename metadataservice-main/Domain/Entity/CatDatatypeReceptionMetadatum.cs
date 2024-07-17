using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class CatDatatypeReceptionMetadatum
{
    public byte Id { get; set; }

    public int CodeMetadataDatatype { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CatReceptionExtensible> CatReceptionExtensibles { get; set; } = new List<CatReceptionExtensible>();
}
