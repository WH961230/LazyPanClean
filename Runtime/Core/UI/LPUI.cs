using System.Collections.Generic;
using UnityEngine;

namespace LazyPanClean {
    public class LPUI : LPSingletonMonoBehaviour<LPUI> {
        private LPComp _uiLpComp;
        private Dictionary<string, LPComp> uICompAlwaysDics = new Dictionary<string, LPComp>();
        private Dictionary<string, LPComp> uICompExchangeDics = new Dictionary<string, LPComp>();
        private Dictionary<string, LPComp> uICompDics = new Dictionary<string, LPComp>();
        private Transform UIRoot;//根节点

        public void Preload() {
            UIRoot = LPLoader.LoadGo("画布", "Global/Global_UIRoot", null, true).transform;
            List<string> keys = LPUIConfig.GetKeys();
            int length = keys.Count;
            uICompDics.Clear();
            uICompExchangeDics.Clear();
            uICompAlwaysDics.Clear();
            for (int i = 0; i < length; i++) {
                string key = keys[i];
                GameObject uiGo = LPLoader.LoadGo(LPUIConfig.Get(key).Description, string.Concat("UI/", key), UIRoot, false);
                switch (LPUIConfig.Get(key).Type) {
                    case 0:
                        uICompExchangeDics.Add(key, uiGo.GetComponent<LPComp>());
                        break;
                    case 1:
                        uICompAlwaysDics.Add(key, uiGo.GetComponent<LPComp>());
                        break;
                }
            
                uICompDics.Add(key, uiGo.GetComponent<LPComp>());
            }
        }

        public LPComp Open(string name) {
            if (uICompExchangeDics.TryGetValue(name, out LPComp uiExchangeComp)) {
                if (_uiLpComp != null) {
                    _uiLpComp.gameObject.SetActive(false);
                }

                _uiLpComp = uiExchangeComp;
                _uiLpComp.gameObject.SetActive(true);
                return _uiLpComp;
            }

            if (uICompAlwaysDics.TryGetValue(name, out LPComp uiAlwaysComp)) {
                uiAlwaysComp.gameObject.SetActive(true);
                return uiAlwaysComp;
            }

            return null;
        }

        public LPComp Get(string name) {
            if (uICompDics.TryGetValue(name, out LPComp comp)) {
                return comp;
            }

            return null;
        }

        public string GetExchangeUIName() {
            if (_uiLpComp != null) {
                return _uiLpComp.gameObject.name;
            }

            return null;
        }

        public bool IsAlwaysUIName(string name) {
            return uICompAlwaysDics.TryGetValue(name, out LPComp comp);
        }

        public bool IsExchangeUI() {
            return _uiLpComp != null;
        }

        public void Close() {
            Close(_uiLpComp);
            _uiLpComp = null;
        }

        public void Close(string name) {
            if (uICompAlwaysDics.TryGetValue(name, out LPComp uiAlwaysComp)) {
                Close(uiAlwaysComp);
            }
        }

        private void Close(LPComp lpComp) {
            if (lpComp != null) {
                lpComp.gameObject.SetActive(false);
            }
        }
    }
}