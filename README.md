# 몬스터 헌터 2D
![title](https://github.com/user-attachments/assets/25e266da-dca9-4f85-93ad-586c3d301098)
## 프로젝트 소개
이 게임은 몬스터 헌터 시리즈를 2D 게임으로 제작해 본 것으로, 탑다운 시점 헌팅 액션 게임입니다.

여러 공격 연계가 가능하고 몬스터의 공격 패턴이 다양해, 다채로운 플레이가 가능한 것이 특징입니다.

![게임플레이](https://github.com/user-attachments/assets/fe5f8f2c-aee5-410d-97ba-0dfc5c5dc08a)

![게임플레이2](https://github.com/user-attachments/assets/93ab41fc-0177-42e0-a340-723e61f2fc37)
## 게임 플레이 정보
- **기본 조작법**

  - 이동: 방향키

  - 스텝: Space Bar

  - 아이템 사용: F

  - 아이템 변경: Q, E

- **납도 중 조작법**

  - 달리기: Shift

  - 대화: NPC 앞에서 Z

  - 발도: Z

- **발도 중 조작법**

  - 공격: Z, X

  - 가드: Ctrl

  - 납도: Shift

- **주요 시스템**
  - 공격 관련 시스템: 총 4가지의 공격 모션을 연계하여 사용할 수 있고, 일부 공격은 차지가 가능해 더욱 강력한 공격을 할 수 있습니다.
    
    ![공격연계](https://github.com/user-attachments/assets/635232cf-0c59-43a2-aa5c-5595c2d30039)


  - 예리도 시스템: 몬스터를 공격할 때마다 예리도가 감소하며 일정 수치 이상 감소하면 예리도 아이콘의 색상이 바뀝니다.
    예리도 아이콘의 색상이 붉은색에 가까워질수록 데미지가 감소하므로 숫돌 아이템을 사용해 예리도를 회복해야 합니다.

    ![예리도](https://github.com/user-attachments/assets/e477df85-0687-4e96-a21c-f3dfa54ab773)

  - 체력 관련 시스템: 공격 피격 시 받은 데미지의 절반이 붉은 체력이 됩니다. 붉은 체력이 있다면 체력이 서서히 회복됩니다.
    단, 상태 이상에 걸렸다면 체력이 회복되지 않으며, 화상 상태에 걸렸다면 붉은 체력이 우선적으로 감소합니다.
    
    ![빨간체력회복](https://github.com/user-attachments/assets/e647cefc-8a72-4cfb-9225-923139e43436)

 
  - 상태 이상 시스템: 몬스터의 특정 공격에 맞으면 상태 이상에 걸릴 수 있습니다. 상태 이상에 걸리면 체력이 지속적으로 감소합니다.
    화상 상태와 독 상태가 있으며 각각 스텝을 세 번 사용하거나 해독약을 사용하면 해제할 수 있습니다.

    ![브레스](https://github.com/user-attachments/assets/fc31b8f4-8c6c-47a4-ab45-a377d5a3affd)
    ![독](https://github.com/user-attachments/assets/bf164ef6-993c-4fa3-94f4-0ce01c70d90d)
## 사용 기술 및 개발 환경
- Unity 엔진 2022.03 버전
- C#
- Photoshop을 사용해 캐릭터와 몬스터 도트 제작
- 싱글톤 패턴, 상태 패턴 사용
- PlayerPrefs를 사용해 게임 내 설정 데이터 관리
- 몬스터의 AI 구현
- Unity의 UI 시스템을 활용해 UI 구현
- Unity의 Animator와 Animation Event를 활용
- Audio Source를 활용해 사운드 구현
## 게임 설치 및 실행 방법
1. https://drive.google.com/file/d/1FSDnmlHpsBZFinopmvK991ESe5pvfDHz/view?usp=sharing 해당 링크에서 Zip 파일 다운로드
2. Zip 파일 압축 해제
3. 'Monster Hunter 2D.exe' 실행
## 사용 소스 및 참고 자료
- 골드메탈 TopDown 2D RPG BE3 애셋
- 골드메탈 Undead Suvivor 애셋
- Brown Rock TileSet 애셋
- Pixel Art Top Down - Basic 애셋
- PokemonGSK2 폰트 https://cafe.naver.com/hansicgu/27995
- 몬스터 헌터 시리즈 BGM 및 효과음
- 몬스터 헌터 시리즈 아이템 이미지 및 리오레우스 디자인
- 몬스터 헌터 로고 이미지 및 시리즈 세계관 설정
- 던그리드 캐릭터 디자인 참고
