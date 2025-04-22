using UnityEngine;

[CreateAssetMenu(fileName = "new Penguin ItemDatas", menuName = "Data/PenguinItem")]
public class PenguinLoadItemScriptable : ScriptableObject
{
    public PenguinLoadItem[] penguinLoadItem;
}

[System.Serializable]
public class PenguinLoadItem
{
    public string id;
    public GameObject[] itemLoadData;
}

