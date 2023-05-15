using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Levels;

namespace UI
{
    public class UIManager : MonoBehaviour
    {

        [Header("UI Documents:")]
        [SerializeField] private UIDocument deathScreenUI;
        [SerializeField] private UIDocument winScreenUI;
        [SerializeField] private UIDocument gameplayUI;

        private bool hide = false;
        private bool refresh = false;
        private float time = 0f;
        private float? totalMana = null;
        private int octobearsOnStart;
        private int manaOnStart;
        private int octobearsCollected = 0;
        private int manasCollected = 0;

        void Awake()
        {
            SubscribeEvents();

            VisualElement deathUIRootElement = deathScreenUI.rootVisualElement;
            deathUIRootElement.Q<Button>("RestartBtn").clicked += OnRestartClicked;
            deathUIRootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
            deathUIRootElement.style.visibility = Visibility.Hidden;

            VisualElement winUIRootElement = winScreenUI.rootVisualElement;
            winUIRootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
            winUIRootElement.Q<Button>("RepeatBtn").clicked += OnRestartClicked;
            winUIRootElement.Q<Button>("NextBtn").clicked += OnNextClicked;
            winUIRootElement.style.visibility = Visibility.Hidden;

        }

        void Start()
        {
            StartOrRefreshUI();
        }

        void Update()
        {
            if (hide)
            {
                VisualElement deathUIRootElement = deathScreenUI.rootVisualElement;
                deathUIRootElement.style.visibility = Visibility.Hidden;

                VisualElement winUIRootElement = winScreenUI.rootVisualElement;
                winUIRootElement.style.visibility = Visibility.Hidden;

                hide = false;
            }

            if (refresh)
            {
                StartOrRefreshUI();
                time = 0f;
            }

            UpdateAvailableSpellsUI();

            if (winScreenUI.rootVisualElement.style.visibility == Visibility.Hidden)
            {
                time += Time.deltaTime;
            }

            VisualElement gameplayUIRootElement = gameplayUI.rootVisualElement;
            gameplayUIRootElement.Q<Label>("Timer").text = $"{Mathf.FloorToInt(time)} s";

        }

        private void UpdateCollectableCount(string collectable)
        {
            VisualElement gameplayUIRootElement = gameplayUI.rootVisualElement;

            switch (collectable)
            {
                case "mana":
                    manasCollected++;
                    break;
                case "octobear":
                    octobearsCollected++;
                    break;
                default:
                    break;
            }

            var octobears = FindObjectsOfType<OctobearTrophyCollectable>().Length;
            octobearsOnStart = octobears;
            gameplayUIRootElement.Q<Label>("OctobearsCollected").text = $" {octobearsCollected}/{octobears}";

            var manas = FindObjectsOfType<ManaCollectable>().Length;
            manaOnStart = manas;
            gameplayUIRootElement.Q<Label>("ManaCollected").text = $" {manasCollected}/{manas}";
        }

        private void UpdateManaBarUI(float mana)
        {
            float percentage = (mana * 100) / (float)totalMana;

            VisualElement gameplayUIRootElement = gameplayUI.rootVisualElement;

            gameplayUIRootElement.Q<VisualElement>("ManaBar").style.width = new StyleLength(new Length(percentage, LengthUnit.Percent));
        }

        private void UpdateAvailableSpellsUI()
        {
            var frog = FindObjectOfType<Frog>();

            if (frog != null)
            {
                VisualElement gameplayUIRootElement = gameplayUI.rootVisualElement;

                var rotateSpellUI = gameplayUIRootElement.Q<VisualElement>("RotateSpell");
                var timeSpellUI = gameplayUIRootElement.Q<VisualElement>("TimeSpell");
                var invisibleSpellUI = gameplayUIRootElement.Q<VisualElement>("InvisibleSpell");


                rotateSpellUI.style.visibility = Visibility.Hidden;
                timeSpellUI.style.visibility = Visibility.Hidden;
                invisibleSpellUI.style.visibility = Visibility.Hidden;

                if (frog.Mana >= 5 && frog.Mana < 10)
                {
                    rotateSpellUI.style.visibility = Visibility.Visible;
                }
                else if (frog.Mana >= 10 && frog.Mana < 20)
                {
                    rotateSpellUI.style.visibility = Visibility.Visible;
                    timeSpellUI.style.visibility = Visibility.Visible;
                }
                else if (frog.Mana >= 20)
                {
                    rotateSpellUI.style.visibility = Visibility.Visible;
                    timeSpellUI.style.visibility = Visibility.Visible;
                    invisibleSpellUI.style.visibility = Visibility.Visible;
                }
            }
        }

        private void StartOrRefreshUI()
        {
            VisualElement gameplayUIRootElement = gameplayUI.rootVisualElement;

            var octobears = FindObjectsOfType<OctobearTrophyCollectable>().Length;
            gameplayUIRootElement.Q<Label>("OctobearsCollected").text = $" 0/{octobears}";

            var manas = FindObjectsOfType<ManaCollectable>().Length;
            gameplayUIRootElement.Q<Label>("ManaCollected").text = $" 0/{manas}";

            octobearsCollected = 0;
            manasCollected = 0;

            if (totalMana == null)
            {
                var frog = FindObjectOfType<Frog>();
                if (frog != null)
                {
                    totalMana = frog.MaxMana;
                    UpdateManaBarUI(frog.Mana);
                }
            }

            refresh = false;
        }

        private void ShowWinUI()
        {
            winScreenUI.rootVisualElement.style.visibility = Visibility.Visible;

            int stars = 0;

            LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayerAlreadyCompleted = true;

            if (LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersBestTime == 0)
                LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersBestTime = Mathf.CeilToInt(time);
            else if (LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersBestTime > time)
            {

                LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersBestTime = Mathf.CeilToInt(time);
                stars++;
            }

            var octobears = FindObjectsOfType<OctobearTrophyCollectable>().Length;

            if (manasCollected == manaOnStart) stars++;
            if (octobearsCollected == octobearsOnStart) stars++;

            if (LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersManaCollected < manasCollected)
                LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersManaCollected = manasCollected;

            if (LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersOctobearsCollected < octobearsCollected)
                LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersOctobearsCollected = octobearsCollected;

            if (LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersStars < stars)
                LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersStars = stars;
        }

        private void ShowDefeatUI(string deathMessage)
        {
            VisualElement deathUIRootElement = deathScreenUI.rootVisualElement;
            deathUIRootElement.Q<Label>("DefeatMessage").text = deathMessage;
            deathScreenUI.rootVisualElement.style.visibility = Visibility.Visible;
        }

        private void OnRestartClicked()
        {
            UnsubscribeEvents();
            ProgressEventManager.EmitSaveData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnHomeClicked()
        {
            UnsubscribeEvents();
            ProgressEventManager.EmitSaveData();
            ProgressEventManager.RefreshData();
            SceneManager.LoadScene("Menu");
        }

        private void OnNextClicked()
        {
            UnsubscribeEvents();
            ProgressEventManager.EmitSaveData();
            LevelsInfo.CurrentLevel++;
            SceneManager.LoadScene(LevelsInfo.CurrentLevel.ToString());
        }

        private void SubscribeEvents()
        {
            UIEventManager.ShowDefeatUI += ShowDefeatUI;
            UIEventManager.ShowWinUI += ShowWinUI;
            UIEventManager.UpdateManaBarUI += UpdateManaBarUI;
            UIEventManager.GotCollectableToUI += UpdateCollectableCount;
        }

        private void UnsubscribeEvents()
        {
            UIEventManager.ShowDefeatUI -= ShowDefeatUI;
            UIEventManager.ShowWinUI -= ShowWinUI;
            UIEventManager.UpdateManaBarUI -= UpdateManaBarUI;
            UIEventManager.GotCollectableToUI -= UpdateCollectableCount;
        }
    }
}

