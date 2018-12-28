using System;
using System.Collections.Generic;
using System.Dynamic;
using CitizenFX.Core.Native;
using CitizenFX.Core;

namespace FGMM.SDK.Client.UI
{
    public class UIManager
    {
        EventHandlerDictionary Events { get; set; }

        public UIManager(EventHandlerDictionary events)
        {
            Events = events;
        }

        #region NUI Implementation
        public void RegisterNUICallback(string msg, Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            Events.Add($"__cfx_nui:{msg}", new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                CallbackDelegate err = callback.Invoke(body, resultCallback);
            }));
        }
        #endregion
    }

    public static class DictionaryExtensions
    {
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.ContainsKey(key))
                if (dict[key] is T)
                    return (T)dict[key];
            return defaultVal;
        }
    }
}
