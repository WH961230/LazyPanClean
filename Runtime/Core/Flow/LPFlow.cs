using System;
using System.Collections.Generic;

namespace LazyPanClean {
    public abstract class LPFlow : LPIFlow {
        public LPFlow BaseLpFlow;
        public LPFlow CurrentLpFlow;
        public Dictionary<Type, LPFlow> FlowDic = new Dictionary<Type, LPFlow>();

        //进入流程
        public virtual void Init(LPFlow baseLpFlow) {
            BaseLpFlow = baseLpFlow;
        }

        //结束流程
        public virtual void Clear() {
            if (CurrentLpFlow != null) {
                CurrentLpFlow.Clear();
                CurrentLpFlow = null;
            }

            foreach (LPFlow tempFlow in FlowDic.Values) {
                tempFlow.Clear();
            }
            FlowDic.Clear();
        }

        //切换流程
        public virtual void ChangeFlow<T>() {
            if (CurrentLpFlow != null) {
                CurrentLpFlow.Clear();
            }

            if (FlowDic.ContainsKey(typeof(T))) {
                CurrentLpFlow = FlowDic[typeof(T)];
                LPLogUtil.LogFormat("进入流程: {0}", CurrentLpFlow.GetType().Name);
                CurrentLpFlow.Init(this);
            }

            if (FlowDic.Count == 0) {
                LPLogUtil.LogError(string.Concat(typeof(T), "字典为空!"));
            }
        }

        public virtual T GetFlow<T>() where T : LPFlow {
            if (FlowDic.ContainsKey(typeof(T))) {
                return FlowDic[typeof(T)] as T;
            }

            return null;
        }

        public virtual void EndFlow() {
            Clear();
        }
    }
}