﻿namespace LazyPanClean {
    public static class LPLogUtil {
        public static void Log(object message) {
            UnityEngine.Debug.Log(message);
        }
        
        public static void LogError(object message) {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogFormat(string message, params object[] param) {
            UnityEngine.Debug.LogFormat(message, param);
        }
        
        public static void LogErrorFormat(string message, params object[] param) {
            UnityEngine.Debug.LogErrorFormat(message, param);
        }
    }
}
