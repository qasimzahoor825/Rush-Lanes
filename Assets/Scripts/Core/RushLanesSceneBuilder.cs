using UnityEngine;

namespace RushLanes
{
    [ExecuteAlways]
    public class RushLanesSceneBuilder : MonoBehaviour
    {
        void Reset()
        {
            Build();
        }

        void Awake()
        {
            Build();
        }

        void Update()
        {
            if (!Application.isPlaying)
                Build();
        }

        void Build()
        {
            if (GameObject.Find("RushLanesRoot") == null)
            {
                var root = new GameObject("RushLanesRoot");
                root.transform.position = Vector3.zero;
            }

            var rootObj = GameObject.Find("RushLanesRoot");
            if (rootObj == null)
                return;

            if (rootObj.transform.Find("Ground") == null)
            {
                var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ground.name = "Ground";
                ground.transform.SetParent(rootObj.transform, false);
                ground.transform.localPosition = new Vector3(0f, 0f, 0f);
                ground.transform.localScale = new Vector3(12f, 0.6f, 30f);
                ground.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.2f, 0.22f, 0.28f));
                Object.DestroyImmediate(ground.GetComponent<BoxCollider>());
                ground.AddComponent<BoxCollider>();
            }

            if (rootObj.transform.Find("Player") == null)
            {
                var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                player.name = "Player";
                player.transform.SetParent(rootObj.transform, false);
                player.transform.localPosition = new Vector3(0f, 1.1f, 0f);
                player.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
                player.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.15f, 0.8f, 1f), true);
                player.tag = GameIds.PlayerTag;
                Object.DestroyImmediate(player.GetComponent<Collider>());
                player.AddComponent<CapsuleCollider>();
                player.AddComponent<Rigidbody>();
                var rb = player.GetComponent<Rigidbody>();
                rb.useGravity = true;
                rb.mass = 1f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            if (rootObj.transform.Find("LaneMarkerA") == null)
            {
                CreateLaneMarker(rootObj.transform, "LaneMarkerA", -2.4f);
                CreateLaneMarker(rootObj.transform, "LaneMarkerB", 0f);
                CreateLaneMarker(rootObj.transform, "LaneMarkerC", 2.4f);
            }

            var mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.SetParent(rootObj.transform, false);
                mainCam.transform.localPosition = new Vector3(0f, 6.2f, -9.5f);
                mainCam.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = new Color(0.8f, 0.86f, 0.92f);
            }

            if (GameObject.Find("Directional Light") == null)
            {
                var lightGo = new GameObject("Directional Light");
                lightGo.transform.SetParent(rootObj.transform, false);
                lightGo.transform.rotation = Quaternion.Euler(35f, -40f, 0f);
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.1f;
                light.color = new Color(1f, 0.98f, 0.9f);
            }
        }

        static void CreateLaneMarker(Transform parent, string name, float x)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = name;
            marker.transform.SetParent(parent, false);
            marker.transform.localPosition = new Vector3(x, 0.31f, 0f);
            marker.transform.localScale = new Vector3(0.1f, 0.1f, 28f);
            marker.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.7f, 0.75f, 0.85f));
            Object.DestroyImmediate(marker.GetComponent<BoxCollider>());
        }
    }
}
