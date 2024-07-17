using Domain.Interfaces;
using Domain.Request;

namespace Domain.Entity.Dto
{
    public class TransportData
    {
        public int InvoiceId { get; set; }
        public int InvoiceIdEnterpriseSupplier {  get; set; }
        public int IdEnterprise { get; set; }
        public List<MetadataRequest> Metadata { get; set; } = [];
        public ILogAzure LogAzure { get; set; } = null!;
    }
}
