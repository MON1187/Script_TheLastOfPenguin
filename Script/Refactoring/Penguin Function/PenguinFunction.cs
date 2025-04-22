//System
using System.Collections;

//Unity
using UnityEngine;
using UnityEngine.InputSystem;

//Mirror
using Mirror;

//Lop
using Lop.Survivor.inventroy;
using Lop.Survivor.inventroy.Item;

using static BlockType;
using static PenguinSituation;
using static AnimationId;
public class PenguinFunction : NetworkBehaviour
{
    private Map map;
    [Header("Inventory Cs")]
    [SerializeField] ToolWheel toolWheel;
    public Inventory inventory;
    [SerializeField]private InventoryHandler inventoryHandler;
    [Header("Penguin Character Cs")]
    [SerializeField] PenguinBody body;
    [SerializeField] PenguinStatus status;
    [SerializeField] NetworkPlayer netPlayer;
    [Header("Equipment Cs")]
    [SerializeField] EquipmentSlingshot equipmentSlingshot;
    [SerializeField] EquipmentBoxingGlove equipmentBoxingGlove;
    [SerializeField] EquipmentFishing equipmentFishing;

    [Header("Unity Component")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Animator animator;

    public Transform playerCheckPos;

    private void Start()
    {
        isJump = true;

        if(inventoryHandler == null)
        inventoryHandler = GetComponentInChildren<InventoryHandler>();
        if(map == null)
        map = FindAnyObjectByType<MapSettingManager>().Map;
        SetAngle();

        StartLoadData();
    }
    private void Update()
    {
        HandleMovement();
        PlaySlideIsCheck();
        Swiming();
        PlayStepSound();
    }

    #region Penguin Controller
    private float hAxis;        //�Է� x�� ����
    private float vAxis;        //�Է� z�� ����

    private Vector3 moveVec;
    private Vector3 cheackGroundPos;
    //Movement management according to camera view
    private Vector3 forward;
    private Vector3 right;

    #region Input System

    private string groundedVoxelType = default;

    #region Moveing
    public void OnMove(InputValue value)
    {
        if (!isLocalPlayer) return;

        Vector2 input = value.Get<Vector2>();
        hAxis = input.x;
        vAxis = input.y;

        input.Normalize();
    }
    private void HandleMovement()
    {
        if (isPenguinController) return;

        moveVec = (right * hAxis + forward * vAxis).normalized * status.status_move_speed;

        if (inventoryHandler.currentState != InventoryHandlerState.Default)
        {
            //�ʱ�ȭ
            moveVec = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }

        rigid.velocity = new Vector3(moveVec.x, rigid.velocity.y, moveVec.z);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, status.status_rotation_speed * Time.deltaTime);
            CmdSetBool(aniIsMove, true);
        }
        else
        {
            CmdSetBool(aniIsMove, false);
        }

        if (moveVec != Vector3.zero && isPlayFishing)
        {
            isPlayFishing = false;
        }
    }

    public void MoveControllerOffFunction()
    {
        rigid.velocity = Vector3.zero;
        moveVec = Vector3.zero;
        CmdSetBool(aniIsMove, false);
    }
    #endregion

    #region Jump
    public void OnJump()
    {
        if (!isLocalPlayer) return;

        if (isPenguinController) return;
        DoJump();
    }
    private void DoJump()
    {
        Bounds bounds = capsuleCollider.bounds;
        cheackGroundPos = new Vector3(bounds.center.x, bounds.min.y - 0.2f, bounds.center.z);

        bool cheackGroundTag = CheackTag(cheackGroundPos, .25f, GroundTag);

        if (isJump && cheackGroundTag)
        {
            StartCoroutine(JumpTime());

            if (isPlayFishing)
            {
                isPlayFishing = false;
            }
        }
    }
    private IEnumerator JumpTime()
    {
        isJump = false;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(Vector3.up * status.status_jump_strength, ForceMode.Impulse);
        CmdSetTrigger(aniJump_02);
        yield return new WaitForSeconds(0.75f);

        isJump = true;
    }
    private bool CheackTag(Vector3 cheackPos, float radius, string cheackTag)
    {
        Collider[] colliders = Physics.OverlapSphere(cheackPos, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == cheackTag)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Action
    public void OnAct(InputValue value)
    {
        if (!isLocalPlayer) return;
        if (isPenguinController) return;

        if (currentItem != null)
        {
            if (!isSwiming)
            {
                HidController(currentItemData.itemType);
                UseCurrentItem(currentItemData.itemType);
            }
        }

    }
    #endregion

    #region Slid
    private bool isPlaySlide = false;   //�����̵� ������ Ȯ�ο�
    private void PlaySlideIsCheck()
    {
        groundedVoxelType = map.GetVoxelType(playerCheckPos.position);
        //Debug.Log(groundedVoxelType);
        if (groundedVoxelType == Ice)
        {
            isSlide = true;
            if (!isPlaySlide) { PlayMoveSFXSound("IceStep"); }
        }
        else
        {
            isSlide = false;

            if (isPlaySlide)
            {
                isPlaySlide = false;
                CmdSetBool(aniIsSliding, false);
                status.status_move_speed = status.return_speed;
            }
        }
    }

    public void OnSliding(InputValue value)
    {
        if (!isLocalPlayer) return;
        if (isPenguinController) return;

        Debug.Log($"{isSlide} + {isPlaySlide}");
        if (isSlide && !isPlaySlide) { Slide(); }
    }
    
    private void Slide()
    {
        if (currentItem != null) currentItem.SetActive(false);

        status.status_move_speed = status.status_slide_speed;
        SoundManager.Instance.PlaySFX("PlayerSlide");
        isPlaySlide = true;
        CmdSetBool(aniIsSliding, true);  //�ִϸ��̼� ����, �´� �̸����� ���� �ϸ��.
    }
    #endregion

    #region swiming
    private void Swiming()
    {
        if (netPlayer.GroundedVoxelType == Water)
        {
            if (!isSwiming)
            {
                CmdSetBool(aniIsSwimming, true);
                isSwiming = true;
                if (currentItem != null) { currentItem.SetActive(false); }
            }
        }
        else
        {
            if (isSwiming)
            {
                CmdSetBool(aniIsSwimming, false);
                isSwiming = false;
                if (currentItem != null) { currentItem.SetActive(true); }
            }
        }
    }
    #endregion

    #endregion

    //�ӽ� ���� �ű�
    private const string GroundTag = nameof(Ground);

    //Adjust movement according to changed view
    private void SetAngle()
    {
        Transform cameraTransform = Camera.main.transform;
        forward = cameraTransform.forward;
        right = cameraTransform.right;

        // ���� �̵��� �ʿ��ϹǷ� y ���� 0���� ����
        forward.y = 0f;
        right.y = 0f;

        // ����ȭ�Ͽ� ���� ���� ���� 1�� ����
        forward.Normalize();
        right.Normalize();
    }
    #endregion

    #region Penguin Animation

    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }

    #region Trigger Aniamtion
    [Command]
        public void CmdSetTrigger(string trigger)
        {
            if (!LopNetworkManager.isLoading)
                RpcSetTrigger(trigger);
        }

        [ClientRpc]
        private void RpcSetTrigger(string trigger)
        {
            if (body.status.id != string.Empty)
                animator.SetTrigger(trigger);

            if (trigger == "isAxe" && !isLocalPlayer)
            {
                //axePrefab.OnFelling();
            }
        }

        #endregion

    #region SetBool Animation
    [Command]
    public void CmdSetBool(string name, bool value)
    {
        if (!LopNetworkManager.isLoading)
            RpcSetBool(name, value);
    }

    [ClientRpc]
    private void RpcSetBool(string name, bool value)
    {
        if (body.status.id != string.Empty) { animator.SetBool(name, value); }
        if ((name == "isSwimming" || name == "isSliding") && currentItem != null)
        {
            currentItem.SetActive(!value);
        }
    }
    #endregion
    #endregion

    #region Penguin LoadMesh
    [Header("Penguin LoadMesh")]
    public GameObject[] myBag;

    public GameObject[] characterEquipmentMesh;

    public DiggingWood axePrefabCs;             //����
    public AttackSwordDamagble attackDamagble;  //sword

    public void StartLoadData()
    {
        BagLoad(status.id);
        HidObject();
    }

    private void HidObject()
    {
        HidObjectArry(characterEquipmentMesh);
    }
    public void HidObjectArry(GameObject[] equipment)
    {
        foreach (var current in equipment)
        {
            current.gameObject.SetActive(false);
        }
    }
    #region LoadBag
    public void BagLoad(string id)
    {
        HidingBag();

        WearBag(id);
    }

    private void HidingBag()
    {
        HideBag(myBag);
    }

    private void HideBag(GameObject[] hidingBag)
    {
        foreach (var current in hidingBag)
        {
            current.gameObject.SetActive(false);
        }
    }

    private void WearBag(string id)
    {
        int index = (int)GetComponent<PenguinBody>().netId;

        if (myBag != null)
        {
            myBag[index % myBag.Length].SetActive(true);
        }
    }
    #endregion

    #endregion

    #region Penguin Equipment
    [Header("Penguin Equipment")]
    [HideInInspector]public GameObject currentItem = default;

    [HideInInspector]public ItemData currentItemData = default;
    #region ItemFunction

    /// <summary>
    /// �κ��丮 ���ã�⿡ �ִ� �������� Ȯ���� �� ���� �˴ϴ�.
    /// </summary>
    /// <param name="currentWheel"></param>
    public void HidCharacterFavoritesCheack()
    {
        currentItemData = toolWheel.returnItemData();
        if (!isSwiming)
        {
            HidController(currentItemData.itemType);
            if (!toolWheel.toolWheelPanel.activeSelf)
            {
                UseCurrentItem(currentItemData.itemType);
            }
        }
    }

    /// <summary>
    /// �κ��丮 ĭ �ȿ� �ִ� �������� �޾� �� �� ���� �˴ϴ�.
    /// </summary>
    /// <param name="_itemData"></param>
    public void HidCharacterInventoryCheack()
    {
        if (isSwiming) { return; }
        if (inventory.currentSlot.slotItemData != null)
        {
            currentItemData = inventory.currentSlot.slotItemData;
            HidController(currentItemData.itemType);
        }
        else return;
    }

    /// <summary>
    /// ��� ������ Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="itemType"></param>
    public void HidController(ItemType itemType)
    {
        //�տ� ���� ����ִٸ� ��Ȱ��ȭ �ϰ� �˻������� �ö�
        if (currentItem != null)
            currentItem.SetActive(false);

        if (characterEquipmentMesh == null) return; // ��ȿ���� ���� id�� ����

        currentItemData.itemType = itemType;

        int itemIndex = GetItemIndex(itemType);
        if (itemIndex >= 0 && itemIndex < characterEquipmentMesh.Length)
        {
            currentItem = characterEquipmentMesh[itemIndex];
            currentItem.SetActive(true);
        }
        else
        {
            currentItem = null; // ��ȿ���� ���� itemType�̸� null�� ����
        }
    }
    private int GetItemIndex(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Fork => 0,
            ItemType.Hammer => 1,
            ItemType.Spoon => 2,
            ItemType.Axe => 3,
            ItemType.BoxingGlove => 4,
            ItemType.Sword => 5,
            ItemType.SlingShot => 6,
            ItemType.Torch => 7,
            ItemType.FishingRod => 8,
            ItemType.Scissors => 9,
            _ => -1 // ��ȿ���� ���� itemType�� -1 ��ȯ
        };
    }

    /// <summary>
    /// �����ۿ� ���� �ִϸ��̼��� ���� �մϴ�.
    /// </summary>
    /// <param name="itemType"></param>
    public void UseCurrentItem(ItemType itemType)
    {
        //�ƹ��͵� �ȵ�� �ְų� ���� ���̶�� �ѱ�.
        if (currentItem == null || isSwiming) return;
        if (isAction || toolWheel.ReturnActive()) return;
        if(body.isOpenUI) { return; }
        StartCoroutine(DelayedAnimation(0.65f));

        switch (itemType)
        {
            //��ũ
            case ItemType.Fork:
                map.GetChunkFromVector3(netPlayer.highLightBlock.position).EditSoilTiling(netPlayer.highLightBlock.position, inventory);
                CmdSetTrigger(aniIsFarming);
                return;
            //��ġ
            case ItemType.Hammer:
                Debug.Log("Use Hammer");
                netPlayer.LoadA();
                PlayWaitingTimeSFXSound("ToolHammer", 0.2f);
                CmdSetTrigger(aniIsMining);
                return;
            //������
            case ItemType.Spoon:
                Debug.Log("Use Spoon");
                netPlayer.LoadA();
                PlayWaitingTimeSFXSound("ToolShovel", 0.2f);
                CmdSetTrigger(aniIsSpoon);
                return;
            //����
            case ItemType.Axe:
                Debug.Log("Use Axe");
                axePrefabCs.OnFelling();
                CmdSetTrigger(aniIsAxe);
                return;
            //��
            case ItemType.Sword:
                if (!isAttack)
                {
                    attackDamagble.OnCollider();
                    isAttack = true;
                    PlaySFXSound("ToolSword");
                    Invoke(nameof(ResetValue), 0.5f);
                    CmdSetTrigger(aniIsSword);
                }
                return;

            case ItemType.BoxingGlove:
                CmdSetTrigger(aniIsPunch);
                inventory.MinusDurability();
                equipmentBoxingGlove.Play();
                return;

            case ItemType.FishingRod:
                equipmentFishing.Play(inventory);
                return;

            case ItemType.SlingShot:
                CmdSetTrigger(aniIsSlingshot);
                Invoke(nameof(Slingshoot), 0.5f);
                return;
            //�ƹ��͵� ������
            default:
                return;
        }
    }
    private void Slingshoot()
    {
        equipmentSlingshot.ShootProjectile(inventory);
    }
    private void ResetValue()
    {
        isAttack = false;
    }

    private IEnumerator DelayedAnimation(float delayTime)
    {
        isAction = true;
        //MoveControllerOnFunction();
        yield return new WaitForSeconds(delayTime);
        //isNotFunction = false;
        isAction = false;
    }

    #endregion

    #endregion

    #region Penguin Load Prefab Data
    public void LoadPrefabData(PenguinPrefabData data)
    {
        animator = data.animator;
        
        myBag = data.myBags;
        characterEquipmentMesh = data.characterEquipmentMesh;

        equipmentSlingshot = data.equipmentSlingshot;
        equipmentSlingshot.SetPenguinBody(body);

        equipmentFishing = data.equipmentFishing;
        equipmentFishing.SetPenguinFunction(this);

        equipmentBoxingGlove = data.equipmentBoxingGlove;

        axePrefabCs = data.diggingWood;
        attackDamagble = data.attackSwordDamagble;
    }
    #endregion

    #region Sound
    public float soundInterval = 0.5f;      //Sound Play Time
    private float timeSinceLastSound = 0f;

    private void PlayMoveSFXSound(string id)
    {
        if (moveVec != Vector3.zero)
        {
            timeSinceLastSound += Time.deltaTime;
            if (timeSinceLastSound >= soundInterval)
            {
                PlaySFXSound(id);
                timeSinceLastSound = 0f;
            }
        }
    }
    private void PlaySFXSound(string id)
    {
        SoundManager.Instance.PlaySFX(id);
    }
    private IEnumerator PlayWaitingTimeSFXSound(string id, float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        PlaySFXSound(id);
    }
    private void PlayStepSound()
    {
        if (groundedVoxelType == Snow) { PlayMoveSFXSound("SnowStep"); }   //���ϰ��              
        else if (groundedVoxelType == TilledSoil ||
                groundedVoxelType == WetTilledSoil ||
                groundedVoxelType == Ground) { PlayMoveSFXSound("SoilStep"); }    //���� ��� ����
    }
    #endregion
}