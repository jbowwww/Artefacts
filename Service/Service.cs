using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Artefacts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using System.IO;

namespace ServiceHost
{
    public class Service
    {
        private HttpListener _listener = new HttpListener();

        public string UriBase { get; private set; }

        public Service(string uriBase)
        {
            UriBase = uriBase;
            _listener.Prefixes.Add(UriBase);
            _listener.Start();
            Console.WriteLine(nameof(Service) + ": " + UriBase + ": listening");
            _listener.GetContextAsync().ContinueWith(
                (Task<HttpListenerContext> contextTask) =>
                {
                    try
                    {
                        HttpListenerContext context = contextTask.Result;
                        HandleRequest(context.Request, context.Response);

                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(Console.Error.NewLine + nameof(Service) + ": " + ex.ToString() + Console.Error.NewLine);
                    }
                });

        }

        public virtual void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.Segments.Length > 1)
            {
                if (string.Compare(request.Url.Segments[1], 0, "artefacts", 0, 9, true) == 0)
                {
                    Artefact artefact = BsonSerializer.Deserialize<Artefact>(new JsonReader(new StreamReader(new BsonStreamAdapter(request.InputStream))));
                    //(BsonDeserializationContext.Builder builder) =>
                    //{
                    //    builder.DynamicDocumentSerializer = ArtefactSerializer.Instance;
                    //});
                    Console.WriteLine(nameof(Service) + ": " + UriBase
                         /*.TrimEnd('/') + '/' + string.Join("/", request.Url.Segments) + '/'*/
                         + Console.Out.NewLine + artefact.ToJson());//.ToString());
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        using (var bsonwriter = new JsonWriter(writer))
                        {
                            BsonSerializer.Serialize(bsonwriter, typeof(Artefact), artefact);
                        }
                    }
                    //response.StatusCode = 200;
                    //response.OutputStream.Flush();
                    //response.OutputStream.Close();
                }
            }
        }
    }
}
