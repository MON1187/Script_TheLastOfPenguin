using Lop.Survivor.inventroy;
using System.Collections;
using UnityEngine;

using static BlockType;
using static PenguinSituation;
public class EquipmentFishing : MonoBehaviour
{
    //Penguin
    [SerializeField] private PenguinFunction penguinFunction;

    [SerializeField] private Transform itemDropPoint;
    string checkWater;
    bool pickFishing;               //Play The fishing?
    IEnumerator weelingFishing;

    private Map map;
    private Inventory inventory;
    private void Start()
    {
        map = FindAnyObjectByType<MapSettingManager>().Map;
    }
    private void Update()
    {
        if(!isPlayFishing && pickFishing)
        {
            FishingExitCoroutine();
        }
    }


    public void Play(Inventory _inventory)
    {
        if (inventory == null) { inventory = _inventory; }

        Fishing();
    }

    private bool IsFishRound()
    {
        Vector3 Down = transform.position;
        Down.y -= 1;
        Vector3 checkPos = transform.forward * 1 + Down;

        checkWater = map.GetVoxelType(checkPos);
        if (checkWater == Water) { return true; }
        else { return false; }
    }
    private void Fishing()
    {
        if (IsFishRound())
        {
            if (!isPlayFishing)
            {
                isPlayFishing = true;
                pickFishing = false;
                SoundManager.Instance.PlaySFX("FishingThrowing");   //추 던지기 SFX Play
                inventory.MinusDurability();        //내구도 까기
                penguinFunction.CmdSetTrigger("isFishing"); //애니메이션 실행
                weelingFishing = WeelingFishing();  //코루틴 재정의
                StartCoroutine(weelingFishing);
            }
        }
    }
    private IEnumerator WeelingFishing()
    {
        yield return new WaitForSeconds(0.75f);
        SoundManager.Instance.PlaySFX("FisingFallWater");
        float longTime = 0;
        float playTime = 5f;
        while (longTime < playTime)
        {
            yield return new WaitForSeconds(1);

            longTime += 1;
            RandomBoxFsih();
        }

        FishingExitCoroutine();
    }

    private void FishingExitCoroutine()
    {
        SoundManager.Instance.PlaySFX("FisingTakeOut");
        penguinFunction.CmdSetTrigger("isEndFishing");
        isPlayFishing = false;
        pickFishing = true;
        StopCoroutine(weelingFishing);
    }
    private void RandomBoxFsih()
    {
        float minValue = 0f;
        float centerValue = 70f;
        float maxValue = 100f;

        float currentValue = Random.Range(minValue, maxValue);
        Debug.Log(currentValue);
        if (currentValue > centerValue) //Get Fishing
        {
            inventory.DropItemGameObject(RandomGetFishingGenerator(2), itemDropPoint.transform.position);
            FishingExitCoroutine();
        }
    }

    private string RandomGetFishingGenerator(int fishingNumber)
    {
        int index = Random.Range(0, fishingNumber);
        return index switch
        {
            0 => ItemCategory.Fish_red,
            1 => ItemCategory.Fish_blue,
            _ => null
        };
    }

    public void SetPenguinFunction(PenguinFunction penguinFunction)
    {
        this.penguinFunction = penguinFunction;
    }
}
