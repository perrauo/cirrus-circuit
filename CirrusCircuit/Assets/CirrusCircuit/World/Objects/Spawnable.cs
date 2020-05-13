using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.Utils;
using Mirror;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cirrus.MirrorExt;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.World.Objects
{
    public class Spawnable : MonoBehaviour
    {
        public int _id = -1;

        public int Id
        {

            get
            {
                if (_id < 0) _id = ObjectLibrary.Instance.Objects.IndexOf(this);

                return _id;
            }
        }


        public void OnValidate()
        {
            if (_id < 0) _id = Id;
        }

    }
}
