using Artefacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.IO;
using MongoDB.Bson.Serialization;
using ServiceHost;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;

namespace SerializationTests
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
			Console.WriteLine("Console size: " + Console.WindowWidth + "x" + Console.WindowHeight);
			//Console.
			//Console.SetWindowSize(Console.WindowWidth + 8, Console.WindowHeight/* + 8*/);
			//Console.WriteLine("Console size: " + Console.WindowWidth + "x" + Console.WindowHeight);

			Console.WriteLine();
			bool serverExit = false;

			try
			{
				Task serverTask = Task.Factory.StartNew(() =>
				{
					try
					{
						object buf;
						//Service service = new Service("http://localhost:80/Artefacts/");
						TcpListener listener = new TcpListener(IPAddress.Any, 99);
						listener.Start();
						while (!serverExit)
						{
							while (!listener.Pending())
							{
								System.Threading.Thread.Sleep(100);
							}

							TcpClient server = listener.AcceptTcpClient();
							object result = BsonSerializer.Deserialize(new BsonBinaryReader(server.GetStream()), typeof(object));
							//new ByteBufferStream(ByteBufferFactory.Create(
							//new SingleChunkBuffer(new ByteArrayChunk(2048), 2048), 2048)

							Artefact artefact = result as Artefact;
							Console.WriteLine("\nSERVER: {0}\n\tartefact: {1}\n\n", result, artefact);

						}
					}
					catch (Exception ex)
					{
						Console/*.Error*/.WriteLine(nameof(Service) + ".ServiceTask: \n" + ex.ToString() + "\n");
						//Console./*.Error*/.NewLine + + Console/*.Error*/.NewLine);
					}
				});

				//Service service = new Service("http://localhost:80/Artefacts/");
				//BsonSerializer.RegisterSerializer<Artefact>(ArtefactSerializer.Instance);

				Artefact dsArtefact;
				dynamic a1 = new Artefact();
				a1.prop1 = "one";
				a1.prop2 = 2;
				a1.date = DateTime.Now;

				SocketClient client = new SocketClient("localhost", 99);
				IRepository repo = new RepositoryProxy(client);
				repo.Save(a1);
			}
			catch (Exception ex)
			{
				Console/*.Error*/.WriteLine(nameof(Service) + ": \n" + ex.ToString() + "\n");
				//Console/*.Error*/.NewLine+ Console/*.Error*/.NewLine);
			}
			finally
			{
				Console.ReadKey();
				serverExit = true;
				//await se
			}
		}

		void oldMain()
		{
				//    StringWriter sw = new StringWriter(new StringBuilder());
				//    BsonSerializer.Serialize<Artefact>(new JsonWriter(sw), a1);     // Console.Out), a1);
				//    string json = sw.ToString();
				//    Console.WriteLine(nameof(JsonWriter) + ": " + sw.GetStringBuilder().Length + " characters:" + Console.Out.NewLine + json);

			//    Console.WriteLine();
			//    dsArtefact = BsonSerializer.Deserialize<Artefact>(json);
			//    Console.WriteLine("Deserialized artefact: " + Console.Out.NewLine + dsArtefact.ToString() + Console.Out.NewLine + dsArtefact.ToJson());

			//    Console.WriteLine();
			//    BsonStreamAdapter mem = new BsonStreamAdapter(new MemoryStream());
			//    BsonSerializer.Serialize<Artefact>(new BsonBinaryWriter(mem), a1);//.OpenStandardOutput()), a1);
			//    byte[] bytes = ((MemoryStream)mem.BaseStream).ToBytes();
			//    Console.WriteLine(nameof(BsonBinaryWriter) + ": " + mem.Length + " bytes:" + Console.Out.NewLine + bytes.ToHex());  // mem.);//.ToHex());    //Convert.ToBase64String(

			//    Console.WriteLine();
			//    dsArtefact = BsonSerializer.Deserialize<Artefact>(bytes); //.ToArray()));
			//    Console.WriteLine("Deserialized artefact: " + Console.Out.NewLine + dsArtefact.ToString() + Console.Out.NewLine + dsArtefact.ToJson());

			//    Console.ReadKey();

			//    using (var client = new WebClient())

			//    //using (var client = new HttpClient())
			//    {
			//        string url = @"http://localhost/Artefacts/";
			//        string response = client.UploadString(url, json);
			//        Console.WriteLine(Console.Out.NewLine + url + ": " + response + Console.Out.NewLine);

			//        //            client.BaseAddress = new Uri("http://localhost/");
			//        //            client.MaxResponseContentBufferSize = 1024 * 1024 * 16;
			//        //            client.Timeout = TimeSpan.FromSeconds(15);
			//        //            client.PostAsync("Artefacts /", new StringContent(json)).ContinueWith(
			//        //                (Task<HttpResponseMessage> taskResponse) =>
			//        //                {
			//        //                    try
			//        //                    {
			//        //                        Console.WriteLine(Console.Out.NewLine + taskResponse.Result.RequestMessage.RequestUri + ": " + taskResponse.Result.Content.ToString());
			//        //                        //(Task<string> taskContent) =>
			//        //                        //{
			//        //                        //    Console.WriteLine(Console.Out.NewLine + taskResponse.Result.RequestMessage.RequestUri + ": " + taskContent.Result);

			//        //                        //});
			//        //                        //});
			//        //                    }
			//        //                    catch (Exception ex)
			//        //                    {
			//        //                        Console.Error.WriteLine(nameof(Service) + ".ServiceTask: " + Console.Error.NewLine + ex.ToString() + Console.Error.NewLine);
			//        //                    }
			//        //});
			//    }
			//}
		}
	}

    static class Extensions
    {
        public static char NibbleToHex(char nibble)
        {
            nibble += '0';
            if (nibble > '9')
                nibble += (char)('a' - '9' - 1);
            return nibble;
        }
        public static string ToHex(this byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 4 + 2);
            int i = 0;
            foreach (byte b in data)
            {
                if (i++ % 4 == 0 && i > 1)
                    sb.Append(' ');
                //sb.Append(NibbleToHex((char)((b & 0xF0) >> 4)) + NibbleToHex((char)(b & 0x0F)));
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        public static byte[] ToBytes(this MemoryStream ms)
        {
            return ms.GetBuffer().Take((int)ms.Length).ToArray();
        }
        public static string ToHex(this MemoryStream ms)
        {
            return ms.ToBytes().ToHex();
        }
    }
}
