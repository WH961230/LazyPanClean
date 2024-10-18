using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace LazyPanClean {
    public partial class LPLoader {
        private static string SPRITE_PATH = "Assets/LazyPan/Bundles/Arts/Images/";
        private static string SPRITE_SUFFIX = ".png";

        public static LPSetting LoadSetting() {
            return Addressables.LoadAssetAsync<LPSetting>("Assets/LazyPan/Bundles/Configs/Setting/Setting.asset").WaitForCompletion();
        }

        public static LPLocationInformationSetting LoadLocationInfSetting(string sign) {
            return LoadAsset<LPLocationInformationSetting>(AssetType.ASSET,
                string.Concat("Setting/LocationInformationSetting/", sign));
        }

        public static LPGameSetting LoadGameSetting() {
            return Addressables.LoadAssetAsync<LPGameSetting>($"Packages/evoreek.lazypan/Runtime/Bundles/GameSetting/GameSetting.asset").WaitForCompletion();
        }

        public static T LoadAsset<T>(AssetType type, string assetName) {
            (string, string) addressData = LoadGameSetting().GetAddress(type);
            return Addressables.LoadAssetAsync<T>(string.Concat(addressData.Item1, assetName, addressData.Item2)).WaitForCompletion();
        }

        public static T LoadAsset<T>(CommonAssetType type) {
            string addressData = LoadGameSetting().GetCommonAddress(type);
            return Addressables.LoadAssetAsync<T>(addressData).WaitForCompletion();
        }

        /*加载游戏物体*/
        public static GameObject LoadGo(string finalName, string assetName, Transform parent, bool active) {
            (string, string) addressData = LoadGameSetting().GetAddress(AssetType.PREFAB);
            GameObject go = Addressables.InstantiateAsync(string.Concat(addressData.Item1, assetName, addressData.Item2), parent).WaitForCompletion();
            go.SetActive(active);
            go.name = finalName;
            return go;
        }

        public static LPComp LoadComp(string finalName, string assetName, Transform parent, bool isActive) {
            GameObject go = LoadGo(finalName, assetName, parent, isActive);
            return go.GetComponent<LPComp>();
        }

        public static AsyncOperation LoadSceneAsync(string name) {
            return SceneManager.LoadSceneAsync(name);
        }
    }
}