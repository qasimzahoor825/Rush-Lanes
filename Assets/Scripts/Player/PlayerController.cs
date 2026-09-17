using UnityEngine;

namespace RushLanes
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public bool Alive { get; private set; } = true;
        public int Coins { get; private set; }
        public int Score { get; private set; }
        public float Distance { get; private set; }
        public float Speed { get; private set; } = GameIds.BaseSpeed;
        public bool HasShield => _shieldTime > 0f;
        public bool HasMagnet => _magnetTime > 0f;

        int _lane = 1;
        float _yVelocity;
        float _slideTime;
        float _shieldTime;
        float _magnetTime;
        float _boostTime;
        bool _grounded = true;
        CapsuleCollider _col;
        Rigidbody _rb;
        PlayerVisual _visual;
        GameFlow _flow;

        public void Setup(GameFlow flow)
        {
            _flow = flow;
            gameObject.name = GameIds.PlayerObjectName;
            try { gameObject.tag = GameIds.PlayerTag; } catch { /* Player tag missing */ }

            _col = gameObject.AddComponent<CapsuleCollider>();
            _col.center = Vector3.up * 0.9f;
            _col.radius = 0.32f;
            _col.height = 1.8f;

            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.useGravity = false;

            _visual = gameObject.AddComponent<PlayerVisual>();
            _visual.Build(CharacterCatalog.Selected);

            var swipe = gameObject.AddComponent<SwipeInput>();
            swipe.OnLeft = () => TryLane(-1);
            swipe.OnRight = () => TryLane(1);
            swipe.OnUp = Jump;
            swipe.OnDown = Slide;

            Speed = GameIds.BaseSpeed;
            transform.position = new Vector3(0f, GameIds.PlayerHeight, 0f);
        }

        void Update()
        {
            if (!Alive || _flow.State != GameState.Playing)
                return;

            Speed = Mathf.Min(GameIds.MaxSpeed, Speed + GameIds.SpeedGainPerSecond * Time.deltaTime * (_boostTime > 0f ? 1.6f : 1f));
            Distance += Speed * Time.deltaTime;
            Score = Mathf.FloorToInt(Distance / GameIds.DistanceScoreDivisor) + Coins * GameIds.CoinScore;

            if (_slideTime > 0f)
                _slideTime -= Time.deltaTime;
            if (_shieldTime > 0f)
                _shieldTime -= Time.deltaTime;
            if (_magnetTime > 0f)
                _magnetTime -= Time.deltaTime;
            if (_boostTime > 0f)
                _boostTime -= Time.deltaTime;

            bool sliding = _slideTime > 0f;
            _col.height = sliding ? 0.9f : 1.8f;
            _col.center = Vector3.up * (sliding ? 0.45f : 0.9f);
            _visual.Tick(sliding, !_grounded, _grounded, HasShield);
        }

        void FixedUpdate()
        {
            if (!Alive || _flow.State != GameState.Playing)
                return;

            float targetX = (_lane - 1) * GameIds.LaneWidth;
            Vector3 pos = _rb.position;
            pos.x = Mathf.Lerp(pos.x, targetX, 14f * Time.fixedDeltaTime);
            pos.z += Speed * Time.fixedDeltaTime;

            _yVelocity -= GameIds.Gravity * Time.fixedDeltaTime;
            pos.y += _yVelocity * Time.fixedDeltaTime;
            if (pos.y <= GameIds.PlayerHeight)
            {
                pos.y = GameIds.PlayerHeight;
                _yVelocity = 0f;
                _grounded = true;
            }

            _rb.MovePosition(pos);

            if (pos.y < -4f)
                Die();
        }

        void TryLane(int dir)
        {
            if (!Alive || _flow.State != GameState.Playing)
                return;
            int next = Mathf.Clamp(_lane + dir, 0, GameIds.LaneCount - 1);
            if (next == _lane)
                return;
            _lane = next;
            AudioManager.Instance.Whoosh();
        }

        void Jump()
        {
            if (!Alive || _flow.State != GameState.Playing || !_grounded)
                return;
            _grounded = false;
            _slideTime = 0f;
            _yVelocity = GameIds.JumpVelocity;
            AudioManager.Instance.Jump();
        }

        void Slide()
        {
            if (!Alive || _flow.State != GameState.Playing)
                return;
            _slideTime = GameIds.SlideDuration;
            if (!_grounded)
                _yVelocity = Mathf.Min(_yVelocity, -6f);
            AudioManager.Instance.Slide();
        }

        public void CollectCoin()
        {
            if (!Alive)
                return;
            Coins++;
            AudioManager.Instance.Coin();
        }

        public void ApplyPower(PowerUpKind kind)
        {
            if (!Alive)
                return;
            AudioManager.Instance.Power();
            switch (kind)
            {
                case PowerUpKind.Shield:
                    _shieldTime = 8f;
                    break;
                case PowerUpKind.Magnet:
                    _magnetTime = 8f;
                    break;
                case PowerUpKind.Boost:
                    _boostTime = 4f;
                    Speed = Mathf.Min(GameIds.MaxSpeed, Speed + 3f);
                    break;
            }
        }

        public void Hit()
        {
            if (!Alive)
                return;
            if (HasShield)
            {
                _shieldTime = 0f;
                AudioManager.Instance.Whoosh();
                return;
            }

            Die();
        }

        void Die()
        {
            if (!Alive)
                return;
            Alive = false;
            AudioManager.Instance.Hit();
            AudioManager.Instance.GameOver();
            _flow.EndRun(this);
        }
    }
}
