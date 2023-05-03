using System.Data;

namespace Inventory.Application.Interfaces.Persistence
{
    public class DapperConfig
    {
        public string ConnectionString { get; set; } = default!;
    }
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
