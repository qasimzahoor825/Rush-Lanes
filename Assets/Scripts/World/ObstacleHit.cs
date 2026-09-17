using UnityEngine;

namespace RushLanes
{
    public class ObstacleHit : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            Handle(collision.collider);
        }

        void OnTriggerEnter(Collider other)
        {
            Handle(other);
        }

        static void Handle(Collider other)
        {
            if (!other.CompareTag(GameIds.PlayerTag))
                return;
            var player = other.GetComponent<PlayerController>() ?? other.GetComponentInParent<PlayerController>();
            player?.Hit();
        }
    }
}
