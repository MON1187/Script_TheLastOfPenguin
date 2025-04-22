using Lop.Survivor.inventroy;
using UnityEngine;
public class EquipmentSlingshot : MonoBehaviour
{
    [SerializeField] private PenguinBody body;

    //������Ʈ ��
    [SerializeField] private GameObject slingshotAmmunition;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float projectileSpeed = 20f;

    private Inventory inventory;

    public void ShootProjectile(Inventory _inventory)
    {
        if(inventory == null) inventory = _inventory;

        foreach (var inventorySlot in inventory.slotDatas)
        {
            if (inventorySlot.slotItemData == null) continue;
            if (inventorySlot.slotItemData.itemName == ItemCategory.Stone)  //���� �־�� ����
            {
                if (inventorySlot.itemCount > 0)
                {
                    inventory.MinusCount(inventorySlot.slotItemData.id);
                    CmdShootProjectile();
                }
            }
        }
        CmdShootProjectile();
    }

    //[Command]
    void CmdShootProjectile()
    {
        RpcShootProjectile();
    }

    //[ClientRpc]
    void RpcShootProjectile()
    {
        if (LopNetworkManager.isLoading) return;

        inventory.MinusDurability();

        // ����ü ���� �� ���� ����
        GameObject projectile = Instantiate(slingshotAmmunition, shootPoint.position, Quaternion.identity);
        Vector3 direction = transform.forward;
        projectile.GetComponent<AttackShot>().damaged = body.status.status_str;

        // ����ü�� �ӵ� ����
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;

        DestroyObj(projectile, 3f);
    }

    void DestroyObj(GameObject obj, float time)
    {
        Destroy(obj, time);
    }

    public void SetPenguinBody(PenguinBody penguinBody)
    {
        body = penguinBody;
    }
}
