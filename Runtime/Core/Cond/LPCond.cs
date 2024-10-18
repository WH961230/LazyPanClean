using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace LazyPanClean {
    public class LPCond : LPSingleton<LPCond> {
        #region 全局通用实体

        public LPEntity GetCameraEntity() {
            if (LPEntityRegister.TryGetRandEntityByType("Camera", out LPEntity entity)) {
                return entity;
            } else {
                return null;
            }
        }

        public LPEntity GetPlayerEntity() {
            if (LPEntityRegister.TryGetRandEntityByType("Player", out LPEntity entity)) {
                return entity;
            } else {
                return null;
            }
        }

        public LPEntity GetGlobalEntity() {
            if (LPEntityRegister.TryGetRandEntityByType("Global", out LPEntity entity)) {
                return entity;
            } else {
                return null;
            }
        }

        public bool GetEntityByID(int id, out LPEntity lpEntity) {
            return LPEntityRegister.TryGetEntityByID(id, out lpEntity) ;
        }

        #endregion

        #region 查标签组件
        public T Get<T>(LPEntity lpEntity, string label) where T : Object {
            if (lpEntity == null) { return default; }
#if UNITY_EDITOR
            if (lpEntity.lpComp == null) {
                LPLogUtil.LogErrorFormat("请检查 entity:{0} 没有挂 Comp 组件!", lpEntity.LpObjConfig.Sign);
                EditorApplication.isPaused = true;
            }
#endif
            return lpEntity.lpComp.Get<T>(label);
        }
        public T Get<T>(LPComp lpComp, string label) where T : Object {
            if (lpComp == null) { return default; }
            return lpComp.Get<T>(label);
        }

        #endregion

        #region 查标签数据

        public bool GetData<T>(LPEntity lpEntity, string label, out T t) {
            if (lpEntity == null) {
                t = default;
                return false;
            }
#if UNITY_EDITOR
            if (lpEntity.lpData == null) {
                LPLogUtil.LogErrorFormat("请检查 entity:{0} 没有挂 Data 组件!", lpEntity.LpObjConfig.Sign);
                EditorApplication.isPaused = true;
            }
#endif
            return lpEntity.lpData.Get(label, out t);
        }

        public bool GetData<T1, T2>(LPEntity lpEntity, string label, out T2 t) where T1 : LPData {
            if (lpEntity == null) {
                t = default;
                return false;
            }
#if UNITY_EDITOR
            if (lpEntity.lpData == null) {
                LPLogUtil.LogErrorFormat("请检查 entity:{0} 没有挂 Data 组件!", lpEntity.LpObjConfig.Sign);
                EditorApplication.isPaused = true;
            }
#endif
            T1 data = lpEntity.lpData as T1;
            return data.Get(label, out t);
        }

        #endregion

        #region 查标签事件

        public UnityEvent GetEvent(LPEntity lpEntity, string label) {
            return lpEntity.lpComp.GetEvent(label);
        }
        public UnityEvent GetEvent(LPComp lpComp, string label) {
            return lpComp.GetEvent(label);
        }

        #endregion
    }
}