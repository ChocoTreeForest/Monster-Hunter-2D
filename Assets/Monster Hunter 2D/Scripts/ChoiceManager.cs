using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public GameObject[] choiceOptions; //선택지 UI 요소들
    public Image[] choiceCursors; //선택지 커서 이미지들
    public GameObject choiceBox;
    public int selectedChoiceIndex = 0; //현재 선택된 선택지
    public bool isChoiceActive = false; //선택지 UI가 활성화된 상태인지

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
                choiceCursors[i].gameObject.SetActive(i == selectedChoiceIndex); //처음에는 첫번째 선택지에만 커서 표시
            }
            else
            {
                choiceOptions[i].SetActive(false);
                choiceCursors[i].gameObject.SetActive(false); //사용되지 않는 커서 비활성화
            }
        }

        selectedChoiceIndex = 0; //첫번째 선택지로 초기화
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
            choiceCursors[i].gameObject.SetActive(i == selectedChoiceIndex); //선택된 항목에만 커서 표시
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
