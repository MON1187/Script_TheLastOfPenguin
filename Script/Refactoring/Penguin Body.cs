//System
using System.Collections;
//Unity
using UnityEngine.InputSystem;
using UnityEngine;
//Mirror
using Mirror;

//Penguin
using static PenguinSituation;
using Lop.Survivor;
public abstract class PenguinBody : NetworkBehaviour, IDamageable
{
    //Unity Function
    public Rigidbody rigid;
    public PlayerInput inputSystem;

    //Penguin System Function
    public PenguinStatus status;
    //public PenguinItemFunction itemFunction;
    public PenguinStatusUI statusUI;
    //public PenguinFunction penguinFunction;
    [SerializeField] private Transform playerCheckPos;  //Pos : _Assets -> Transform Point -> PlayerPos

    //Camera
    public CameraShake shake;

    #region Values ​​for reference elsewhere
    public Transform dropItemPotion;
    public bool isBuildingEnter;
    public bool isOpenUI;
    #endregion

    public void StartLocalPlayer(string syncId, int syncIdNumber, int syncColorNumber)
    {
        //penguinFunction.LoadMesh(syncId, syncIdNumber, syncColorNumber);

        if (!isServer)
        {
            gameObject.SetActive(false);
        }
    }

    public void DealDamage(float damaged)
    {
        if (isLocalPlayer)
        {
            shake.CameraShakeing(1);
            statusUI.FlashRed();
            status.status_hp -= damaged;
            if (status.status_hp > 0) { SoundManager.Instance.PlaySFX("PlayerTakeDamage"); }
        }
        //else 로컬 연결
        //{
        //    DamageMessage damageMessage = new DamageMessage
        //    {
        //        damage = damaged
        //    };
        //    localPlayerConn.Send(damageMessage);
        //}
    }

    public void OverGame()
    {
        if (status.status_hp <= 0 && isLife) { UponDeath(); }
    }

    private void UponDeath()
    {
        SoundManager.Instance.PlaySFX("PlayerDie");

        rigid.velocity = Vector3.zero;
        //moveVec = Vector3.zero; 대체방법 찾아야함.

        UnInput(false);

        //if (itemFunction.currentItem != null) { itemFunction.currentItem.SetActive(false); }

        isLife = false;
        //penguinFunction.CmdSetTrigger("isDeath");

        FadeManager.Instance.FadeIn(() => GameManager.Instance.OnResultPenel());
    }
    
    //Player MoveController On(true) is Moveing, and Off(false) the Not Moveing
    public void UnInput(bool switchs)
    {
        inputSystem.enabled = switchs;
    }
    //public struct DamageMessage : NetworkMessage
    //{
    //    public float damage;
    //}

    public IEnumerator OnTick() // TODO: OnTick을 사용할 수 있게 해 주세요.
    {
        if (isLocalPlayer)
        {
            isLife = true;

            //Hp가 0 이상 일시 실행
            while (status.status_hp > 0)
            {
                //임시 | 프레임 마다 체크
                yield return new WaitForSeconds(1);
                //To Do : Tick Manger로 관리   //현재는 보호상태로 사용 불가
                yield return TickManager.Instance.elapsedTicks;

                //온도에 따른 hp 감소
                status.status_hp += status.temperature_decrease_hp[GetMyTemperature(status.status_temperature_gauge)];

                //온도에 따른 게이지 감소/ 증가
                status.status_temperature_gauge += status.temperature_guage[TemperatureManager.Instance.GetTemperature(playerCheckPos.position)];

                //자신의 온도의 따라 색깔 변경
                statusUI.tempFell.color = Fell_ChangeColorTemperatureBar(status.status_temperature_gauge);
                statusUI.tempLoag.color = Loag_ChangeColorTemperatureBar(status.status_temperature_gauge);

                // 시간 
                statusUI.BarTime(TimeManager.Instance.GetGlobalTimer());
            }
        }
    }

    #region UIChangColor
    private int GetMyTemperature(float temp)
    {
        if (temp > 75)  //Very Hot
        {
            return 0;
        }
        else if (temp < 74 && temp > 65)    //Hot
        {
            return 1;
        }
        else if (temp < 64 && temp > 36)    //Normal
        {
            return 2;
        }
        else if (temp < 35 && temp > 16)    //Cold
        {
            return 3;
        }
        else if (temp < 15)                 //Very Cold
        {
            return 4;
        }
        else return 0;
    }

    private Color Loag_ChangeColorTemperatureBar(float temp)
    {
        if (temp >= 80)
        {
            return Color.red;
        }
        else if (temp <= 79 && temp >= 60)
        {
            return new Color(2f, 1f, 0f, 1f);
        }
        else if (temp <= 59 && temp >= 40)
        {
            return Color.white;
        }
        else if (temp <= 39 && temp >= 20)
        {
            return Color.cyan;
        }
        else if (temp <= 19)
        {
            return Color.gray;
        }
        else { return default; }
    }

    private Color Fell_ChangeColorTemperatureBar(float temp)
    {
        if (temp >= 80)
        {
            return Color.red;
        }
        else if (temp <= 79 && temp >= 60)
        {
            return new Color(1.0f, 0.64f, 0.0f, 1);
        }
        else if (temp <= 59 && temp >= 40)
        {
            return Color.green;
        }
        else if (temp <= 39 && temp >= 20)
        {
            return Color.cyan;
        }
        else if (temp <= 19)
        {
            return Color.blue;
        }
        else return default;
    }
    #endregion
}
