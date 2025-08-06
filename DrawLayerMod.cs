using System;
using System.Collections.Generic;
using System.IO;
using Expansions.Missions.Editor;
using UnityEngine;

namespace com.github.lhervier.ksp {
	
	[KSPAddon(KSPAddon.Startup.PSystemSpawn, false)]
    public class DrawLayerMod : MonoBehaviour {
        
        private static bool DEBUG = false;
        private static readonly string CONFIG_FILE = "draw_layer.cfg";

        private static void LogInternal(string level, string message) {
            Debug.Log($"[DrawLayerMod][{level}] {message}");
        }

        private static void LogInfo(string message) {
            LogInternal("INFO", message);
        }

        private static void LogDebug(string message) {
            if( !DEBUG ) {
                return;
            }
            LogInternal("DEBUG", message);
        }

        private static void LogError(string message) {
            LogInternal("ERROR", message);
        }

        private static void InitDebugMode() {
            try {
                // Get the directory where the mod DLL is located
                string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string modDirectory = Path.GetDirectoryName(dllPath);
                string configPath = Path.Combine(modDirectory, CONFIG_FILE);

                if (File.Exists(configPath)) {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines) {
                        string trimmedLine = line.Trim();
                        if( trimmedLine.StartsWith("#") ) {
                            continue;
                        }
                        if (trimmedLine.StartsWith("debug=")) {
                            string value = trimmedLine.Substring(6).Trim().ToLower();
                            DEBUG = (value == "true" || value == "1" || value == "yes");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                Debug.LogError($"[DrawLayerMod] Error reading config file: {ex.Message}");
            }
        }

        // ================================================================

        protected void Awake() 
        {
            InitDebugMode();
            LogInfo("Awaked");
            DontDestroyOnLoad(this);
        }

        public void Start() {
            LogInfo("Plugin started");
        }

        public void OnDestroy() {
            LogInfo("Plugin stopped");
        }
    }
}
