using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class CatReceptionExtensible
{
    public byte Id { get; set; }

    public int CodeExtensible { get; set; }

    public byte IdExtensibleType { get; set; }

    public byte IdDatatypeReceptionMetadata { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsListed { get; set; }

    public bool? IsChild { get; set; }

    public byte? IdRoot { get; set; }

    public int? IdCatExtensibleType { get; set; }

    public virtual ICollection<EnterpriseCatReceptionExtensible> EnterpriseCatReceptionExtensibles { get; set; } = new List<EnterpriseCatReceptionExtensible>();

    public virtual CatDatatypeReceptionMetadatum IdDatatypeReceptionMetadataNavigation { get; set; } = null!;

    public virtual CatReceptionExtensible? IdRootNavigation { get; set; }

    public virtual ICollection<CatReceptionExtensible> InverseIdRootNavigation { get; set; } = new List<CatReceptionExtensible>();
}
