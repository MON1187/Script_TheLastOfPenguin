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
    private float hAxis;        //입력 x축 저장
    private float vAxis;        //입력 z축 저장

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
            //초기화
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
    private bool isPlaySlide = false;   //슬라이드 중인지 확인용
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
        CmdSetBool(aniIsSliding, true);  //애니메이션 실행, 맞는 이름으로 변경 하면됨.
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

    //임시 추후 옮김
    private const string GroundTag = nameof(Ground);

    //Adjust movement according to changed view
    private void SetAngle()
    {
        Transform cameraTransform = Camera.main.transform;
        forward = cameraTransform.forward;
        right = cameraTransform.right;

        // 수평 이동만 필요하므로 y 값을 0으로 설정
        forward.y = 0f;
        right.y = 0f;

        // 정규화하여 방향 벡터 길이 1로 유지
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

    public DiggingWood axePrefabCs;             //도끼
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
    /// 인벤토리 즐겨찾기에 있는 아이텝을 확인한 후 실행 됩니다.
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
    /// 인벤토리 칸 안에 있는 아이템을 받아 온 후 실행 됩니다.
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
    /// 장비 아이템 활성화/비활성화
    /// </summary>
    /// <param name="itemType"></param>
    public void HidController(ItemType itemType)
    {
        //손에 뭔가 들고있다면 비활성화 하고 검색문으로 올라감
        if (currentItem != null)
            currentItem.SetActive(false);

        if (characterEquipmentMesh == null) return; // 유효하지 않은 id는 무시

        currentItemData.itemType = itemType;

        int itemIndex = GetItemIndex(itemType);
        if (itemIndex >= 0 && itemIndex < characterEquipmentMesh.Length)
        {
            currentItem = characterEquipmentMesh[itemIndex];
            currentItem.SetActive(true);
        }
        else
        {
            currentItem = null; // 유효하지 않은 itemType이면 null로 설정
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
            _ => -1 // 유효하지 않은 itemType은 -1 반환
        };
    }

    /// <summary>
    /// 아이템에 따라 애니메이션을 실행 합니다.
    /// </summary>
    /// <param name="itemType"></param>
    public void UseCurrentItem(ItemType itemType)
    {
        //아무것도 안들고 있거나 수영 중이라면 넘김.
        if (currentItem == null || isSwiming) return;
        if (isAction || toolWheel.ReturnActive()) return;
        if(body.isOpenUI) { return; }
        StartCoroutine(DelayedAnimation(0.65f));

        switch (itemType)
        {
            //포크
            case ItemType.Fork:
                map.GetChunkFromVector3(netPlayer.highLightBlock.position).EditSoilTiling(netPlayer.highLightBlock.position, inventory);
                CmdSetTrigger(aniIsFarming);
                return;
            //망치
            case ItemType.Hammer:
                Debug.Log("Use Hammer");
                netPlayer.LoadA();
                PlayWaitingTimeSFXSound("ToolHammer", 0.2f);
                CmdSetTrigger(aniIsMining);
                return;
            //숟가락
            case ItemType.Spoon:
                Debug.Log("Use Spoon");
                netPlayer.LoadA();
                PlayWaitingTimeSFXSound("ToolShovel", 0.2f);
                CmdSetTrigger(aniIsSpoon);
                return;
            //도끼
            case ItemType.Axe:
                Debug.Log("Use Axe");
                axePrefabCs.OnFelling();
                CmdSetTrigger(aniIsAxe);
                return;
            //검
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
            //아무것도 없을시
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
        if (groundedVoxelType == Snow) { PlayMoveSFXSound("SnowStep"); }   //눈일경우              
        else if (groundedVoxelType == TilledSoil ||
                groundedVoxelType == WetTilledSoil ||
                groundedVoxelType == Ground) { PlayMoveSFXSound("SoilStep"); }    //땅일 경우 실행
    }
    #endregion
}