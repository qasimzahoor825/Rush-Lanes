using UnityEngine;
using UnityEngine.EventSystems;

namespace RushLanes
{
    public class SwipeInput : MonoBehaviour
    {
        public System.Action OnLeft;
        public System.Action OnRight;
        public System.Action OnUp;
        public System.Action OnDown;

        Vector2 _start;
        bool _held;
        const float Min = 70f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                OnLeft?.Invoke();
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                OnRight?.Invoke();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
                OnUp?.Invoke();
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                OnDown?.Invoke();

            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    if (OverUi(t.fingerId))
                        return;
                    _held = true;
                    _start = t.position;
                }
                else if ((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) && _held)
                {
                    _held = false;
                    Resolve(t.position - _start);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (OverUi(-1))
                    return;
                _held = true;
                _start = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0) && _held)
            {
                _held = false;
                Resolve((Vector2)Input.mousePosition - _start);
            }
        }

        static bool OverUi(int fingerId)
        {
            if (EventSystem.current == null)
                return false;
            return fingerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(fingerId)
                : EventSystem.current.IsPointerOverGameObject();
        }

        void Resolve(Vector2 delta)
        {
            if (delta.magnitude < Min)
                return;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0) OnRight?.Invoke();
                else OnLeft?.Invoke();
            }
            else
            {
                if (delta.y > 0) OnUp?.Invoke();
                else OnDown?.Invoke();
            }
        }
    }
}
