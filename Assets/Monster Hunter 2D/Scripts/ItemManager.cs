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
    private ItemData currentItem; // 사용 중인 아이템을 저장하기 위한 변수
    private int currentIndex = 0;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sharpnessManager = GetComponent<SharpnessManager>();

        items.Add(new ItemData { itemName = "회복약", itemType = ItemType.Potion, quantity = 10, icon = Resources.Load<Sprite>("Potion") });
        items.Add(new ItemData { itemName = "해독약", itemType = ItemType.Antidote, quantity = 5, icon = Resources.Load<Sprite>("Antidote") });
        items.Add(new ItemData { itemName = "숫돌", itemType = ItemType.Whetstone, quantity = 1, icon = Resources.Load<Sprite>("Whetstone") });

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

            currentItem = items[currentIndex]; //현재 사용 중인 아이템 저장

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

            //아이템 수량이 0이 되면 리스트에서 삭제 (숫돌 제외)
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
