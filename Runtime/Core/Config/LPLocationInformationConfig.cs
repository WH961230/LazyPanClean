using System;
using System.Collections.Generic;

namespace LazyPanClean {
    public class LPLocationInformationConfig {
		public string Sign;
		public string Description;

        private static bool isInit;
        private static string content;
        private static string[] lines;
        private static Dictionary<string, LPLocationInformationConfig> dics = new Dictionary<string, LPLocationInformationConfig>();

        public LPLocationInformationConfig(string line) {
            try {
                string[] values = line.Split(',');
				Sign = values[0];
				Description = values[1];

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Init() {
            if (isInit) {
                return;
            }
            LPReadCSV.Instance.Read("LocationInformationConfig", out content, out lines);
            dics.Clear();
            for (int i = 0; i < lines.Length; i++) {
                if (i > 2) {
                    LPLocationInformationConfig config = new LPLocationInformationConfig(lines[i]);
                    dics.Add(config.Sign, config);
                }
            }

            isInit = true;
        }

        public static LPLocationInformationConfig Get(string sign) {
            if (dics.TryGetValue(sign, out LPLocationInformationConfig config)) {
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