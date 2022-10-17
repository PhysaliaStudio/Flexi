using System;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Warn(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        public static void Fatal(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
