using ProxyPool.Common.Components.DependencyInjection;
using ProxyPool.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services
{
    public interface ITestService
    {
        string Get();

    }

    [AutoInjection(true)]
    public class TestService : ITestService
    {
        private readonly IProxyPoolService _proxyPoolService;

        public TestService(IProxyPoolService proxyPoolService)
        {
            _proxyPoolService = proxyPoolService;
        }

        public string Get()
        {
            _proxyPoolService.GetType();
            throw new NotImplementedException();
        }
    }
}
