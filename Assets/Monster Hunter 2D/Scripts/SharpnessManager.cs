using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharpnessManager : MonoBehaviour
{
    public int maxSharpness = 60;
    public int currentSharpness;
    public Image sharpnessIcon;
    public Color greenSharpness = Color.green;
    public Color yellowSharpness = Color.yellow;
    public Color orangeSharpness = new Color(1f, 0.5f, 0f); //ÁÖÈ²»ö
    public Color redSharpness = Color.red;

    private HunterAttack hunterAttack;

    void Awake()
    {
        currentSharpness = maxSharpness;
        hunterAttack = GetComponent<HunterAttack>();
        UpdateSharpnessUI();
    }

    public void DecreaseSharpness()
    {
        if (currentSharpness > 0)
        {
            currentSharpness--;
            currentSharpness = Mathf.Clamp(currentSharpness, 0, maxSharpness);
            UpdateSharpnessUI();
        }
    }

    public void IncreaseSharpness(int amount)
    {
        currentSharpness += amount;
        currentSharpness = Mathf.Clamp(currentSharpness, 0, maxSharpness);
        UpdateSharpnessUI();
    }

    private void UpdateSharpnessUI()
    {
        if (currentSharpness > 40)
        {
            sharpnessIcon.color = greenSharpness;
            hunterAttack.SetDamageMultiplier(1f);
        }
        else if (currentSharpness > 20)
        {
            sharpnessIcon.color = yellowSharpness;
            hunterAttack.SetDamageMultiplier(0.75f);
        }
        else if (currentSharpness > 0)
        {
            sharpnessIcon.color = orangeSharpness;
            hunterAttack.SetDamageMultiplier(0.5f);
        }
        else
        {
            sharpnessIcon.color = redSharpness;
            hunterAttack.SetDamageMultiplier(0.01f);
        }
    }
}
