using System.Collections;
using UnityEngine;

public class AttackShot : MonoBehaviour, IsAttackble
{
    public float damaged;
    public float Damaged => damaged;

    public SphereCollider myCollider;

    WaitForSeconds GetTime = new WaitForSeconds(0.75f);
    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;

        Quaternion rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(rotation.eulerAngles);
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
        if (other.gameObject != null)
        {
            if(other.gameObject.tag == "Monster")
            Destroy(this.gameObject);
        }
    }
}
