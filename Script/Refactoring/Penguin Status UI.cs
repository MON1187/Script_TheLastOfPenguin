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
            Debug.Log("투명화 ABCDEFG");
            damageImage.color = new Color(1, 0, 0, 0); // 완전 투명한 빨간색
            damageImage2.color = new Color(1, 0, 0, 0); // 완전 투명한 빨간색
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

    #region 피격 연출
    [Header("Hit Effect Assets")]
    public Image damageImage;          // 붉은색으로 깜빡일 Image
    public Image damageImage2;          // 붉은색으로 깜빡일 Image
    public float flashDuration = 0.5f; // 붉은색이 유지되는 시간
    public float fadeSpeed = 2f;       // 붉은색이 사라지는 속도

    public void FlashRed()
    {
        // 피격 시 빨간색으로 변경
        if (damageImage != null)
        {
            damageImage.color = new Color(1, 0, 0, 0.5f); // 반투명한 빨간색
            damageImage2.color = new Color(1, 0, 0, 0.5f);
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(flashDuration);

        // 점점 투명해지도록
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
