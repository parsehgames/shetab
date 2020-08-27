using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

static class FuntorySettings
{
    private class Package
    {
        public string package = string.Empty;
        public string name = string.Empty;
        public string description = string.Empty;
        public string checker = string.Empty;
        public string symbol = string.Empty;
    }

    private const string packageName = "com.funtory.mobile";

    private static List<Package> packages = new List<Package>()
    {
        new Package()
        {
            package = "funtory.Infrastructure.unitypackage",
            name = "Infrastructure",
            description = "Infrastructure will creates base classes. This is neccessary for use other tools and libraries.",
            checker = "Infrastructure"
        },
        new Package()
        {
            package = "funtory.abr.unitypackage",
            name = "Abr Studio",
            description = "Adds Abs studio plugin including advertisement system and market functions.",
            checker = "AbrStudio",
            symbol = "ABR"
        },
        new Package()
        {
            package = "funtory.purchase.unitypackage",
            name = "Purchase Plugin",
            description = "Adds Purchase plugin including Iab system for CafeBazaar and Myket.\nNote: Builder system must disable specific jar file and modify manifest file to switch between markets.\nFiles included: Bazaar.jar + Myket.jar + AndroidManifestTemplate.xml",
            checker = "Purchase",
        },
        new Package()
        {
            package = "funtory.profile.gems.unitypackage",
            name = "Resource Gems",
            description = "Append Gem resource to the game.\nFiles included: FuntoryGems.cs + Gem.cs\nEffected classes: Fun.Profile.Data + Profile + ProfilePreset + PhysicsLayer + FunFactory.Gem + PlayModel.Stats + RewardMode.Pack",
            checker = "Profile/Gems"
        },
        new Package()
        {
            package = "funtory.profile.coins.unitypackage",
            name = "Resource Coins",
            description = "Append Coin resource to the game.\nFiles included: FuntoryCoins.cs + Coin.cs\nEffected classes: Fun.Profile.Data + Profile + ProfilePreset + PhysicsLayer + FunFactory.Gem + PlayModel.Stats + RewardMode.Pack",
            checker = "Profile/Coins"
        },
        new Package()
        {
            package = "funtory.profile.keys.unitypackage",
            name = "Resource Keys",
            description = "Append Key resource to the game.\nFiles included: FuntoryKeys.cs + Key.cs\nEffected classes: Fun.Profile.Data + Profile + ProfilePreset + PhysicsLayer + FunFactory.Gem + PlayModel.Stats + RewardMode.Pack",
            checker = "Profile/Keys"
        },
        new Package()
        {
            package = "funtory.roads.unitypackage",
            name = "Roads System",
            description = "Adds Road system to project including tools for creating and traversing through splines.\n+Fields in FunConfig.\n+Functions in FunFactory.\n+RoadSeeker for moving on roads.",
            checker = "Roads",
            symbol = "SX_SPLINE;SX_MESH"
        },
        new Package()
        {
            package = "funtory.template.unitypackage",
            name = "Game Template",
            description = "Adds game template to the project including scripts, prefabs and media files!",
            checker = "Template"
        },
    };

    private static string PackageResourcesPath
    {
        get
        {
            string packagePath = Path.GetFullPath("Packages/" + packageName + "/Package Resources/");
            if (Directory.Exists(packagePath)) return packagePath;

            packagePath = Path.GetFullPath("Assets/UnityPackage/Funtory/Package Resources/");
            if (Directory.Exists(packagePath)) return packagePath;

            return string.Empty;
        }
    }

    private static bool IsAlreadyInstalled(string folder)
    {
        string packagePath = Path.GetFullPath("Assets/Funtory/" + folder);
        return Directory.Exists(packagePath) || File.Exists(packagePath);
    }

    private static string AddRemoveSymbol(string current, string symbol, bool addsymbols)
    {
        if (string.IsNullOrEmpty(symbol)) return current;

        var cursymbols = new List<string>(current.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        var newsymbols = new List<string>(symbol.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

        //  remove duplicated symbols
        foreach (var item in newsymbols)
            cursymbols.RemoveAll(x => x == item);

        //  add new symbols
        if (addsymbols)
            foreach (var item in newsymbols)
                cursymbols.Add(item);

        cursymbols.Sort();
        return string.Join(";", cursymbols.ToArray());
    }


    //////////////////////////////////////////////////////
    /// Settings Provider
    //////////////////////////////////////////////////////
    [SettingsProvider]
    public static SettingsProvider CreateSettings()
    {
        var provider = new SettingsProvider("Project/Funtory", SettingsScope.Project)
        {
            label = "Funtory",
            keywords = new HashSet<string>(new[] { "Funtory", "Settings", "Core" })
        };

        provider.guiHandler = (searchContext) =>
        {
            EditorGUILayout.BeginVertical();

            foreach (var item in packages)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(50));
                EditorGUILayout.LabelField(item.name, EditorStyles.boldLabel, GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField(item.description, EditorStyles.wordWrappedLabel, GUILayout.ExpandHeight(true));
                if (IsAlreadyInstalled(item.checker))
                    GUILayout.Box("Installed", GUILayout.ExpandHeight(true), GUILayout.Width(100));
                else if (GUILayout.Button("Install", GUILayout.ExpandHeight(true), GUILayout.Width(100)))
                {
                    AssetDatabase.ImportPackage(PackageResourcesPath + item.package, true);

                    var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
                    var resutl = AddRemoveSymbol(currentSymbols, item.symbol, true);
                    if (resutl != currentSymbols)
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, resutl);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        };


        return provider;
    }
}