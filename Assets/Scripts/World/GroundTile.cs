using UnityEngine;

namespace RushLanes
{
    public class GroundTile : MonoBehaviour
    {
        public float EndZ { get; private set; }

        public void Build(float startZ, bool spawnItems, float difficulty, PlayerController player)
        {
            EndZ = startZ + GameIds.TileLength;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Pavement";
            ground.transform.SetParent(transform, false);
            ground.transform.localPosition = new Vector3(0f, 0f, GameIds.TileLength * 0.5f);
            ground.transform.localScale = new Vector3(GameIds.GroundWidth, 0.4f, GameIds.TileLength);
            ground.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.12f, 0.14f, 0.2f));

            for (int lane = 0; lane < 3; lane++)
            {
                var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.transform.SetParent(transform, false);
                stripe.transform.localPosition = new Vector3((lane - 1) * GameIds.LaneWidth, 0.21f, GameIds.TileLength * 0.5f);
                stripe.transform.localScale = new Vector3(0.08f, 0.02f, GameIds.TileLength * 0.92f);
                Object.Destroy(stripe.GetComponent<Collider>());
                stripe.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.85f, 0.9f, 1f, 1f), true);
            }

            SpawnRail(-GameIds.GroundWidth * 0.5f - 0.35f);
            SpawnRail(GameIds.GroundWidth * 0.5f + 0.35f);
            SpawnCity(-1, startZ);
            SpawnCity(1, startZ);

            if (!spawnItems)
                return;

            SpawnObstacle(difficulty);
            SpawnCoins(player, difficulty);
            if (Random.value < 0.12f + difficulty * 0.05f)
                SpawnPowerUp();
        }

        void SpawnRail(float x)
        {
            var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.transform.SetParent(transform, false);
            rail.transform.localPosition = new Vector3(x, 0.45f, GameIds.TileLength * 0.5f);
            rail.transform.localScale = new Vector3(0.25f, 0.9f, GameIds.TileLength);
            rail.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.2f, 0.55f, 0.9f), true);
        }

        void SpawnCity(int side, float startZ)
        {
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                float h = Random.Range(3f, 10f);
                var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                b.transform.SetParent(transform, false);
                b.transform.localPosition = new Vector3(side * Random.Range(7.5f, 12f), h * 0.5f, 2f + i * 4.2f);
                b.transform.localScale = new Vector3(Random.Range(1.6f, 3.2f), h, Random.Range(1.6f, 3.4f));
                Object.Destroy(b.GetComponent<Collider>());
                float c = Random.Range(0.08f, 0.18f);
                b.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(c, c + 0.02f, c + 0.08f));
            }
        }

        void SpawnObstacle(float difficulty)
        {
            int lane = Random.Range(0, GameIds.LaneCount);
            var kind = PickKind(difficulty);
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Obstacle";
            go.transform.SetParent(transform, false);
            go.AddComponent<ObstacleHit>();
            go.GetComponent<Collider>().isTrigger = true;

            float x = (lane - 1) * GameIds.LaneWidth;
            var mat = MaterialFactory.ColorMat(new Color(0.95f, 0.25f, 0.32f), true);

            switch (kind)
            {
                case ObstacleKind.JumpBar:
                    go.transform.localPosition = new Vector3(x, 0.45f, GameIds.TileLength * 0.62f);
                    go.transform.localScale = new Vector3(1.4f, 0.7f, 0.7f);
                    break;
                case ObstacleKind.SlideGate:
                    go.transform.localPosition = new Vector3(x, 1.55f, GameIds.TileLength * 0.62f);
                    go.transform.localScale = new Vector3(1.5f, 1.6f, 0.6f);
                    break;
                default:
                    go.transform.localPosition = new Vector3(x, 0.85f, GameIds.TileLength * 0.62f);
                    go.transform.localScale = new Vector3(1.35f, 1.5f, 1.1f);
                    break;
            }

            go.GetComponent<Renderer>().material = mat;

            if (difficulty > 0.55f && Random.value < 0.35f)
            {
                int extraLane = (lane + (Random.value < 0.5f ? 1 : 2)) % 3;
                if (extraLane != lane)
                    CloneBlock(extraLane, kind);
            }
        }

        void CloneBlock(int lane, ObstacleKind kind)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Obstacle";
            go.transform.SetParent(transform, false);
            go.AddComponent<ObstacleHit>();
            go.GetComponent<Collider>().isTrigger = true;
            go.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(0.95f, 0.4f, 0.15f), true);
            float x = (lane - 1) * GameIds.LaneWidth;
            if (kind == ObstacleKind.JumpBar)
            {
                go.transform.localPosition = new Vector3(x, 0.45f, GameIds.TileLength * 0.78f);
                go.transform.localScale = new Vector3(1.4f, 0.7f, 0.7f);
            }
            else
            {
                go.transform.localPosition = new Vector3(x, 0.85f, GameIds.TileLength * 0.78f);
                go.transform.localScale = new Vector3(1.35f, 1.5f, 1.1f);
            }
        }

        static ObstacleKind PickKind(float difficulty)
        {
            float r = Random.value;
            if (r < 0.42f) return ObstacleKind.Block;
            if (r < 0.72f) return ObstacleKind.JumpBar;
            return ObstacleKind.SlideGate;
        }

        void SpawnCoins(PlayerController player, float difficulty)
        {
            int lane = Random.Range(0, GameIds.LaneCount);
            int count = Random.Range(4, 8);
            for (int i = 0; i < count; i++)
            {
                var coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                coin.name = "Coin";
                coin.transform.SetParent(transform, false);
                coin.transform.localScale = new Vector3(0.45f, 0.06f, 0.45f);
                coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                coin.transform.localPosition = new Vector3((lane - 1) * GameIds.LaneWidth, 1.1f, 2f + i * 1.15f);
                var col = coin.GetComponent<Collider>();
                col.isTrigger = true;
                coin.GetComponent<Renderer>().material = MaterialFactory.ColorMat(new Color(1f, 0.84f, 0.2f), true);
                coin.AddComponent<CoinPickup>().Bind(player);
            }
        }

        void SpawnPowerUp()
        {
            int lane = Random.Range(0, GameIds.LaneCount);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "PowerUp";
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3((lane - 1) * GameIds.LaneWidth, 1.2f, GameIds.TileLength * 0.35f);
            go.GetComponent<Collider>().isTrigger = true;
            var kind = (PowerUpKind)Random.Range(0, 3);
            Color c = kind == PowerUpKind.Shield
                ? new Color(0.35f, 0.85f, 1f)
                : kind == PowerUpKind.Magnet
                    ? new Color(1f, 0.55f, 0.95f)
                    : new Color(0.4f, 1f, 0.45f);
            go.GetComponent<Renderer>().material = MaterialFactory.ColorMat(c, true);
            var p = go.AddComponent<PowerUpPickup>();
            p.Kind = kind;
        }
    }
}
