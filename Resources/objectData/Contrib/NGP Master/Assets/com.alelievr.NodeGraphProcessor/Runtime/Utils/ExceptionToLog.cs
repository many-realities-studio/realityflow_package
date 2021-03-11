using UnityEngine;
using System;

namespace RealityFlow.Plugin.Contrib
{
    public static class ExceptionToLog
    {
        public static void Call(Action a)
        {
#if UNITY_EDITOR
            try
            {
#endif
                a?.Invoke();
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }
    }
}