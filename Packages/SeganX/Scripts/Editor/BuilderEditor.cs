using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SeganX
{
    [CustomEditor(typeof(Builder))]
    public class BuilderEditor : Editor
    {
        const string disabled = ".Disabled";

        private class BackupFiles
        {
            public string filename = string.Empty;
            public byte[] data = null;
        }

        public override void OnInspectorGUI()
        {
            var builder = target as Builder;

            // verify files
            if (ValidateFiles(builder) == false)
            {
                EditorGUILayout.HelpBox("File refrence(s) missing! Please check file refrences!", MessageType.Error);
            }
            else if (GUILayout.Button("Build"))
            {
                backupFiles.Clear();
                StartBuild(builder);
            }

            base.OnInspectorGUI();

        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static List<BackupFiles> backupFiles = new List<BackupFiles>();

        private static void StartBuild(Builder builder)
        {
            // update filenames
            if (ValidateFiles(builder) == false)
            {
                return;
            }

            var path = Directory.GetParent(Application.dataPath).Parent.FullName + "/Bin/";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var folder = EditorUtility.SaveFolderPanel("Select Destination Folder", path, null);
            if (string.IsNullOrEmpty(folder)) return;

            PlayerSettings.bundleVersion = builder.version + "." + builder.bundleVersionCode;
            PlayerSettings.Android.bundleVersionCode = builder.bundleVersionCode;
            PlayerSettings.Android.keystorePass = builder.keystore.storePassword;
            PlayerSettings.Android.keyaliasPass = builder.keystore.alisePassword;
            var currentArchitectures = PlayerSettings.Android.targetArchitectures;

            foreach (var disbale in builder.preBuild.disables)
                DisableFile(disbale, false);

            bool buildResult = true;
            for (int index = 0; buildResult && index < builder.builds.Count; index++)
            {
                var build = builder.builds[index];
                if (build.activated == false) continue;
                PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)build.architecture;

                foreach (var replace in builder.preBuild.replaces)
                    ReplaceInFile(replace);

                foreach (var replace in build.replaces)
                    ReplaceInFile(replace);

                foreach (var disbale in build.disables)
                    DisableFile(disbale, false);

                EditorUserBuildSettings.buildAppBundle = build.buildAppBundle;

                Thread.Sleep(1000); // wait for file preparation
                buildResult = Build(Path.Combine(folder, string.Format(build.fileName + (build.buildAppBundle ? ".aab" : ".apk"), Application.version)), index == builder.buildAndRunIndex);
                Thread.Sleep(1000); // wait for file preparation

                EditorUserBuildSettings.buildAppBundle = false;

                foreach (var disbale in build.disables)
                    DisableFile(disbale, true);

                // restore file
                foreach (var item in backupFiles)
                    if (item.data != null)
                        File.WriteAllBytes(item.filename, item.data);
                backupFiles.Clear();
            }

            foreach (var disbale in builder.preBuild.disables)
                DisableFile(disbale, true);

            PlayerSettings.Android.targetArchitectures = currentArchitectures;

            Debug.Log("Finished building process.");
        }

        private static bool ValidateFiles(Builder builder)
        {
            foreach (var item in builder.preBuild.disables)
            {
                if (item.file == null) return false;
                item.filename = AssetDatabase.GetAssetPath(item.file);
            }

            foreach (var item in builder.preBuild.replaces)
            {
                if (item.file == null) return false;
                item.filename = AssetDatabase.GetAssetPath(item.file);
            }

            foreach (var option in builder.builds)
            {
                foreach (var item in option.disables)
                {
                    if (item.file == null) return false;
                    item.filename = AssetDatabase.GetAssetPath(item.file);
                    item.newname = item.filename + disabled;
                }

                foreach (var item in option.replaces)
                {
                    if (item.file == null) return false;
                    item.filename = AssetDatabase.GetAssetPath(item.file);
                }
            }
            return true;
        }

        private static void BackupFile(string filename)
        {
            if (File.Exists(filename) == false) return;
            if (backupFiles.Exists(x => x.filename.Equals(filename, System.StringComparison.OrdinalIgnoreCase))) return;

            var backup = new BackupFiles();
            backup.filename = filename;
            try
            {
                backup.data = File.ReadAllBytes(filename);
                backupFiles.Add(backup);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Can't open file " + filename + "\n" + e.Message);
            }
        }

        private static void ReplaceInFile(Builder.Replaced item)
        {
            if (File.Exists(item.filename) == false) return;

            BackupFile(item.filename);

            var text = File.ReadAllText(item.filename);
            text = text.Replace(item.what, item.with);
            File.WriteAllText(item.filename, text);
        }

        private static void DisableFile(Builder.Disabled item, bool undo)
        {
            if (undo)
            {
                try
                {
                    File.Move(item.newname, item.filename);
                    File.Move(item.newname + ".meta", item.filename + ".meta");
                    Thread.Sleep(1000);
                }
                catch { }
            }
            else
            {
                try
                {
                    File.Move(item.filename, item.newname);
                    File.Move(item.filename + ".meta", item.newname + ".meta");
                    Thread.Sleep(1000);
                }
                catch { }
            }
        }

        private static bool Build(string locationPathName, bool autoRun)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.locationPathName = locationPathName;

            if (autoRun)
                buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;

            try
            {
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                switch (summary.result)
                {
                    case BuildResult.Succeeded:
                        Debug.Log("Build succeeded in " + (int)summary.totalTime.TotalSeconds + " seconds with " + summary.totalSize + " bytes");
                        return true;
                    case BuildResult.Failed:
                        Debug.Log("Build failed!");
                        return false;
                    case BuildResult.Cancelled:
                        Debug.Log("Build cancelled!");
                        return false;
                    default:
                        Debug.Log("Build result is unknown!");
                        return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Build failed: " + e.Message);
                return false;
            }
        }
    }
}