namespace LazyPanClean {
    public abstract class LPBehaviour {
        public string BehaviourSign;
        public LPData BehaviourLpData;
        public LPEntity LpEntity;

        protected LPBehaviour(LPEntity lpEntity, string behaviourSign) {
            this.LpEntity = lpEntity;
            BehaviourSign = behaviourSign;
            LPConsoleEx.Instance.ContentSave("behaviour", $"ID:{lpEntity.ID} 注册行为:{LPBehaviourConfig.Get(BehaviourSign).Name}");
        }

        public void SetBehaviourData(LPData lpData) {
            BehaviourLpData = lpData;
        }

        public abstract void DelayedExecute();

        public virtual void Clear() {
            LPConsoleEx.Instance.ContentSave("behaviour", $"ID:{LpEntity.ID} 注销行为:{LPBehaviourConfig.Get(BehaviourSign).Name}");
        }
    }
}
