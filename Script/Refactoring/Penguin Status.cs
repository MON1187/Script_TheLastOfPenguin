using Ookii.Dialogs;
using UnityEngine;

public class PenguinStatus : MonoBehaviour
{
    //Penguin Status Save Object.
    [SerializeField] private PenguinDataScriptable statusData; 

    [Header("Stats")]
    //Player Stauts Value.
    #region status
    public string id;
    public float status_hp;                 //현재 체력
    public float status_MaxHp;              //최대 체력
    public float status_str;                //힘
    public float status_dex;                //손재주
    public float status_wis;                //지능
    public float status_move_speed;         //속도
    public float status_jump_strength;      //점프 강도
    public float status_slide_speed;        //슬라이딩 속도
    public float status_rotation_speed;     //회전 속도
    public float status_temperature_gauge;  //온도
    public float temperature_set_value;     //최초 온도 셋팅
    public float[] temperature_guage;       //온도 변화량
    public float[] temperature_decrease_hp; //체력 변화량
    
    //사용되는 체력과 온도 변수
    public float healthf { get { return status_hp; } }

    public float temperature { get { return status_temperature_gauge; } }

    //반환 할때 사용
    //속도 반환 할때 사용
    [HideInInspector]public float return_speed;
    //시작 시 무슨 id인지 확인 용도
    // [HideInInspector] public int idNumber;
    [HideInInspector] public int colorNumber;

    #endregion

    public void LoadPrefab(string syncId, int syncColorNumber)
    {
        id = syncId;
        colorNumber = syncColorNumber;

        CharacterLoadData();

        //if (!isLocalPlayer)
        //    HidController(currentSlotItem);
    }


    private void Update()
    {
        OverValueGetNormalizationI();
    }

    //According to Id Get Status.
    private void CharacterLoadData()
    {
        foreach (PenguinData data in statusData.penguinDatas)
        {
            if (data.id == id)
            {
                status_hp = data.status_hp;
                status_MaxHp = data.status_hp;
                status_str = data.status_str;
                status_dex = data.status_dex;
                status_wis = data.status_wis;
                status_move_speed = data.status_move_speed;
                temperature_set_value = data.temperature_set_value;
                status_temperature_gauge = temperature_set_value;

                temperature_guage = new float[data.temperature_guage.Length];
                data.temperature_guage.CopyTo(temperature_guage, 0);

                temperature_decrease_hp = new float[data.temperature_decrease_hp.Length];
                data.temperature_decrease_hp.CopyTo(temperature_decrease_hp, 0);

                Transform penguinTransform = Instantiate(data.model, Vector3.zero, Quaternion.identity).transform;
                penguinTransform.SetParent(transform, false);

                penguinTransform.GetComponent<PenguinPrefabData>().skinnedMeshRenderer.material = data.colors[colorNumber];

                GetComponent<PenguinFunction>().LoadPrefabData(penguinTransform.GetComponent<PenguinPrefabData>());

                break;
            }
        }

        status_rotation_speed = 10f;
        status_jump_strength = 5f;

        //다 주어졌다면 반환 값 지정
        status_slide_speed = status_move_speed * 1.5f;
        return_speed = status_move_speed;
    }

    //Player Status Recovery action in case of overflow
    private void OverValueGetNormalizationI()
    {
        if (status_hp > status_MaxHp)
        {
            status_hp = status_MaxHp;
        }

        if(status_temperature_gauge > 100)
        {
            status_temperature_gauge = 100;
        }
    }
}
