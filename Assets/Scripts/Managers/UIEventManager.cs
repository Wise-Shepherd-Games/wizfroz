using UnityEngine.Events;

public static class UIEventManager
{
    public static UnityAction ShowWinUI;
    public static void EmitShowWinUI() => ShowWinUI?.Invoke();

    public static UnityAction<string> ShowDefeatUI;
    public static void EmitShowDefeatUI(string defeatMessage) => ShowDefeatUI?.Invoke(defeatMessage);

    public static UnityAction<float> UpdateManaBarUI;
    public static void EmitUpdateManaBarUI(float currentMana) => UpdateManaBarUI?.Invoke(currentMana);

    public static UnityAction<string> GotCollectableToUI;
    public static void EmitGotCollectableToUI(string collectable) => GotCollectableToUI?.Invoke(collectable);
}
