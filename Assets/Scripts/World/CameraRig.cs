using UnityEngine;

namespace RushLanes
{
    public class CameraRig : MonoBehaviour
    {
        Transform _target;
        Vector3 _offset = new Vector3(0f, 6.2f, -9.5f);

        public void Follow(Transform target)
        {
            _target = target;
        }

        void LateUpdate()
        {
            if (_target == null)
                return;
            Vector3 want = _target.position + _offset;
            want.x = 0f;
            transform.position = Vector3.Lerp(transform.position, want, Time.deltaTime * 8f);
            transform.LookAt(_target.position + Vector3.up * 1.2f + Vector3.forward * 4f);
        }
    }
}
