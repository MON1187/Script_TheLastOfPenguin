using UnityEngine;

public class PenguinPrefabData : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("GameObject")]
    public GameObject[] myBags;
    public GameObject[] characterEquipmentMesh;

    [Header("C# Script")]
    public EquipmentSlingshot equipmentSlingshot; // FirePoint Ʈ������, PenguinBody �ʿ�
    public EquipmentBoxingGlove equipmentBoxingGlove;
    public EquipmentFishing equipmentFishing; // FishingPickPos Ʈ������, PenguinFunction �ʿ�

    public DiggingWood diggingWood;
    public AttackSwordDamagble attackSwordDamagble; // PenguinBody �ʿ�

    [Header("Unity Component")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
}
