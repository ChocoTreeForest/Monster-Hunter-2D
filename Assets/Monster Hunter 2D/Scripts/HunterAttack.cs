using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAttack : MonoBehaviour
{
    public bool moa = false;
    public bool gmoa = false;
    public bool garo = false;
    public bool yup = false;
    public bool inputCooldown = false;
    public float inputCooldownTime = 0.5f;
    public float attackTimer = 0f;
    public AttackState attackState = AttackState.None;
    public GuardDirection guardDirection = GuardDirection.None;
    public int damage;
    public int chargeLevel = -1;

    float chargeTime = -1f;

    private Animator anim;
    private Hunter hunter;
    private const float maxChargeTime = 5f;
    private float attackTimeout = 2f;
    private float damageMultiplier = 1f; // 데미지 배율
    private bool damageApplied; // 데미지 적용 여부
    private HashSet<Rathalos> damagedTargets = new HashSet<Rathalos>(); // 데미지를 받았는지 추적

    public enum AttackState
    {
        None,
        Ready,
        Charging,
        AttackEnd
    }

    public enum GuardDirection
    {
        None,
        FL,
        FR,
        BL,
        BR
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        hunter = GetComponent<Hunter>();
        damageApplied = false;
    }

    void Update()
    {
        if (inputCooldown)
        {
            inputCooldownTime -= Time.deltaTime;
            if (inputCooldownTime <= 0)
            {
                inputCooldown = false;
            }
        }
        else
        {
            AttackInput();
        }

        if (hunter.stepping)
        {
            attackTimer = 2f; //스텝으로 공격 후딜레이 스킵
        }

        AttackTimeout();

        // 애니메이션 태그가 Attack이면 큐 실행

    }

    private void AttackInput()
    {
        if (inputCooldown || hunter.isDead) return;

        if (hunter.currentState == Hunter.State.Baldo)
        {
            if (moa == false && gmoa == false && garo == false && yup == false)
            {
                if (Input.GetButton("Attack1"))
                {
                    MoaChargeAnim();
                }
                else if (Input.GetButtonUp("Attack1"))
                {
                    ChargeAttack();
                    moa = true;
                }

                if (Input.GetButtonDown("Attack2"))
                {
                    GaroAttack();
                    garo = true;
                }

                if (Input.GetButton("Guard"))
                {
                    Guard();
                }
                else if (Input.GetButtonUp("Guard"))
                {
                    anim.SetBool("Guard", false);
                    hunter.SetCanMove(true);
                    guardDirection = GuardDirection.None;
                }
            }
            else if (moa == false && gmoa == false && garo == true && yup == false)
            {
                if (Input.GetButton("Attack1"))
                {
                    MoaChargeAnim();
                }
                else if (Input.GetButtonUp("Attack1"))
                {
                    ChargeAttack();
                    moa = true;
                    garo = false;
                }
            }
            else if (moa == true && gmoa == false && garo == false && yup == false)
            {
                if (Input.GetButtonDown("Attack1"))
                {
                    YupAttack();
                    yup = true;
                }
                else if (Input.GetButtonDown("Attack2"))
                {
                    GaroAttack();
                    garo = true;
                }
            }
            else if ((moa == true && gmoa == false && garo == false && yup == true) || (moa == true && gmoa == false && garo == true && yup == false))
            {
                if (Input.GetButton("Attack1"))
                {
                    GMoaChargeAnim();
                }
                else if (Input.GetButtonUp("Attack1"))
                {
                    ChargeAttack();
                    gmoa = true;
                }
            }
            else if ((moa == true && gmoa == true && garo == false && yup == true) || (moa == true && gmoa == true && garo == true && yup == false))
            {
                if (Input.GetButtonDown("Attack2"))
                {
                    GaroAttack();
                    moa = false;
                    gmoa = false;
                    garo = false;
                    yup = false;
                }
            }
            else
            {
                moa = false;
                gmoa = false;
                garo = false;
                yup = false;
            }
        }
    }

    private void AttackTimeout()
    {
        if (attackState == AttackState.AttackEnd)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= 1f && Input.GetButtonDown("Step") && !hunter.stepping)
            {
                hunter.UseStep();
                ReturnToBaldo();
            }

            if (attackTimer >= attackTimeout) ReturnToBaldo();
        }
    }

    private void MoaChargeAnim()
    {
        hunter.SetCanMove(false);
        anim.SetBool("Charging", true);
        attackState = AttackState.Charging;
        chargeTime += Time.deltaTime;
        attackTimer = 0f;
        if (chargeTime >= chargeLevel && chargeLevel < maxChargeTime)
        {
            if(chargeLevel >= 0 && chargeLevel < 2)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.Charge);
            }
            else if (chargeLevel == 2)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.ChargeMax);
            }

            chargeLevel++;
            anim.SetInteger("MoaCharge", chargeLevel);

            if (chargeLevel >= 5)
            {
                ChargeAttack();
            }
        }
    }

    private void GMoaChargeAnim()
    {
        hunter.SetCanMove(false);
        anim.SetBool("Charging", true);
        attackState = AttackState.Charging;
        chargeTime += Time.deltaTime;
        attackTimer = 0f;
        if (chargeTime >= chargeLevel && chargeLevel < maxChargeTime)
        {
            if (chargeLevel >= 0 && chargeLevel < 2)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.Charge);
            }
            else if (chargeLevel == 2)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.ChargeMax);
            }

            chargeLevel++;
            anim.SetInteger("GMoaCharge", chargeLevel);

            if (chargeLevel >= 5)
            {
                ChargeAttack();
            }
        }
    }

    private void ChargeAttack()
    {
        IsDamaged();
        hunter.SetCanMove(false);
        anim.SetBool("Charging", false);
        attackState = AttackState.AttackEnd;
        attackTimer = 0f;
        chargeTime = -1f;
        chargeLevel = -1;
        inputCooldown = true;
        inputCooldownTime = 0.5f;
        anim.SetInteger("MoaCharge", chargeLevel);
        anim.SetInteger("GMoaCharge", chargeLevel);
    }

    private void GaroAttack()
    {
        IsDamaged();
        hunter.SetCanMove(false);
        anim.SetTrigger("X");
        attackState = AttackState.AttackEnd;
        attackTimer = 0f;
        inputCooldown = true;
        inputCooldownTime = 0.5f;
    }

    private void YupAttack()
    {
        IsDamaged();
        hunter.SetCanMove(false);
        anim.SetTrigger("Z");
        attackState = AttackState.AttackEnd;
        attackTimer = 0f;
        inputCooldown = true;
        inputCooldownTime = 0.5f;
    }

    private void ReturnToBaldo()
    {
        if (hunter.stun) return;

        anim.SetTrigger("EndAttack");
        attackState = AttackState.Ready;
        hunter.SetCanMove(true); //공격 후 이동 가능
        chargeLevel = -1;
        moa = false;
        gmoa = false;
        garo = false;
        yup = false;
        attackTimer = 0f;
    }

    public void SetChargeLevel(int value)
    {
        chargeLevel = value;
    }

    private void Guard()
    {
        hunter.SetCanMove(false);
        anim.SetBool("Guard", true);
        attackTimer = 0f;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Hunter_FL_Guard")) guardDirection = GuardDirection.FL;
        else if (stateInfo.IsName("Hunter_FR_Guard")) guardDirection = GuardDirection.FR;
        else if (stateInfo.IsName("Hunter_BL_Guard")) guardDirection = GuardDirection.BL;
        else if (stateInfo.IsName("Hunter_BR_Guard")) guardDirection = GuardDirection.BR;
    }

    private void IsDamaged()
    {
        damageApplied = false;
        damagedTargets.Clear();
    }

    public void AttackMotion(string attackMotion)
    {
        switch (attackMotion)
        {
            case "Garo": damage = 36; break;
            case "Garo1c": damage = 52; break;
            case "Garo2c": damage = 66; break;
            case "Garo3c": damage = 110; break;
            case "Moa": damage = 48; break;
            case "Moa1c": damage = 65; break;
            case "Moa2c": damage = 77; break;
            case "Moa3c": damage = 110; break;
            case "GMoa": damage = 52; break;
            case "GMoa1c": damage = 70; break;
            case "GMoa2c": damage = 85; break;
            case "GMoa3c": damage = 115; break;
            case "Yup": damage = 25; break;
            case "End": damage = 0; break;
        }
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    public void ApplyDamage(Rathalos rathalos)
    {
        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier); //RoundToInt => 반올림

        if (!damagedTargets.Contains(rathalos) && !damageApplied)
        {
            rathalos.TakeDamage(finalDamage);
            GetComponentInParent<SharpnessManager>().DecreaseSharpness();
            damageApplied = true;
            damagedTargets.Add(rathalos);
        }
    }
}
