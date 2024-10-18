using UnityEngine;

namespace LazyPanClean {
    public class LPObj : LPSingleton<LPObj> {
        public Transform ObjRoot;//根节点

        public void Init() {
            
        }

        public void Preload() {
            ObjRoot = LPLoader.LoadGo("物体", "Global/Global_ObjRoot", null, true).transform;
        }

        //加载物体
        public LPEntity LoadEntity(string sign) {
            LPEntity instanceLpEntity = new LPEntity();
            instanceLpEntity.Init(sign);
            return instanceLpEntity;
        }

        //销毁实体
        public void UnLoadEntity(LPEntity lpEntity) {
            if (LPEntityRegister.HasEntity(lpEntity)) {
                lpEntity.Clear();
            }
        }
    }
}