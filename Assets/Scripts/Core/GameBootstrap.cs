using UnityEngine;

namespace RushLanes
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (Object.FindAnyObjectByType<GameFlow>() != null)
                return;

            SaveSystem.Load();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            var root = new GameObject("RushLanes");
            Object.DontDestroyOnLoad(root);

            var audio = root.AddComponent<AudioManager>();
            audio.Init();

            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                cam.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.03f, 0.04f, 0.08f);
            cam.nearClipPlane = 0.2f;
            cam.farClipPlane = 180f;
            cam.fieldOfView = 60f;
            if (cam.GetComponent<AudioListener>() == null)
                cam.gameObject.AddComponent<AudioListener>();
            if (cam.GetComponent<CameraRig>() == null)
                cam.gameObject.AddComponent<CameraRig>();

            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.05f, 0.07f, 0.12f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.ambientLight = new Color(0.18f, 0.22f, 0.32f);

            if (Object.FindAnyObjectByType<Light>() == null)
            {
                var lightGo = new GameObject("Sun");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = new Color(0.75f, 0.85f, 1f);
                light.intensity = 1.15f;
                lightGo.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
            }

            var ui = root.AddComponent<GameUI>();
            var flow = root.AddComponent<GameFlow>();
            ui.Build(flow);
            flow.Init(ui, cam);

            SpawnMenuWorld();
        }

        static void SpawnMenuWorld()
        {
            if (Object.FindAnyObjectByType<TrackSpawner>() != null)
                return;
            var preview = new GameObject("MenuPreview");
            var tile = preview.AddComponent<GroundTile>();
            tile.Build(-GameIds.TileLength, false, 0f, null);
        }
    }
}
