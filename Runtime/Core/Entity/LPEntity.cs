using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace LazyPanClean {
    [Serializable]
    public class LPEntity {
        public int ID;//身份ID
        public string Sign;//标识
        public string Type;//类型

        public GameObject Prefab;//物体
        public LPComp lpComp;//组件
        public LPData lpData;//数据
        public LPObjConfig LpObjConfig;//配置

        public void Init(string sign) {
            //设置ID
            ID = ++LPEntityRegister.EntityID;
            //设置配置
            LPObjConfig lpObjConfig = LPObjConfig.Get(sign);
            LpObjConfig = lpObjConfig;
            //设置标识
            Sign = lpObjConfig.Sign;
            //设置类型
            Type = lpObjConfig.Type;
            //获取对象池的物体 如数量不足 对象池预加载
            Prefab = LPLoader.LoadGo(null,
                string.Concat(LPSceneConfig.Get(SceneManager.GetActiveScene().name).DirPath, lpObjConfig.Sign),
                LPObj.Instance.ObjRoot, true);
            //设置Comp
            lpComp = Prefab.GetComponent<LPComp>();
            //设置Data
            lpData = Prefab.GetComponent<LPData>();
            //物体名赋值
            Prefab.name = string.Concat("[", ID, "]", lpObjConfig.Name);
            //读取配置位置初始化
            if (!string.IsNullOrEmpty(lpObjConfig.SetUpLocationInformationSign)) {
                LPLocationInformationSetting setting = LPLoader.LoadLocationInfSetting(lpObjConfig.SetUpLocationInformationSign);
                LocationInformationData data = null;
                if (setting.locationInformationDatas.Count > 1) {
                    data = setting.locationInformationDatas[UnityEngine.Random.Range(0, setting.locationInformationDatas.Count)];
                } else if (setting.locationInformationDatas.Count == 1) {
                    data = setting.locationInformationDatas[0];
                } else {
                    LPLogUtil.LogErrorFormat("错误！位置配置{0}信息数据为空！", setting.name);
#if UNITY_EDITOR
                    EditorApplication.isPaused = true;
#endif
                }

                SetBeginLocationInfo(data);
            }
            //注册实体
            LPEntityRegister.AddEntity(ID, this);
            //注册配置行为
            if (!string.IsNullOrEmpty(lpObjConfig.SetUpBehaviourName)) {
                string[] behaviourArray = lpObjConfig.SetUpBehaviourName.Split("|");
                for (int i = 0; i < behaviourArray.Length; i++) {
                    LPBehaviourRegister.RegisterBehaviour(ID, behaviourArray[i], out LPBehaviour outBehaviour);
                }
            }
        }

        public void SetBeginLocationInfo(LocationInformationData data) {
            CharacterController characterController = LPCond.Instance.Get<CharacterController>(this, LPLabel.CHARACTERCONTROLLER);
            if (characterController != null) {
                characterController.enabled = false;
            }

            if (LPCond.Instance.Get<Transform>(this, LPLabel.FOOT) != null) {
                LPCond.Instance.Get<Transform>(this, LPLabel.FOOT).position = data.Position;
                LPCond.Instance.Get<Transform>(this, LPLabel.FOOT).rotation = Quaternion.Euler(data.Rotation);
            }

            characterController = LPCond.Instance.Get<CharacterController>(this, LPLabel.CHARACTERCONTROLLER);
            if (characterController != null) {
                characterController.enabled = true;
            }
        }

        public void Clear() {
            //注销行为
            LPBehaviourRegister.UnRegisterAllBehaviour(ID);
            //注销实体
            LPEntityRegister.RemoveEntity(ID);
            //物体销毁
            Object.Destroy(Prefab);
            Prefab = null;
            //销毁配置
            LpObjConfig = null;
            //ID重置
            ID = 0;
        }
    }
}