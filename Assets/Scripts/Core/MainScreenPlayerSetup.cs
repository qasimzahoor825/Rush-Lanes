using UnityEngine;

namespace RushLanes
{
    public static class MainScreenPlayerSetup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Setup()
        {
            if (GameObject.Find("MainScreenGround") == null)
            {
                var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "MainScreenGround";
                ground.transform.position = new Vector3(0f, 0f, 0f);
                ground.transform.localScale = new Vector3(6f, 1f, 8f);
                ground.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.38f, 0.38f, 0.4f));
            }

            if (GameObject.Find("MainScreenPlayer") == null)
            {
                var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                player.name = "MainScreenPlayer";
                player.transform.position = new Vector3(0f, 1f, 0f);
                player.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
                player.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.14f, 0.75f, 1f), true);
                player.tag = GameIds.PlayerTag;
                Object.Destroy(player.GetComponent<Collider>());
                player.AddComponent<CapsuleCollider>();
            }

            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0f, 4.5f, -8.5f);
                cam.transform.rotation = Quaternion.Euler(18f, 0f, 0f);
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.82f, 0.88f, 0.95f);
            }
        }
    }
}
