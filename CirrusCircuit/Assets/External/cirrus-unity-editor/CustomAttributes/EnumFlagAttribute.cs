using UnityEngine;
using System;

namespace Cirrus.UnityEditorExt
{

    /// <summary>
    /// Display multi-select popup for Flags enum correctly.
    /// </summary>
	[AttributeUsage(AttributeTargets.Field)]
    public class EnumFlagAttribute : PropertyAttribute
    {
    }

}