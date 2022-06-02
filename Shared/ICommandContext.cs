using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Shared;

public interface ICommandContext<T>
{
    Task ReplyAsync(string message);
}