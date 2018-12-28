using System;
using System.Threading.Tasks;

namespace FGMM.SDK.Client.Events
{
    public interface ITickManager
    {
        void Attach(Func<Task> callback);
        void Detach(Func<Task> callback);
    }
}
