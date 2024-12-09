using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
#endif

public class Rathalos : MonoBehaviour
{
    public float speed;
    public float detectionRadius = 8f;
    public float detectionTime = 10f;
    public float roarDuration = 3f;
    public float randomMoveDuration = 2f;
    public float randomMoveCooldown = 5f;
    public float breathSpeed = 8f;
    public float maxBreathAngle = 5f;
    public float minBreathDistance = 5f;
    public float stunDuration = 8f;
    public float roarCooldown = 100f;
    public float roarRange = 8f;
    public int hp = 3000;
    public int damage;
    public bool canMove = true;
    public bool isAttacking = false;
    public bool hunterInBiteRange = false;
    public bool hunterInClawRange = false;
    public bool hunterInTailRange = false;
    public bool isStunned = false;
    public Rigidbody2D target;
    public LayerMask hunterLayer;
    public State currentState = State.Idle;
    public Direction currentDirection = Direction.Other;
    public HashSet<string> availableAttacks = new HashSet<string>(); //���� ������ ������ ����
    public GameObject breathPrefab;
    public Transform breathSpawnPoint; // �극�� �߻� ��ġ
    public Transform hunterTransform;
    public GameManager gameManager;

    private bool isLive = true;
    private bool isMovingRandomly = false;
    private bool isHunterDetected = false;
    private bool secondTailEnd = false;
    private bool isRoaring = false;
    private bool hunterDead = false;
    private bool bgmChanged = false;
    private int accumDamage = 0;
    private float detectionTimer = 0f;
    private float roarTimer = 0f;
    private Rigidbody2D rigid;
    private Animator anim;
    private Vector2 lastPosition;
    private Vector2 currentVelocity;
    private RathalosAttack attack;
    private SpriteRenderer spriteRenderer;

    public enum State
    {
        Idle,
        Detecting,
        Chasing,
        Attacking
    }

    public enum Direction
    {
        L,
        R,
        Other
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        attack = GetComponent<RathalosAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!isLive) return;

        if (currentState == State.Idle)
        {
            bgmChanged = false;
            CheckHunter();
            if (!isMovingRandomly) StartCoroutine(RandomMove());
        }
        else if (currentState == State.Detecting && !hunterDead)
        {
            if (!bgmChanged)
            {
                bgmChanged = true;
                AudioManager.instance.StopBGM();
                AudioManager.instance.PlayBGM(AudioManager.BGM.RathalosBGM);
            }
            StartCoroutine(RoarAndChase());
        }
        else if (currentState == State.Chasing && canMove && !hunterDead)
        {
            ChaseHunter();
        }

        roarTimer += Time.fixedDeltaTime;
        if(roarTimer >= roarCooldown && !isStunned && !isAttacking)
        {
            StartCoroutine(PerformRoar());
        }

        // ���� �ӵ��� ���� ����Ͽ� currentVelocity�� ����
        currentVelocity = (rigid.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = rigid.position;
    }

    void LateUpdate()
    {
        // ���� �ӵ��� ������� Speed �Ķ���� ����
        anim.SetFloat("Speed", currentVelocity.magnitude);
    }

    void CheckHunter()
    {
        if (hunterDead) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, hunterLayer);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<Rigidbody2D>() == target)
                {
                    isHunterDetected = true;
                    break;
                }
            }
        }
        else
        {
            isHunterDetected = false;
        }

        if (isHunterDetected)
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionTime)
            {
                currentState = State.Detecting;
                StopCoroutine(RandomMove());
            }
        }
        else
        {
            detectionTimer = 0f;
        }
    }

    IEnumerator RoarAndChase()
    {
        if (isRoaring) yield break;

        isRoaring = true;
        anim.SetTrigger("Roar");
        canMove = false;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, roarRange, hunterLayer);
        foreach (var hitCollider in hitColliders)
        {
            Hunter hunter = hitCollider.GetComponent<Hunter>();
            if (hunter != null && !hunter.status.invincible)
            {
                hunter.TriggerRoarStun(1f, 3f);
            }
        }

        yield return new WaitForSeconds(roarDuration); // ��ȿ�ϴ� ���� ���

        currentState = State.Chasing;
        canMove = true;
        isRoaring = false;
    }

    IEnumerator PerformRoar()
    {
        if (isRoaring) yield break;

        isRoaring = true;
        anim.SetTrigger("Roar");
        canMove = false;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, roarRange, hunterLayer);
        foreach(var hitCollider in hitColliders)
        {
            Hunter hunter = hitCollider.GetComponent<Hunter>();
            if(hunter != null && !hunter.status.invincible)
            {
                hunter.TriggerRoarStun(1f, 3f);
            }
        }

        yield return new WaitForSeconds(roarDuration); // ��ȿ�ϴ� ���� ���

        currentState = State.Chasing;
        canMove = true;
        isRoaring = false;
        roarTimer = 0f;
    }

    public void RoarSFX()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosRoar);
    }

    void ChaseHunter()
    {
        Vector2 dirVec = target.position - rigid.position; // ���� ����
        if (canMove)
        {
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
            UpdateDirection(dirVec);
        }
    }

    void UpdateDirection(Vector2 direction)
    {
        // ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Mathf.Atan2(float y, float x) : (0,0)�� (x,y)�� �����ϴ� ���� ���⸦ ����Ͽ� �� ���� ������ ���� ������ ��ȯ. ������ x���� �������� �ݽð� �������� ����
        // Mathf.Rad2Deg : ������ ��(degree)�� ��ȯ�ϴ� ����� �� 57.2958 (1 ����  57.2958��)

        // ������ ���� �ִϸ��̼� Ʈ���� ����
        if (angle > -22.5f && angle <= 22.5f)
        {
            anim.SetTrigger("R");
            currentDirection = Direction.R;
        }
        else if (angle > 22.5f && angle <= 67.5f)
        {
            anim.SetTrigger("BR");
            currentDirection = Direction.Other;
        }
        else if (angle > 67.5f && angle <= 112.5f)
        {
            anim.SetTrigger("B");
            currentDirection = Direction.Other;
        }
        else if (angle > 112.5f && angle <= 157.5f)
        {
            anim.SetTrigger("BL");
            currentDirection = Direction.Other;
        }
        else if ((angle > 157.5f && angle <= 180f) || (angle > -180f && angle <= -157.5f))
        {
            anim.SetTrigger("L");
            currentDirection = Direction.L;
        }
        else if (angle > -157.5f && angle <= -112.5f)
        {
            anim.SetTrigger("FL");
            currentDirection = Direction.Other;
        }
        else if (angle > -112.5f && angle <= -67.5f)
        {
            anim.SetTrigger("F");
            currentDirection = Direction.Other;
        }
        else if (angle > -67.5f && angle <= -22.5f)
        {
            anim.SetTrigger("FR");
            currentDirection = Direction.Other;
        }
    }

    IEnumerator RandomMove()
    {
        isMovingRandomly = true;
        while (currentState == State.Idle)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float moveDuration = Random.Range(1f, randomMoveDuration);

            for (float timer = 0; timer < moveDuration; timer += Time.fixedDeltaTime)
            {
                if (currentState != State.Idle) yield break;
                Vector2 nextVec = randomDirection * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec);
                UpdateDirection(randomDirection);
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(randomMoveCooldown); // ������ �̵� ��Ÿ�ӵ��� ���
        }
        isMovingRandomly = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roarRange);
    }

    public void TakeDamage(int damage)
    {
        if (!isLive) return;

        hp -= damage;
        accumDamage += damage;

        AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosHit);
        StartCoroutine(BlinkEffect());

        if (accumDamage >= 750)
        {
            if (isStunned)
            {
                accumDamage = 0;
                return;
            }

            StartCoroutine(Stun());
            accumDamage = 0;
        }

        if (hp <= 0)
        {
            Die();
        }

        if (currentState == State.Idle)
        {
            currentState = State.Detecting;
            StopCoroutine(RandomMove());
        }
    }

    private IEnumerator BlinkEffect()
    {
        if (hp <= 0) yield break;

        spriteRenderer.color = Color.black;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator Stun()
    {
        isStunned = true;
        isAttacking = false;
        canMove = false;
        speed = 0;
        anim.SetTrigger("Stun");

        yield return new WaitForSeconds(stunDuration);

        currentState = State.Chasing;
        speed = 3;
        anim.SetTrigger("Stand");
        isStunned = false;
        canMove = true;
    }

    private void Die()
    {
        //���콺 �״� �ִϸ��̼�, 20�� �� ��ȯ�մϴ� �ؽ�Ʈ ��� �� ���������� �̵�
        if (!isLive) return;

        anim.SetTrigger("Dead");
        isLive = false;
        canMove = false;
        StopAllCoroutines();

        StartCoroutine(gameManager.RathalosDead());
    }

    public string GetRandomAttack()
    {       
        int randomIndex = Random.Range(0, availableAttacks.Count);
        string[] attackArray = new string[availableAttacks.Count];
        availableAttacks.CopyTo(attackArray);
        return attackArray[randomIndex];
    }

    public IEnumerator PerformAttack(string attackType)
    {
        if (isAttacking || isStunned) yield break;
        canMove = false;
        speed = 0;
        currentState = State.Attacking;

        anim.SetTrigger(attackType);
        isAttacking = true;

        yield return null;
    }

    private void BreathAttack()
    {
        //�극�� ������ �ν��Ͻ�ȭ
        GameObject breath = Instantiate(breathPrefab, breathSpawnPoint.position, Quaternion.identity);
                
        Vector2 direction = (target.position - (Vector2)breathSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleLimit = Mathf.Clamp(angle, -maxBreathAngle, maxBreathAngle); //�극�� ���� ����
        float finalAngle = angleLimit * Mathf.Deg2Rad;
        Vector2 limitedDir = new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle));

        breath.GetComponent<Rigidbody2D>().velocity = direction * breathSpeed;

        breath.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 270));
    }

    public void TailAttackStart()
    {
        isAttacking = true;
        canMove = false;
        speed = 0;
        currentState = State.Attacking;
    }

    public void TailAttackEnd()
    {
        if (isStunned) return;

        if (!secondTailEnd)
        {
            StartCoroutine(SecondTailAttack());
        }
        else
        {
            secondTailEnd = false;
            StartCoroutine(Stay(1.0f));
        }
    }

    private IEnumerator SecondTailAttack()
    {
        yield return new WaitForSeconds(0.5f);

        secondTailEnd = true;
        anim.SetTrigger("Tail2");
    }

    private IEnumerator Stay(float delay)
    {
        if (isStunned) yield break;

        yield return new WaitForSeconds(delay);

        canMove = true;
        speed = 3;

        if (!hunterDead)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }

        isAttacking = false;
    }

    public void AttackEnd()
    {
        StartCoroutine(Stay(0.5f));
    }

    public void AttackMotion(string attackMotion)
    {
        switch (attackMotion)
        {
            case "Bite":
                damage = 15;
                AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosBite);
                break;
            case "Claw":
                damage = 25;
                AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosClaw);
                break;
            case "Tail":
                damage = 30;
                AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosTail);
                break;
            case "Breath":
                damage = 0;
                AudioManager.instance.PlaySFX(AudioManager.SFX.RathalosBreath);
                break;
            case "End":
                damage = 0; break;
        }
    }

    public void ApplyDamage(Hunter hunter)
    {
        hunter.status.TakeDamage(damage, GetAttackDirection());
    }

    public HunterAttack.GuardDirection GetAttackDirection()
    {
        Vector2 direction = hunterTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45) return HunterAttack.GuardDirection.FL;
        else if (angle > 45 && angle <= 135) return HunterAttack.GuardDirection.FR;
        else if (angle > -135 && angle <= -45) return HunterAttack.GuardDirection.BL;
        else return HunterAttack.GuardDirection.BR;
    }

    public void StateToIdle()
    {
        StopAllCoroutines();
        hunterDead = true;
        currentState = State.Idle;
    }

    public void HunterRevive()
    {
        hunterDead = false;
    }
}
