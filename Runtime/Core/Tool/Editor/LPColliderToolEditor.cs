using UnityEditor;
using UnityEngine;

namespace LazyPanClean {
    [CustomEditor(typeof(LPColliderTool))]
    public class LPColliderToolEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            LPColliderTool tool = (LPColliderTool)target;
            if (GUILayout.Button("添加碰撞体并注入Comp脚本")) {
                GameObject instanceParent = new GameObject("Parent");
                instanceParent.transform.parent = tool.gameObject.transform;

                GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trigger.transform.parent = instanceParent.transform;
                trigger.transform.position += Vector3.up * 0.5f;
                trigger.transform.name = "Trigger";
                trigger.GetComponent<Collider>().isTrigger = true;
                LPComp triggerLpComp = trigger.AddComponent<LPComp>();

                tool.GetComponent<LPComp>().Transforms.Add(new LPComp.TransformData() {
                    Sign = "Foot",
                    Tran = instanceParent.transform,
                });

                tool.GetComponent<LPComp>().Comps.Add(new LPComp.CompData() {
                    Sign = "Trigger",
                    lpComp = triggerLpComp,
                });
            }
        }
    }
}