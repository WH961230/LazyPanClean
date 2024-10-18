using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LazyPanClean {
    public class LPStage : MonoBehaviour {
        [FormerlySerializedAs("loadingUIComp")] public LPComp loadingUILpComp; //加载界面
        private StageWork work; //当前作业
        private Queue<StageWork> works = new Queue<StageWork>(); //所有作业

        public void Load(float delayTime, string sceneName) {
            works.Clear();
            if (delayTime != 0) {
                works.Enqueue(new LoadLoadingUI(new LoadLoadingUIParameters() {
                    uiRoot = transform
                }, this));
            }

            works.Enqueue(new LoadScene(new LoadSceneParameters() {
                sceneName = sceneName
            }));
            works.Enqueue(new LoadGlobal(new LoadGlobalParameters() {
                sceneName = sceneName, delayTime = delayTime
            }, this));
        }

        public void Update() {
            //获取当前作业
            if (work == null && works.Count > 0) {
                work = works.Dequeue();
                work?.Start();
            }

            work?.Update();
            if (work != null) {
                if (work.IsDone) {
                    work.Complete();
                    work = null;
                }
            }
        }
    }

    public class LoadGlobalParameters : StageParameters {
        public string sceneName;
        public float delayTime;
        public float progress;
    }

    public class LoadGlobal : StageWork {
        private LoadGlobalParameters Parameters;
        private LPStage _lpStage;
        private LPGame _lpGame;
        private LPClock _lpClock;
        private float delayDeployTime;

        public LoadGlobal(StageParameters Parameters, LPStage lpStage) : base(Parameters) {
            this.Parameters = (LoadGlobalParameters)Parameters;
            this._lpStage = lpStage;
        }

        public override void Start() {
            Parameters.progress = 0;
            delayDeployTime = 0;
        }

        public override void Update() {
            if (SceneManager.GetActiveScene().name == Parameters.sceneName && _lpGame == null) {
                _lpGame = LPLoader.LoadGo("全局", "Global/Global", null, true).GetComponent<LPGame>();
                _lpGame.Init();
                _lpClock = LPClockUtil.Instance.AlarmAfter(Parameters.delayTime, () => { Parameters.progress = 1f; });
            }

            if (_lpGame != null) {
                if (Parameters.progress == 1) {
                    IsDone = true;
                    LPClockUtil.Instance.Stop(_lpClock);
                }

                LoadingUI(_lpStage.loadingUILpComp, "", Parameters.progress);
            }
        }

        private void LoadingUI(LPComp loadingUILpComp, string description, float progress) {
            if (loadingUILpComp) {
                Slider loadingSlider = LPCond.Instance.Get<Slider>(loadingUILpComp, "LoadingSlider");
                TextMeshProUGUI loadingText = LPCond.Instance.Get<TextMeshProUGUI>(loadingUILpComp, "LoadingText");
                if (delayDeployTime < Parameters.delayTime) {
                    delayDeployTime += 1 * Time.deltaTime / Parameters.delayTime;
                    progress = delayDeployTime;
                }

                loadingSlider.value = progress;
                loadingText.text = string.Concat(description, " ", Mathf.Round(loadingSlider.value * 100f), "%");
            }
        }

        public override void Complete() {
            if (_lpStage.loadingUILpComp != null) {
                Object.DestroyImmediate(_lpStage.loadingUILpComp.gameObject);
            }
        }
    }

    public class LoadSceneParameters : StageParameters {
        public string sceneName;
    }

    public class LoadScene : StageWork {
        LoadSceneParameters Parameters;

        public LoadScene(StageParameters Parameters) : base(Parameters) {
            this.Parameters = (LoadSceneParameters)Parameters;
        }

        public override void Start() {
            AsyncOperation operation = LPLoader.LoadSceneAsync(Parameters.sceneName);
            operation.allowSceneActivation = false;
            while (!operation.isDone) {
                if (operation.progress >= 0.9f) {
                    operation.allowSceneActivation = true;
                    IsDone = true;
                    break;
                }
            }
        }

        public override void Update() {
        }

        public override void Complete() {
        }
    }

    public class LoadLoadingUIParameters : StageParameters {
        public Transform uiRoot;
    }

    public class LoadLoadingUI : StageWork {
        LoadLoadingUIParameters Parameters;
        LPStage _lpStage;

        public LoadLoadingUI(StageParameters Parameters, LPStage lpStage) : base(Parameters) {
            this.Parameters = (LoadLoadingUIParameters)Parameters;
            this._lpStage = lpStage;
        }

        public override void Start() {
            _lpStage.loadingUILpComp = LPLoader.LoadComp("加载界面", "UI/UI_Loading", Parameters.uiRoot, true);
        }

        public override void Update() {
            if (_lpStage.loadingUILpComp != null) {
                _lpStage.loadingUILpComp.gameObject.SetActive(true);
                IsDone = true;
            }
        }

        public override void Complete() {
        }
    }

    public class StageParameters {
        public string Description;
    }

    public abstract class StageWork {
        public bool IsDone;
        public StageParameters Parameters;

        public StageWork(StageParameters Parameters) {
            this.Parameters = Parameters;
        }

        public abstract void Start();
        public abstract void Update();
        public abstract void Complete();
    }
}