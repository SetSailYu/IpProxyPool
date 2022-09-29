using ProxyPool.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public static class FetcherHelper
    {

    }

    /// <summary>
    /// 基础抓取
    /// </summary>
    public abstract class BaseFetcher
    {
        protected abstract Task<ProxiesFetcherModel> DoFetcherAsync();
    }

}
