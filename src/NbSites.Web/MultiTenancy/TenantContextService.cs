namespace NbSites.Web.MultiTenancy
{
    public class TenantContextService : ITenantContextService
    {
        public static TenantContext Mock = new TenantContext();

        public TenantContext GetCurrentTenantContext()
        {
            return Mock;
        }
    }
}