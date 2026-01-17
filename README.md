# Litetris
GameJam Project

게임 이름 : Litetris

1. 게임 소개
장르 : 회피, 생존
플랫폼 : PC (Windows)

2. 조작 방법
캐릭터 이동 : WASD or 방향키(상하좌우)
UI 선택 및 클릭 : 마우스

3. 사용된 기술
Engine : Unity
Language : C#
Libraries : UniTask, DOTween, TextMeshPro, Post Processing(Bloom)

4. 구현하고 싶은 주요 기능
AudioSettings.dspTime를 이용해 오디오 샘플링 속도를 기준으로 비트로 나눠 비트에 맞춰
패턴이 나오게끔 구현, UniTask와 DOTween을 적극 사용하여 연출에 사용

5. 음악
- 해당 게임에 사용된 음악은 AI 음악 생성 사이트인 suno에서 제작되었습니다
- 해당 게임에는 총 7개의 음악이 들어가 있으며 선택해서 플레이하는게 아닌 랜덤으로 재생되어 이어서 듣기로 되어있습니다

6. 주요 시스템
GameManager : 게임의 전체적인 상태(State)와 승리/패배 조건을 관리하는 컨트롤 타워

RhythmManager : 음악의 비트를 간격 분석하고 게임의 난이도 흐름(Phase) 관리자

PatternManager : 공격 패턴 전략을 보관 관리자

BoardManager : 게임이 진행되는 물리적인 격자(Grid) 무대와 타일 데이터를 관리자

SoundManager : 게임 내 모든 효과음(SFX)과 배경음(BGM)의 재생 및 설정을 관리자

CameraManager : 카메라 연출 관리자

FadeManager : 화면의 암전(Fade In/Out) 효과 연출 관리자

=========================================================================


26/01/12 ===========================================

[Bug Fix] 게임 플레이 도중 카메라가 점점 멀어지는 현상 수정

26/01/14 ===========================================

[Refactoring] 대미지 이펙트 및 하트 UI 시스템 MVP 패턴 도입

 + Presenters: DamagePresenter.cs, HeartPresenter.cs
 + Views: DamageFlashView.cs, HeartView.cs

[Optimization] UIManager 내 대미지/하트 관련 의존성 제거 및 코드 정리

26/01/15 ===========================================

[Refactoring] 텍스트 연출 및 인게임 정보 UI MVP 패턴 도입

 + Presenters: TextPresenter.cs
 + Views: CountdownView.cs, PhaseTextView.cs, survivalTimeView.cs

[Feature] 비활성화되어 있던 실시간 생존 시간(Survival Time) 표시 기능 활성화

[Optimization]인게임 텍스트 제어 로직의 TextPresenter 이관

26/01/17 ===========================================

[Refactoring] UI Open/Close 애니메이션 로직 분리 및 UIPanelEffect 컴포넌트 도입

 + Add Component : UIPanelEffect.cs

[Optimization] UIManager의 UIopen/close 애니메이션을 분리하여 결합도 낮춤

