using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HunterAttack;

public class HunterStatus : MonoBehaviour
{
    public int maxHP = 150;
    public int currentHP;
    public int redHP = 0;

    public int maxSP = 150;
    public float currentSP;

    public float stepCost = 25f; //���� ���׹̳� �Ҹ�

    public Slider hpBar;
    public Slider redHPBar;
    public Slider spBar;

    public bool healingSP = false;
    public bool running = false; //�޸��� ������ üũ
    public bool stepping = false; //������ ����ߴ��� üũ
    public bool invincible = false; //�������� üũ
    public bool isBurned = false;
    public bool isPoisoned = false;

    public GameObject rathalos;
    public GameObject burnIcon;
    public GameObject poisonIcon;
    public Rigidbody2D hunterRb;

    public Hunter hunter;
    public HunterAttack attack;

    private int stepCounter = 0;

    private Coroutine redHPHeal;
    private Coroutine burnCoroutine;
    private Coroutine poisonCoroutine;

    void Awake()
    {
        currentHP = maxHP;
        currentSP = maxSP;

        UpdateUI();
    }

    void FixedUpdate()
    {
        if (healingSP && currentSP < maxSP && !running && !stepping && attack.guardDirection == GuardDirection.None)
        {
            HealSP(0.3f); //���׹̳� ȸ�� �ӵ�
        }

        UpdateUI();
    }

    public void TakeDamage(int damage, HunterAttack.GuardDirection attackDirection)
    {
        if (invincible && damage > 1) return; //���� ������ �� ������ ����

        if (attack.guardDirection != HunterAttack.GuardDirection.None && CanGuard(attackDirection) && currentSP >= 25 && damage > 1)
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX.Guard);
            damage = Mathf.RoundToInt(damage * 0.25f);
            UseSP(25f);
        } // ���� �� ���� + ���׹̳� �Ҹ�

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); //ü���� ������ �������� �ʰ� maxHealth�� ���� �ʰ�

        if(currentHP <= 0)
        {
            redHP = 0;
            RemoveBurn();
            RemovePoison();
            hunter.Die();
            UpdateUI();
        }

        if (damage >= 3 && currentHP > 0)
        {
            if (attack.guardDirection == HunterAttack.GuardDirection.None) AudioManager.instance.PlaySFX(AudioManager.SFX.HunterHit);

            StartCoroutine(Damaged());

            if (redHPHeal != null)
            {
                StopCoroutine(redHPHeal);
                redHP = 0;
            }

            redHP = (damage / 2) + currentHP;

            redHPHeal = StartCoroutine(RedHPHeal());

            UpdateUI();
        }

        if (isBurned && redHP > currentHP)
        {
            redHP -= damage;
        }

        UpdateUI();
    }

    private bool CanGuard(HunterAttack.GuardDirection attackDirection)
    {
        switch (attack.guardDirection)
        {
            case HunterAttack.GuardDirection.FR:
            case HunterAttack.GuardDirection.BR:
                return attackDirection == HunterAttack.GuardDirection.FR || attackDirection == HunterAttack.GuardDirection.BR;
            case HunterAttack.GuardDirection.FL:
            case HunterAttack.GuardDirection.BL:
                return attackDirection == HunterAttack.GuardDirection.FL || attackDirection == HunterAttack.GuardDirection.BL;
            default:
                return false;
        }
    }

    public void ApplyBurn()
    {
        if (isBurned || invincible) return;

        isBurned = true;
        burnIcon.SetActive(true);
        stepCounter = 0;
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        burnCoroutine = StartCoroutine(BurnDamageCoroutine());
        StartCoroutine(hunter.BurnEffect());
    }

    

    public void RemoveBurn()
    {
        isBurned = false;
        burnIcon.SetActive(false);
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        StopCoroutine(hunter.BurnEffect());
    }

    private IEnumerator BurnDamageCoroutine()
    {
        while (isBurned)
        {
            yield return new WaitForSeconds(2f);
            TakeDamage(1, HunterAttack.GuardDirection.None);
        }
    }

    public void ApplyPoison()
    {
        if (isPoisoned) return;

        isPoisoned = true;
        poisonIcon.SetActive(true);
        if (poisonCoroutine != null) StopCoroutine(poisonCoroutine);
        poisonCoroutine = StartCoroutine(PoisonDamageCoroutine());
        StartCoroutine(hunter.PoisonEffect());
    }

    public void RemovePoison()
    {
        isPoisoned = false;
        poisonIcon.SetActive(false);
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        StopCoroutine(hunter.PoisonEffect());
    }

    private IEnumerator PoisonDamageCoroutine()
    {
        for (float timer = 0f; timer < 30f; timer += 2f)
        {
            TakeDamage(1, HunterAttack.GuardDirection.None);
            if (redHP == 0) redHP = currentHP + 1;

            yield return new WaitForSeconds(2f);
        }

        RemovePoison();
    }

    private IEnumerator Damaged()
    {
        invincible = true;
        StartCoroutine(hunter.BlinkEffect());

        yield return new WaitForSeconds(1.0f);

        invincible = false;
    }

    private IEnumerator RedHPHeal()
    {
        while (redHP > 0)
        {
            yield return new WaitForSeconds(2f);

            if(currentHP < redHP)
            {
                currentHP += 1;
                UpdateUI();
            }
        }

        redHPHeal = null;
    }

    public void HealHP(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); //���׹̳��� ������ �������� �ʰ� maxStamina�� ���� �ʰ�
        UpdateUI();
    }

    public void UseSP(float amount)
    {
        currentSP -= amount;
        currentSP = Mathf.Clamp(currentSP, 0, maxSP); //���׹̳��� ������ �������� �ʰ� maxStamina�� ���� �ʰ�
        UpdateUI();
    }

    void HealSP(float amount)
    {
        currentSP += amount;
        currentSP = Mathf.Clamp(currentSP, 0, maxSP); //���׹̳��� ������ �������� �ʰ� maxStamina�� ���� �ʰ�
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpBar != null) hpBar.value = (float)currentHP / maxHP;
        if (redHPBar != null) redHPBar.value = (float)redHP / maxHP; ;
        if (spBar != null) spBar.value = currentSP / maxSP;

        if (redHP <= currentHP)
        {
            redHP = 0;
        }
    }

    public void StartStep()
    {
        if(currentSP >= stepCost && !stepping)
        {
            UseSP(stepCost);
            StartCoroutine(Step());
            OnStepAction();
        }
    }

    IEnumerator Step()
    {
        stepping = true;
        invincible = true;

        //0.3�ʰ� ����
        yield return new WaitForSeconds(0.3f);
        invincible = false;

        //0.49�� �� ���� ���׹̳� ȸ��
        yield return new WaitForSeconds(0.19f);
        stepping = false;
    }

    public void OnStepAction()
    {
        if (!isBurned) return;

        stepCounter++;
        if(stepCounter >= 3)
        {
            RemoveBurn();
        }
    }

    public void Restore()
    {
        currentHP = maxHP;
        currentSP = maxSP;

        UpdateUI();
    }
}
