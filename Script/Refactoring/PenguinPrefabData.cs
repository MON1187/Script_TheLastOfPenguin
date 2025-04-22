using UnityEngine;

public class PenguinPrefabData : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("GameObject")]
    public GameObject[] myBags;
    public GameObject[] characterEquipmentMesh;

    [Header("C# Script")]
    public EquipmentSlingshot equipmentSlingshot; // FirePoint 트랜스폼, PenguinBody 필요
    public EquipmentBoxingGlove equipmentBoxingGlove;
    public EquipmentFishing equipmentFishing; // FishingPickPos 트랜스폼, PenguinFunction 필요

    public DiggingWood diggingWood;
    public AttackSwordDamagble attackSwordDamagble; // PenguinBody 필요

    [Header("Unity Component")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
}
