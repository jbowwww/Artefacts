using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serialize.Linq;

namespace Artefacts
{
    public interface IClient
    {
	//BinaryFormatter Formatter {  get;}
		void Serialize<T>(T dto);
		T Deserialize<T>();
	}

    interface IClient<T> : IRepository<T>, IClient
    {

    }
}
