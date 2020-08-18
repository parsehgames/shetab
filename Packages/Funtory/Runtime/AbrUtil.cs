using UnityEngine;

namespace AbrStudio
{
	public class AbrUtil {

		public static void OpenReviewPage() {
#if UNITY_ANDROID
			AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		
			AndroidJavaClass abrUtilClass = new AndroidJavaClass("co.abrstudio.utils.AbrUtil");
			abrUtilClass.CallStatic("openReviewPage", activity);
#endif
		}
		
		public static void OpenAppPage() {
#if UNITY_ANDROID
			AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		
			AndroidJavaClass abrUtilClass = new AndroidJavaClass("co.abrstudio.utils.AbrUtil");
			abrUtilClass.CallStatic("openAppPage", activity);
#endif
		}
		
		public static void OpenDeveloperPage() {
#if UNITY_ANDROID
			AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		
			AndroidJavaClass abrUtilClass = new AndroidJavaClass("co.abrstudio.utils.AbrUtil");
			abrUtilClass.CallStatic("openDeveloperPage", activity);
#endif
		}
	}
}
