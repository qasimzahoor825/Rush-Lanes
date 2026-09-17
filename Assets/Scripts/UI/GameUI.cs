using UnityEngine;
using UnityEngine.UI;

namespace RushLanes
{
    public class GameUI : MonoBehaviour
    {
        GameFlow _flow;
        RectTransform _safe;
        GameObject _splash, _menu, _tutorial, _characters, _missions, _hud, _pause, _over;

        Text _hudScore, _hudCoins, _hudSpeed, _hudMission, _overBody, _splashSub;
        PlayerController _player;
        MissionManager _missionsLogic;
        float _splashTimer;

        public void Build(GameFlow flow)
        {
            _flow = flow;
            UiFactory.AddEventSystem();
            var canvas = UiFactory.CreateCanvas(transform);
            _safe = new GameObject("SafeArea", typeof(RectTransform)).GetComponent<RectTransform>();
            _safe.SetParent(canvas.transform, false);
            UiFactory.ApplySafeArea(_safe);

            _splash = MakeSplash();
            _menu = MakeMenu();
            _tutorial = MakeTutorial();
            _characters = MakeCharacters();
            _missions = MakeMissions();
            _hud = MakeHud();
            _pause = MakePause();
            _over = MakeGameOver();
            Show(GameState.Splash);
        }

        void LateUpdate()
        {
            UiFactory.ApplySafeArea(_safe);
            if (_flow.State == GameState.Splash)
            {
                _splashTimer += Time.unscaledDeltaTime;
                if (_splashSub != null)
                    _splashSub.color = new Color(1, 1, 1, 0.55f + Mathf.Sin(Time.unscaledTime * 3f) * 0.25f);
                if (_splashTimer > 2.2f)
                    _flow.GoMenu();
            }

            if (_flow.State == GameState.Playing && _player != null)
            {
                _hudScore.text = "SCORE  " + _player.Score;
                _hudCoins.text = "COINS  " + _player.Coins;
                _hudSpeed.text = "SPEED  " + _player.Speed.ToString("0.0");
                if (_missionsLogic != null)
                    _hudMission.text = _missionsLogic.HudLine;
            }
        }

        public void Show(GameState state)
        {
            _splash.SetActive(state == GameState.Splash);
            _menu.SetActive(state == GameState.Menu);
            _tutorial.SetActive(state == GameState.Tutorial);
            _characters.SetActive(state == GameState.Characters);
            _missions.SetActive(state == GameState.Missions);
            _hud.SetActive(state == GameState.Playing);
            _pause.SetActive(state == GameState.Paused);
            _over.SetActive(state == GameState.GameOver);
            if (state == GameState.Characters)
                RebuildCharacters();
            if (state == GameState.Missions)
                RebuildMissions();
        }

        public void BindRun(PlayerController player, MissionManager missions)
        {
            _player = player;
            _missionsLogic = missions;
        }

        public void ShowGameOver(PlayerController player, bool newBest)
        {
            _overBody.text =
                $"SCORE  {player.Score}\nHIGH SCORE  {SaveSystem.Data.highScore}\nDISTANCE  {Mathf.FloorToInt(player.Distance)} m\nCOINS  {player.Coins}\nBANK  {SaveSystem.Data.totalCoins}"
                + (newBest ? "\n\nNEW BEST RUN" : "");
            Show(GameState.GameOver);
        }

        GameObject MakeSplash()
        {
            var root = UiFactory.Panel(_safe, "Splash", new Color(0.04f, 0.05f, 0.1f, 1f)).gameObject;
            var title = UiFactory.Label(root.transform, GameIds.ProductName, 92, new Color(0.3f, 0.95f, 1f));
            title.rectTransform.offsetMax = new Vector2(-40, -220);
            title.rectTransform.offsetMin = new Vector2(40, 220);
            _splashSub = UiFactory.Label(root.transform, "ENDLESS NIGHT CIRCUIT", 32, Color.white);
            _splashSub.rectTransform.anchorMin = new Vector2(0.1f, 0.28f);
            _splashSub.rectTransform.anchorMax = new Vector2(0.9f, 0.38f);
            return root;
        }

        GameObject MakeMenu()
        {
            var root = UiFactory.Panel(_safe, "Menu", new Color(0.05f, 0.06f, 0.12f, 0.96f)).gameObject;
            var title = UiFactory.Label(root.transform, GameIds.ProductName, 76, new Color(0.35f, 0.95f, 1f));
            title.rectTransform.anchorMin = new Vector2(0.05f, 0.72f);
            title.rectTransform.anchorMax = new Vector2(0.95f, 0.92f);

            var stats = UiFactory.Label(root.transform, "", 28, new Color(0.85f, 0.9f, 1f));
            stats.rectTransform.anchorMin = new Vector2(0.08f, 0.62f);
            stats.rectTransform.anchorMax = new Vector2(0.92f, 0.72f);
            stats.gameObject.AddComponent<MenuStatsBinder>();

            StackButtons(root.transform, new (string, System.Action)[]
            {
                ("PLAY", () => _flow.StartRun()),
                ("HOW TO PLAY", () => _flow.SetState(GameState.Tutorial)),
                ("CHARACTERS", () => _flow.SetState(GameState.Characters)),
                ("MISSIONS", () => _flow.SetState(GameState.Missions)),
                ("SOUND  ON/OFF", ToggleSound),
                ("QUIT", Application.Quit)
            }, 0.08f, 0.58f);
            return root;
        }

        GameObject MakeTutorial()
        {
            var root = UiFactory.Panel(_safe, "Tutorial", new Color(0.05f, 0.06f, 0.12f, 0.97f)).gameObject;
            var body = UiFactory.Label(root.transform,
                "SWIPE LEFT / RIGHT  change lane\nSWIPE UP or TAP SPACE  jump\nSWIPE DOWN  slide\n\nRED BLOCK  switch lanes\nLOW BAR  jump\nHIGH GATE  slide\nGOLD COINS  score + bank\nCYAN / PINK / GREEN orbs  power-ups\n\nShield absorbs one hit. Magnet pulls coins. Boost raises speed.",
                32, Color.white, TextAnchor.MiddleLeft);
            body.rectTransform.offsetMin = new Vector2(70, 160);
            body.rectTransform.offsetMax = new Vector2(-70, -80);
            PlaceButton(root.transform, "GOT IT", new Vector2(0.5f, 0.1f), () => _flow.FinishTutorial());
            return root;
        }

        GameObject MakeCharacters()
        {
            var root = UiFactory.Panel(_safe, "Characters", new Color(0.05f, 0.06f, 0.12f, 0.97f)).gameObject;
            UiFactory.Label(root.transform, "SELECT RUNNER", 48, new Color(0.35f, 0.95f, 1f)).rectTransform.anchorMin = new Vector2(0, 0.82f);
            return root;
        }

        GameObject MakeMissions()
        {
            var root = UiFactory.Panel(_safe, "Missions", new Color(0.05f, 0.06f, 0.12f, 0.97f)).gameObject;
            UiFactory.Label(root.transform, "MISSIONS", 48, new Color(0.35f, 0.95f, 1f)).rectTransform.anchorMin = new Vector2(0, 0.82f);
            return root;
        }

        GameObject MakeHud()
        {
            var root = new GameObject("HUD", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(_safe, false);
            UiFactory.Stretch(root);

            _hudScore = Chip(root, "SCORE  0", new Vector2(0.02f, 0.86f), new Vector2(0.38f, 0.97f));
            _hudCoins = Chip(root, "COINS  0", new Vector2(0.40f, 0.86f), new Vector2(0.68f, 0.97f));
            _hudSpeed = Chip(root, "SPEED  0", new Vector2(0.70f, 0.86f), new Vector2(0.86f, 0.97f));
            _hudMission = Chip(root, "", new Vector2(0.02f, 0.76f), new Vector2(0.62f, 0.85f));

            var pause = UiFactory.Button(root, "II", new Vector2(110, 90), new Color(0.1f, 0.12f, 0.2f, 0.85f), () => _flow.Pause());
            var prt = pause.GetComponent<RectTransform>();
            prt.anchorMin = new Vector2(0.9f, 0.86f);
            prt.anchorMax = new Vector2(0.98f, 0.97f);
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;
            return root.gameObject;
        }

        GameObject MakePause()
        {
            var root = UiFactory.Panel(_safe, "Pause", new Color(0.02f, 0.03f, 0.08f, 0.82f)).gameObject;
            UiFactory.Label(root.transform, "PAUSED", 64, Color.white).rectTransform.anchorMin = new Vector2(0, 0.6f);
            StackButtons(root.transform, new (string, System.Action)[]
            {
                ("RESUME", () => _flow.Resume()),
                ("RESTART", () => _flow.StartRun()),
                ("MENU", () => _flow.GoMenu())
            }, 0.22f, 0.55f);
            return root;
        }

        GameObject MakeGameOver()
        {
            var root = UiFactory.Panel(_safe, "GameOver", new Color(0.06f, 0.04f, 0.08f, 0.94f)).gameObject;
            var title = UiFactory.Label(root.transform, "GAME OVER", 72, new Color(1f, 0.35f, 0.4f));
            title.rectTransform.anchorMin = new Vector2(0.05f, 0.72f);
            title.rectTransform.anchorMax = new Vector2(0.95f, 0.9f);
            _overBody = UiFactory.Label(root.transform, "", 34, Color.white);
            _overBody.rectTransform.anchorMin = new Vector2(0.1f, 0.38f);
            _overBody.rectTransform.anchorMax = new Vector2(0.9f, 0.7f);
            StackButtons(root.transform, new (string, System.Action)[]
            {
                ("RESTART", () => _flow.StartRun()),
                ("MENU", () => _flow.GoMenu())
            }, 0.1f, 0.32f);
            return root;
        }

        void RebuildCharacters()
        {
            for (int i = _characters.transform.childCount - 1; i >= 1; i--)
                Destroy(_characters.transform.GetChild(i).gameObject);

            for (int i = 0; i < CharacterCatalog.All.Length; i++)
            {
                int index = i;
                var def = CharacterCatalog.All[i];
                bool unlocked = SaveSystem.Data.unlocked[i];
                bool selected = SaveSystem.Data.selectedCharacter == i;
                string label = unlocked
                    ? (selected ? def.Name + "  EQUIPPED" : "EQUIP  " + def.Name)
                    : $"UNLOCK  {def.Name}  ({def.Cost})";
                var btn = UiFactory.Button(_characters.transform, label + "\n" + def.Blurb, new Vector2(820, 110),
                    selected ? new Color(0.08f, 0.45f, 0.5f) : new Color(0.13f, 0.16f, 0.24f),
                    () =>
                    {
                        AudioManager.Instance.Click();
                        if (!unlocked)
                        {
                            if (!CharacterCatalog.Unlock(index))
                                return;
                        }
                        else
                        {
                            SaveSystem.Data.selectedCharacter = index;
                            SaveSystem.Save();
                        }
                        RebuildCharacters();
                    });
                var rt = btn.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.62f - i * 0.16f);
                rt.anchorMax = new Vector2(0.5f, 0.62f - i * 0.16f);
                rt.anchoredPosition = Vector2.zero;
            }

            PlaceButton(_characters.transform, "BACK", new Vector2(0.5f, 0.1f), () => _flow.GoMenu());
        }

        void RebuildMissions()
        {
            for (int i = _missions.transform.childCount - 1; i >= 1; i--)
                Destroy(_missions.transform.GetChild(i).gameObject);

            for (int i = 0; i < MissionManager.Definitions.Length; i++)
            {
                var d = MissionManager.Definitions[i];
                bool done = SaveSystem.Data.missionComplete[i];
                int prog = SaveSystem.Data.missionProgress[i];
                var box = UiFactory.Box(_missions.transform, d.Title, new Vector2(900, 120),
                    done ? new Color(0.08f, 0.35f, 0.22f) : new Color(0.13f, 0.16f, 0.24f));
                UiFactory.Label(box, $"{d.Title}\n{d.Hint}\n{Mathf.Min(prog, d.Target)}/{d.Target}" + (done ? "  DONE" : ""), 28, Color.white);
                var rt = box;
                rt.anchorMin = new Vector2(0.5f, 0.64f - i * 0.18f);
                rt.anchorMax = new Vector2(0.5f, 0.64f - i * 0.18f);
            }

            PlaceButton(_missions.transform, "BACK", new Vector2(0.5f, 0.1f), () => _flow.GoMenu());
        }

        void ToggleSound()
        {
            SaveSystem.Data.soundOn = !SaveSystem.Data.soundOn;
            SaveSystem.Save();
            AudioManager.Instance.PlayMusic(true);
            AudioManager.Instance.Click();
        }

        static Text Chip(RectTransform parent, string text, Vector2 min, Vector2 max)
        {
            var box = UiFactory.Box(parent, text, Vector2.zero, new Color(0.05f, 0.07f, 0.12f, 0.72f));
            box.anchorMin = min;
            box.anchorMax = max;
            box.offsetMin = Vector2.zero;
            box.offsetMax = Vector2.zero;
            return UiFactory.Label(box, text, 26, Color.white);
        }

        void StackButtons(Transform parent, (string, System.Action)[] items, float bottom, float top)
        {
            float step = (top - bottom) / items.Length;
            for (int i = 0; i < items.Length; i++)
            {
                int index = i;
                var btn = UiFactory.Button(parent, items[i].Item1, new Vector2(640, 88), new Color(0.12f, 0.42f, 0.55f), () =>
                {
                    AudioManager.Instance.Click();
                    items[index].Item2();
                });
                var rt = btn.GetComponent<RectTransform>();
                float y = top - step * (i + 0.5f);
                rt.anchorMin = new Vector2(0.5f, y);
                rt.anchorMax = new Vector2(0.5f, y);
                rt.anchoredPosition = Vector2.zero;
            }
        }

        void PlaceButton(Transform parent, string caption, Vector2 anchor, System.Action action)
        {
            var btn = UiFactory.Button(parent, caption, new Vector2(420, 90), new Color(0.12f, 0.42f, 0.55f), () =>
            {
                AudioManager.Instance.Click();
                action();
            });
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.anchoredPosition = Vector2.zero;
        }
    }

    public class MenuStatsBinder : MonoBehaviour
    {
        Text _text;
        void Awake() => _text = GetComponent<Text>();
        void OnEnable()
        {
            if (_text != null)
                _text.text = $"BEST  {SaveSystem.Data.highScore}     BANK  {SaveSystem.Data.totalCoins}     RUNNER  {CharacterCatalog.Selected.Name}";
        }
    }
}
