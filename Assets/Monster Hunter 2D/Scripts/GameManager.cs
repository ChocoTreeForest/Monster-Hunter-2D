using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TalkManager talkManager;
    public Hunter hunter;
    public Rathalos rathalos;
    public Animator namePanel;
    public Animator talkPanel;
    public Animator systemPanel;
    public Text nameText;
    public Text systemText;
    public TypeEffect talk;
    public GameObject talkObject;
    public ChoiceManager choiceManager;
    public GameStatus gameStatus;
    public Image fadeImage;
    public GameObject escPanel;
    public bool isAction;
    public bool escPanelOpen = false;
    public int talkIndex;

    private void Awake()
    {
        StartCoroutine(FadeIn(1f));
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "TitleScene" && Input.GetKeyDown(KeyCode.Escape) && escPanelOpen == false)
        {
            escPanelOpen = true;
            escPanel.SetActive(true);
        }
        else if (SceneManager.GetActiveScene().name != "TitleScene" && Input.GetKeyDown(KeyCode.Escape) && escPanelOpen == true)
        {
            escPanelOpen = false;
            escPanel.SetActive(false);
        }
    }

    public void Action(GameObject talkObj)
    {
        talkObject = talkObj;
        ObjData objData = talkObject.GetComponent<ObjData>();

        string npcName = talkManager.GetName(objData.id);
        nameText.text = npcName;

        if (gameStatus.returnToVillage && !gameStatus.questClear && objData.id == 1000)
        {
            objData.SetID();
        }

        Talk(objData.id);

        //����� ù ��ȭ �� ���� ��ȭ���� �ٸ� ��� ���
        if(!isAction && talkIndex == 0 && !gameStatus.questClear && objData.id == 1000)
        {
            objData.SetID();
        }

        namePanel.SetBool("isShow", isAction);
        talkPanel.SetBool("isShow", isAction);

    }

    void Talk(int id)
    {
        string talkData = "";
        if (talk.isAnim)
        {
            talk.SetMsg("");
            return;
        }
        else
        {
            talkData = talkManager.GetTalk(id, talkIndex, gameStatus.questClear);
        }

        if ((id == 1100 || id == 101) && !choiceManager.isChoiceActive && talkIndex == 1)
        {
            string[] choices = new string[] { "��", "�ƴϿ�" };
            choiceManager.ShowChoices(choices);
            isAction = true;
            return;
        }

        if (talkData != null)
        {
            if (!choiceManager.isChoiceActive && id != 1200)
            {
                if (talkIndex == 0) AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
                else AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
            }

            talk.SetMsg(talkData);
            isAction = true;
            talkIndex++;
        }
        else
        {
            if (!choiceManager.isChoiceActive) AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick1);
            isAction = false;
            talkIndex = 0;

            if (id == 1200)
            {
                StartCoroutine(ChangeToQuestScene());
            }
        }
    }

    public void HandleChoice(int choiceIndex)
    {
        ObjData objData = talkObject.GetComponent<ObjData>();

        if (objData.id == 1100 && choiceIndex == 0) //��
        {
            objData.SetID();
            talk.SetMsg("");
            talkIndex = 0;
            Talk(objData.id);
        }
        else if(objData.id == 1100 && choiceIndex == 1) //�ƴϿ�
        {
            Talk(objData.id);
        }
        else if(objData.id == 101 && choiceIndex == 0) //��
        {
            StartCoroutine(ReturnToVillageScene());
        }
    }

    public IEnumerator HunterDead()
    {
        AudioManager.instance.StopBGM();
        AudioManager.instance.PlayBGM(AudioManager.BGM.HunterDead);

        systemText.text = $"���� ���߽��ϴ�... {hunter.deathCount}/3";
        systemPanel.SetBool("isShow", true);

        rathalos.StateToIdle();

        yield return new WaitForSeconds(5f);
        systemPanel.SetBool("isShow", false);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOut(1f));
        hunter.Revive();
        yield return StartCoroutine(FadeIn(1f));
        AudioManager.instance.PlayBGM(AudioManager.BGM.NatureAudio);
        rathalos.HunterRevive();
    }

    public IEnumerator QuestFail()
    {
        AudioManager.instance.StopBGM();
        AudioManager.instance.PlayBGM(AudioManager.BGM.HunterDead);

        systemText.text = $"���� ���߽��ϴ�... 3/3";
        systemPanel.SetBool("isShow", true);
        rathalos.StateToIdle();
        yield return new WaitForSeconds(3f);

        systemPanel.SetBool("isShow", false);
        yield return new WaitForSeconds(0.5f);

        systemText.text = "����Ʈ ����! ������ ���ư��ϴ�.";
        systemPanel.SetBool("isShow", true);
        yield return new WaitForSeconds(3f);
        systemPanel.SetBool("isShow", false);

        yield return StartCoroutine(FadeOut(1f));
        SceneManager.LoadScene("VillageScene");
    }

    public IEnumerator RathalosDead()
    {
        AudioManager.instance.StopBGM();
        AudioManager.instance.PlayBGM(AudioManager.BGM.QuestClear);

        systemText.text = $"����Ʈ Ŭ����! 10�� �� ������ ���ư��ϴ�.";
        gameStatus.questClear = true;
        systemPanel.SetBool("isShow", true);
        yield return new WaitForSeconds(3f);
        systemPanel.SetBool("isShow", false);

        yield return new WaitForSeconds(7f);
        yield return StartCoroutine(FadeOut(1f));
        SceneManager.LoadScene("VillageScene");
    }

    public IEnumerator FadeOut(float duration)
    {
        float currentTime = 0f;
        Color color = fadeImage.color;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, currentTime / duration); // ���� 0���� 1�� ����
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn(float duration)
    {
        float currentTime = 0f;
        Color color = fadeImage.color;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, currentTime / duration); // ���� 1���� 0���� ����
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
    }

    public IEnumerator ReturnToVillageScene()
    {
        gameStatus.returnToVillage = true;
        yield return StartCoroutine(FadeOut(1f));
        SceneManager.LoadScene("VillageScene");
    }

    public IEnumerator ChangeToQuestScene()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.HunterDepart);
        yield return StartCoroutine(FadeOut(2f));
        SceneManager.LoadScene("QuestScene");
    }
}
