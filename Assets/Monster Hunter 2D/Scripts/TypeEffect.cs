using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeEffect : MonoBehaviour
{
    public GameObject EndCursor;
    public int CharPerSeconds;
    public bool isAnim;

    string targetMsg;
    Text msgText;
    int index;
    float interval; //글자 출력 간격

    private void Awake()
    {
        msgText = GetComponent<Text>();
    }

    public void SetMsg(string msg)
    {
        if (isAnim)
        {
            msgText.text = targetMsg;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            targetMsg = msg;
            EffectStart();
        }
    }

    void EffectStart()
    {
        msgText.text = ""; //텍스트 초기화
        index = 0; //인덱스 초기화
        EndCursor.SetActive(false); //대화 커서 비활성화

        interval = 1.0f / CharPerSeconds; //타이핑 속도

        isAnim = true; //타이핑 이펙트 활성화
        Invoke("Effecting", interval); //타이핑 시작
    }

    void Effecting()
    {
        if (msgText.text == targetMsg)
        {
            EffectEnd(); //모든 글자 출력 시 타이핑 이펙트 종료
            return;
        }

        msgText.text += targetMsg[index]; // 한 글자씩 추가
        index++;

        Invoke("Effecting", interval);
    }

    void EffectEnd()
    {
        isAnim = false; //타이핑 이펙트 종료
        EndCursor.SetActive(true); //대화 커서 표시
    }
}