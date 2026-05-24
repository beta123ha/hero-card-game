# PROJECT_CONTEXT.md — Hero Card Game

> Cập nhật gần nhất: 2026-05-24  
> Mục đích file này: giúp người hỗ trợ đọc nhanh tình trạng thật của project, tránh hướng dẫn sai tên scene, tên folder, tên script, tên biến hoặc viết lệch kiến trúc hiện tại.

---

## 0. Cập nhật mới nhất — 2026-05-24

Mốc mới nhất của project là bổ sung **điều kiện kích hoạt effect** để hero passive không bị hiểu nhầm là buff vĩnh viễn vô điều kiện.

### Lý do cập nhật

- Khi gán thử effect vào `HeroCardData.passiveEffects`, nếu dùng `durationType = Permanent` cho mọi nội tại thì thiết kế sẽ bị sai.
- Nội tại hero theo thiết kế có thể chỉ phát huy khi điều kiện đúng, ví dụ hero đứng trên địa hình phù hợp thì được buff.
- Khi điều kiện không còn đúng, effect phải ngừng tác dụng.

### Code mới đã thêm / đã sửa

Đã thêm enum mới:

```text
Assets/scripts/effects/core/EffectConditionType.cs
```

Nội dung định hướng:

```csharp
public enum EffectConditionType
{
    Always,
    HeroOnFavoredTerrain,
    HeroOnSpecificTerrain,
    HeroHasTag,
    EnemyHasTag
}
```

Đã sửa:

```text
Assets/scripts/effects/core/EffectData.cs
```

`EffectData` hiện có thêm nhóm field condition:

```csharp
[Header("Condition")]
public EffectConditionType conditionType = EffectConditionType.Always;
public TerrainData requiredTerrain;
public TagData requiredTag;
```

### Quy ước dùng condition/duration

```text
Always:
- dùng cho effect không cần điều kiện đặc biệt.

HeroOnFavoredTerrain:
- dùng cho passive kiểu hero đứng trên địa hình phù hợp với tag thì được buff.

HeroOnSpecificTerrain:
- dùng cho passive/tactic yêu cầu đúng một loại địa hình cụ thể.

HeroHasTag:
- dùng cho effect yêu cầu hero có tag cụ thể.

EnemyHasTag:
- dùng cho effect tương tác với enemy ở ô đối diện hoặc enemy mục tiêu có tag cụ thể.
```

Quy tắc quan trọng:

```text
Passive phụ thuộc điều kiện: durationType = WhileConditionTrue
Tactic buff ngắn hạn: durationType = UntilEndOfTurn hoặc duration nhiều lượt
Effect thật sự tồn tại lâu dài trong trận: durationType = Permanent
```

### Data effect hiện tại

Đã bắt đầu tạo folder:

```text
Assets/game_data/effects/
```

Các file/folder mới cần được Git theo dõi:

```text
Assets/game_data/effects.meta
Assets/game_data/effects/
Assets/scripts/effects/core/EffectConditionType.cs
Assets/scripts/effects/core/EffectConditionType.cs.meta
```

Các tactic asset đã có thay đổi để thử gắn effect:

```text
Assets/game_data/tactics/entrenched_hold.asset
Assets/game_data/tactics/river_stake_ambush.asset
```

### Trạng thái cần nhớ

- Hero passive đã đúng hướng là dùng `passiveEffects`, nhưng không phải mọi passive đều là `Permanent`.
- Tactic effect vẫn dùng `tacticEffects`.
- Effect asset cụ thể nên nằm trong `Assets/game_data/effects/`, không nằm trong `Assets/scripts/effects/`.
- `Assets/scripts/effects/` chỉ chứa code C#.
- Battle runtime vẫn chưa làm xong; condition hiện mới là data/schema, phần kiểm tra condition thật sẽ cần nối vào battle sau.

### Việc làm tiếp gần nhất

```text
1. Kiểm tra Unity compile sau khi thêm EffectConditionType.cs.
2. Kiểm tra các effect asset trong Assets/game_data/effects.
3. Gán effect vào tacticEffects của một số tactic card.
4. Gán passiveEffects cho một hero theo kiểu WhileConditionTrue.
5. Test lại opponent_deck_preview xem effect hiển thị đúng chưa.
6. Sau đó bắt đầu BattleInitializer.
```

---

## 0.1. Mốc trước — 2026-05-22

Mốc mới nhất của project là chuyển hệ thống hiệu ứng của **hero passive** và **tactic card** sang hướng dùng `ScriptableObject` effect.

### Đã thay đổi trong hướng code hiện tại

- `HeroCardData` dùng:

```csharp
public List<EffectData> passiveEffects = new List<EffectData>();
```

- `TacticCardData` dùng:

```csharp
public List<EffectData> tacticEffects = new List<EffectData>();
```

- `AIDeckScorer.cs` đã được chỉnh theo hướng mới:
  - không chấm tactic bằng `attackBonus`, `defenseBonus`, `healthBonus` nữa;
  - chấm hero bằng chỉ số gốc + điểm từ `passiveEffects`;
  - chấm tactic bằng danh sách `tacticEffects`;
  - nếu effect là `StatModifierEffectData` thì AI đọc `statType`, `value`, `targetType`, `durationType`, `stackingType` để tính điểm;
  - giảm chỉ số của enemy được hiểu là có lợi cho AI;
  - buff nhiều mục tiêu có điểm cao hơn buff một mục tiêu;
  - effect lâu dài / có stack được chấm cao hơn effect ngắn hạn.

- `AIDeckPlanner.cs` hiện **không cần sửa lớn**, vì nó vẫn chỉ gọi:

```csharp
AIDeckScorer.ScoreHero(...)
AIDeckScorer.ScoreTactic(...)
AIDeckScorer.CanUseTactic(...)
```

- Điều kiện tactic được phép chọn vẫn dựa vào:
  - `isShared`
  - `requiredTags`
  - `hero.tags`

### Lỗi mới đã gặp trong Unity

Sau khi bỏ field bonus cũ trong `TacticCardData`, Unity báo lỗi kiểu:

```text
DeckPreviewCardUI.cs(...): error CS1061: 'TacticCardData' does not contain a definition for 'healthBonus'
```

Nguyên nhân:

```text
DeckPreviewCardUI.cs vẫn còn code cũ đang gọi tactic.attackBonus / tactic.defenseBonus / tactic.healthBonus.
```

Cách sửa đã chốt:

- `DeckPreviewCardUI.cs` phải hiển thị tactic bằng `tactic.tacticEffects`.
- Không dùng lại các field cũ `attackBonus`, `defenseBonus`, `healthBonus` trong UI preview tactic.
- Nên có hàm helper dạng:
  - `BuildEffectText(List<EffectData> effects)`
  - `BuildSingleEffectText(EffectData effect)`
- Nếu mở Unity còn lỗi đỏ tương tự, dùng tìm kiếm toàn project:

```text
tactic.attackBonus
tactic.defenseBonus
tactic.healthBonus
```

và sửa những chỗ đó sang đọc `tactic.tacticEffects`.

### Trạng thái cần nhớ

- Từ mốc này trở đi, hướng đúng là: **tactic effect và hero passive đều là effect object**.
- Không thêm lại `attackBonus`, `defenseBonus`, `healthBonus` vào `TacticCardData` chỉ để sửa lỗi compile.
- `TerrainData` hiện vẫn có `attackBonus`, `defenseBonus`, `healthBonus`; phần này **chưa bắt buộc đổi sang EffectData**.
- Battle runtime thật vẫn chưa làm xong; effect module hiện chủ yếu mới được nối vào data và AI deck scoring.


## 1. Tổng quan project

Tên project hiện tại:

```text
Hero Card Game
```

Đây là game thẻ bài chiến thuật làm bằng Unity. Chủ đề game là đấu thẻ bài theo quốc gia/lịch sử, trước mắt tập trung vào chế độ đấu với máy.

Phiên bản Unity đang dùng theo `ProjectSettings/ProjectVersion.txt`:

```text
Unity 6000.3.11f1
```

Project đã được đưa lên GitHub thành công. Khi làm tiếp, cần luôn cập nhật file này cùng code.

---

## 2. Luồng scene hiện tại

Các scene chính đang có trong `Assets/scenes/`:

```text
menu
enemy_setup
player_setup
deck_setup
opponent_deck_preview
terrain_setup
battle
```

Thứ tự scene hiện tại:

```text
menu
-> enemy_setup
-> player_setup
-> deck_setup
-> opponent_deck_preview
-> terrain_setup
-> battle
```

Các scene này đã có trong Build Settings / Scene List:

```text
Assets/scenes/menu.unity
Assets/scenes/enemy_setup.unity
Assets/scenes/player_setup.unity
Assets/scenes/deck_setup.unity
Assets/scenes/opponent_deck_preview.unity
Assets/scenes/terrain_setup.unity
Assets/scenes/battle.unity
```

Lưu ý quan trọng:

- Nên test game từ scene `menu`.
- Không nên test thẳng từ `deck_setup`, `opponent_deck_preview`, `terrain_setup` vì `GameSession` được tạo từ scene `menu`.
- Nếu test thẳng scene giữa, `GameSession.Instance` có thể bị null hoặc thiếu dữ liệu.

---

## 3. Gameplay đã chốt cho bản hiện tại

Luật setup trước trận:

- Người chơi chọn quốc gia cho AI đối thủ.
- Người chơi chọn độ khó AI đối thủ.
- AI tự random phong cách chơi đầu trận.
- Người chơi chọn quốc gia cho bản thân.
- Người chơi chọn deck gồm:
  - 15 hero
  - 9 tactic
- AI tự chọn deck enemy gồm:
  - 15 hero
  - 9 tactic
- Sau khi chọn deck, người chơi được xem deck đối thủ trong `opponent_deck_preview`.
- Thời gian xem deck đối thủ hiện tại:
  - 60 giây
- Sau đó vào `terrain_setup`.
- Mỗi bên có 7 ô địa hình.
- Trong `terrain_setup`:
  - Player được đổi thứ tự địa hình của mình.
  - Enemy cũng tự đổi thứ tự địa hình bằng AI.
  - Player nhìn thấy enemy terrain.
  - Enemy nhìn player terrain để quyết định swap counter.
  - Thời gian setup terrain hiện tại:
    - 15 giây
- Hết thời gian hoặc bấm Next:
  - lưu `playerTerrainOrder`
  - lưu `enemyTerrainOrder`
  - chuyển sang `battle`

Luật battle đã chốt trên thiết kế, nhưng chưa code battle thật:

- Mỗi bên có 100 máu.
- Deck runtime được tạo từ 15 hero + 9 tactic đã chọn.
- Deck được shuffle.
- Mỗi bên rút 5 lá đầu.
- Người đi trước được random.
- Hero chỉ tấn công hero ở ô đối diện.
- Nếu hero tấn công vào ô đối diện trống thì gây damage trực tiếp vào người chơi đối phương.

---

## 4. Cấu trúc thư mục quan trọng

Trong `Assets/` hiện có các thư mục chính:

```text
Assets/
├── game_data/
├── prefabs/
├── scenes/
├── scripts/
├── Settings/
└── TextMesh Pro/
```

Trong `Assets/scripts/` hiện có:

```text
Assets/scripts/
├── ai/
│   ├── battle/
│   ├── core/
│   ├── deck/
│   ├── profiles/
│   └── terrain/
├── battle/
├── core/
├── data/
├── effects/
│   ├── core/
│   ├── runtime/
│   └── stat/
└── ui/
```

Trong `Assets/game_data/` hiện có:

```text
Assets/game_data/
├── ai_profiles/
├── countries/
├── effects/
├── heroes/
├── tactics/
├── tags/
└── terrains/
```

Hiện đã bắt đầu có folder:

```text
Assets/game_data/effects
```

Folder này dùng để chứa effect asset như `attack_plus_2`, `defense_plus_2`, `attack_minus_2`, `defense_minus_2` và các passive/tactic effect cụ thể.

---

## 5. Data ScriptableObject

### 5.1. TagData

File:

```text
Assets/scripts/data/TagData.cs
```

Vai trò:

- Tạo tag cho hero/tactic.
- Các tag này dùng cho combo, tactic requirement, AI scoring và terrain synergy.

Các tag hiện có trong `Assets/game_data/tags/`:

```text
brute_force
guardian
land_warfare
mobile_warfare
naval_warfare
positional_warfare
shock_assault
strategic_mind
```

### 5.2. TerrainData

File:

```text
Assets/scripts/data/TerrainData.cs
```

Field chính:

```csharp
public string terrainName;
public string description;
public List<TagData> favoredTags = new List<TagData>();
public int attackBonus;
public int defenseBonus;
public int healthBonus;
```

Terrain hiện có trong `Assets/game_data/terrains/`:

```text
high_land
plain
water
```

Giá trị hiện tại:

```text
high_land
- terrainName: High-land
- favoredTags: land_warfare, guardian, positional_warfare
- attackBonus: 1
- defenseBonus: 3
- healthBonus: 2

plain
- terrainName: Plain
- favoredTags: land_warfare, brute_force, shock_assault
- attackBonus: 3
- defenseBonus: 1
- healthBonus: 2

water
- terrainName: Water
- favoredTags: naval_warfare, strategic_mind
- attackBonus: 2
- defenseBonus: 2
- healthBonus: 2
```

Lưu ý:

- Hiện các bonus terrain chủ yếu đang phục vụ AI xếp địa hình.
- Battle chưa nối terrain bonus vào tính chỉ số thật.

### 5.3. HeroCardData

File:

```text
Assets/scripts/data/HeroCardData.cs
```

Field chính:

```csharp
public string heroName;
public string countryName;

public int baseAttack = 1;
public int baseDefense = 1;
public int baseHealth = 1;

public List<TagData> tags = new List<TagData>();

public string passiveDescription;

public List<EffectData> passiveEffects = new List<EffectData>();

public Sprite artwork;
```

Lưu ý:

- `passiveDescription` là text mô tả cho người chơi đọc.
- `passiveEffects` là danh sách effect object thật để game xử lý sau này.
- Hiện battle chưa xử lý hero passive thật.
- Hiện nhiều hero vẫn đang có chỉ số placeholder `1/1/1`.

Hero asset hiện có trong `Assets/game_data/heroes/`:

```text
dinh_bo_linh
ho_chi_minh
le_hoan
le_loi
ly_thuong_kiet
ngo_quyen
nguyen_trai
pham_ngu_lao
quang_trung
tran_hung_dao
tran_quang_khai
tran_quoc_toan
trieu_thi_trinh
trung_nhi
trung_trac
vo_nguyen_giap
```

### 5.4. TacticCardData

File:

```text
Assets/scripts/data/TacticCardData.cs
```

Field chính:

```csharp
public string tacticName;
public string countryName;
public bool isShared;

public List<TagData> requiredTags = new List<TagData>();

public List<EffectData> tacticEffects = new List<EffectData>();

public string effectDescription;

public Sprite artwork;
```

Lưu ý:

- `tacticEffects` là danh sách effect object thật của tactic.
- Không chấm tactic bằng `attackBonus`, `defenseBonus`, `healthBonus` nữa.
- Nếu còn lỗi compile nhắc đến `healthBonus` hoặc các bonus cũ trong `TacticCardData`, nghĩa là vẫn còn script chưa được sửa sang `tacticEffects`.
- Cần kiểm tra lại Inspector của từng tactic asset để gán đúng effect asset vào `tacticEffects`.

Tactic asset hiện có trong `Assets/game_data/tactics/`:

```text
call_to_arms
entrenched_hold
field_medicine
iron_wall_formation
master_campaign_plan
overwhelming_force
rapid_redeployment
river_stake_ambush
terrain_offensive
thunder_charge
```

Ghi chú sau khi chuyển sang effect object:

```text
Các tactic asset cần được kiểm tra lại trong Inspector.
Mỗi tactic nên dùng tacticEffects để chứa effect thật.
Các giá trị bonus cũ kiểu ATK+ / DEF+ / HP+ không còn là nguồn xử lý chính của AI deck scorer.
```

### 5.5. CountryData

File:

```text
Assets/scripts/data/CountryData.cs
```

Field chính:

```csharp
public string countryName;
public List<TerrainData> battlefieldTerrains = new List<TerrainData>();
public List<HeroCardData> heroPool = new List<HeroCardData>();
public List<TacticCardData> tacticPool = new List<TacticCardData>();
```

Country hiện có:

```text
Assets/game_data/countries/viet_nam.asset
```

Thông tin hiện tại:

```text
countryName: Viet Nam
heroPool: 16 hero
tacticPool: 10 tactic
battlefieldTerrains: 7 terrain
```

Thứ tự terrain trong `viet_nam.asset` hiện tại:

```text
Slot 1: high_land
Slot 2: high_land
Slot 3: water
Slot 4: water
Slot 5: plain
Slot 6: plain
Slot 7: plain
```

---

## 6. GameSession

File:

```text
Assets/scripts/core/GameSession.cs
```

Vai trò:

- Singleton giữ dữ liệu xuyên scene.
- Được gắn vào object `game_session` trong scene `menu`.
- Có `DontDestroyOnLoad(gameObject)`.

Biến quan trọng:

```csharp
public CountryData enemyCountry;
public AIDifficulty enemyDifficulty = AIDifficulty.Normal;

public AIPlayStyle enemyBasePlayStyle = AIPlayStyle.Balanced;
public AIPlayStyle enemyCurrentPlayStyle = AIPlayStyle.Balanced;

public CountryData playerCountry;

public List<HeroCardData> selectedHeroes = new List<HeroCardData>();
public List<TacticCardData> selectedTactics = new List<TacticCardData>();

public List<HeroCardData> enemySelectedHeroes = new List<HeroCardData>();
public List<TacticCardData> enemySelectedTactics = new List<TacticCardData>();

public List<TerrainData> playerTerrainOrder = new List<TerrainData>();
public List<TerrainData> enemyTerrainOrder = new List<TerrainData>();
```

Ý nghĩa:

```text
selectedHeroes / selectedTactics
= deck người chơi đã chọn

enemySelectedHeroes / enemySelectedTactics
= deck enemy do AI chọn

playerTerrainOrder / enemyTerrainOrder
= thứ tự terrain đã khóa trước khi vào battle
```

Lưu ý:

- `enemyDifficulty` hiện là enum `AIDifficulty`, không còn là string.
- Không nên đổi tên `selectedHeroes` và `selectedTactics` lúc này vì đang được nhiều script dùng.
- Nếu cần rõ nghĩa hơn, có thể đổi sau khi battle đã ổn.

---

## 7. AI module

AI được tách riêng trong:

```text
Assets/scripts/ai/
```

Nguyên tắc kiến trúc đã chốt:

- AI module chỉ nhận dữ liệu và trả về quyết định.
- AI không trực tiếp sửa UI.
- AI không tự chuyển scene.
- AI không tự Instantiate button.
- UI Controller chỉ điều khiển scene/UI.
- `GameSession` chỉ lưu trạng thái xuyên scene.
- Battle sau này không nên nhét toàn bộ AI vào `BattleManager`.

### 7.1. AI core

Folder:

```text
Assets/scripts/ai/core/
```

Files:

```text
AIDifficulty.cs
AIPlayStyle.cs
AIStyleSelector.cs
AISituationSnapshot.cs
AIStyleController.cs
```

Enum style:

```csharp
public enum AIPlayStyle
{
    Aggressive,
    Defensive,
    Balanced
}
```

Enum difficulty:

```csharp
public enum AIDifficulty
{
    Easy,
    Normal,
    Hard
}
```

Ý nghĩa:

```text
AIPlayStyle = bot muốn chơi theo kiểu gì
AIDifficulty = bot chơi thông minh tới mức nào
```

Không được hiểu:

```text
Easy = Aggressive
Normal = Balanced
Hard = Defensive
```

Cách đúng:

```text
Aggressive Easy / Normal / Hard
Defensive Easy / Normal / Hard
Balanced Easy / Normal / Hard
```

### 7.2. AI profile

Folder:

```text
Assets/scripts/ai/profiles/
Assets/game_data/ai_profiles/
```

Script:

```text
AIPlayStyleProfile.cs
```

Profile asset hiện có:

```text
aggressive_profile
defensive_profile
balanced_profile
```

Các profile chứa weight cho:

```text
Deck Scoring
Terrain Scoring
Battle Behavior
Randomness
```

### 7.3. AI chọn deck

Folder:

```text
Assets/scripts/ai/deck/
```

Files:

```text
AIDeckSelectionResult.cs
AIDeckScorer.cs
AIDeckPlanner.cs
```

Vai trò:

- AI tự chọn 15 hero + 9 tactic cho enemy.
- Dựa vào:
  - `enemyCountry`
  - `enemyDifficulty`
  - `AIPlayStyleProfile`

Luồng hiện tại:

```text
DeckSetupController.ConfirmAndGoNext()
-> SavePlayerDeck()
-> CreateAndSaveEnemyDeck()
-> AIDeckPlanner.BuildDeck(...)
-> lưu GameSession.enemySelectedHeroes
-> lưu GameSession.enemySelectedTactics
-> LoadScene("opponent_deck_preview")
```

`AIDeckSelectionResult` chứa:

```csharp
public List<HeroCardData> selectedHeroes;
public List<TacticCardData> selectedTactics;
public bool IsComplete();
```

Điều kiện complete:

```text
15 hero
9 tactic
```

Cập nhật 2026-05-22:

```text
AIDeckScorer hiện đã chuyển sang đọc EffectData.
Hero được chấm thêm điểm từ passiveEffects.
Tactic được chấm điểm từ tacticEffects.
AIDeckPlanner vẫn giữ vai trò chọn danh sách sau khi scorer trả điểm.
```

Các helper logic quan trọng trong hướng mới:

```text
ScoreEffectList
ScoreEffect
ScoreStatModifierEffect
ScoreSpecialEffect
GetTargetMultiplier
GetDurationMultiplier
GetStackMultiplier
```

Lưu ý:

```text
Nếu còn code trong AI gọi tactic.attackBonus, tactic.defenseBonus hoặc tactic.healthBonus thì đó là code cũ cần sửa.
```


### 7.4. AI xếp terrain

Folder:

```text
Assets/scripts/ai/terrain/
```

Files:

```text
AITerrainScorer.cs
AITerrainPlanner.cs
AITerrainSwapDecision.cs
```

Vai trò hiện tại:

- Tạo terrain order ban đầu cho enemy bằng shuffle.
- Trong `terrain_setup`, AI nhìn terrain player và deck hai bên để quyết định có swap terrain của enemy không.
- AI tính theo slot đối diện vì luật hero chỉ tấn công ô đối diện.

Mapping slot:

```text
player slot 0 <-> enemy slot 0
player slot 1 <-> enemy slot 1
player slot 2 <-> enemy slot 2
player slot 3 <-> enemy slot 3
player slot 4 <-> enemy slot 4
player slot 5 <-> enemy slot 5
player slot 6 <-> enemy slot 6
```

Hàm quan trọng:

```csharp
AITerrainPlanner.CreateInitialOrder(CountryData country)

AITerrainPlanner.DecideNextSwap(
    List<TerrainData> enemyTerrainOrder,
    List<TerrainData> playerTerrainOrder,
    List<HeroCardData> enemyHeroes,
    List<HeroCardData> playerHeroes,
    AIPlayStyleProfile profile,
    AIDifficulty difficulty
)
```

Difficulty ảnh hưởng chọn swap:

```text
Easy: chọn ngẫu nhiên trong top 5 candidate
Normal: chọn ngẫu nhiên trong top 3 candidate
Hard: chọn candidate tốt nhất
```

### 7.5. AI battle

Folder đã có:

```text
Assets/scripts/ai/battle/
```

Hiện trạng:

- Chưa có file battle AI thật.
- Chưa có `AIBattlePlanner`.
- Chưa có `AIBattleScorer`.

---

## 8. Effect module

Folder:

```text
Assets/scripts/effects/
```

Mục tiêu:

- Tách logic effect khỏi `BattleManager`.
- Không sửa trực tiếp chỉ số gốc của card.
- Không làm kiểu `hero.baseAttack += 2`.
- Dùng `EffectData`, `EffectInstance`, `EffectResolver`, `StatCalculator`.

Nguyên tắc đã chốt:

```text
CardData = dữ liệu gốc
CardInstance / HeroInstance = đối tượng runtime sau này
EffectData = dữ liệu gốc của hiệu ứng
EffectInstance = hiệu ứng thật đang tồn tại trong trận
EffectResolver = thêm / xử lý stack / tick / gỡ effect
StatCalculator = tính chỉ số hiện tại
```

### 8.1. Effect core

Folder:

```text
Assets/scripts/effects/core/
```

Files:

```text
EffectData.cs
EffectConditionType.cs
EffectDurationType.cs
EffectTargetType.cs
EffectInstance.cs
EffectStackingType.cs
```

`EffectData` là abstract ScriptableObject, có:

```csharp
public string effectName;
public string description;
public EffectTargetType targetType;

public EffectConditionType conditionType;
public TerrainData requiredTerrain;
public TagData requiredTag;

public EffectDurationType durationType;
public int durationTurns;
public EffectStackingType stackingType;
public int maxStacks;
public string stackKey;
```

Condition hiện có:

```text
Always
HeroOnFavoredTerrain
HeroOnSpecificTerrain
HeroHasTag
EnemyHasTag
```

Duration hiện có:

```text
Instant
UntilEndOfTurn
Permanent
WhileConditionTrue
```

Target hiện có:

```text
SelfHero
SelectedAllyHero
SelectedEnemyHero
AllAllyHeroes
AllEnemyHeroes
OwnerPlayer
OpponentPlayer
```

Stacking hiện có:

```text
NotStackableKeepStrongest
NotStackableRefreshDuration
Stackable
StackableWithLimit
```

### 8.2. Stat effect

Folder:

```text
Assets/scripts/effects/stat/
```

Files:

```text
StatType.cs
StatModifierEffectData.cs
```

`StatType` hiện có:

```text
Attack
Defense
Health
```

`StatModifierEffectData` dùng cho mọi effect tăng/giảm chỉ số:

```csharp
public StatType statType = StatType.Attack;
public int value = 0;
```

Không tạo file riêng kiểu:

```text
attack_plus_2.cs
defense_minus_2.cs
```

Cách đúng:

- Một class `StatModifierEffectData.cs`.
- Mỗi effect cụ thể là một asset trong Unity Inspector.

### 8.3. Effect runtime

Folder:

```text
Assets/scripts/effects/runtime/
```

Files:

```text
EffectResolver.cs
StatCalculator.cs
```

`EffectResolver` đang làm:

```text
AddEffect
TickEndOfTurn
RemoveExpiredEffects
xử lý stacking theo stackKey
```

`StatCalculator` đang làm:

```text
CalculateAttack
CalculateDefense
CalculateHealth
```

Công thức:

```text
currentStat = baseStat + tổng value * stackCount của các StatModifierEffectData còn hiệu lực
```

Lưu ý:

- Hiện `StatCalculator` chặn chỉ số nhỏ hơn 1.
- Nếu sau này muốn ATK/DEF có thể về 0 thì sửa rule này.

### 8.4. Data effect asset

Folder asset effect:

```text
Assets/game_data/effects/
```

Trạng thái hiện tại:

```text
Đã bắt đầu tạo folder Assets/game_data/effects.
Đã có thay đổi ở một số tactic asset để thử gắn effect.
Cần kiểm tra tên và Inspector của từng effect asset trực tiếp trong Unity.
```

Các effect asset giai đoạn đầu nên có:

```text
attack_plus_2
defense_plus_2
attack_minus_2
defense_minus_2
```

Cách đặt đúng:

```text
Code effect: Assets/scripts/effects/
Data effect asset: Assets/game_data/effects/
```

Ví dụ passive hero phụ thuộc địa hình:

```text
Condition Type: HeroOnFavoredTerrain
Duration Type: WhileConditionTrue
Target Type: SelfHero
```

Ví dụ tactic buff/debuff ngắn hạn:

```text
Condition Type: Always
Duration Type: UntilEndOfTurn
Target Type: SelectedAllyHero hoặc SelectedEnemyHero
```

Còn cần làm:

```text
1. Kiểm tra các effect asset trong Assets/game_data/effects.
2. Gắn effect asset đầy đủ vào tacticEffects của tactic card.
3. Gắn passiveEffects cho hero bằng condition/duration hợp lý.
4. Test EffectResolver + StatCalculator hoặc test qua battle runtime sau này.
```

---

## 9. UI controllers hiện có

Folder:

```text
Assets/scripts/ui/
```

Files:

```text
MainMenuController.cs
EnemySetupController.cs
PlayerSetupController.cs
DeckSetupController.cs
DeckCardButtonUI.cs
OpponentDeckPreviewController.cs
DeckPreviewCardUI.cs
TerrainSetupController.cs
TerrainSlotButtonUI.cs
BattleUIController.cs
```

### 9.1. MainMenuController

File:

```text
Assets/scripts/ui/MainMenuController.cs
```

Vai trò:

- Nút Play reset session rồi sang `enemy_setup`.
- Nút Quit gọi `Application.Quit()`.

### 9.2. EnemySetupController

File:

```text
Assets/scripts/ui/EnemySetupController.cs
```

Vai trò:

- Chọn enemy country.
- Chọn difficulty bằng enum `AIDifficulty`.
- Random AI base style bằng `AIStyleSelector.PickRandomStyle()`.
- Lưu vào `GameSession`:
  - `enemyCountry`
  - `enemyDifficulty`
  - `enemyBasePlayStyle`
  - `enemyCurrentPlayStyle`
- Sau đó load `player_setup`.

### 9.3. PlayerSetupController

File:

```text
Assets/scripts/ui/PlayerSetupController.cs
```

Vai trò:

- Chọn player country.
- Lưu `GameSession.Instance.playerCountry`.
- Sau đó load `deck_setup`.

### 9.4. DeckSetupController

File:

```text
Assets/scripts/ui/DeckSetupController.cs
```

Vai trò:

- Đọc `GameSession.Instance.playerCountry`.
- Tạo button hero/tactic từ `heroPool` và `tacticPool`.
- Cho player chọn đúng:
  - 15 hero
  - 9 tactic
- Check tactic:
  - nếu `isShared = true` thì chọn được luôn.
  - nếu có `requiredTags` thì cần hero có tag phù hợp.
- Lưu player deck vào `GameSession`.
- Gọi AI chọn enemy deck.
- Lưu enemy deck vào `GameSession`.
- Load `opponent_deck_preview`.

Inspector của `deck_setup_manager` cần kéo:

```text
Country Text
Hero Count Text
Tactic Count Text
Message Text
Hero List Parent
Tactic List Parent
Button Prefab = deck_card_button
Aggressive Profile = aggressive_profile
Defensive Profile = defensive_profile
Balanced Profile = balanced_profile
```

### 9.5. OpponentDeckPreviewController

File:

```text
Assets/scripts/ui/OpponentDeckPreviewController.cs
```

Vai trò:

- Hiển thị enemy heroes.
- Hiển thị enemy tactics.
- Đếm ngược 60 giây.
- Hết giờ hoặc bấm Next thì load `terrain_setup`.

Inspector cần kéo:

```text
Title Text
Timer Text
Message Text
Enemy Hero List Parent
Enemy Tactic List Parent
Preview Card Prefab = deck_preview_card
Preview Time = 60
```

Cập nhật 2026-05-22:

```text
DeckPreviewCardUI đã cần đổi theo hướng mới để preview tacticEffects.
Không được hiển thị tactic bằng tactic.attackBonus / tactic.defenseBonus / tactic.healthBonus nữa.
```

Lỗi từng gặp:

```text
error CS1061: 'TacticCardData' does not contain a definition for 'healthBonus'
```

Nguyên nhân:

```text
TacticCardData đã chuyển sang tacticEffects nhưng DeckPreviewCardUI vẫn còn đọc field cũ.
```

Cách xử lý đúng:

```text
SetupTactic(TacticCardData tactic)
-> đọc tactic.tacticEffects
-> dùng BuildEffectText để tạo text preview
```


Lưu ý:

- `Enemy Hero List Parent` và `Enemy Tactic List Parent` phải là object `Content` trong Scroll View, không phải panel/viewport.

### 9.6. TerrainSetupController

File:

```text
Assets/scripts/ui/TerrainSetupController.cs
```

Vai trò:

- Hiển thị player terrain.
- Hiển thị enemy terrain.
- Player swap terrain.
- Enemy tự swap terrain theo AI.
- Hết 15 giây hoặc bấm Next thì lưu terrain order và load `battle`.

Inspector của `terrain_setup_manager` cần kéo:

```text
Timer Text
Message Text
Player Terrain List Parent
Player Terrain Slot Prefab = terrain_slot_button
Enemy Terrain List Parent
Enemy Terrain Slot Prefab = terrain_slot_button
Aggressive Profile = aggressive_profile
Defensive Profile = defensive_profile
Balanced Profile = balanced_profile
Setup Time = 15
Enemy Swap Interval = 1.5
```

### 9.7. BattleUIController

File:

```text
Assets/scripts/ui/BattleUIController.cs
```

Hiện trạng:

- Đây mới là UI battle rất đơn giản.
- Có biến tạm:
  - `playerHp = 100`
  - `enemyHp = 100`
- Hiển thị:
  - Player HP
  - Enemy HP
  - Turn: Player
- Có hàm `backToMenu()`.
- Chưa đọc `GameSession`.
- Chưa tạo deck runtime.
- Chưa shuffle.
- Chưa draw 5.
- Chưa random turn.
- Chưa có board/hand/combat/effect trong battle.

---

## 10. Prefab hiện có

Folder:

```text
Assets/prefabs/
```

Prefabs:

```text
deck_card_button
deck_preview_card
terrain_slot_button
```

### deck_card_button

Dùng trong `deck_setup`.

Script:

```text
DeckCardButtonUI.cs
```

Yêu cầu:

- Object gốc có `Button`.
- Có child `Text (TMP)`.
- Kéo child `Text (TMP)` vào `DeckCardButtonUI.labelText`.

### deck_preview_card

Dùng trong `opponent_deck_preview`.

Script:

```text
DeckPreviewCardUI.cs
```

Yêu cầu:

- Object gốc có `Image`.
- Có child `Text (TMP)`.
- Kéo hoặc để code tự tìm `labelText`.
- Nên set Text TMP:
  - màu tối
  - alignment Top Left
  - wrapping bật
  - stretch full card

### terrain_slot_button

Dùng trong `terrain_setup`.

Script:

```text
TerrainSlotButtonUI.cs
```

Yêu cầu:

- Object gốc có `Button`.
- Có child `Text (TMP)`.
- Kéo child `Text (TMP)` vào `TerrainSlotButtonUI.labelText`.

---

## 11. Trạng thái hiện tại đã làm được

Đã làm được:

```text
1. Đưa project Unity lên GitHub.
2. Tạo data ScriptableObject cho tag, terrain, hero, tactic, country.
3. Tạo country Viet Nam với 16 hero, 10 tactic, 7 terrain.
4. Tạo GameSession singleton giữ dữ liệu xuyên scene.
5. Làm menu -> enemy_setup.
6. Làm enemy_setup chọn enemy country + difficulty.
7. AI random base style ở enemy_setup.
8. Làm player_setup chọn player country.
9. Làm deck_setup cho player chọn 15 hero + 9 tactic.
10. Làm AI chọn enemy deck.
11. Làm opponent_deck_preview xem enemy deck 60 giây.
12. Làm terrain_setup:
    - player swap terrain
    - enemy swap terrain bằng AI counter
    - hai bên nhìn thấy terrain
    - lưu terrain order
13. Làm effect module cơ bản:
    - EffectData
    - EffectInstance
    - EffectResolver
    - StatCalculator
    - StatModifierEffectData
14. Battle scene hiện có UI rất đơn giản nhưng chưa có battle runtime thật.

15. Chuyển hướng AI deck scorer sang đọc `EffectData`:
    - hero dùng `passiveEffects`
    - tactic dùng `tacticEffects`
    - AI không phụ thuộc vào bonus cũ của tactic nữa
16. Phát hiện lỗi `DeckPreviewCardUI` còn gọi `tactic.healthBonus` sau khi đổi sang effect object và đã chốt cách sửa sang hiển thị `tacticEffects`.
17. Bổ sung `EffectConditionType.cs` để effect có điều kiện kích hoạt.
18. Sửa `EffectData.cs` để có `conditionType`, `requiredTerrain`, `requiredTag`.
19. Bắt đầu tạo folder `Assets/game_data/effects` để chứa effect asset.
20. Bắt đầu gắn thử effect vào một số tactic asset như `entrenched_hold` và `river_stake_ambush`.
```

---

## 12. Việc đang dở

Các phần chưa hoàn thiện:

```text
1. Cần kiểm tra đầy đủ các effect asset trong Assets/game_data/effects.
2. Cần gán đầy đủ tacticEffects cho từng tactic card trong Inspector.
3. Cần gán passiveEffects cho hero bằng condition/duration hợp lý.
4. Chưa test EffectResolver bằng EffectTestRunner.
5. Chưa có logic battle kiểm tra EffectConditionType thật.
6. Chưa có battle runtime thật.
7. Chưa có CardInstance.
8. Chưa có HeroInstance.
9. Chưa có PlayerBattleState.
10. Chưa có BattleState.
11. Chưa có BoardSlot.
12. Chưa có BattleInitializer.
13. Chưa có BattleManager thật.
14. Chưa có TurnManager.
15. Chưa có CombatResolver.
16. Chưa có TacticService.
17. Chưa có enemy AI đánh bài trong battle.
18. Chưa nối terrain bonus vào battle.
19. Chưa nối hero passiveEffects vào battle runtime thật.
20. Chưa nối tacticEffects vào battle runtime thật.
```

---

## 13. Mốc làm tiếp được khuyến nghị

Mốc tiếp theo nên làm theo thứ tự:

```text
Bước 1: Hoàn thiện test effect module
- kiểm tra Assets/game_data/effects và file .meta đã được Git theo dõi
- kiểm tra/tạo attack_plus_2
- kiểm tra/tạo defense_plus_2
- kiểm tra/tạo attack_minus_2
- kiểm tra/tạo defense_minus_2
- với passive phụ thuộc địa hình, dùng WhileConditionTrue + condition phù hợp
- với tactic ngắn hạn, dùng UntilEndOfTurn hoặc duration phù hợp
- gắn thử vào vài tactic/passive
- tạo EffectTestRunner hoặc test nhỏ để kiểm tra StatCalculator

Bước 2: Tạo battle runtime cơ bản
- CardInstance
- HeroInstance
- PlayerBattleState
- BoardSlot
- BattleState
- BattleInitializer

Bước 3: Khi vào battle
- đọc GameSession.selectedHeroes
- đọc GameSession.selectedTactics
- đọc GameSession.enemySelectedHeroes
- đọc GameSession.enemySelectedTactics
- đọc GameSession.playerTerrainOrder
- đọc GameSession.enemyTerrainOrder
- playerHealth = 100
- enemyHealth = 100
- tạo deck runtime
- shuffle deck
- draw 5 lá
- random người đi trước

Bước 4: Làm battle UI tối thiểu
- player HP
- enemy HP
- player deck count
- enemy deck count
- player hand
- 7 slot player
- 7 slot enemy
- terrain hai bên

Bước 5: Làm gameplay đơn giản trước
- player đặt hero vào slot
- turn đơn giản
- hero tấn công slot đối diện
- damage trực tiếp nếu slot đối diện trống

Bước 6: Sau khi battle chạy ổn
- nối tacticEffects vào battle
- nối hero passiveEffects
- nối terrain bonus
- làm AI battle
```

Mốc nhỏ nhất nên làm ngay sau file này:

```text
Kiểm tra Unity compile sau khi thêm EffectConditionType.cs, sau đó kiểm tra/gán effect asset trong Assets/game_data/effects cho tacticEffects và passiveEffects.
```

Hoặc nếu muốn bỏ qua test effect tạm thời:

```text
Làm BattleInitializer đọc GameSession và hiển thị HP, deck count, hand 5 lá đầu, terrain hai bên.
```

---

## 14. Quy tắc khi người khác hỗ trợ project này

Khi hỗ trợ project này, cần tuân thủ:

```text
1. Không tự bịa tên file, tên folder, tên scene, tên biến.
2. Nếu thiếu code hiện tại thì yêu cầu gửi đúng file liên quan.
3. Không đổi kiến trúc lớn nếu chưa có lý do rõ ràng.
4. Không nhét logic AI vào UI controller.
5. Không nhét toàn bộ effect vào BattleManager.
6. Không sửa trực tiếp chỉ số gốc trong HeroCardData hoặc TacticCardData.
7. Không thêm lại các field cũ `attackBonus` / `defenseBonus` / `healthBonus` vào `TacticCardData` chỉ để sửa lỗi compile; code cũ phải được chuyển sang đọc `tacticEffects`.
8. Khi thêm hệ thống mới, ưu tiên tách module:
   - battle runtime
   - combat
   - tactic
   - effect
   - AI
9. Giải thích bằng tiếng Việt rõ ràng, từng bước.
10. Code C# dùng tên class/file đúng với project.
11. Không mặc định mọi hero passive là Permanent; passive phụ thuộc điều kiện nên dùng WhileConditionTrue và EffectConditionType phù hợp.
```

---

## 15. Câu tóm tắt để gửi vào chat mới

```text
Tôi đang làm Hero Card Game bằng Unity 6000.3.11f1. Project hiện có flow scene: menu -> enemy_setup -> player_setup -> deck_setup -> opponent_deck_preview -> terrain_setup -> battle. Đã có GameSession singleton giữ dữ liệu xuyên scene. Đã có data ScriptableObject cho CountryData, HeroCardData, TacticCardData, TerrainData, TagData. Country Viet Nam có 16 hero, 10 tactic, 7 terrain. Đã tách AI thành module trong Assets/scripts/ai gồm core, profiles, deck, terrain, battle. AI hiện đã random play style, chọn enemy deck 15 hero + 9 tactic, và tự swap terrain theo hướng counter trong terrain_setup. Đã có scene opponent_deck_preview hiển thị deck địch 60 giây. Đã có effect module trong Assets/scripts/effects gồm EffectData, EffectConditionType, EffectInstance, EffectResolver, StatCalculator, StatModifierEffectData và enum liên quan. Hero passive dùng passiveEffects, tactic card dùng tacticEffects, AIDeckScorer đọc EffectData thay vì attackBonus/defenseBonus/healthBonus cũ. Mốc mới nhất là bổ sung EffectConditionType và thêm conditionType/requiredTerrain/requiredTag vào EffectData để passive không bị hiểu nhầm là Permanent vô điều kiện; passive phụ thuộc địa hình nên dùng WhileConditionTrue. Đã bắt đầu tạo Assets/game_data/effects và chỉnh thử một số tactic asset như entrenched_hold, river_stake_ambush. Battle hiện mới có BattleUIController rất đơn giản, chưa có battle runtime thật. Việc tiếp theo nên làm là kiểm tra/gán effect asset cho tactic/passive, test lại deck preview, rồi bắt đầu BattleInitializer đọc GameSession, tạo deck runtime, shuffle, draw 5 lá, random turn và hiển thị battle UI tối thiểu.
```
