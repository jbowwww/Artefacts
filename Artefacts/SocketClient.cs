using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MongoDB.Bson;
using Serialize.Linq;
using Serialize.Linq.Serializers;
using Serialize.Linq.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using System.Collections;
using System.IO;

namespace Artefacts
{
    public class SocketClient : IClient, IDisposable
    {
		private TcpClient _client;
		private readonly ExpressionSerializer _expressionSerializer = new ExpressionSerializer(new BinarySerializer());
		
		public SocketClient(string hostname, int port)
		{
			_client = new TcpClient(hostname, port);
			if (!_client.Connected)
				throw new ApplicationException("Unable to connect to " + hostname + ":" + port);
		}

		void IDisposable.Dispose()
		{
			// SHould happen automatically when _client goes out of scope??
			//((IDisposable)_client).Dispose();
		}

		public void Serialize<T>(T dto)
		{
			if (typeof(Expression).IsAssignableFrom(typeof(T)))
				_expressionSerializer.Serialize(_client.GetStream(), (Expression)(object)dto);
			MemoryStream ms = new MemoryStream(2048);
			BsonSerializer.Serialize<T>(new BsonBinaryWriter((_client.GetStream())), dto);

		}
		public T Deserialize<T>()
		{
			return BsonSerializer.Deserialize<T>(_client.GetStream());
		}

		
	}
}
