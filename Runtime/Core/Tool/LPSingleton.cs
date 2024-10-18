namespace LazyPanClean {
    public class LPSingleton<T> where T : new() {
        static T instance = default;

        public static T Instance {
            get {
                if (instance != null) {
                    return instance;
                }

                instance = new T();
                return instance;
            }
        }
    }
}