﻿using System;
using System.Collections.Generic;

namespace LazyPanClean {
    public class LPUIConfig {
		public string Sign;
		public string Description;
		public int Type;
		public int RenderQueue;

        private static bool isInit;
        private static string content;
        private static string[] lines;
        private static Dictionary<string, LPUIConfig> dics = new Dictionary<string, LPUIConfig>();

        public LPUIConfig(string line) {
            try {
                string[] values = line.Split(',');
				Sign = values[0];
				Description = values[1];
				Type = int.Parse(values[2]);
				RenderQueue = int.Parse(values[3]);

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Init() {
            if (isInit) {
                return;
            }
            LPReadCSV.Instance.Read("LPUIConfig", out content, out lines);
            dics.Clear();
            for (int i = 0; i < lines.Length; i++) {
                if (i > 2) {
                    LPUIConfig config = new LPUIConfig(lines[i]);
                    dics.Add(config.Sign, config);
                }
            }

            isInit = true;
        }

        public static LPUIConfig Get(string sign) {
            if (dics.TryGetValue(sign, out LPUIConfig config)) {
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