using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public GameObject[] choiceOptions; //������ UI ��ҵ�
    public Image[] choiceCursors; //������ Ŀ�� �̹�����
    public GameObject choiceBox;
    public int selectedChoiceIndex = 0; //���� ���õ� ������
    public bool isChoiceActive = false; //������ UI�� Ȱ��ȭ�� ��������

    void Awake()
    {
        gameObject.SetActive(false);
        choiceBox.SetActive(false);
    }
    void Update()
    {
        if (isChoiceActive)
        {
            HandleChoiceInput();
        }
    }

    public void ShowChoices(string[] choices)
    {
        isChoiceActive = true;
        gameObject.SetActive(true);
        choiceBox.SetActive(true);

        for (int i = 0; i < choiceOptions.Length; i++)
        {
            if(i < choices.Length)
            {
                choiceOptions[i].SetActive(true);
                choiceOptions[i].GetComponent<Text>().text = choices[i];
                choiceCursors[i].gameObject.SetActive(i == selectedChoiceIndex); //ó������ ù��° ���������� Ŀ�� ǥ��
            }
            else
            {
                choiceOptions[i].SetActive(false);
                choiceCursors[i].gameObject.SetActive(false); //������ �ʴ� Ŀ�� ��Ȱ��ȭ
            }
        }

        selectedChoiceIndex = 0; //ù��° �������� �ʱ�ȭ
        UpdateChoiceUI();
    }

    void HandleChoiceInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedChoiceIndex = (selectedChoiceIndex - 1 + choiceOptions.Length) % choiceOptions.Length;
            UpdateChoiceUI();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedChoiceIndex = (selectedChoiceIndex + 1) % choiceOptions.Length;
            UpdateChoiceUI();
        }
        else if (Input.GetButtonDown("Attack1"))
        {
            ConfirmChoice();
        }
    }

    void UpdateChoiceUI()
    {
        for(int i = 0; i < choiceOptions.Length; i++)
        {
            choiceCursors[i].gameObject.SetActive(i == selectedChoiceIndex); //���õ� �׸񿡸� Ŀ�� ǥ��
        }
    }

    void ConfirmChoice()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        isChoiceActive = false;
        gameObject.SetActive(false);
        choiceBox.SetActive(false);
        FindObjectOfType<GameManager>().HandleChoice(selectedChoiceIndex);
    }
}
