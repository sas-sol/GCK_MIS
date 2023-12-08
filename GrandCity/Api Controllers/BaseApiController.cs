using MeherEstateDevelopers.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MeherEstateDevelopers.Api_Controllers
{
    public class BaseApiController : ApiController
    {

        public IHttpActionResult Unauthorized(string Message)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, new API
            {
                Meta = new MetaObj
                {
                    Message = Message,
                    Status = ((int)HttpStatusCode.Unauthorized).ToString()
                },
                Response = null,
            }));
        }
        public IHttpActionResult OK(Object obj)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, new API
            {
                Meta = new MetaObj
                {
                    Message = "OK",
                    Status = ((int)HttpStatusCode.OK).ToString()
                },
                Response = new Dictionary<string, object> { { "Response", obj } },
            }));
        }
        public IHttpActionResult InternalServerError(string Message)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, new API
            {
                Meta = new MetaObj
                {
                    Message = Message,
                    Status = ((int)HttpStatusCode.InternalServerError).ToString(),
                },
                Response = null
            }));
        }
        public IHttpActionResult NotFound(string Message)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, new API
            {
                Meta = new MetaObj
                {
                    Message = Message,
                    Status = ((int)HttpStatusCode.NotFound).ToString(),
                },
                Response = null
            }));
        }
        public IHttpActionResult NoContent(string Message)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent, new API
            {
                Meta = new MetaObj
                {
                    Message = Message,
                    Status = ((int)HttpStatusCode.NoContent).ToString(),
                },
                Response = null
            }));
        }
        public IHttpActionResult Conflict(string Message)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, new API
            {
                Meta = new MetaObj
                {
                    Message = Message,
                    Status = ((int)HttpStatusCode.NotFound).ToString(),
                },
                Response = null
            }));
        }



    }
}
