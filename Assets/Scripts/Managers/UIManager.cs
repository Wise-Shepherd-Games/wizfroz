using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;

        [Header("UI Documents:")]
        [SerializeField] private UIDocument deathScreenUI;
        [SerializeField] private UIDocument winScreenUI;
        [SerializeField] private UIDocument gameplayUI;

        private bool hide = false;
        private bool refresh = false;
        private float time = 0f;
        private float? totalMana = null;
        private int octobearsCollected = 0;
        private int manasCollected = 0;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            UIEventManager.ShowDefeatUI += ShowDefeatUI;
            UIEventManager.ShowWinUI += ShowWinUI;
            UIEventManager.UpdateManaBarUI += UpdateManaBarUI;
            UIEventManager.GotCollectableToUI += UpdateCollectableCount;

            VisualElement deathUIRootElement = deathScreenUI.rootVisualElement;
            deathUIRootElement.Q<Button>("RestartBtn").clicked += OnRestartClicked;
            deathUIRootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
            deathUIRootElement.style.visibility = Visibility.Hidden;

            VisualElement winUIRootElement = winScreenUI.rootVisualElement;
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

            time += Time.deltaTime;
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
            gameplayUIRootElement.Q<Label>("OctobearsCollected").text = $" {octobearsCollected}/{octobears}";

            var manas = FindObjectsOfType<ManaCollectable>().Length;
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
        }

        private void ShowDefeatUI(string deathMessage)
        {
            VisualElement deathUIRootElement = deathScreenUI.rootVisualElement;
            deathUIRootElement.Q<Label>("DefeatMessage").text = deathMessage;
            deathScreenUI.rootVisualElement.style.visibility = Visibility.Visible;
        }

        private void OnRestartClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            hide = true;
            refresh = true;
            totalMana = null;
        }

        private void OnHomeClicked()
        {

        }

        private void OnNextClicked()
        {

        }
    }
}

