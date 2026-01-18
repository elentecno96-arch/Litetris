# Litetris
GameJam Project

게임 이름 : Litetris

1. 게임 소개
장르 : 회피, 생존

플랫폼 : PC (Windows)

3. 조작 방법
캐릭터 이동 : WASD or 방향키(상하좌우)
UI 선택 및 클릭 : 마우스

4. 사용된 기술
Engine : Unity
Language : C#
Libraries : UniTask, DOTween, TextMeshPro, Post Processing(Bloom)

5. 구현하고 싶은 주요 기능
AudioSettings.dspTime를 이용해 오디오 샘플링 속도를 기준으로 비트로 나눠 비트에 맞춰
패턴이 나오게끔 구현, UniTask와 DOTween을 적극 사용하여 연출에 사용

6. 음악
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

[Refactoring] UI Button 애니메이션 로직 분리 및 UIButtonEffect 컴포넌트 도입    

[Refactoring] 사운드 바(음량 관련) MVP로 Option과 SoundOptionView로 분리

 + Add Component : UIPanelEffect.cs, UIButtonEffect.cs
 + Presenters : OptionPresenter.cs
 + Views : SoundOptionView.cs

[Optimization] UIManager의 UIopen/close 애니메이션을 분리하여 결합도 낮춤

[Optimization] UIManager의 Button 애니메이션과 SoundBar를 분리하여 결합도 낮춤

[Optimization] 렌더링 파이프라인 전환 및 성능 최적화 (Built-in -> URP)

+ Graphics: Universal Render Pipeline(URP) 도입
+ Rendering: SRP Batcher 활성화 및 Batching 최적화
+ Post-Processing: URP Global Volume 기반 Bloom 효과 재구축

[Technical Achievements]
- CPU 렌더링 부하 대폭 감소: SetPass Calls 약 175 -> 25 (약 85% 개선)
- 확장성 확보: 배경 연출(좌우 큐브 배치 등) 추가 시에도 성능 저하를 최소화

26/01/18 ===========================================

[Refactoring] 보드 및 패턴 시스템 최적화

BoardManager.cs
 + 가로, 세로, 전체 리스트를 미리 캐싱하여 전략 클래스와의 결합도 낮춤
 + 범위 기반 람다 필터(GetCubeInFilter)도입

PatternManager.cs
 + 패턴 정렬 시 거리 제곱 연산 최적화

PatternStrategy.cs
 + 10여 종의 패턴 리펙토링
 + 전략 내 이중 루프 및 중복 로직 전면 제거
 + 기술적으로 어려운 수학 공식을 AI를 이용해 풀었음

[Optimization] 패턴 실행 효율성
 + 패턴 실행 시 발생하는 불필요한 new Vector2및 리스트 중복 할당 제거
 + 샐로운 패턴 추가 시 수학적 판정식만 정의하면 되는 구조 확립

[Feature] MaterialPropertyBlock을 이용한 최적화
 + 각 큐브 상태의 단계 별 색상 및 강도를 독립적으로 제어
 + MaterialPropertyBlock(MPB)를 사용하여 수백 개의 다른 발광 수치를 가지고 있어도
   GPU Instancing이 깨지지 않도록 설계
 + MPB 도입으로 인해 DOColor => DOTween.To로 변경

[Feature] 큐브 상태 중 Warning상태의 색상을 노란색 => 푸른색으로 변경
