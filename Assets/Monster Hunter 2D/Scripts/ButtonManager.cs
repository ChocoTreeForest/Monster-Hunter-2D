using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameStatus gameStatus;
    public SettingManager settingManager;
    public void StartButton()
    {
        StartCoroutine(ChangeToVillageScene());
    }

    public void SettingButton()
    {
        settingManager.OpenSetting();
    }

    public void QuitButton()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        Application.Quit();
    }

    public void ContinueButton()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick2);
        gameManager.escPanelOpen = false;
        gameManager.escPanel.SetActive(false);
    }

    private IEnumerator ChangeToVillageScene()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX.MenuClick3);
        yield return StartCoroutine(gameManager.FadeOut(2f));
        gameStatus.returnToVillage = false;
        gameStatus.questClear = false;
        SceneManager.LoadScene("VillageScene");
    }
}
