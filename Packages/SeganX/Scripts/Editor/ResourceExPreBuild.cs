using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SeganX
{
    public class ResourceExPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            ResourceEx.OnRuntimeMethodLoad();
        }
    }
}