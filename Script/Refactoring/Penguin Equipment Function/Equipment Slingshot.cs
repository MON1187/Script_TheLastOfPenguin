using Lop.Survivor.inventroy;
using UnityEngine;
public class EquipmentSlingshot : MonoBehaviour
{
    [SerializeField] private PenguinBody body;

    //오브젝트 값
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
            if (inventorySlot.slotItemData.itemName == ItemCategory.Stone)  //돌이 있어야 실행
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

        // 투사체 생성 및 방향 설정
        GameObject projectile = Instantiate(slingshotAmmunition, shootPoint.position, Quaternion.identity);
        Vector3 direction = transform.forward;
        projectile.GetComponent<AttackShot>().damaged = body.status.status_str;

        // 투사체에 속도 적용
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
