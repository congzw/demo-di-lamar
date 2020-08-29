using System;

namespace NbSites.Libs.Services
{
    public interface IHelloService
    {
        Guid InstanceId { get; set; }
        string Hello();
    }

    public class HelloService : IHelloService
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();

        public string Hello()
        {
            return this.GetType().Name;
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}", InstanceId, Hello());
        }
    }

    public class Hello2Service : IHelloService
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();

        public string Hello()
        {
            return this.GetType().Name;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", InstanceId, Hello());
        }
    }
}
