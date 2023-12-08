using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeherEstateDevelopers.ApiModels
{
    public class API
    {
        public MetaObj Meta { get; set; }
        public Dictionary<string, object> Response { get; set; }
        public API()
        {
            this.Meta = new MetaObj();
            this.Response = new Dictionary<string, object>();
        }
    }
    public class MetaObj
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public MetaObj(string Status)
        {

        }

        public MetaObj()
        {
            this.Status = "404";
            this.Message = "Not Found";
        }
    }
}