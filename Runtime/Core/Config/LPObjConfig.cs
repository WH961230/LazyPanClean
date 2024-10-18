using System;
using System.Collections.Generic;

namespace LazyPanClean {
    public class LPObjConfig {
		public string Sign;
		public string Flow;
		public string Type;
		public string Name;
		public string SetUpLocationInformationSign;
		public string SetUpBehaviourName;

        private static bool isInit;
        private static string content;
        private static string[] lines;
        private static Dictionary<string, LPObjConfig> dics = new Dictionary<string, LPObjConfig>();

        public LPObjConfig(string line) {
            try {
                string[] values = line.Split(',');
				Sign = values[0];
				Flow = values[1];
				Type = values[2];
				Name = values[3];
				SetUpLocationInformationSign = values[4];
				SetUpBehaviourName = values[5];

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Init() {
            if (isInit) {
                return;
            }
            LPReadCSV.Instance.Read("LPObjConfig", out content, out lines);
            dics.Clear();
            for (int i = 0; i < lines.Length; i++) {
                if (i > 2) {
                    LPObjConfig config = new LPObjConfig(lines[i]);
                    dics.Add(string.Concat(config.Flow, "|", config.Sign), config);
                }
            }

            isInit = true;
        }

        public static LPObjConfig Get(string sign) {
            string[] flowBase = LPFlo.Instance.CurFlowSign.Split("_");
            string tmpSign = string.Concat(flowBase[1], "|", sign);
            if (dics.TryGetValue(tmpSign, out LPObjConfig config)) {
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