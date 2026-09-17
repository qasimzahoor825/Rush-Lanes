using UnityEngine;

namespace RushLanes
{
    public class CoinPickup : MonoBehaviour
    {
        PlayerController _player;

        public void Bind(PlayerController player)
        {
            _player = player;
        }

        void Update()
        {
            transform.Rotate(0f, 180f * Time.deltaTime, 0f, Space.World);
            if (_player == null || !_player.Alive)
                return;

            if (_player.HasMagnet && Vector3.Distance(transform.position, _player.transform.position) < 6.5f)
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position + Vector3.up, 28f * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameIds.PlayerTag))
                return;
            var player = other.GetComponent<PlayerController>() ?? _player;
            player?.CollectCoin();
            Destroy(gameObject);
        }
    }
}
