using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static HunterAttack;

public class Hunter : MonoBehaviour
{
    public Vector2 inputVec;
    public State currentState = State.Nabdo; //현재 헌터 상태를 나타내는 변수
    public GameManager manager;
    public HunterStatus status;
    public GameManager gameManager;
    public GameObject burnEffect;
    public GameObject poisonEffect;
    public GameObject healEffect;
    public GameObject antidoteEffect;
    public Transform effect;
    public float speed;
    public float stepSpeed = 20f; 
    public float stepDuration = 0.1f;
    public int deathCount = 0;
    public bool stepping = false;
    public bool isDead = false;
    public bool stun = false;

    public enum State
    {
        Baldo,
        Nabdo,
    }

    private bool canMove = true;
    private bool baldoing = false;
    private bool nabdoing = false;
    private bool isNPCDetected;

    private Rigidbody2D rigid;
    private Animator anim;
    private Vector3 dirVec;
    private Vector3 initialPosition;
    private Vector2 stepDir; //스텝 방향
    private GameObject talkObject;
    private HunterAttack hunterAttack;
    private ItemManager itemManager;
    private string currentScene;
    private float originalSpeed; //스텝 사용 전 이동 속도 저장
    private float stepCooldown = 0.5f;
    private bool stepOnCooldown = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hunterAttack = GetComponent<HunterAttack>();
        itemManager = GetComponent<ItemManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 현재 씬 이름을 가져와서 저장
        currentScene = SceneManager.GetActiveScene().name;

        originalSpeed = speed;
        initialPosition = transform.position;
    }

    void Update()
    {
        //대화 중 애니메이션 정지
        if (manager.isAction)
        {
            anim.speed = 0;
        }
        else
        {
            anim.speed = 1;
        }

        //Direction
        if (!stepping && !manager.isAction)
        {
            if (inputVec.y == 1)
                dirVec = Vector3.up;
            else if (inputVec.y == -1)
                dirVec = Vector3.down;
            else if (inputVec.x == -1)
                dirVec = Vector3.left;
            else if (inputVec.x == 1)
                dirVec = Vector3.right;
        }

        //발도
        if (Input.GetButtonDown("Attack1") && canMove && currentState == State.Nabdo && !IsInVillageScene() && !isNPCDetected) Baldo();

        //납도
        if (Input.GetKeyDown(KeyCode.LeftShift) && canMove && currentState == State.Baldo) Nabdo();

        //대화
        if (Input.GetButtonDown("Attack1") && talkObject != null && currentState == State.Nabdo)
            manager.Action(talkObject);

        UseStep();
    }

    void FixedUpdate()
    {
        if (canMove && !nabdoing && !baldoing && !manager.isAction && !stepping)
        {
            // 위치 이동
            Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }

        //Raycast
        Debug.DrawRay(rigid.position, dirVec * 1.0f, Color.yellow);
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1.0f, LayerMask.GetMask("CanTalk"));

        if (rayHit.collider != null)
        {
            talkObject = rayHit.collider.gameObject;
            isNPCDetected = true; //NPC 감지
        }
        else
        {
            talkObject = null;
            isNPCDetected = false;
        }

        if (canMove && !status.running && currentState == State.Nabdo)
        {
            speed = 4;
        }
        
        if(canMove && !status.running && currentState == State.Baldo)
        {
            speed = 2;
        }
        
        if(!canMove)
        {
            speed = 0;
        }

        Running();
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        //방향 별 애니메이션 재생
        if (!manager.isAction)
        {
            anim.SetFloat("Speed", inputVec.magnitude);
            anim.SetInteger("hAxisRaw", (int)inputVec.x);
            anim.SetInteger("vAxisRaw", (int)inputVec.y);
        }
    }

    void Baldo()
    {
        if (currentState == State.Nabdo && !manager.isAction)
        {
            //발도 애니메이션 재생
            anim.SetTrigger("Baldo");
            currentState = State.Baldo;
            hunterAttack.attackState = HunterAttack.AttackState.Ready;
            hunterAttack.SetChargeLevel(-1);
            canMove = false;
            baldoing = true;
            hunterAttack.inputCooldown = true;
            hunterAttack.inputCooldownTime = 0.5f;
        }// 발도 상태에서는 아무 작업도 하지 않음
        else if (currentState == State.Baldo) return;
    }

    void Nabdo()
    {
        if (currentState == State.Baldo)
        {
            //납도 애니메이션 재생
            anim.SetTrigger("Nabdo");
            currentState = State.Nabdo;
            hunterAttack.attackState = HunterAttack.AttackState.None;
            hunterAttack.SetChargeLevel(-1);
            canMove = false;
            nabdoing = true;
            hunterAttack.inputCooldown = true;
            hunterAttack.inputCooldownTime = 0.5f;
        }// 납도 상태에서는 아무 작업도 하지 않음
        else if (currentState == State.Nabdo) return;
    }

    void Running()
    {
        if (IsInVillageScene())
        {
            if (Input.GetKey(KeyCode.LeftShift) && !manager.isAction)
            {
                speed = 6;
            }
            else
            {
                speed = 4;
            }
        }
        else
        {
            if (currentState == State.Nabdo && Input.GetKey(KeyCode.LeftShift) && !manager.isAction && inputVec != Vector2.zero && status.currentSP > 0 && speed > 0)
            {
                status.running = true;
                if (status.currentSP > 25)
                {
                    speed = 6;
                    status.UseSP(0.3f);
                }
                else if (status.currentSP <= 25)
                {
                    speed = 2;
                    status.UseSP(0.3f);
                }
            }
            else
            {
                status.running = false;
                if (status.currentSP != status.maxSP)
                {
                    status.healingSP = true;
                }
                else if (status.currentSP == status.maxSP)
                {
                    status.healingSP = false;
                }

                if (currentState != State.Baldo)
                {
                    speed = 4;
                }
            }
        }
    }

    public void UseStep()
    {
        if (stepOnCooldown)
        {
            stepCooldown -= Time.deltaTime;
            if (stepCooldown <= 0)
            {
                stepOnCooldown = false;
                stepCooldown = 0.5f; //쿨타임 리셋
            }
        }

        if (Input.GetButtonDown("Step") && !stepping && !stepOnCooldown && !isDead && !manager.isAction && !itemManager.itemUsing && !stun && status.currentSP >= status.stepCost && (canMove || hunterAttack.attackTimer >= 1f))
        {
            SetCanMove(true);
            stepOnCooldown = true;
            //스텝 입력 시 방향 고정
            stepDir = inputVec.normalized; //스텝 시작 시 방향 저장
            if (stepDir == Vector2.zero)
                stepDir = dirVec; //방향키 입력이 없을 경우 마지막 방향 사용

            StartCoroutine(Step());
        }
    }

    IEnumerator Step()
    {
        stepping = true;
        status.StartStep(); //스텝 시 스테미나 감소 및 무적 적용

        float startTime = Time.time;

        //스텝 지속시간 동안 방향키를 떼도 이동
        while (Time.time < startTime + stepDuration)
        {
            rigid.MovePosition(rigid.position + stepDir * stepSpeed * Time.fixedDeltaTime); //방향키와 상관 없이 방향 유지
            yield return new WaitForFixedUpdate();
        }

        stepping = false;
        speed = originalSpeed;
    }

    public IEnumerator BlinkEffect()
    {
        if (status.currentHP <= 0) yield break;

        float blinkDuration = 1.0f;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.1f * 2;
        }
        spriteRenderer.color = Color.white;
    }

    public IEnumerator BurnEffect()
    {
        while (status.isBurned)
        {
            GameObject burnInstance = Instantiate(burnEffect, effect.position, Quaternion.identity, transform);
            Destroy(burnInstance, 0.4f);
            yield return new WaitForSeconds(5f);
        }
    }

    public IEnumerator PoisonEffect()
    {
        while (status.isPoisoned)
        {
            GameObject poisonInstance = Instantiate(poisonEffect, effect.position, Quaternion.identity, transform);
            Destroy(poisonInstance, 0.4f);
            yield return new WaitForSeconds(5f);
        }
    }

    public void HealEffect()
    {
        GameObject healInstance = Instantiate(healEffect, effect.position, Quaternion.identity, transform);
        Destroy(healInstance, 0.5f);
    }

    public void AntidoteEffect()
    {
        GameObject antidoteInstance = Instantiate(antidoteEffect, effect.position, Quaternion.identity, transform);
        Destroy(antidoteInstance, 0.5f);
    }

    public void TriggerRoarStun(float delay, float stunDuration)
    {
        if (!stun && !status.invincible && !isDead && hunterAttack.guardDirection == HunterAttack.GuardDirection.None)
        {
            StartCoroutine(RoarStun(delay, stunDuration));
        }
    }

    private IEnumerator RoarStun (float delay, float stunDuration)
    {
        yield return new WaitForSeconds(delay);

        stun = true;
        canMove = false;

        if(currentState == State.Baldo)
        {
            anim.Play("Hunter_FR_Baldo_Stand");
            anim.ResetTrigger("EndAttack");
            hunterAttack.attackState = AttackState.Ready;
            hunterAttack.chargeLevel = -1;
            hunterAttack.attackTimer = 2f;
            hunterAttack.moa = false;
            hunterAttack.gmoa = false;
            hunterAttack.garo = false;
            hunterAttack.yup = false;
        }
        else
        {
            anim.Play("Hunter_FR_Stand");
        }

        float elapsed = 0f;
        Vector3 originalPosition = transform.position;
        while (elapsed < stunDuration)
        {
            float shake = 0.2f * Mathf.Sin(Time.time * 50f); //흔들림 강도와 속도
            transform.position = new Vector3(originalPosition.x + shake, originalPosition.y, originalPosition.z);

            elapsed += Time.fixedDeltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        stun = false;
        canMove = true;
    }

    //발, 납도 애니메이션 이벤트에서 호출할 함수
    public void EnableMovement()
    {
        canMove = true;
        baldoing = false;
        nabdoing = false;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    private bool IsInVillageScene()
    {
        return currentScene == "VillageScene";
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        status.redHP = 0;
        anim.SetTrigger("Dead");
        canMove = false;
        deathCount++;

        if (deathCount < 3)
        {
            StartCoroutine(gameManager.HunterDead());
        }
        else
        {
            StartCoroutine(gameManager.QuestFail());
        }
    }

    public void Revive()
    {
        transform.position = initialPosition;
        canMove = true;
        currentState = State.Nabdo;
        anim.Play("Hunter_FR_Stand");
        isDead = false;
        status.Restore();
    }
}
