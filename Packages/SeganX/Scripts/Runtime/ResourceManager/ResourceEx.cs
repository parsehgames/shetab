using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class ResourceEx : StaticConfig<ResourceEx>
    {
        [System.Serializable]
        public class File
        {
            public string path = string.Empty;
            public string dire = string.Empty;
            public string name = string.Empty;
            public List<string> tags = new List<string>();
            public List<int> ids = new List<int>();
            public int Id => ids[0];
        }

        public bool justFilesWithId = true;
        public List<File> files = new List<File>();

        protected override void OnInitialize()
        {

        }

        ///////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ///////////////////////////////////////////////////////////////
        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public static T Load<T>(string dire, int id) where T : Object
        {
            if (dire.EndsWith("/") == false) dire += '/';
            var file = Instance.files.Find(x => IsSimilarPath(x.dire, dire, false) && x.Id == id);
            return file != null ? Resources.Load<T>(file.path) : default;
        }

        public static List<File> LoadAll(string dire, bool subfolders)
        {
            if (dire.EndsWith("/") == false) dire += '/';
            List<File> res = null;
            res = Instance.files.FindAll(x => IsSimilarPath(x.dire, dire, subfolders));
            res.Sort((x, y) => x.Id - y.Id);
            return res;
        }

        public static List<File> LoadAll(string dire, bool subfolders, System.Predicate<File> match)
        {
            if (dire.EndsWith("/") == false) dire += '/';
            List<File> res = null;
            res = Instance.files.FindAll(x => IsSimilarPath(x.dire, dire, subfolders) && match(x));
            res.Sort((x, y) => x.Id - y.Id);
            return res;
        }

        public static List<T> LoadAll<T>(string dire, bool subfolders, System.Predicate<File> match) where T : Object
        {
            if (dire.EndsWith("/") == false) dire += '/';
            var res = new List<T>();
            var files = Instance.files.FindAll(x => IsSimilarPath(x.dire, dire, subfolders) && match(x));
            if (files.Count == 0) return res;

            files.Sort((x, y) => x.Id - y.Id);
            foreach (var item in files)
            {
                var loaded = Resources.Load<T>(item.path);
                if (loaded != null) res.Add(loaded);
            }

            return res;
        }


        ////////////////////////////////////////////////////////////
        /// STATIC HELPER FUNCTIONS
        ////////////////////////////////////////////////////////////
        private static bool IsSimilarPath(string path, string str, bool subfolders)
        {
            if (path == null || path.Length < 0) return false;

            if (subfolders)
            {
                var index = path.IndexOf(str, System.StringComparison.OrdinalIgnoreCase);
                return index == 0;
            }
            else
            {
                return path == str;
            }
        }

#if UNITY_EDITOR
        private static void AddFile(string filepath)
        {
            var resname = System.IO.Path.GetFileNameWithoutExtension(filepath);
            if (resname.IsNullOrEmpty()) return;

            var item = new File
            {
                name = resname,
                dire = System.IO.Path.GetDirectoryName(filepath).Replace("\\", "/") + "/",
                path = filepath.ExcludeFileExtention().Replace("\\", "/")
            };

            var parts = item.name.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
            item.ids.Add(parts.Length > 0 ? parts[0].ToInt(-1) : -1);

            if (Instance.justFilesWithId)
            {
                if (item.Id == -1) return;
                if (Instance.files.Exists(x => x.name == item.name && x.dire == item.dire && x.path == item.path && x.ids == item.ids)) return;
            }
            else if (Instance.files.Exists(x => x.name == item.name && x.dire == item.dire && x.path == item.path)) return;

            for (int i = 1; i < parts.Length; i++)
            {
                var id = parts[i].ToInt(-1);
                if (id >= 0)
                    item.ids.Add(id);
                else
                    item.tags.Add(parts[i]);
            }

            Instance.files.Add(item);
        }

        private static void AddFilesToList(List<string> list, string path)
        {
            if (System.IO.Directory.Exists(path) == false) return;

            var files = System.IO.Directory.GetFiles(path);
            foreach (var item in files)
            {
                if (System.IO.Path.GetExtension(item) == ".meta") continue;
                list.Add(System.IO.Path.GetFullPath(item).MakeRelative(Application.dataPath + "/Resources/"));
            }

            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var item in dirs)
                AddFilesToList(list, item);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void OnRuntimeMethodLoad()
        {
            Instance.files.Clear();

            var list = new List<string>(10);
            AddFilesToList(list, Application.dataPath + "/Resources/");
            foreach (var item in list)
                AddFile(item);

            Instance.files.Sort((x, y) => x.Id - y.Id);
            UnityEditor.EditorUtility.SetDirty(Instance);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}