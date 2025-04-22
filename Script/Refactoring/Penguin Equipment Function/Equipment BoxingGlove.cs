using System.Collections;
using UnityEngine;
using static PenguinSituation;
public class EquipmentBoxingGlove : MonoBehaviour
{
    private void PlayBoxingGlove()
    {
        isAttack = true;
        gameObject.SetActive(true);
    }
    private void BoxingRelease()
    {
        isAttack = false;
        gameObject.SetActive(false);
    }
    public void Play()
    {
        StartCoroutine(PlayBoxing());
    }
    private IEnumerator PlayBoxing()
    {
        PlayBoxingGlove();
        SoundManager.Instance.PlaySFX("ToolPunch");
        yield return new WaitForSeconds(0.35f);
        SoundManager.Instance.PlaySFX("ToolPunch");
        BoxingRelease();
    }
}
