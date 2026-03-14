# TowerBreaker Study — 프로젝트 가이드

## 클래스 간 의존성 설계 원칙

이 프로젝트에서 클래스 간 통신은 목적에 따라 두 가지로 구분한다.

| 목적 | 방법 |
|---|---|
| "나 뭔가 일어났어" — 알림, 상태 변화 전파 | **이벤트(Action) 구독** |
| "너 지금 어때?" — 매 프레임 데이터 조회, 지속적 참조 | **인터페이스 또는 직접 참조** |

---

## 1. 알림 → 이벤트(Action) 구독

### 언제 쓰는가

- 어떤 일이 **발생했음을 다른 클래스에 알려야** 할 때
- 알림을 보내는 쪽이 받는 쪽을 **몰라도 될 때**
- 같은 이벤트에 **여러 클래스가 반응**해야 할 가능성이 있을 때

### 구조

```
발행자 (Publisher)          구독자 (Subscriber)
────────────────           ────────────────────
public event Action OnXxx   Start:     publisher.OnXxx += Handler
OnXxx?.Invoke()             OnDestroy: publisher.OnXxx -= Handler
```

### 프로젝트 적용 예시

**PlayerStats — 플레이어 사망 알림**

```csharp
// PlayerStats.cs (발행)
public event Action OnPlayerDead;

protected override void OnDead()
{
    OnPlayerDead?.Invoke();
}
```

```csharp
// StageManager.cs (구독)
playerStats.OnPlayerDead += HandlePlayerDead;

private void HandlePlayerDead()
{
    ShowResult(false);
}
```

PlayerStats는 StageManager의 존재를 전혀 모른다.
StageManager가 구독을 끊으면 PlayerStats 코드는 변경 없이 그대로 재사용 가능하다.

---

**MonsterGroup — 라운드 클리어 / 벽 충돌 알림**

```csharp
// MonsterGroup.cs (발행)
public event Action OnRoundCleared;
public event Action OnWallHit;

private void RemoveMonster(MonsterMover monster)
{
    _monsters.Remove(monster);
    if (_monsters.Count == 0)
        OnRoundCleared?.Invoke();
}

public void OnPlayerHitWall()
{
    OnWallHit?.Invoke();
    StopAllMonsters();
}
```

```csharp
// StageManager.cs (구독)
monsterGroup.OnRoundCleared += HandleRoundCleared;
monsterGroup.OnWallHit      += HandleWallHit;
```

---

**WallCollisionReporter — 벽 충돌 감지 알림**

```csharp
// WallCollisionReporter.cs (발행)
public event Action OnHitWallEvent;

private void OnCollisionEnter2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag(wallTag)) return;
    OnHitWallEvent?.Invoke();
}
```

```csharp
// StageManager.cs (연결 중계)
monsterGroup.SubscribeToWallReporter(wallCollisionReporter);
// → 내부적으로 reporter.OnHitWallEvent += monsterGroup.OnPlayerHitWall
```

---

### 이벤트 구독 시 주의사항

**반드시 OnDestroy에서 구독 해제**해야 한다.
구독 해제를 빠뜨리면 씬 전환 후 파괴된 오브젝트가 이벤트를 받아 NullReferenceException이 발생한다.

```csharp
private void Start()
{
    playerStats.OnPlayerDead += HandlePlayerDead;
}

// 해제 — 반드시 쌍으로 작성
private void OnDestroy()
{
    playerStats.OnPlayerDead -= HandlePlayerDead;
}
```

---

## 2. 데이터 조회 → 인터페이스 또는 직접 참조

### 언제 쓰는가

- 매 프레임 상대방의 **값(데이터)을 가져와야** 할 때
- 단순 알림이 아니라 **결과를 받아서 내 로직에 써야** 할 때

### 직접 참조 vs 인터페이스 비교

| | 직접 참조 | 인터페이스 |
|---|---|---|
| 코드량 | 적음 | 인터페이스 파일 추가 필요 |
| 교체 가능성 | 낮음 (구체 타입에 묶임) | 높음 (다른 구현체로 교체 가능) |
| 적합한 상황 | 상대 클래스가 하나로 고정 | 구현체가 바뀔 수 있거나 테스트가 필요한 경우 |

### 프로젝트 적용 예시

**MonsterContactHandler — 몬스터 속도 조회**

MonsterContactHandler는 매 FixedUpdate마다 선두 몬스터의 속도를 가져와
플레이어를 같은 속도로 밀어야 한다. 이건 알림이 아니라 **데이터 조회**다.

```csharp
// IMonsterVelocityProvider.cs (인터페이스)
public interface IMonsterVelocityProvider
{
    Rigidbody2D[] GetAllRigidbodies();
}
```

```csharp
// MonsterGroup.cs — 인터페이스 구현
public class MonsterGroup : MonoBehaviour, IMonsterVelocityProvider
{
    public Rigidbody2D[] GetAllRigidbodies() { ... }
}
```

```csharp
// MonsterContactHandler.cs — 인터페이스로 조회
[SerializeField] private MonsterGroup monsterGroup;   // Inspector에서 연결
private IMonsterVelocityProvider _velocityProvider;

private void Awake()
{
    _velocityProvider = monsterGroup;  // 인터페이스로 캐싱
}

private void FixedUpdate()
{
    Rigidbody2D[] rbs = _velocityProvider.GetAllRigidbodies();
    if (rbs.Length > 0)
        _rb.velocity = new Vector2(rbs[0].velocity.x, _rb.velocity.y);
}
```

Inspector 연결은 MonsterGroup으로 하되, 실제 사용은 인터페이스를 통한다.
나중에 MonsterGroup을 다른 클래스로 교체해도 MonsterContactHandler 코드는 바꾸지 않아도 된다.

---

## 3. StageManager의 역할 — 조율자(Coordinator)

이벤트 패턴을 도입하면 자연스럽게 **누군가 이벤트 연결을 책임져야** 한다.
이 프로젝트에서는 StageManager가 씬 내 모든 이벤트 연결을 담당한다.

```
StageManager
  ├─ playerStats.OnPlayerDead       → HandlePlayerDead
  ├─ monsterGroup.OnRoundCleared    → HandleRoundCleared
  ├─ monsterGroup.OnWallHit         → HandleWallHit
  ├─ playerAttack.OnAttackStarted   → monsterGroup.ResumeAllMonsters  (SubscribeToAttack)
  └─ wallReporter.OnHitWallEvent    → monsterGroup.OnPlayerHitWall    (SubscribeToWallReporter)
```

각 클래스는 자신의 이벤트를 발행하기만 하고,
**누가 구독하는지는 StageManager만 안다.**

---

## 4. ScriptableObject 데이터 관리 규칙

### 네이밍

ScriptableObject 클래스와 에셋 파일명은 **`~Data`** 접미사를 사용한다.

```
PlayerStatsData   (클래스명 / 파일명)
StageData         (클래스명 / 파일명)
```

### 저장 위치

모든 ScriptableObject 에셋은 **`Assets/Resources/`** 하위에 카테고리별 폴더로 저장한다.

```
Assets/Resources/
  PlayerData/
    PlayerStatsData.asset
  StageData/
    StageData_1.asset
    StageData_2.asset
    StageData_3.asset
```

### 로드 방식 — 동적 로드 (Inspector 연결 금지)

에셋을 사용하는 쪽에서 `Resources.Load<T>` 로 직접 불러온다.
**Inspector의 `[SerializeField]` 슬롯에 드래그하는 방식은 사용하지 않는다.**

```csharp
// Awake 에서 한 번만 로드해 필드에 캐싱
PlayerStatsData data = Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData");
if (data != null)
{
    _guardCooldown  = data.guardCooldown;
    _guardPushForce = data.guardPushForce;
}
else
{
    Debug.LogWarning("PlayerStatsData를 Resources/PlayerData 폴더에서 찾을 수 없습니다.");
}
```

### 경로 규칙

`Resources.Load` 경로는 `Resources/` 이후의 상대 경로 + 확장자 제외 파일명이다.

| 에셋 실제 경로 | Load 경로 |
|---|---|
| `Resources/PlayerData/PlayerStatsData.asset` | `"PlayerData/PlayerStatsData"` |
| `Resources/StageData/StageData_1.asset` | `"StageData/StageData_1"` |

---

## 요약 흐름도

```
[WallCollisionReporter]
    OnHitWallEvent ──────────────────────────────────┐
                                                      ↓
[PlayerAttack]                               [MonsterGroup]
    OnAttackStarted ──────────────────────→  ResumeAllMonsters
                                                      │
                                            OnWallHit │ OnRoundCleared
                                                ↓     ↓
                                           [StageManager]
                                           HandleWallHit → playerStats.TakeDamage
                                           HandleRoundCleared → StartRound / OnStageCleared

[PlayerStats]
    OnPlayerDead ────────────────────────→ [StageManager]
                                           HandlePlayerDead → ShowResult(false)

[MonsterContactHandler]
    _velocityProvider.GetAllRigidbodies() ─→ [IMonsterVelocityProvider]
                                                     ↑ 구현
                                             [MonsterGroup]
```
