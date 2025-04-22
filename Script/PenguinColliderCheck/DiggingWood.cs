using Lop.Survivor.inventroy;
using Lop.Survivor.Island.Buildingbase.Lop.Survivor.Island.Basicbuildingscript;
using System.Collections;
using UnityEngine;

public class DiggingWood : MonoBehaviour
{
    public SphereCollider myCollider;


    WaitForSeconds GetTime;

    Inventory inventory;
    bool isFirstHitl;
    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;

        GetTime = new WaitForSeconds(0.75f);
    }
    private void Start()
    {
        if(inventory == null) { inventory = FindFirstObjectByType<Inventory>(); }
    }

    public void OnFelling()
    {
        if (myCollider.enabled) { Debug.Log("Round A"); return; }

        StartCoroutine(ReEnabled());
    }
    public void OffFelling()
    {
        isFirstHitl = false;
        myCollider.enabled = false;
    }

    IEnumerator ReEnabled()
    {
        Debug.Log("Round Start Tree");

        isFirstHitl = true;
        myCollider.enabled = true;
        yield return GetTime;
        OffFelling();
    }

    //public const string tree = "Tree";
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(BuildingTagAnim.Tree))
        {
            if(isFirstHitl)
            {
                isFirstHitl = false;
                OffFelling();
                inventory.MinusDurability();
            }
            other.GetComponent<TreesBone>().NormalChop();
            if(transform.root.GetComponent<PenguinBody>().isLocalPlayer)
                SoundManager.Instance.PlaySFX("WoodBreak");

        }
        else if (other.gameObject.CompareTag(BuildingTagAnim.TreeC))
        {
            if(isFirstHitl)
            {
                isFirstHitl= false;
                OffFelling();
                inventory.MinusDurability();
            }
            other.GetComponent<TreesBone>().FruitChop();
            if (transform.root.GetComponent<PenguinBody>().isLocalPlayer)
                SoundManager.Instance.PlaySFX("WoodBreak");
        }
    }
}
