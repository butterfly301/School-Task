using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneFlowType flowType = SceneFlowType.Gameplay;
    public AssetReference sceneReference;
}

public enum SceneFlowType
{
    Gameplay,
    Menu,
    Loading,
    Video,
    Persistent
}
