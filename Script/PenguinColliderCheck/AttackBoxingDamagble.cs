using System.Collections;
using UnityEngine;

public class AttackBoxingDamagble : MonoBehaviour, IsAttackble
{
    public SphereCollider myCollider;

    private float damaged;
    public float Damaged => damaged;

    WaitForSeconds GetTime = new WaitForSeconds(0.75f);
    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;
    }
    public void OnCollider()
    {
        StartCoroutine(ReEnabled());
    }
    public IEnumerator ReEnabled()
    {
        myCollider.enabled = true;
        yield return GetTime;
        myCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<Monster>())
        {
            SoundManager.Instance.PlaySFX("Hit");
        }
    }
}
