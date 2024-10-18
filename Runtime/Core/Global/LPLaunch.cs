using UnityEditor;
using UnityEngine;

namespace LazyPanClean {
    public class LPLaunch : MonoBehaviour {
        public static LPLaunch instance;
        public bool OpenConsole;
        public string StageLoadScene;
        [HideInInspector] public Transform UIDontDestroyRoot;
        private void Awake() {
            if (instance == null) {
                instance = this;
                LPConfig.Instance.Init(); 
                LPObj.Instance.Init();

                UIDontDestroyRoot = LPLoader.LoadGo("加载画布", "Global/Global_Loading_UIRoot", null, true).transform;
                UIDontDestroyRoot.gameObject.AddComponent<LPStage>();
                UIDontDestroyRoot.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                DontDestroyOnLoad(UIDontDestroyRoot.gameObject);

                DontDestroyOnLoad(gameObject);

                LPConsoleEx.Instance.Init(OpenConsole);
                    
                StageLoad(StageLoadScene);
            }
        }

        //加载阶段
        public void StageLoad(string sceneName) {
            LPStage lpStage = UIDontDestroyRoot.gameObject.GetComponent<LPStage>();
            lpStage.Load(LPSceneConfig.Get(sceneName).DelayTime, sceneName);
        }

        //结束游戏
        public void QuitGame() {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}