using System.Collections.Generic;
using UnityEngine;

namespace LazyPanClean {
    public class LPEntityRegister {
        public static int EntityID;//为实体分配的ID
        public static Dictionary<int, LazyPanClean.LPEntity> EntityDic = new Dictionary<int, LazyPanClean.LPEntity>();

        #region 增删改查

        //增
        public static bool AddEntity(int id, LazyPanClean.LPEntity lpEntity) {
            if (EntityDic.TryAdd(id, lpEntity)) {
                LPConsoleEx.Instance.ContentSave("entity", $"ID:{id} 注册实体:{lpEntity.LpObjConfig.Name}");
                return true;
            }

            return false;
        }

        //删
        public static void RemoveEntity(int id) {
            if (EntityDic.ContainsKey(id)) {
                LPConsoleEx.Instance.ContentSave("entity", $"ID:{id} 移除实体:{EntityDic[id].LpObjConfig.Name}");
                EntityDic.Remove(id);
            }
        }
        
        //查ID
        public static bool TryGetEntityByID(int id, out LazyPanClean.LPEntity lpEntity) {
            if (EntityDic.TryGetValue(id, out lpEntity)) {
                return true;
            }

            return false;
        }
        
        //查标识
        public static bool TryGetEntityBySign(string objSign, out LazyPanClean.LPEntity lpEntity) {
            foreach (LazyPanClean.LPEntity tmpEntity in EntityDic.Values) {
                if (objSign == tmpEntity.LpObjConfig.Sign) {
                    lpEntity = tmpEntity;
                    return true;
                }
            }

            lpEntity = default;
            return false;
        }
        
        //查类型
        public static bool TryGetEntitiesByType(string type, out List<LazyPanClean.LPEntity> entity) {
            entity = new List<LazyPanClean.LPEntity>();
            foreach (LazyPanClean.LPEntity tmpEntity in EntityDic.Values) {
                if (type == tmpEntity.Type) {
                    entity.Add(tmpEntity);
                }
            }

            if (entity.Count > 0) {
                return true;
            }

            entity = default;
            return false;
        }

        //查组件
        public static bool TryGetEntityByComp(LPComp lpComp, out LazyPanClean.LPEntity lpEntity) {
            foreach (LazyPanClean.LPEntity tempEntity in EntityDic.Values) {
                if (tempEntity.lpComp == lpComp) {
                    lpEntity = tempEntity;
                    return true;
                }
            }

            lpEntity = null;
            return false;
        }
        
        //查BodyInstanceID
        public static bool TryGetEntityByBodyPrefabID(int id, out LazyPanClean.LPEntity lpEntity) {
            foreach (LazyPanClean.LPEntity tempEntity in EntityDic.Values) {
                Transform bodyTran = LPCond.Instance.Get<Transform>(tempEntity, LPLabel.BODY);
                if (bodyTran != null && bodyTran.gameObject != null && bodyTran.gameObject.GetInstanceID() == id) {
                    lpEntity = tempEntity;
                    return true;
                }
            }

            lpEntity = null;
            return false;
        }
        
        //查某类型的随机一个实体
        public static bool TryGetRandEntityByType(string type, out LazyPanClean.LPEntity lpEntity) {
            bool findTypeEntities = TryGetEntitiesByType(type, out List<LPEntity> entities);
            if (!findTypeEntities) {
                lpEntity = default;
                return false;
            }

            lpEntity = GetRandEntity(entities);
            return true;
        }
        
        //查距离内的所有指定类型的实体
        public static bool TryGetEntitiesWithinDistance(string type, Vector3 fromPoint, float distance, out List<LazyPanClean.LPEntity> entity) {
            entity = new List<LazyPanClean.LPEntity>();
            if (TryGetEntitiesByType(type, out List<LazyPanClean.LPEntity> tmpEntities)) {
                foreach (LazyPanClean.LPEntity tmpEntity in tmpEntities) {
                    float disSqrt = distance * distance;
                    Transform body = LPCond.Instance.Get<Transform>(tmpEntity, LPLabel.BODY);
                    if (body != null) {
                        if ((body.position - fromPoint).sqrMagnitude < disSqrt) {
                            entity.Add(tmpEntity);
                        }
                    }
                }
            }

            if (entity.Count > 0) {
                return true;
            }

            return false;
        }

        //查实体列表内获取随机实体
        private static LazyPanClean.LPEntity GetRandEntity(List<LazyPanClean.LPEntity> entities) {
            return entities[Random.Range(0, entities.Count)];
        }

        //是否有该实体
        public static bool HasEntity(LazyPanClean.LPEntity lpEntity) {
            foreach (LazyPanClean.LPEntity tempEntity in EntityDic.Values) {
                if (tempEntity == lpEntity) {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}