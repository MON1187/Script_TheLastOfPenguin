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
    public float status_hp;                 //���� ü��
    public float status_MaxHp;              //�ִ� ü��
    public float status_str;                //��
    public float status_dex;                //������
    public float status_wis;                //����
    public float status_move_speed;         //�ӵ�
    public float status_jump_strength;      //���� ����
    public float status_slide_speed;        //�����̵� �ӵ�
    public float status_rotation_speed;     //ȸ�� �ӵ�
    public float status_temperature_gauge;  //�µ�
    public float temperature_set_value;     //���� �µ� ����
    public float[] temperature_guage;       //�µ� ��ȭ��
    public float[] temperature_decrease_hp; //ü�� ��ȭ��
    
    //���Ǵ� ü�°� �µ� ����
    public float healthf { get { return status_hp; } }

    public float temperature { get { return status_temperature_gauge; } }

    //��ȯ �Ҷ� ���
    //�ӵ� ��ȯ �Ҷ� ���
    [HideInInspector]public float return_speed;
    //���� �� ���� id���� Ȯ�� �뵵
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

        //�� �־����ٸ� ��ȯ �� ����
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
