using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;
using UnityEngine.Events;

public static class ProgressEventManager
{
    public static UnityAction SaveData;
    public static void EmitSaveData() => SaveData?.Invoke();

    public static UnityAction RefreshData;
    public static void EmitRefrestData() => RefreshData?.Invoke();
}
