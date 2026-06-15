using UnityEngine;

namespace com.github.lhervier.ksp {
    
    public static class Logger {
        private static bool debugMode = false;
        
        public static void SetDebugMode(bool enabled) {
            debugMode = enabled;
        }
        
        private static void LogInternal(string level, string message) {
            Debug.Log($"[DrawLayerMod][{level}] {message}");
        }
        
        public static void LogInfo(string message) {
            LogInternal("INFO ", message);
        }
        
        public static void LogDebug(string message) {
            if (!debugMode) {
                return;
            }
            LogInternal("DEBUG", message);
        }
        
        public static void LogError(string message) {
            LogInternal("ERROR", message);
        }
        
        public static void LogWarning(string message) {
            LogInternal("WARN ", message);
        }
    }
} 