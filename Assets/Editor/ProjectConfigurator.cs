using UnityEditor;
using UnityEngine;

namespace RushLanes.EditorTools
{
    public static class ProjectConfigurator
    {
        [MenuItem("Rush Lanes/Apply Project Settings")]
        static void Apply()
        {
            PlayerSettings.companyName = "RushLanes";
            PlayerSettings.productName = "Rush Lanes";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.rushlanes.game");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.backgroundColor = new Color(0.04f, 0.05f, 0.1f, 1f);

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/AppIcon.png");
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
            }

            const string path = "Assets/Scenes/Main.unity";
            if (System.IO.File.Exists(path))
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };

            AssetDatabase.SaveAssets();
            Debug.Log("Rush Lanes project settings applied.");
        }
    }
}
