using Microsoft.EntityFrameworkCore;

namespace ProxyPool.Repository.DataSeed
{
    public interface IConfigureDataSeed
    {
        void ConfigureDataSeed(ModelBuilder builder);
    }

}
