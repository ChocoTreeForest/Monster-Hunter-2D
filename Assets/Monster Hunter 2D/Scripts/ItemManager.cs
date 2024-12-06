using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public Hunter hunter;
    public GameManager manager;
    public HunterStatus status;
    public List<ItemData> items = new List<ItemData>();
    public Image itemSlot;
    public Image itemIcon;
    public Text itemQuantityText;
    public bool itemUsing = false;

    private Animator anim;
    private SharpnessManager sharpnessManager;
    private ItemData currentItem; // ��� ���� �������� �����ϱ� ���� ����
    private int currentIndex = 0;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sharpnessManager = GetComponent<SharpnessManager>();

        items.Add(new ItemData { itemName = "ȸ����", itemType = ItemType.Potion, quantity = 10, icon = Resources.Load<Sprite>("Potion") });
        items.Add(new ItemData { itemName = "�ص���", itemType = ItemType.Antidote, quantity = 5, icon = Resources.Load<Sprite>("Antidote") });
        items.Add(new ItemData { itemName = "����", itemType = ItemType.Whetstone, quantity = 1, icon = Resources.Load<Sprite>("Whetstone") });

        UpdateUI();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
            PreviousItem();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
            NextItem();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseItem();
        }

        if (manager.isAction && itemSlot != null)
        {
            itemSlot.gameObject.SetActive(false);
        }
        else if (itemSlot != null)
        {
            itemSlot.gameObject.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        if (itemSlot == null || itemIcon == null || itemQuantityText == null) return;
        itemIcon.sprite = items[currentIndex].icon;
        itemQuantityText.text = items[currentIndex].quantity.ToString();
    }

    private void PreviousItem()
    {
        currentIndex = (currentIndex > 0) ? currentIndex - 1 : items.Count - 1;
        UpdateUI();
    }

    private void NextItem()
    {
        currentIndex = (currentIndex < items.Count - 1) ? currentIndex + 1 : 0;
        UpdateUI();
    }

    private void UseItem()
    {
        if (items[currentIndex].quantity > 0 && items[currentIndex].itemType != ItemType.Whetstone && !manager.isAction && !hunter.stepping && !itemUsing && hunter.currentState == Hunter.State.Nabdo)
        {
            hunter.SetCanMove(false);
            itemUsing = true;

            currentItem = items[currentIndex]; //���� ��� ���� ������ ����

            if (items[currentIndex].itemType == ItemType.Potion)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.Potion);
                anim.SetTrigger("Heal");
            }
            else if (items[currentIndex].itemType == ItemType.Antidote)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX.Potion);
                anim.SetTrigger("Antidote");
            }

            //������ ������ 0�� �Ǹ� ����Ʈ���� ���� (���� ����)
            if (items[currentIndex].quantity <= 0 && items[currentIndex].itemType != ItemType.Whetstone)
            {
                items.RemoveAt(currentIndex);

                if (currentIndex >= items.Count)
                {
                    currentIndex = Mathf.Clamp(currentIndex, 0, items.Count - 1);
                }

                if (items.Count > 0)
                {
                    UpdateUI();
                }
                else
                {
                    itemIcon.sprite = null;
                }
            }
        }
        else if(items[currentIndex].itemType == ItemType.Whetstone && !manager.isAction && !hunter.stepping && !itemUsing && hunter.currentState == Hunter.State.Nabdo)
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX.Whetstone);
            hunter.SetCanMove(false);
            itemUsing = true;
            anim.SetTrigger("Whetstone");
        }
    }

    public void Potion()
    {
        status.HealHP(50);
        hunter.HealEffect();

        if(currentItem != null && currentItem.itemType == ItemType.Potion)
        {
            currentItem.quantity--;
        }

        UpdateUI();
    }
    public void PotionEnd()
    {
        hunter.SetCanMove(true);
        itemUsing = false;
    }

    public void Antidote()
    {
        status.RemovePoison();
        hunter.AntidoteEffect();

        if (currentItem != null && currentItem.itemType == ItemType.Antidote)
        {
            currentItem.quantity--;
        }

        UpdateUI();
    }
    public void AntidoteEnd()
    {
        hunter.SetCanMove(true);
        itemUsing = false;
    }

    public void Whetstone()
    {
        sharpnessManager.IncreaseSharpness(30);
        hunter.SetCanMove(true);
        itemUsing = false;
    }
}
