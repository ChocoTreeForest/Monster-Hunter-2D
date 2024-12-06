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
    float interval; //���� ��� ����

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
        msgText.text = ""; //�ؽ�Ʈ �ʱ�ȭ
        index = 0; //�ε��� �ʱ�ȭ
        EndCursor.SetActive(false); //��ȭ Ŀ�� ��Ȱ��ȭ

        interval = 1.0f / CharPerSeconds; //Ÿ���� �ӵ�

        isAnim = true; //Ÿ���� ����Ʈ Ȱ��ȭ
        Invoke("Effecting", interval); //Ÿ���� ����
    }

    void Effecting()
    {
        if (msgText.text == targetMsg)
        {
            EffectEnd(); //��� ���� ��� �� Ÿ���� ����Ʈ ����
            return;
        }

        msgText.text += targetMsg[index]; // �� ���ھ� �߰�
        index++;

        Invoke("Effecting", interval);
    }

    void EffectEnd()
    {
        isAnim = false; //Ÿ���� ����Ʈ ����
        EndCursor.SetActive(true); //��ȭ Ŀ�� ǥ��
    }
}