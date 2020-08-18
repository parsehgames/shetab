using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfig<T> : StaticConfigBase where T : StaticConfig<T>
    {
        private static T instance = default;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe(CreateInstance<T>(), typeof(T).Name);
#endif
                if (instance == null)
                {
                    instance = Resources.Load<T>("Configs/" + typeof(T).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }


        protected static void SaveData(object data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefsEx.SetString(Instance.name + instance.version, json);
        }

        protected static D LoadData<D>(D defauleObj)
        {
            var json = PlayerPrefsEx.GetString(Instance.name + instance.version, string.Empty);
            if (json.Length > 5)
                return JsonUtility.FromJson<D>(json);
            else
                return defauleObj;
        }
    }
}
