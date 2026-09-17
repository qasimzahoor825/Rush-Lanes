using System.Collections.Generic;
using UnityEngine;

namespace RushLanes
{
    public class TrackSpawner : MonoBehaviour
    {
        readonly List<GroundTile> _tiles = new List<GroundTile>();
        float _nextZ;
        PlayerController _player;
        int _spawned;

        public void Begin(PlayerController player)
        {
            _player = player;
            _nextZ = -GameIds.TileLength;
            _spawned = 0;
            for (int i = 0; i < 10; i++)
                Spawn();
        }

        void Update()
        {
            if (_player == null || !_player.Alive)
                return;

            while (_nextZ < _player.transform.position.z + GameIds.TileLength * 8f)
                Spawn();

            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                if (_tiles[i].EndZ < _player.transform.position.z - GameIds.TileLength * 2f)
                {
                    Destroy(_tiles[i].gameObject);
                    _tiles.RemoveAt(i);
                }
            }
        }

        void Spawn()
        {
            var go = new GameObject("Tile_" + _spawned);
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(0f, 0f, _nextZ);
            var tile = go.AddComponent<GroundTile>();
            float difficulty = Mathf.Clamp01(_spawned / 40f);
            bool items = _spawned >= 2;
            tile.Build(_nextZ, items, difficulty, _player);
            _tiles.Add(tile);
            _nextZ += GameIds.TileLength;
            _spawned++;
        }
    }
}
