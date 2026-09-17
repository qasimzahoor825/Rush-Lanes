using UnityEngine;

namespace RushLanes
{
    public class GameFlow : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Splash;

        GameUI _ui;
        Camera _cam;
        CameraRig _rig;
        Transform _worldRoot;
        PlayerController _player;
        MissionManager _missions;
        TrackSpawner _track;

        bool _playAfterTutorial;

        public void Init(GameUI ui, Camera cam)
        {
            _ui = ui;
            _cam = cam;
            _rig = cam.gameObject.GetComponent<CameraRig>() ?? cam.gameObject.AddComponent<CameraRig>();
            SetState(GameState.Splash);
            AudioManager.Instance.PlayMusic(true);
        }

        public void SetState(GameState state)
        {
            State = state;
            Time.timeScale = state == GameState.Paused ? 0f : 1f;
            _ui.Show(state);
        }

        public void GoMenu()
        {
            ClearRun();
            if (GameObject.Find("MenuPreview") == null)
            {
                var preview = new GameObject("MenuPreview");
                preview.AddComponent<GroundTile>().Build(-GameIds.TileLength, false, 0f, null);
            }
            SetState(GameState.Menu);
            AudioManager.Instance.PlayMusic(true);
            FrameMenuCamera();
        }

        public void StartRun()
        {
            if (!SaveSystem.Data.tutorialSeen)
            {
                _playAfterTutorial = true;
                SetState(GameState.Tutorial);
                return;
            }

            BeginRunInternal();
        }

        public void FinishTutorial()
        {
            SaveSystem.Data.tutorialSeen = true;
            SaveSystem.Save();
            if (_playAfterTutorial)
            {
                _playAfterTutorial = false;
                BeginRunInternal();
                return;
            }

            GoMenu();
        }

        void BeginRunInternal()
        {
            ClearRun();
            var preview = GameObject.Find("MenuPreview");
            if (preview != null)
                Object.Destroy(preview);

            Time.timeScale = 1f;
            _worldRoot = new GameObject("WorldRoot").transform;

            var playerGo = new GameObject(GameIds.PlayerObjectName, typeof(Rigidbody), typeof(PlayerController));
            playerGo.transform.SetParent(_worldRoot, false);
            _player = playerGo.GetComponent<PlayerController>();
            _player.Setup(this);

            _track = _worldRoot.gameObject.AddComponent<TrackSpawner>();
            _track.Begin(_player);

            _missions = _worldRoot.gameObject.AddComponent<MissionManager>();
            _missions.BeginRun();

            _cam.transform.position = new Vector3(0f, 6.2f, -9.5f);
            _rig.Follow(_player.transform);
            _ui.BindRun(_player, _missions);
            SetState(GameState.Playing);
            AudioManager.Instance.PlayMusic(true);
        }

        public void Pause()
        {
            if (State == GameState.Playing)
                SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State == GameState.Paused)
                SetState(GameState.Playing);
        }

        public void EndRun(PlayerController player)
        {
            int previousBest = SaveSystem.Data.highScore;
            SaveSystem.SubmitRun(player.Score, Mathf.FloorToInt(player.Distance), player.Coins);
            _missions?.Track(player.Coins, player.Distance, 0f);
            SetState(GameState.GameOver);
            _ui.ShowGameOver(player, player.Score > previousBest);
        }

        void Update()
        {
            if (State == GameState.Playing && _player != null && _player.Alive)
                _missions.Track(_player.Coins, _player.Distance, Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (State == GameState.Playing) Pause();
                else if (State == GameState.Paused) Resume();
                else if (State != GameState.Splash) GoMenu();
            }
        }

        void ClearRun()
        {
            _player = null;
            _track = null;
            _missions = null;
            _rig.Follow(null);
            if (_worldRoot != null)
                Destroy(_worldRoot.gameObject);
        }

        void FrameMenuCamera()
        {
            _cam.transform.position = new Vector3(0f, 8f, -14f);
            _cam.transform.rotation = Quaternion.Euler(18f, 0f, 0f);
        }
    }
}
