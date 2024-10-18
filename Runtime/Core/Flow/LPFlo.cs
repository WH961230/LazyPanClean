using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace LazyPanClean {
    public class LPFlo : LPSingleton<LPFlo> {
        Dictionary<Type, LPFlow> flows = new Dictionary<Type, LPFlow>();
        public string CurFlowSign;
        public void Preload() {
            string sceneName = SceneManager.GetActiveScene().name;
            LPSceneConfig lpSceneConfig = LPSceneConfig.Get(sceneName);
            CurFlowSign = string.Concat("Flow_", lpSceneConfig.Flow);
            Type type = Assembly.Load("Assembly-CSharp").GetType(string.Concat("LazyPan.", CurFlowSign));
            LPFlow lpFlow = (LPFlow) Activator.CreateInstance(type);
            flows.Clear();
            flows.Add(type, lpFlow);
            lpFlow.Init(null);
        }

        public bool GetFlow<T>(out T flow) where T : LPFlow {
            if (flows.ContainsKey(typeof(T))) {
                flow = (T)flows[typeof(T)];
                return true;
            }

            flow = default;
            return false;
        }
    }
}