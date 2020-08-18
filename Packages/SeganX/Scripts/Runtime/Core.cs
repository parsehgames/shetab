using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class Core : ScriptableObject
    {
        [System.Serializable]
        public class OnlineOptions
        {
            public int gameId = 0;
            public string onlineDomain = "seganx.ir";
        }

        [System.Serializable]
        public class SecurityOptions
        {
            public string cryptokey = "replace crypto key here";
            public string salt = "replace salt";
        }

#if UNITY_EDITOR
        [System.Serializable]
        public class TestDevices
        {
            public bool active = false;
            public int index = 0;
            [Header("List of devices for test"), Space]
            public string[] list = new string[0];
        }
#endif

        [SerializeField] private SecurityOptions securityOptions = new SecurityOptions();
        [Space]
        [SerializeField] private OnlineOptions onlineOptions = new OnlineOptions();

#if UNITY_EDITOR
        [Space]
        [SerializeField] private TestDevices testDevices = new TestDevices();
#endif


        private string baseDeviceId = string.Empty;
        private string deviceId = string.Empty;
        private string hashsalt = string.Empty;
        private byte[] cryptoKey = null;

        protected void OnInitialize()
        {
#if UNITY_EDITOR
            baseDeviceId = deviceId = testDevices.active ? testDevices.list[testDevices.index] : SystemInfo.deviceUniqueIdentifier;
#else
            baseDeviceId = SystemInfo.deviceUniqueIdentifier;
            deviceId = ComputeMD5(baseDeviceId, securityOptions.salt);
#endif
            hashsalt = ComputeMD5(securityOptions.salt, securityOptions.salt);
            cryptoKey = System.Text.Encoding.ASCII.GetBytes(securityOptions.cryptokey);

#if UNITY_EDITOR
#else
            securityOptions.cryptokey = string.Empty;
            securityOptions.salt = string.Empty;
#endif
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static string BaseDeviceId { get { return Instance.baseDeviceId; } }
        public static string DeviceId { get { return Instance.deviceId; } }
        public static string Salt { get { return Instance.hashsalt; } }
        public static byte[] CryptoKey { get { return Instance.cryptoKey; } }
        public static int GameId { get { return Instance.onlineOptions.gameId; } }
        public static string OnlineDomain { get { return Instance.onlineOptions.onlineDomain; } }

        private static Core instance = default;

        public static Core Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe(CreateInstance<Core>(), typeof(Core).Name);
#endif
                if (instance == null)
                {
                    instance = Resources.Load<Core>("Configs/" + typeof(Core).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }

        private static string ComputeMD5(string str, string salt)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str + salt);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var res = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                res.Append(hashBytes[i].ToString("X2"));
            return res.ToString();
        }



#if UNITY_EDITOR
        private static void CreateMe(Object instance, string name)
        {
            var path = "/Resources/Configs/";
            var appath = Application.dataPath + path;
            var fileName = path + name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (!System.IO.Directory.Exists(appath)) System.IO.Directory.CreateDirectory(appath);
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets" + fileName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}