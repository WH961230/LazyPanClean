using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

namespace LazyPanClean {
    public class LPConsoleEx : LPSingleton<LPConsoleEx> {
        private Dictionary<int, List<string>> contentDic = new Dictionary<int, List<string>>();
        private LPComp _lpComp;
        private bool firstSendCode;

        public void Init(bool initOpen) {
#if UNITY_EDITOR
            LPInputRegister.Instance.Dispose(LPInputRegister.Console);
            LPInputRegister.Instance.Load(LPInputRegister.Console, ConsoleEvent);
            _lpComp =
                LPLoader.LoadComp("控制台界面", "UI/UI_Console", LPLaunch.instance.UIDontDestroyRoot, initOpen);
            ContentClear();
#endif
        }

        private void ConsoleEvent(InputAction.CallbackContext obj) {
            if (obj.performed) {
                bool hasDebug = _lpComp.gameObject.activeSelf;
                if (hasDebug) {
                    _lpComp.gameObject.SetActive(false);
                } else {
                    _lpComp.gameObject.SetActive(true);
                    firstSendCode = true;
                    //绑定按键
                    LPCond.Instance.Get<TMP_InputField>(_lpComp, LPLabel.CODE).text = "";
                    LPCond.Instance.Get<TMP_InputField>(_lpComp, LPLabel.CODE).ActivateInputField();
                    LPCond.Instance.Get<TMP_InputField>(_lpComp, LPLabel.CODE).onEndEdit.RemoveAllListeners();
                    LPCond.Instance.Get<TMP_InputField>(_lpComp, LPLabel.CODE).onEndEdit.AddListener(SendCode);
                }
            }
        }

        //发送命令
        private void SendCode(string content) {
            if (Keyboard.current.enterKey.isPressed) {
                if (!string.IsNullOrEmpty(content)) {
                    Content("you", content);
                    CodeAction(content);
                }
                LPCond.Instance.Get<TMP_InputField>(_lpComp, LPLabel.CODE).ActivateInputField();
            }
        }

        //命令的触发表现
        private void CodeAction(string code) {
            if (code == "help") {
                Content("computer",
                    "code: help[帮助]\n" +
                    "clear[清空控制台]\n" +
                    "flow[流程数据]\n" +
                    "behaviour[行为数据]\n" +
                    "entity[实体数据]\n" +
                    "normal[普通数据]\n");
                return;
            }

            if (code == "clear") {
                ContentClear();
                return;
            }

            if (contentDic.TryGetValue(code.GetHashCode(), out List<string> contentVal)) {
                string originalText = LPCond.Instance.Get<TextMeshProUGUI>(_lpComp, LPLabel.CONTENT).text;
                string tempContent = "";
                foreach (string content in contentVal) {
                    string[] info = content.Split("@");
                    tempContent += string.Concat(info[0], " : ", info[1], "\n");
                }

                SetText(string.Concat(tempContent, "\n", originalText));
            }
        }

        //新增内容
        public void Content(string who, string content) {
#if UNITY_EDITOR
            string originalText = LPCond.Instance.Get<TextMeshProUGUI>(_lpComp, LPLabel.CONTENT).text;
            SetText(string.Concat(who, " : ", content, "\n", originalText));
#endif
        }

        //控制台内容存字典
        public void ContentSave(string hashKey, string content) {
            if (contentDic.TryGetValue(hashKey.GetHashCode(), out List<string> values)) {
                values.Add(string.Concat(System.DateTime.Now, "@", content));
            } else {
                List<string> contentVal = new List<string>();
                contentVal.Add(string.Concat(System.DateTime.Now, "@", content));
                contentDic.Add(hashKey.GetHashCode(), contentVal);
            }
        }

        //清空内容
        private void ContentClear() {
            LPCond.Instance.Get<TextMeshProUGUI>(_lpComp, LPLabel.CONTENT).text = null;
        }

        //设置输入内容
        private void SetText(string content) {
            LPCond.Instance.Get<TextMeshProUGUI>(_lpComp, LPLabel.CONTENT).text = content;
        }
    }
}