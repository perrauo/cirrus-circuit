using System;
using System.Collections.Generic;
//using Cirrus.Editor.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Cirrus.UnityEditorExt
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HideTypePicker : Attribute
    {

        public HideTypePicker()
        {
        }
    }
}
