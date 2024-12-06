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
        //마을 표지판
        nameData.Add(100, "표지판");
        beforeClear.Add(100, new string[] { "여기부터 투디마을" });
        //퀘스트 표지판1
        nameData.Add(101, "표지판");
        beforeClear.Add(101, new string[] { "투디마을로 돌아가시겠습니까?" });//선택지 나오고 예 누르면 마을로 돌아감
        //퀘스트 표지판2
        nameData.Add(102, "표지판");
        beforeClear.Add(102, new string[] { "초보 헌터를 위한 안내문",
                                         "Z : 발도, 공격   X : 공격   Ctrl : 가드\nShift : 납도, 달리기   Space Bar : 스텝",
                                         "Q, E : 아이템 스크롤   F : 아이템 사용",
                                         "일부 공격은 길게 눌러서 차지할 수 있다.\n차지 중에는 적 공격에 취약해지므로 신중하게 사용해야 한다.",
                                         "적을 공격하면 예리도가 감소한다.\n예리도가 감소하면 데미지가 약해지므로 숫돌을 사용해 예리도를 회복시키자.",
                                         "적의 상태 이상 공격에 주의!\n상태 이상에 걸렸다면 적절한 조치를 취해야 한다." });
        //마을 게시판
        nameData.Add(200, "게시판");
        beforeClear.Add(200, new string[] { "사용한 소스들",
                                         "골드메탈 TopDown 2D RPG BE3 애셋\n골드메탈 Undead Survivor 애셋",
                                         "Brown Rock TileSet 애셋\nPixel Art Top Down - Basic 애셋",
                                         "PokemonGSK2 폰트 https://cafe.naver.com/hansicgu/27995",
                                         "몬스터 헌터 시리즈 BGM 및 효과음\n몬스터 헌터 시리즈 아이템 이미지 및 리오레우스 디자인", 
                                         "몬스터 헌터 로고 이미지 및 시리즈 세계관 설정\n던그리드 캐릭터 디자인 참고"});
        //촌장
        nameData.Add(1000, "투디마을 촌장");
        beforeClear.Add(1000, new string[] { "오오. 자네가 우리 마을을 도우러 와준 헌터인가?",
                                          "반갑네. 나는 투디마을의 촌장이네.",
                                          "이미 알고 있겠지만, 최근 우리 마을 근처에\n화룡 리오레우스가 나타나서 정말 골치가 아파.",
                                          "우리 마을은 조그만 시골 마을이라 자네처럼 강한 헌터가 별로 없어.",
                                          "내가 10년만 젊었어도 리오레우스 정도는\n가볍게 토벌했을 텐데 말이야. 하하",
                                          "아무튼 리오레우스를 토벌할 때 까지\n우리 마을을 고향이라고 생각하고 편하게 지내게.",
                                          "리오레우스를 토벌할 준비가 되면\n나에게 다시 말을 걸어주게." });
        afterClear.Add(1000, new string[] { "리오레우스를 토벌했다고? 정말 수고했네.\n자네가 해낼 거라고 믿고 있었어!",
                                          "덕분에 마을이 다시 평화로워지겠어.", 
                                          "마을에 머무는 동안 필요한 게 있다면 뭐든 말하게.\n가능하다면 전부 지원해 주겠네. 자네는 마을의 영웅이나 다름없으니까!" });
        nameData.Add(1100, "투디마을 촌장");
        beforeClear.Add(1100, new string[] { "리오레우스를 토벌하러 가겠나?",
                                          "준비가 끝나면 다시 말을 걸어주게." });
        nameData.Add(1200, "투디마을 촌장");
        beforeClear.Add(1200, new string[] { "그럼 조심히 다녀오게.\n행운을 빌겠네." });
        //젊은 주민
        nameData.Add(1001, "젊은 주민");
        beforeClear.Add(1001, new string[] { "아주 먼 옛날 흑룡 밀라보레아스라는 몬스터가\n하룻밤 사이에 어느 왕국을 멸망시켰다는 전설이 있어.",
                                          "헌터 길드에서 조사대를 파견했지만\n누구도 돌아오지 못했다고 하던데......",
                                          "이 전설이 사실일까?" });
        afterClear.Add(1001, new string[] { "들었어. 리오레우스를 토벌했다며?",
                                          "골칫거리였던 리오레우스를 이렇게 빨리 처치하다니......\n실력이 정말 굉장한걸?", 
                                          "앞으로 더욱 강해져서 흑룡 밀라보레아스에 관한 전설을 파헤쳐 줘!" });
        //잡화상인
        nameData.Add(1002, "잡화상인");
        beforeClear.Add(1002, new string[] { "리오레우스 때문에 물자를 조달 받을 수가 없어서 곤란해요.",
                                          "옛날 같으면 촌장님께서 마을을 지켜주셨겠지만\n지금은 촌장님도 나이를 드셔서 힘드신 모양이에요." });
        afterClear.Add(1002, new string[] { "돌아오셨군요, 헌터님!\n리오레우스를 토벌해 주셔서 정말 감사해요.",
                                          "물자를 조달 받을 수 있게 되어서 내일부터 다시 장사를 할 수 있겠네요.\n필요한 게 있으시면 말씀해 주세요." });
        //산책하는 주민
        nameData.Add(1003, "산책하는 주민");
        beforeClear.Add(1003, new string[] { "다른 마을들은 아이루가 사람과 같이 생활한다고 해.",
                                          "사람들과 같이 생활하는 아이루들은\n사람의 언어도 할 수 있대!",
                                          "또 어떤 마을은 가루크와도 같이 생활한다고 하더라.",
                                          "우리 마을 근처에는 아이루와 가루크의 서식지가 없어서\n아이루와 가루크를 보기가 힘들어.",
                                          "아이루, 가루크와 함께 수렵하는 헌터의 모습을\n한 번쯤 꼭 보고 싶어!" });
        afterClear.Add(1003, new string[] { "리오레우스를 쓰러뜨렸구나! 정말 대단해!!",
                                          "다음에 투디마을에 올 때는 동반자 아이루도 데려와 줘!\n강한 대형 몬스터 이야기도 같이!" });
        //은퇴한 헌터
        nameData.Add(1004, "은퇴한 헌터");
        beforeClear.Add(1004, new string[] { "나는 반년 전에 신룡 나르가쿠르가를 수렵하다가\n부상을 입고 은퇴했어.",
                                          "부상만 아니었어도 리오레우스 토벌에 함께 했을 텐데\n힘이 되어주지 못해 미안하군......",
                                          "당신도 상위 헌터지? 나처럼 부상당하지 않도록 조심해." });
        afterClear.Add(1004, new string[] { "혼자서 리오레우스를 사냥했다니, 너 꽤 하는군.\n괜한 걱정을 하고 있었어.",
                                          "어쩌면 너는 엄청난 헌터가 될 수도 있겠어.\n저기 저 조각상의 헌터보다 더 대단한 헌터가......" });
        //열정적인 헌터
        nameData.Add(1005, "열정적인 헌터");
        beforeClear.Add(1005, new string[] { "이 석상은 수십 년 전, 투디마을을 지켜준\n전설적인 헌터의 조각상이에요.",
                                          "그거 아세요?\n촌장님도 예전에는 엄청 강력한 G급 헌터였대요!",
                                          "저도 빨리 강해져서 세상에 제 이름을 퍼뜨리고 싶어요!" });
        afterClear.Add(1005, new string[] { "리오레우스를 쓰러뜨렸다고요? 대단해요!",
                                          "어떻게 하면 그렇게 강해질 수 있나요? 알려주세요!\n강해져서 다음엔 제가 마을을 지키고 싶어요!" });
    }

    public string GetTalk(int id, int talkIndex, bool questClear)
    {
        Dictionary<int, string[]> talkData = questClear ? afterClear : beforeClear; //퀘스트 클리어 여부에 따라 다른 대사 출력

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
            return "???"; //이름이 없는 경우
    }
}
