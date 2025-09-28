using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreProtect.Infrastructure.Data;

public interface ICoreProtectSchemaVerifier
{
    Task<IReadOnlyCollection<string>> GetMissingTablesAsync(CancellationToken cancellationToken);
}
