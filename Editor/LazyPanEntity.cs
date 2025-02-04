using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LazyPanClean {
    public class LazyPanEntity : EditorWindow {
        private bool isFoldoutTool;
        private bool isFoldoutData;
        private bool isFoldoutGenerate;
        private string _instanceFlowName;
        private string _instanceTypeName;
        private string _instanceObjName;
        private string _instanceObjChineseName;
        private string[][] EntityConfigStr;
        private string[][] BehaviourConfigStr;
        private string[] behaviourNames;
        private bool[] selectedOptions;
        private LazyPanTool _tool;
        
        public void OnStart(LazyPanTool tool) {
            _tool = tool;
            LPReadCSV.Instance.Read("ObjConfig", out string content, out string[] lines);
            if (lines != null && lines.Length > 0) {
                EntityConfigStr = new string[lines.Length - 3][];
                for (int i = 0; i < lines.Length; i++) {
                    if (i > 2) {
                        //遍历第三行到最后一行
                        //遍历每一行数据
                        string[] lineStr = lines[i].Split(",");
                        EntityConfigStr[i - 3] = new string[lineStr.Length];
                        if (lineStr.Length > 0) {
                            for (int j = 0; j < lineStr.Length; j++) {
                                EntityConfigStr[i - 3][j] = lineStr[j];
                            }
                        }
                    }
                }
            }

            LPReadCSV.Instance.Read("BehaviourConfig", out content, out lines);
            if (lines != null && lines.Length > 0) {
                BehaviourConfigStr = new string[lines.Length - 3][];
                for (int i = 0; i < lines.Length; i++) {
                    if (i > 2) {
                        string[] lineStr = lines[i].Split(",");
                        BehaviourConfigStr[i - 3] = new string[lineStr.Length];
                        if (lineStr.Length > 0) {
                            for (int j = 0; j < lineStr.Length; j++) {
                                BehaviourConfigStr[i - 3][j] = lineStr[j];
                            }
                        }
                    }
                }
            }

            behaviourNames = new string[BehaviourConfigStr.Length];
            for (int i = 0; i < BehaviourConfigStr.Length; i++) {
                string[] tmpStr = BehaviourConfigStr[i];
                for (int j = 0; j < tmpStr.Length; j++) {
                    if (j == 1) {
                        behaviourNames[i] = tmpStr[j];
                    }
                }
            }
            
            isFoldoutTool = true;
            isFoldoutData = true;
            isFoldoutGenerate = true;
        }

        public void OnCustomGUI(float areaX) {
            GUILayout.BeginArea(new Rect(areaX, 60, Screen.width, Screen.height));
            Title();
            AutoTool();
            ManualGeneratePrefabTool();
            PreviewEntityConfigData();
            GUILayout.EndArea();
        }

        private void PreviewEntityConfigData() {
            isFoldoutData = EditorGUILayout.Foldout(isFoldoutData, " 预览实体配置数据", true);
            Rect rect = GUILayoutUtility.GetLastRect();
            float height = 0;
            if (isFoldoutData) {
                ExpandEntityData();
                height += GUILayoutUtility.GetLastRect().height;
            } else {
                GUILayout.Space(10);
            }
            
            LazyPanTool.DrawBorder(new Rect(rect.x + 2f, rect.y - 2f, rect.width - 2f, rect.height + height + 5f), Color.white);

            GUILayout.Space(10);
        }

        private void ManualGeneratePrefabTool() {
            isFoldoutGenerate = EditorGUILayout.Foldout(isFoldoutGenerate, " 手动创建预制体工具", true);
            Rect rect = GUILayoutUtility.GetLastRect();
            float height = 0;
            if (isFoldoutGenerate) {
                GUILayout.BeginVertical();
                GUILayout.Label("");
                GUILayout.BeginHorizontal();
                _instanceFlowName = EditorGUILayout.TextField("流程标识(必填)", _instanceFlowName, GUILayout.Height(20));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                _instanceTypeName = EditorGUILayout.TextField("类型标识(必填)", _instanceTypeName, GUILayout.Height(20));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                _instanceObjName = EditorGUILayout.TextField("实体标识(必填)", _instanceObjName, GUILayout.Height(20));
                GUILayout.EndHorizontal();
                GUI.SetNextControlName("objChineseName");
                GUILayout.BeginHorizontal();
                _instanceObjChineseName = EditorGUILayout.TextField("实体中文名字(必填)", _instanceObjChineseName, GUILayout.Height(20));
                GUILayout.EndHorizontal();
                GUIStyle style = LazyPanTool.GetGUISkin("AButtonGUISkin").GetStyle("button");
                if(GUILayout.Button("创建实体物体", style)) {
                    InstanceCustomObj();
                }
                if(GUILayout.Button("创建实体物体点位配置", style)) {
                    InstanceCustomLocationSetting();
                }
                GUILayout.EndVertical();
                height += GUILayoutUtility.GetLastRect().height;
            } else {
                GUILayout.Space(10);
            }
            
            LazyPanTool.DrawBorder(new Rect(rect.x + 2f, rect.y - 2f, rect.width - 2f, rect.height + height + 5f), Color.white);

            GUILayout.Space(10);
        }

        private void AutoTool() {
            GUIStyle style = LazyPanTool.GetGUISkin("AButtonGUISkin").GetStyle("button");
            isFoldoutTool = EditorGUILayout.Foldout(isFoldoutTool, " 自动化工具", true);
            Rect rect = GUILayoutUtility.GetLastRect();
            float height = 0;
            if (isFoldoutTool) {
                GUILayout.Label("");
                height += GUILayoutUtility.GetLastRect().height;
                if(GUILayout.Button("打开实体配置表 Csv", style)) {
                    GUILayout.BeginHorizontal();
                    OpenEntityCsv();
                    GUILayout.EndHorizontal();
                }
                height += GUILayoutUtility.GetLastRect().height;
            } else {
                GUILayout.Space(10);
            }
            
            LazyPanTool.DrawBorder(new Rect(rect.x + 2f, rect.y - 2f, rect.width - 2f, rect.height + height + 5f), Color.white);

            GUILayout.Space(10);
        }

        private void Title() {
            GUILayout.BeginHorizontal();
            GUIStyle style = LazyPanTool.GetGUISkin("LogoGUISkin").GetStyle("label");
            GUILayout.Label("ENTITY", style);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            style = LazyPanTool.GetGUISkin("AnnotationGUISkin").GetStyle("label");
            GUILayout.Label("@实体 游戏内最小单位", style);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        //选中改变状态
        private void ToggleLayerSelection(int index, string flowSign, string entitySign, string behaviourSign) {
            selectedOptions[index] = !selectedOptions[index];
            //如果点击后是增加 则增加行为数据 反之减少 操作是对行为重新录入
            UpdateEntityBehaviourData(selectedOptions[index], flowSign, entitySign, behaviourSign);
            OnStart(_tool);
        }

        private void UpdateEntityBehaviourData(bool add, string flowSign, string entitySign, string behaviourName) {
            LPReadCSV.Instance.Read("ObjConfig", out string content, out string[] lines);
            for (int i = 0; i < lines.Length; i++) {
                if (i > 2) {
                    string[] linesStr = lines[i].Split(',');
                    if (linesStr[0] == entitySign && linesStr[1] == flowSign) {
                        string[] bindBehaviourName = linesStr[5].Split('|');
                        string newBind = "";
                        foreach (var tmpBindStr in bindBehaviourName) {
                            if (add) {
                                newBind += tmpBindStr + "|";
                            } else {
                                if (tmpBindStr != behaviourName) {
                                    newBind += tmpBindStr + "|";
                                }
                            }
                        }

                        if (add) {
                            newBind += behaviourName;
                        }

                        newBind = newBind.TrimEnd('|');
                        newBind = newBind.TrimStart('|');

                        linesStr[5] = newBind;
                
                        // 将更新后的内容重新拼接回 lines 数组
                        lines[i] = string.Join(",", linesStr);
                        break;
                    }
                }
            }

            LPReadCSV.Instance.Write("ObjConfig", lines);
        }

        private void ExpandEntityData() {
            bool hasContent = false;
            if (EntityConfigStr != null && EntityConfigStr.Length > 0) {
                GUILayout.BeginVertical();
                string entitySign = "";
                int displayCount = 0;
                foreach (var str in EntityConfigStr) {
                    if (str != null) {
                        if (entitySign != str[1]) {
                            entitySign = str[1];
                            GUILayout.Label("");
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        string strContent = "";
                        displayCount = str.Length;
                        for (int i = 0; i < displayCount; i++) {
                            bool isLast = false;
                            strContent = str[i];

                            bool hasPrefab = HasPrefabTips(str);
                            Color fontColor;
                            if (i == 1) {
                                fontColor = Color.cyan;
                            } else if (i == 0) {
                                fontColor = hasPrefab ? Color.green : Color.red;
                            } else {
                                fontColor = Color.green;
                            }

                            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                            labelStyle.normal.textColor = fontColor; // 设置字体颜色

                            //预制体相关判断
                            if (GUILayout.Button(strContent, isLast ? null : labelStyle,
                                GUILayout.Width(Screen.width / (displayCount + 1) - 10))) {
                                switch (i) {
                                    case 0:
                                        PrefabJudge(hasPrefab, str);
                                        break;
                                    case 5:
                                        BehaviourJudge(str);
                                        break;
                                }
                            }

                            string tooltip = "";
                            // 检测鼠标悬停
                            Rect buttonRect = GUILayoutUtility.GetLastRect();
                            if (buttonRect.Contains(Event.current.mousePosition)) {
                                if (!hasPrefab) {
                                    tooltip = "找不到预制体，请添加: " + str[0];
                                }
                            }

                            // 显示悬浮信息
                            if (!string.IsNullOrEmpty(tooltip)) {
                                Vector2 tooltipPosition =
                                    Event.current.mousePosition + new Vector2(10, 10); // 设置悬浮提示位置
                                GUI.Label(new Rect(tooltipPosition.x, tooltipPosition.y, 250, 20), tooltip);
                            }

                            hasContent = true;
                        }

                        strContent = "行为绑定";
                        SelectBindBehaviour(strContent, displayCount, str[1], str[0], str[5]);

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();                
                    }
                }
                GUILayout.EndVertical();
            }

            if(!hasContent || EntityConfigStr == null || EntityConfigStr.Length == 0) {
                GUILayout.Label("找不到 EntityConfig.csv 的配置数据！\n请检查是否存在路径或者配置内数据是否为空！");
            }
        }

        private void SelectBindBehaviour(string buttonName, int displayCount, string flowSign, string entitySign, string behaviourSigns) {
            if (GUILayout.Button(buttonName, GUILayout.Width(Screen.width / (displayCount + 1) - 10))) {
                GenericMenu menu = new GenericMenu();
                List<string> behaviourClips = behaviourSigns.Split('|').ToList();
                selectedOptions = new bool[behaviourNames.Length];
                for (int i = 0; i < behaviourNames.Length; i++) {
                    int index = i;
                    string tmpBehaviourName = behaviourNames[i];
                    bool isSelected = behaviourClips.Contains(tmpBehaviourName);
                    selectedOptions[i] = isSelected;
                    menu.AddItem(new GUIContent(tmpBehaviourName), isSelected, () => ToggleLayerSelection(index, flowSign, entitySign, tmpBehaviourName));
                }
                menu.ShowAsContext();
            }
        }

        //绑定行为相关
        private void BehaviourJudge(string[] str) {
            //Color green
            //Color red
            //去 BehaviourConfig 判断是否配置行为 没有的话 点击 CSV 创建？ 有的话跳转到行为预览？
            Debug.Log("行为点击：" + str[5]);
        }

        //预制体相关
        private void PrefabJudge(bool hasPrefab, string[] str) {
            //点击的实体如果在实体配置存在直接跳转 如果没有游戏物体创建
            if (hasPrefab) {
                string path = $"Assets/LazyPan/Bundles/Prefabs/Obj/{str[1]}/{str[0]}.prefab"; // 修改为你的路径
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null) {
                    Selection.activeObject = prefab;
                    EditorGUIUtility.PingObject(prefab);
                }
            } else {
                _instanceFlowName = str[1];
                _instanceTypeName = str[2];
                _instanceObjName = str[0].Split("_")[2];
                GUI.FocusControl("objChineseName");
            }
        }

        //是否存在预制体
        private bool HasPrefabTips(string[] str) {
            string prefabPath = $"Assets/LazyPan/Bundles/Prefabs/Obj/{str[1]}/{str[0]}.prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null;
        }

        private void OpenEntityCsv() {
            string filePath = Application.dataPath + "/StreamingAssets/Csv/ObjConfig.csv";
            Process.Start(filePath);
        }

        private void InstanceCustomObj() {
            if (_instanceObjName == "" || _instanceTypeName == "" || _instanceFlowName == "" || _instanceObjChineseName == "") {
                return;
            }
            string sourcePath = "Packages/evoreek.lazypan/Runtime/Bundles/Prefabs/Obj/Obj_Sample_Sample.prefab"; // 替换为你的预制体源文件路径
            string targetFolderPath = "Assets/LazyPan/Bundles/Prefabs/Obj"; // 替换为你想要拷贝到的目标文件夹路径
            
            // 获取选中的游戏对象
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab)) {
                // 确保目标文件夹存在
                if (!Directory.Exists(targetFolderPath)) {
                    Directory.CreateDirectory(targetFolderPath);
                }

                // 获取预制体路径
                string prefabPath = AssetDatabase.GetAssetPath(selectedPrefab);
                
                // 拷贝预制体到目标文件夹
                string targetPath = Path.Combine(targetFolderPath, Path.GetFileName(prefabPath));
                AssetDatabase.CopyAsset(prefabPath, targetPath);
                
                // 刷新AssetDatabase
                AssetDatabase.Refresh();
                
                //修改资源的名字为自定义
                AssetDatabase.RenameAsset(targetPath,
                    string.Concat(_instanceFlowName, _instanceFlowName != null ? "/" : "", "Obj_", _instanceTypeName,
                        "_", _instanceObjName));
                AssetDatabase.Refresh();
            }
        }

        private void InstanceCustomLocationSetting() {
            if (_instanceObjName == "" || _instanceTypeName == "" || _instanceFlowName == "" || _instanceObjChineseName == "") {
                return;
            }

            // 创建实例并赋值
            LPLocationInformationSetting testAsset = CreateInstance<LPLocationInformationSetting>();
            testAsset.SettingName = _instanceObjChineseName;
            testAsset.locationInformationDatas = new List<LocationInformationData>();
            testAsset.locationInformationDatas.Add(new LocationInformationData());

            // 替换为你希望保存的目录路径，例如 "Assets/MyFolder/"
            string savePath = "Assets/LazyPan/Bundles/Configs/Setting/LocationInformationSetting/";

            // 确保目标文件夹存在，如果不存在则创建
            if (!AssetDatabase.IsValidFolder(savePath)) {
                AssetDatabase.CreateFolder("Assets", "LazyPan/Bundles/Configs/Setting/LocationInformationSetting");
            }

            // 生成一个唯一的文件名
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(savePath + string.Concat("LocationInf_", _instanceFlowName, "_", _instanceObjName, ".asset"));

            // 将实例保存为.asset文件
            AssetDatabase.CreateAsset(testAsset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}