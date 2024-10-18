using System;
using System.Collections.Generic;
using System.Reflection;

namespace LazyPanClean {
    public class LPBehaviourRegister {
        private static Dictionary<int, List<LPBehaviour>> BehaviourDic = new Dictionary<int, List<LPBehaviour>>();

        #region 增

        //增加注册行为
        public static bool RegisterBehaviour(int id, string name, out LPBehaviour outLpBehaviour) {
            //是否有实体
            if (LPEntityRegister.TryGetEntityByID(id, out LPEntity entity)) {
                if (BehaviourDic.TryGetValue(id, out List<LPBehaviour> behaviours)) {
                    
                    //判断实体已有当前行为
                    foreach (LPBehaviour tempBehaviour in behaviours) {
                        LPBehaviourConfig config = LPBehaviourConfig.Get(tempBehaviour.BehaviourSign);
                        if (config.Name == name) {
                            outLpBehaviour = default;
                            return false;
                        }
                    }

                    string sign = "";
                    foreach (var tmpKey in LPBehaviourConfig.GetKeys()) {
                        LPBehaviourConfig config = LPBehaviourConfig.Get(tmpKey);
                        if (config.Name == name) {
                            sign = config.Sign;
                            break;
                        }
                    }

                    //创建行为实体
                    try {
                        Type type = Assembly.Load("Assembly-CSharp").GetType(string.Concat("LazyPan.", sign));
                        LPBehaviour lpBehaviour = (LPBehaviour) Activator.CreateInstance(type, entity, sign);
                        outLpBehaviour = lpBehaviour;
                        behaviours.Add(lpBehaviour);
                    } catch (Exception e) {
                        LPLogUtil.LogError(name);
                        throw;
                    }

                    return true;
                } else {
                    //创建行为实体
                    try {
                        string sign = "";
                        List<LPBehaviour> instanceBehaviours = new List<LPBehaviour>();
                        foreach (var tmpKey in LPBehaviourConfig.GetKeys()) {
                            LPBehaviourConfig config = LPBehaviourConfig.Get(tmpKey);
                            if (config.Name == name) {
                                sign = config.Sign;
                                break;
                            }
                        }
                        Type type = Assembly.Load("Assembly-CSharp").GetType(string.Concat("LazyPan.", sign));
                        LPBehaviour lpBehaviour = (LPBehaviour) Activator.CreateInstance(type, entity, sign);
                        outLpBehaviour = lpBehaviour;
                        instanceBehaviours.Add(lpBehaviour);
                        BehaviourDic.TryAdd(id, instanceBehaviours);
                    } catch (Exception e) {
                        LPLogUtil.LogError(name);
                        throw;
                    }
                    
                    return true;
                }
            }

            outLpBehaviour = default;
            return false;
        }

        //删除注册的行为
        public static bool UnRegisterBehaviour(int id, string sign) {
            int index = GetBehaviourIndex(id, sign);
            //是否有实体
            if (index != -1) {
                if (BehaviourDic.TryGetValue(id, out List<LPBehaviour> behaviours)) {
                    behaviours[index].Clear();
                    behaviours.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }
        
        //删除注册的行为
        public static bool UnRegisterAllBehaviour(int id) {
            if (BehaviourDic.TryGetValue(id, out List<LPBehaviour> behaviours)) {
                if (behaviours != null && behaviours.Count > 0) {
                    int index = behaviours.Count - 1;
                    for (int i = index; i >= 0; i--) {
                        UnRegisterBehaviour(id, behaviours[i].BehaviourSign);
                    }
                }
                return true;
            }

            return false;
        }

        private static int GetBehaviourIndex(int id, string sign) {
            int index = -1;
            //是否有实体
            if (BehaviourDic.TryGetValue(id, out List<LPBehaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    if (behaviours[i].BehaviourSign == sign) {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        public static bool GetBehaviour<T>(int id, out T t) where T : LPBehaviour {
            //是否有实体
            if (BehaviourDic.TryGetValue(id, out List<LPBehaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    LPBehaviour lpBehaviour = behaviours[i];
                    if (lpBehaviour.GetType() == typeof(T)) {
                        t = (T)lpBehaviour;
                        return true;
                    }
                }
            }

            t = default;
            return false;
        }
        
        public static bool GetBehaviour<T>(LPEntity lpEntity, out T t) where T : LPBehaviour {
            //是否有实体
            if (BehaviourDic.TryGetValue(lpEntity.ID, out List<LPBehaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    LPBehaviour lpBehaviour = behaviours[i];
                    if (lpBehaviour.GetType() == typeof(T)) {
                        t = (T)lpBehaviour;
                        return true;
                    }
                }
            }

            t = default;
            return false;
        }

        #endregion
    }
}