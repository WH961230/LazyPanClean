using UnityEngine;
using UnityEngine.Events;

namespace LazyPanClean {
    public class LPGame : MonoBehaviour {
        public static LPGame instance;
        public UnityEvent OnUpdateEvent = new UnityEvent();
        public UnityEvent OnFixedUpdateEvent = new UnityEvent();
        public UnityEvent OnLateUpdateEvent = new UnityEvent();

        public void Init() {
            instance = this;
            LPObj.Instance.Preload();
            LPUI.Instance.Preload();
            LPFlo.Instance.Preload();
        }

        private void Update() { OnUpdateEvent.Invoke(); }

        private void FixedUpdate() { OnFixedUpdateEvent.Invoke(); }

        private void LateUpdate() { OnLateUpdateEvent.Invoke(); }
    }
}