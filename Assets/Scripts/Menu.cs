using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Linq;

public class Menu : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenu;
    [SerializeField] private UIDocument optionsMenu;
    [SerializeField] private UIDocument playMenu;
    [SerializeField] private List<string> scenesNames;

    private VisualElement rootElemMainMenu;
    private VisualElement rootOptionsMenu;
    private VisualElement rootPlayMenu;

    void Awake()
    {
        rootElemMainMenu = mainMenu.rootVisualElement;
        rootOptionsMenu = optionsMenu.rootVisualElement;
        rootPlayMenu = playMenu.rootVisualElement;

        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Hidden;

        SetMainMenuEvents();
        SetOptionsMenuEvents();

        GenerateLoadLevelButtons();
        SetPlayMenuEvents();
    }

    private void SetMainMenuEvents()
    {
        rootElemMainMenu.Q<Button>("LevelSelect").clicked += HandlePlayButtonClick;
        rootElemMainMenu.Q<Button>("Options").clicked += HandleOptionsButtonClick;
        rootElemMainMenu.Q<Button>("Exit").clicked += HandleExitButtonClick;
    }

    private void HandleOptionsButtonClick()
    {
        rootElemMainMenu.style.visibility = Visibility.Hidden;
        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Visible;
    }

    private void HandlePlayButtonClick()
    {
        rootElemMainMenu.style.visibility = Visibility.Hidden;
        rootPlayMenu.style.visibility = Visibility.Visible;
        rootOptionsMenu.style.visibility = Visibility.Hidden;
    }

    private void HandleExitButtonClick()
    {
        Debug.Log("Exit");
    }

    private void SetOptionsMenuEvents()
    {
        rootOptionsMenu.Q<Slider>("FxVolume").RegisterValueChangedCallback(obj => HandleFxVolumeValueChange(obj));
        rootOptionsMenu.Q<Slider>("MusicVolume").RegisterValueChangedCallback(obj => HandleMusicVolumeValueChange(obj));
        rootOptionsMenu.Q<Button>("Back").clicked += HandleBackToMainMenuButton;
    }

    private void HandleFxVolumeValueChange(ChangeEvent<float> e)
    {
        Debug.Log(e);
    }

    private void HandleMusicVolumeValueChange(ChangeEvent<float> e)
    {
        Debug.Log(e);
    }

    private void HandleBackToMainMenuButton()
    {
        rootElemMainMenu.style.visibility = Visibility.Visible;
        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Hidden;
    }

    private void GenerateLoadLevelButtons()
    {
        var levelContainer = rootPlayMenu.Q<VisualElement>("LevelContainer");

        int index = 1;
        foreach (var name in scenesNames)
        {
            Button newButton = new Button();
            newButton.name = index.ToString();
            newButton.text = index.ToString();
            newButton.AddToClassList("buttons");
            newButton.AddToClassList("RoundButton");
            newButton.RegisterCallback<MouseOverEvent>((callback) => MouseOverOnLevelButton(callback));
            newButton.RegisterCallback<MouseLeaveEvent>((callback) => MouseLeaveOnLevelButton(callback));
            newButton.RegisterCallback<ClickEvent>((callback) => ClickOnLevelButton(callback));
            newButton.CaptureMouse();
            levelContainer.Add(newButton);
            index++;
        }
    }

    private void MouseOverOnLevelButton(MouseOverEvent e)
    {
        string level = e.target.GetType().GetProperty("name").GetValue(e.target).ToString();
        var levelInfo = rootPlayMenu.Q<Label>("LevelInfo");
        var firstStar = rootPlayMenu.Q<VisualElement>("Star1");
        var secondStar = rootPlayMenu.Q<VisualElement>("Star2");
        var thirdStar = rootPlayMenu.Q<VisualElement>("Star3");

        levelInfo.text = $"You've achieved all stars of Level {level}...";

        firstStar.style.visibility = Visibility.Visible;
        secondStar.style.visibility = Visibility.Visible;
        thirdStar.style.visibility = Visibility.Visible;
    }

    private void MouseLeaveOnLevelButton(MouseLeaveEvent e)
    {
        var levelInfo = rootPlayMenu.Q<Label>("LevelInfo");

        levelInfo.text = $"Click the Level button to play it or pass the mouse over <br> the button to get the current progress!";

        var firstStar = rootPlayMenu.Q<VisualElement>("Star1");
        var secondStar = rootPlayMenu.Q<VisualElement>("Star2");
        var thirdStar = rootPlayMenu.Q<VisualElement>("Star3");
        firstStar.style.visibility = Visibility.Hidden;
        secondStar.style.visibility = Visibility.Hidden;
        thirdStar.style.visibility = Visibility.Hidden;
    }

    private void ClickOnLevelButton(ClickEvent e)
    {
        string level = e.target.GetType().GetProperty("name").GetValue(e.target).ToString();
        SceneManager.LoadScene("DevScene");
    }

    private void SetPlayMenuEvents()
    {
        rootPlayMenu.Q<Button>("Back").clicked += HandleBackToMainMenuButton;
    }

    void Start()
    {

    }


    void Update()
    {

    }
}
