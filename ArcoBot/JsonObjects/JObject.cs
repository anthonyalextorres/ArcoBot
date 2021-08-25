using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public abstract class JObject<T>
    {
        public JObject(object json)
        {
            if (json != null)
                JsonConvert.PopulateObject(json.ToString(), this);
        }
        public string Serialize()
        {
           return JsonConvert.SerializeObject(this);
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
