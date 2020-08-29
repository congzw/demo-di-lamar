using System;
using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.MultiTenancy
{
    public class TenantKeyHold
    {
        public IList<string> TenantKeys { get; set; } = new List<string> { string.Empty };

        public bool SameTenant(string tenant, string tenant2)
        {
            if (string.IsNullOrWhiteSpace(tenant))
            {
                return string.IsNullOrWhiteSpace(tenant2);
            }
            return tenant.Equals(tenant2, StringComparison.OrdinalIgnoreCase);
        }

        public static TenantKeyHold Build(params string[] tenants)
        {
            var tenantKeyHold = new TenantKeyHold();
            if (tenants == null || tenants.Length == 0)
            {
                return tenantKeyHold;
            }

            foreach (var tenant in tenants)
            {
                if (string.IsNullOrWhiteSpace(tenant))
                {
                    continue;
                }

                var theOne = tenantKeyHold.TenantKeys.SingleOrDefault(x => x.Equals(tenant.ToLower()));
                if (theOne == null)
                {
                    tenantKeyHold.TenantKeys.Add(tenant.ToLower());
                }
            }
            
            return tenantKeyHold;
        }
    }
}