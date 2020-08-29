using System;

namespace NbSites.Web.MultiTenancy
{
    public class TenantConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantDescription { get; set; }
        public string TenantName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1}), {2}", this.TenantName, this.TenantDescription, this.Id);
        }
    }
}