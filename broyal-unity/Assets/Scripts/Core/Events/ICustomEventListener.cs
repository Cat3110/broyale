
using System;
using UnityEngine;

namespace Scripts.Core.Events
{
    public interface ICustomEventListener
    {
        void OnEvent( Component sender, string evName, EventArgs args = null );
    }
}