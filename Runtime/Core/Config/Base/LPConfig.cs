namespace LazyPanClean {
    public class LPConfig : LPSingleton<LPConfig> {
        public void Init() {
            LPBehaviourConfig.Init();
            LPSceneConfig.Init();
            LPObjConfig.Init();
            LPUIConfig.Init();
        }
    }
}