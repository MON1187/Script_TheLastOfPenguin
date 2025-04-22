using Lop.Survivor.inventroy;
using System.Collections;
using UnityEngine;

public class AttackSwordDamagble : MonoBehaviour, IsAttackble
{
    [SerializeField]PenguinBody penguinBody;

    private float damaged;
    public float Damaged => damaged;

    public SphereCollider myCollider;

    WaitForSeconds GetTime = new WaitForSeconds(0.75f);
    [SerializeField] Inventory inventory;
    bool isFirstHit;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;
    }

    private void Start()
    {
        if (inventory == null) { inventory = LopNetworkManager.GetPlayer().GetComponentInChildren<Inventory>(); }

        if (penguinBody == null) {penguinBody = LopNetworkManager.GetPlayer().GetComponentInChildren<PenguinBody>(); } 
    }
    public void OnCollider()
    {
        damaged = penguinBody.status.status_str;
        StartCoroutine(ReEnabled());
    }
    public void OffCollider()
    {
        myCollider.enabled = false;
    }
    public IEnumerator ReEnabled()
    {
        isFirstHit = true;
        myCollider.enabled = true;
        yield return GetTime;
        myCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.GetComponent<Monster>())
        {
            SoundManager.Instance.PlaySFX("Hit");

            if (isFirstHit)
            {
                Debug.Log("Round IsAttack");
                isFirstHit = false;
                inventory.MinusDurability();
            }
        }
    }
}
