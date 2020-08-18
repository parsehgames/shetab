#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CreateAssetMenu(menuName = "Game/Build Settings")]
    public class Builder : StaticConfig<Builder>
    {
        public enum Architecture : int { ARMV7 = 1, ARM64 = 2, ARMV7_ARM64 = 3 }

        [System.Serializable]
        public class Disabled
        {
            public Object file = null;
            public string filename { set; get; }
            public string newname { set; get; }
        }

        [System.Serializable]
        public class Replaced
        {
            public string what = string.Empty;
            public string with = string.Empty;
            public Object file = null;
            public string filename { set; get; }
        }

        [System.Serializable]
        public class Changes
        {
            public List<Disabled> disables = new List<Disabled>();
            public List<Replaced> replaces = new List<Replaced>();
        }

        [System.Serializable]
        public class Build
        {
            public string fileName = string.Empty;
            public bool activated = false;
            public bool buildAppBundle = false;
            public Architecture architecture = Architecture.ARMV7;
            public List<Disabled> disables = null;
            public List<Replaced> replaces = new List<Replaced>();
        }

        [System.Serializable]
        public class Keystore
        {
            public string storePassword = string.Empty;
            public string alisePassword = string.Empty;
        }

        public int bundleVersionCode = 1;
        [Header("Options:"), Space()]
        public int buildAndRunIndex = -1;
        public bool stopQueueOnError = true;
        [Header("Build Configurations:"), Space()]
        public Keystore keystore = new Keystore();
        public Changes preBuild = new Changes();
        public List<Build> builds = new List<Build>();

        protected override void OnInitialize() { }

        [MenuItem("SeganX/Build")]
        private static void SelectMe()
        {
            Selection.activeObject = Instance;
        }
    }
}
#endif