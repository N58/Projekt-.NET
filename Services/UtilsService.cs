using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Services
{
    public class UtilsService
    {
        public UtilsService()
        {

        }

        public T GetSession<T>(HttpContext ctx, string name) where T : new()
        {
            var jsonSession = ctx.Session.GetString(name);
            if (jsonSession != null)
                return JsonConvert.DeserializeObject<T>(jsonSession, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );
            else
                return new T();
        }

        public void SetSession(HttpContext ctx,  string name, object obj)
        {
            ctx.Session.SetString(name, JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        public void RemoveSession(HttpContext ctx, string name)
        {
            ctx.Session.Remove(name);
        }
    }
}
