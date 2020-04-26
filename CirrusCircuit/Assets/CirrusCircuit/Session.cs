using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cirrus.Circuit.Networking;

using Mirror;

namespace Cirrus.Circuit
{
    public class Session : NetworkBehaviour
    {
        protected static Session _instance;

        public static Session Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Session>();
                }

                return _instance;
            }
        }
    }
}
