using System;
using System.Collections.Generic;

namespace LazyPanClean {
    public class LPSceneConfig {
		public string Sign;
		public string Description;
		public string DirPath;
		public string Flow;
		public float DelayTime;

        private static bool isInit;
        private static string content;
        private static string[] lines;
        private static Dictionary<string, LPSceneConfig> dics = new Dictionary<string, LPSceneConfig>();

        public LPSceneConfig(string line) {
            try {
                string[] values = line.Split(',');
				Sign = values[0];
				Description = values[1];
				DirPath = values[2];
				Flow = values[3];
				DelayTime = float.Parse(values[4]);

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Init() {
            if (isInit) {
                return;
            }
            LPReadCSV.Instance.Read("LPSceneConfig", out content, out lines);
            dics.Clear();
            for (int i = 0; i < lines.Length; i++) {
                if (i > 2) {
                    LPSceneConfig config = new LPSceneConfig(lines[i]);
                    dics.Add(config.Sign, config);
                }
            }

            isInit = true;
        }

        public static LPSceneConfig Get(string sign) {
            if (dics.TryGetValue(sign, out LPSceneConfig config)) {
                return config;
            }

            return null;
        }

        public static List<string> GetKeys() {
              if (!isInit) {
                   Init();
              }
              var keys = new List<string>();
              keys.AddRange(dics.Keys);
              return keys;
        }
    }
}