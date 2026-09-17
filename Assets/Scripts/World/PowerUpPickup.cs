using UnityEngine;

namespace RushLanes
{
    public class PowerUpPickup : MonoBehaviour
    {
        public PowerUpKind Kind;

        void Update()
        {
            transform.Rotate(0f, 220f * Time.deltaTime, 0f, Space.World);
            float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.08f;
            transform.localScale = Vector3.one * 0.7f * pulse;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameIds.PlayerTag))
                return;
            var player = other.GetComponent<PlayerController>() ?? other.GetComponentInParent<PlayerController>();
            player?.ApplyPower(Kind);
            Destroy(gameObject);
        }
    }
}
