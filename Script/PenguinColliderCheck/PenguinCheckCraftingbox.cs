using UnityEngine;

public class PenguinCheckCraftingbox : MonoBehaviour
{
    public CraftingStation craftBox;
    public GameObject craftObj;
    private string targetTag = "Crafting_Table";

    [SerializeField] float radius = 0.5f;
    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider other in colliders)
        {
            if (other.gameObject.tag == targetTag)
            {
                if (!craftObj.activeSelf)
                {
                    craftBox.craft_tier = 2;
                }
            }
        }
    }
}
