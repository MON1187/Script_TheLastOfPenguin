//System
using System.Collections;

//Unity
using UnityEngine;
using UnityEngine.UI;

//TMP
using TMPro;

//LOP
using Lop.Survivor;
public class PenguinStatusUI : MonoBehaviour
{
    [SerializeField] private PenguinStatus status;

    public Canvas playerStatusUICanvas;
    [HideInInspector]public Slider hpBar;
    [HideInInspector]public Slider temperatureBar;
    [HideInInspector]public Slider timeBar;

    [HideInInspector]public Image tempFell;
    [HideInInspector] public Image tempLoag;

    public TextMeshProUGUI dayText;
    #region Set Value UI
    private void Start()
    {
        LoadUi();
    }

    private void LoadUi()
    {
        Transform can = playerStatusUICanvas.transform;

        Transform hpBarTransform = can.transform.Find("Bar_Hp");
        Transform temperatureBarTransform = can.transform.Find("Bar_Temperature");
        Transform timeTransform = can.transform.Find("Bar_Time");

        hpBar = hpBarTransform.GetComponent<Slider>();
        temperatureBar = temperatureBarTransform.GetComponent<Slider>();
        tempFell = temperatureBarTransform.Find("Fell").GetComponent<Image>();
        tempLoag = temperatureBarTransform.Find("Loag").GetComponent<Image>();
        timeBar = timeTransform.GetComponent<Slider>();
        SetUIValue();
    }
    private void SetUIValue()
    {
        hpBar.maxValue = status.status_MaxHp;
        temperatureBar.maxValue = 100;

        if (damageImage != null)
        {
            Debug.Log("����ȭ ABCDEFG");
            damageImage.color = new Color(1, 0, 0, 0); // ���� ������ ������
            damageImage2.color = new Color(1, 0, 0, 0); // ���� ������ ������
        }
    }
    #endregion

    public void BarHp(float healthf)
    {
        hpBar.value = healthf;
    }

    public void BarTemperature(float temperature)
    {
        temperatureBar.value = temperature;
    }
    public void BarTime(float currTime)
    {
        dayText.text = TimeManager.Instance.CurrentDay.ToString();
        timeBar.value = Mathf.Lerp(0, timeBar.maxValue, currTime / TimeManager.Instance.MaxDayTime);
    }

    #region �ǰ� ����
    [Header("Hit Effect Assets")]
    public Image damageImage;          // ���������� ������ Image
    public Image damageImage2;          // ���������� ������ Image
    public float flashDuration = 0.5f; // �������� �����Ǵ� �ð�
    public float fadeSpeed = 2f;       // �������� ������� �ӵ�

    public void FlashRed()
    {
        // �ǰ� �� ���������� ����
        if (damageImage != null)
        {
            damageImage.color = new Color(1, 0, 0, 0.5f); // �������� ������
            damageImage2.color = new Color(1, 0, 0, 0.5f);
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(flashDuration);

        // ���� ������������
        while (damageImage.color.a > 0)
        {
            Color color = damageImage.color;
            Color color2 = damageImage2.color;
            color.a -= Time.deltaTime * fadeSpeed;
            color2.a -= Time.deltaTime * fadeSpeed;
            damageImage.color = color;
            damageImage2.color = color2;
            yield return null;
        }
    }
    #endregion 
}
