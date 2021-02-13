using UnityEngine;

namespace Bob
{
    public class OutputLog
    {
        public static void AddLog(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        public static void AddError(string log)
        {
            UnityEngine.Debug.LogError(log);
        }
    }
}
