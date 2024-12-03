using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rsa.src.http
{
    /* The `IHttpAction` interface defines two methods: `HandleAction()` and `ProcessArgs()`. */
    public interface ICliAction
    {
        Task HandleAction();
        // bool ProcessArgs();

        void SetupInstance();
    }
}