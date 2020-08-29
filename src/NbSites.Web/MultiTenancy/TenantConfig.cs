using System;

namespace NbSites.Web.MultiTenancy
{
    public class TenantConfig
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public string TenantName { get; set; }
        public string TenantKey { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1}), {2}", this.TenantKey, this.TenantName, this.InstanceId);
        }
    }
}