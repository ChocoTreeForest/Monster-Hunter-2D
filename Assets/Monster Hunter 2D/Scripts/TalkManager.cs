using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> beforeClear;
    Dictionary<int, string[]> afterClear;
    Dictionary<int, string> nameData;
    void Awake()
    {
        beforeClear = new Dictionary<int, string[]>();
        afterClear = new Dictionary<int, string[]>();
        nameData = new Dictionary<int, string>();
        GenerateData();
    }

    void GenerateData()
    {
        //���� ǥ����
        nameData.Add(100, "ǥ����");
        beforeClear.Add(100, new string[] { "������� ������" });
        //����Ʈ ǥ����1
        nameData.Add(101, "ǥ����");
        beforeClear.Add(101, new string[] { "�������� ���ư��ðڽ��ϱ�?" });//������ ������ �� ������ ������ ���ư�
        //����Ʈ ǥ����2
        nameData.Add(102, "ǥ����");
        beforeClear.Add(102, new string[] { "�ʺ� ���͸� ���� �ȳ���",
                                         "Z : �ߵ�, ����   X : ����   Ctrl : ����\nShift : ����, �޸���   Space Bar : ����",
                                         "Q, E : ������ ��ũ��   F : ������ ���",
                                         "�Ϻ� ������ ��� ������ ������ �� �ִ�.\n���� �߿��� �� ���ݿ� ��������Ƿ� �����ϰ� ����ؾ� �Ѵ�.",
                                         "���� �����ϸ� �������� �����Ѵ�.\n�������� �����ϸ� �������� �������Ƿ� ������ ����� �������� ȸ����Ű��.",
                                         "���� ���� �̻� ���ݿ� ����!\n���� �̻� �ɷȴٸ� ������ ��ġ�� ���ؾ� �Ѵ�." });
        //���� �Խ���
        nameData.Add(200, "�Խ���");
        beforeClear.Add(200, new string[] { "����� �ҽ���",
                                         "����Ż TopDown 2D RPG BE3 �ּ�\n����Ż Undead Survivor �ּ�",
                                         "Brown Rock TileSet �ּ�\nPixel Art Top Down - Basic �ּ�",
                                         "PokemonGSK2 ��Ʈ https://cafe.naver.com/hansicgu/27995",
                                         "���� ���� �ø��� BGM �� ȿ����\n���� ���� �ø��� ������ �̹��� �� �������콺 ������", 
                                         "���� ���� �ΰ� �̹��� �� �ø��� ����� ����\n���׸��� ĳ���� ������ ����"});
        //����
        nameData.Add(1000, "������ ����");
        beforeClear.Add(1000, new string[] { "����. �ڳװ� �츮 ������ ���췯 ���� �����ΰ�?",
                                          "�ݰ���. ���� �������� �����̳�.",
                                          "�̹� �˰� �ְ�����, �ֱ� �츮 ���� ��ó��\nȭ�� �������콺�� ��Ÿ���� ���� ��ġ�� ����.",
                                          "�츮 ������ ���׸� �ð� �����̶� �ڳ�ó�� ���� ���Ͱ� ���� ����.",
                                          "���� 10�⸸ ����� �������콺 ������\n������ ������� �ٵ� ���̾�. ����",
                                          "�ƹ�ư �������콺�� ����� �� ����\n�츮 ������ �����̶�� �����ϰ� ���ϰ� ������.",
                                          "�������콺�� ����� �غ� �Ǹ�\n������ �ٽ� ���� �ɾ��ְ�." });
        afterClear.Add(1000, new string[] { "�������콺�� ����ߴٰ�? ���� �����߳�.\n�ڳװ� �س� �Ŷ�� �ϰ� �־���!",
                                          "���п� ������ �ٽ� ��ȭ�ο����ھ�.", 
                                          "������ �ӹ��� ���� �ʿ��� �� �ִٸ� ���� ���ϰ�.\n�����ϴٸ� ���� ������ �ְڳ�. �ڳ״� ������ �����̳� �ٸ������ϱ�!" });
        nameData.Add(1100, "������ ����");
        beforeClear.Add(1100, new string[] { "�������콺�� ����Ϸ� ���ڳ�?",
                                          "�غ� ������ �ٽ� ���� �ɾ��ְ�." });
        nameData.Add(1200, "������ ����");
        beforeClear.Add(1200, new string[] { "�׷� ������ �ٳ����.\n����� ���ڳ�." });
        //���� �ֹ�
        nameData.Add(1001, "���� �ֹ�");
        beforeClear.Add(1001, new string[] { "���� �� ���� ��� �ж󺸷��ƽ���� ���Ͱ�\n�Ϸ��� ���̿� ��� �ձ��� ������״ٴ� ������ �־�.",
                                          "���� ��忡�� ����븦 �İ�������\n������ ���ƿ��� ���ߴٰ� �ϴ���......",
                                          "�� ������ ����ϱ�?" });
        afterClear.Add(1001, new string[] { "�����. �������콺�� ����ߴٸ�?",
                                          "��ĩ�Ÿ����� �������콺�� �̷��� ���� óġ�ϴٴ�......\n�Ƿ��� ���� �����Ѱ�?", 
                                          "������ ���� �������� ��� �ж󺸷��ƽ��� ���� ������ ������ ��!" });
        //��ȭ����
        nameData.Add(1002, "��ȭ����");
        beforeClear.Add(1002, new string[] { "�������콺 ������ ���ڸ� ���� ���� ���� ��� ����ؿ�.",
                                          "���� ������ ����Բ��� ������ �����ּ̰�����\n������ ����Ե� ���̸� ��ż� ����� ����̿���." });
        afterClear.Add(1002, new string[] { "���ƿ��̱���, ���ʹ�!\n�������콺�� ����� �ּż� ���� �����ؿ�.",
                                          "���ڸ� ���� ���� �� �ְ� �Ǿ ���Ϻ��� �ٽ� ��縦 �� �� �ְڳ׿�.\n�ʿ��� �� �����ø� ������ �ּ���." });
        //��å�ϴ� �ֹ�
        nameData.Add(1003, "��å�ϴ� �ֹ�");
        beforeClear.Add(1003, new string[] { "�ٸ� �������� ���̷簡 ����� ���� ��Ȱ�Ѵٰ� ��.",
                                          "������ ���� ��Ȱ�ϴ� ���̷����\n����� �� �� �� �ִ�!",
                                          "�� � ������ ����ũ�͵� ���� ��Ȱ�Ѵٰ� �ϴ���.",
                                          "�츮 ���� ��ó���� ���̷�� ����ũ�� �������� ���\n���̷�� ����ũ�� ���Ⱑ �����.",
                                          "���̷�, ����ũ�� �Բ� �����ϴ� ������ �����\n�� ���� �� ���� �;�!" });
        afterClear.Add(1003, new string[] { "�������콺�� �����߷ȱ���! ���� �����!!",
                                          "������ �������� �� ���� ������ ���̷絵 ������ ��!\n���� ���� ���� �̾߱⵵ ����!" });
        //������ ����
        nameData.Add(1004, "������ ����");
        beforeClear.Add(1004, new string[] { "���� �ݳ� ���� �ŷ� �������������� �����ϴٰ�\n�λ��� �԰� �����߾�.",
                                          "�λ� �ƴϾ�� �������콺 ����� �Բ� ���� �ٵ�\n���� �Ǿ����� ���� �̾��ϱ�......",
                                          "��ŵ� ���� ������? ��ó�� �λ������ �ʵ��� ������." });
        afterClear.Add(1004, new string[] { "ȥ�ڼ� �������콺�� ����ߴٴ�, �� �� �ϴ±�.\n���� ������ �ϰ� �־���.",
                                          "��¼�� �ʴ� ��û�� ���Ͱ� �� ���� �ְھ�.\n���� �� �������� ���ͺ��� �� ����� ���Ͱ�......" });
        //�������� ����
        nameData.Add(1005, "�������� ����");
        beforeClear.Add(1005, new string[] { "�� ������ ���� �� ��, �������� ������\n�������� ������ �������̿���.",
                                          "�װ� �Ƽ���?\n����Ե� �������� ��û ������ G�� ���Ϳ����!",
                                          "���� ���� �������� ���� �� �̸��� �۶߸��� �;��!" });
        afterClear.Add(1005, new string[] { "�������콺�� �����߷ȴٰ���? ����ؿ�!",
                                          "��� �ϸ� �׷��� ������ �� �ֳ���? �˷��ּ���!\n�������� ������ ���� ������ ��Ű�� �;��!" });
    }

    public string GetTalk(int id, int talkIndex, bool questClear)
    {
        Dictionary<int, string[]> talkData = questClear ? afterClear : beforeClear; //����Ʈ Ŭ���� ���ο� ���� �ٸ� ��� ���

        if(id == 100 || id == 102 || id == 200)
        {
            talkData = beforeClear;
        }

        if (!talkData.ContainsKey(id) || talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

    public string GetName(int id)
    {
        if (nameData.ContainsKey(id))
            return nameData[id];
        else
            return "???"; //�̸��� ���� ���
    }
}