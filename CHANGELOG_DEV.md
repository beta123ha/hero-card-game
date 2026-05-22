# CHANGELOG_DEV.md — Hero Card Game

> Nhật ký phát triển nội bộ của project.  
> File này dùng để ghi lại các mốc đã làm, thay đổi quan trọng, lỗi đã gặp và việc cần làm tiếp.  
> Khi code xong một phần lớn, hãy cập nhật file này rồi commit cùng code.

---

## 2026-05-22 — Chuyển AI deck và preview tactic sang hệ thống EffectData

### Bối cảnh

- Project đã chuyển hướng thiết kế effect của tactic card và nội tại hero sang `ScriptableObject`.
- Hero passive không nên chỉ là text mô tả nữa, mà cần có danh sách effect thật để battle xử lý sau này.
- Tactic card không nên dùng các field bonus cứng kiểu `attackBonus`, `defenseBonus`, `healthBonus` nữa.
- Hướng đúng từ mốc này:
  - `HeroCardData.passiveEffects`
  - `TacticCardData.tacticEffects`
  - effect cụ thể là asset kế thừa từ `EffectData`, ví dụ `StatModifierEffectData`.

### Đã làm / đã chốt trong chat

- Cập nhật hướng `TacticCardData.cs`:

```csharp
public List<EffectData> tacticEffects = new List<EffectData>();
```

- Cập nhật hướng `HeroCardData.cs`:

```csharp
public List<EffectData> passiveEffects = new List<EffectData>();
```

- Sửa hướng `AIDeckScorer.cs` để AI đọc effect object:
  - `ScoreHero(...)` cộng điểm từ `hero.passiveEffects`.
  - `ScoreTactic(...)` cộng điểm từ `tactic.tacticEffects`.
  - Không phụ thuộc vào `tactic.attackBonus`, `tactic.defenseBonus`, `tactic.healthBonus` nữa.
  - Nếu effect là `StatModifierEffectData`, AI đọc:
    - `statType`
    - `value`
    - `targetType`
    - `durationType`
    - `stackingType`
    - `maxStacks`
  - Debuff lên enemy được tính là có lợi cho AI.
  - Buff nhiều mục tiêu được chấm cao hơn buff một mục tiêu.
  - Effect lâu dài hoặc có thể stack được chấm cao hơn effect ngắn hạn.

- Giữ `AIDeckPlanner.cs` gần như không đổi vì file này chỉ cần gọi scorer:

```csharp
AIDeckScorer.ScoreHero(...)
AIDeckScorer.ScoreTactic(...)
AIDeckScorer.CanUseTactic(...)
```

- Điều kiện tactic hợp lệ vẫn giữ theo thiết kế cũ:
  - tactic dùng chung: `isShared = true`
  - tactic riêng: cần hero đã chọn có tag khớp `requiredTags`.

### Lỗi đã gặp

Unity báo lỗi:

```text
Assets/scripts/ui/DeckPreviewCardUI.cs(...): error CS1061: 'TacticCardData' does not contain a definition for 'healthBonus'
```

Nguyên nhân:

```text
TacticCardData đã chuyển sang dùng tacticEffects, nhưng DeckPreviewCardUI vẫn còn đọc field cũ tactic.healthBonus.
```

### Cách sửa đã chốt

- Sửa `DeckPreviewCardUI.cs` để tactic preview đọc:

```csharp
tactic.tacticEffects
```

- Không dùng lại các field cũ:

```text
tactic.attackBonus
tactic.defenseBonus
tactic.healthBonus
```

- Nên có các hàm helper trong `DeckPreviewCardUI.cs`:

```text
BuildEffectText(List<EffectData> effects)
BuildSingleEffectText(EffectData effect)
```

- Với `StatModifierEffectData`, preview nên hiển thị được:

```text
effectName, targetType, value, statType, durationType
```

### File đã sửa / cần đồng bộ

```text
Assets/scripts/data/TacticCardData.cs
Assets/scripts/data/HeroCardData.cs
Assets/scripts/ai/deck/AIDeckScorer.cs
Assets/scripts/ui/DeckPreviewCardUI.cs
PROJECT_CONTEXT.md
CHANGELOG_DEV.md
```

### Test cần làm khi mở lại Unity

1. Đợi Unity compile lại.
2. Nếu còn lỗi đỏ, tìm toàn project:

```text
tactic.attackBonus
tactic.defenseBonus
tactic.healthBonus
```

3. Những chỗ còn gọi các field trên phải sửa sang `tactic.tacticEffects`.
4. Chạy game từ scene `menu` theo flow:

```text
menu -> enemy_setup -> player_setup -> deck_setup -> opponent_deck_preview
```

5. Kiểm tra:
   - AI vẫn chọn được 15 hero + 9 tactic.
   - `opponent_deck_preview` vẫn hiện hero và tactic.
   - Tactic preview không còn lỗi `healthBonus`.
   - Nếu AI chọn thiếu tactic, kiểm tra `isShared`, `requiredTags`, hero tags và danh sách `tacticEffects`.

### Lưu ý quan trọng

- Không thêm lại `attackBonus`, `defenseBonus`, `healthBonus` vào `TacticCardData` chỉ để hết lỗi compile.
- Nếu lỗi đến từ UI hoặc AI cũ, phải sửa file đang gọi field cũ.
- `TerrainData` hiện vẫn có `attackBonus`, `defenseBonus`, `healthBonus`; phần terrain chưa bắt buộc đổi sang `EffectData`.
- Battle runtime thật vẫn chưa làm, nên effect hiện mới được nối vào data/AI/preview, chưa được xử lý trong trận.

### Việc cần làm tiếp

```text
1. Kiểm tra Unity compile sau khi sửa DeckPreviewCardUI.
2. Gán effect asset vào tacticEffects của từng tactic card.
3. Gán effect asset vào passiveEffects của một vài hero để test.
4. Test lại AI chọn deck.
5. Sau đó bắt đầu BattleInitializer đọc GameSession, tạo deck runtime, shuffle, draw 5 lá và random turn.
```

---

## 2026-05-18 — Thêm tài liệu theo dõi project và đưa project lên GitHub

### Đã làm

- Đưa project Unity lên GitHub thành công.
- Tạo file `PROJECT_CONTEXT.md` để mô tả tình trạng hiện tại của project.
- Tạo file `CHANGELOG_DEV.md` để ghi nhật ký phát triển theo từng mốc.
- Mục tiêu của 2 file này:
  - giúp người hỗ trợ đọc nhanh project đang ở đâu;
  - tránh nhầm tên folder, tên file, tên scene, tên biến;
  - giúp các chat sau tiếp tục đúng mạch hơn;
  - giúp GitHub lưu cả code lẫn bối cảnh phát triển.

### Ghi chú

- `PROJECT_CONTEXT.md` nên đặt ở thư mục gốc project, cùng cấp với:
  - `Assets`
  - `Packages`
  - `ProjectSettings`
- `CHANGELOG_DEV.md` cũng nên đặt ở thư mục gốc project.
- Sau mỗi mốc lớn, nên cập nhật cả 2 file này rồi commit.

### Lệnh Git nên dùng

```bash
git add PROJECT_CONTEXT.md CHANGELOG_DEV.md
git commit -m "add project documentation"
git push
```

---

## 2026-05-18 — Trạng thái tổng thể hiện tại

### Project

Tên project:

```text
Hero Card Game
```

Unity version:

```text
Unity 6000.3.11f1
```

Scene flow hiện tại:

```text
menu
-> enemy_setup
-> player_setup
-> deck_setup
-> opponent_deck_preview
-> terrain_setup
-> battle
```

Các scene hiện có trong `Assets/scenes/`:

```text
battle
deck_setup
enemy_setup
menu
opponent_deck_preview
player_setup
terrain_setup
```

Các scene đã có trong Build Settings / Scene List:

```text
Assets/scenes/menu.unity
Assets/scenes/enemy_setup.unity
Assets/scenes/player_setup.unity
Assets/scenes/deck_setup.unity
Assets/scenes/opponent_deck_preview.unity
Assets/scenes/terrain_setup.unity
Assets/scenes/battle.unity
```

### Luật gameplay đã chốt

- Người chơi chọn quốc gia cho enemy.
- Người chơi chọn độ khó enemy.
- Enemy random play style đầu trận:
  - Aggressive
  - Defensive
  - Balanced
- Người chơi chọn quốc gia cho bản thân.
- Người chơi chọn deck gồm:
  - 15 hero
  - 9 tactic
- AI chọn enemy deck gồm:
  - 15 hero
  - 9 tactic
- Người chơi xem enemy deck trong `opponent_deck_preview`.
- Thời gian preview deck:
  - 60 giây
- Mỗi bên có 7 ô địa hình.
- Trong `terrain_setup`:
  - player swap địa hình của mình;
  - enemy tự swap địa hình bằng AI;
  - hai bên nhìn thấy địa hình của nhau;
  - enemy nhìn địa hình player để counter.
- Thời gian setup terrain:
  - 15 giây
- Hết thời gian thì lưu terrain order rồi vào `battle`.

---

## 2026-05-18 — Data ScriptableObject hiện tại

### Đã có data class

Trong `Assets/scripts/data/` đã có:

```text
TagData.cs
TerrainData.cs
HeroCardData.cs
TacticCardData.cs
CountryData.cs
```

### TagData

Dùng để tạo tag cho hero và tactic.

Tag hiện có trong `Assets/game_data/tags/`:

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

### TerrainData

`TerrainData.cs` hiện đã có thêm dữ liệu phục vụ AI terrain:

```csharp
public List<TagData> favoredTags = new List<TagData>();
public int attackBonus;
public int defenseBonus;
public int healthBonus;
```

Terrain hiện có:

```text
high_land
plain
water
```

Giá trị hiện tại:

```text
high_land:
- favoredTags: land_warfare, guardian, positional_warfare
- attackBonus: 1
- defenseBonus: 3
- healthBonus: 2

plain:
- favoredTags: land_warfare, brute_force, shock_assault
- attackBonus: 3
- defenseBonus: 1
- healthBonus: 2

water:
- favoredTags: naval_warfare, strategic_mind
- attackBonus: 2
- defenseBonus: 2
- healthBonus: 2
```

### HeroCardData

Hero hiện có:

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

Đã thêm field effect cho hero passive:

```csharp
public List<EffectData> passiveEffects = new List<EffectData>();
```

Ghi chú:

- `passiveDescription` là mô tả cho người chơi đọc.
- `passiveEffects` là effect object thật để battle xử lý sau này.
- Hiện chưa nối hero passive vào battle.

### TacticCardData

Tactic hiện có:

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

Đã thêm field effect cho tactic:

```csharp
public List<EffectData> tacticEffects = new List<EffectData>();
```

Vẫn giữ các field cũ:

```csharp
public int attackBonus;
public int defenseBonus;
public int healthBonus;
```

Lý do giữ lại:

- AI deck scorer hiện vẫn có thể đang dùng các bonus cũ.
- Không nên xóa sớm để tránh lỗi dây chuyền.

### CountryData

Country hiện có:

```text
viet_nam
```

Thông tin hiện tại:

```text
countryName: Viet Nam
heroPool: 16 hero
tacticPool: 10 tactic
battlefieldTerrains: 7 terrain
```

Thứ tự terrain trong `viet_nam` hiện tại:

```text
high_land
high_land
water
water
plain
plain
plain
```

---

## 2026-05-18 — GameSession

### Đã làm

File:

```text
Assets/scripts/core/GameSession.cs
```

Vai trò:

- Giữ dữ liệu xuyên scene bằng singleton.
- Dùng `DontDestroyOnLoad`.
- Object chứa script này nằm trong scene `menu`.
- Object tên:

```text
game_session
```

### Dữ liệu đang lưu

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

### Ghi chú quan trọng

- `enemyDifficulty` hiện là enum `AIDifficulty`, không còn là string.
- `selectedHeroes` và `selectedTactics` là deck của player.
- `enemySelectedHeroes` và `enemySelectedTactics` là deck của enemy do AI chọn.
- Không nên test thẳng scene giữa vì có thể thiếu `GameSession`.

---

## 2026-05-18 — AI module

### Đã làm

Đã tách AI thành module riêng:

```text
Assets/scripts/ai/
├── battle/
├── core/
├── deck/
├── profiles/
└── terrain/
```

Nguyên tắc đã chốt:

- AI chỉ nhận dữ liệu và trả về quyết định.
- AI không trực tiếp sửa UI.
- AI không tự chuyển scene.
- AI không tự tạo button.
- Controller chỉ điều khiển màn hình.
- Battle sau này không nên chứa toàn bộ AI trong `BattleManager`.

### AI core

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

Đã có 3 play style:

```text
Aggressive
Defensive
Balanced
```

Đã có 3 difficulty:

```text
Easy
Normal
Hard
```

### AI profiles

Folder script:

```text
Assets/scripts/ai/profiles/
```

Folder asset:

```text
Assets/game_data/ai_profiles/
```

Script:

```text
AIPlayStyleProfile.cs
```

Asset profile hiện có:

```text
aggressive_profile
defensive_profile
balanced_profile
```

### AI chọn deck

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

Đã làm:

- AI chọn 15 hero.
- AI chọn 9 tactic.
- Chọn theo `AIPlayStyleProfile`.
- Có ảnh hưởng bởi `AIDifficulty`.
- Có xét tactic hợp lệ theo tag.
- Kết quả lưu vào:
  - `GameSession.Instance.enemySelectedHeroes`
  - `GameSession.Instance.enemySelectedTactics`

### AI xếp terrain

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

Đã làm:

- Enemy tạo initial terrain order.
- Enemy tự quyết định swap terrain.
- Enemy nhìn:
  - terrain hiện tại của enemy;
  - terrain hiện tại của player;
  - deck hero của enemy;
  - deck hero của player;
  - profile;
  - difficulty.
- AI trả về `AITerrainSwapDecision`.
- `TerrainSetupController` là nơi thực hiện swap và update UI.

### AI battle

Folder đã có:

```text
Assets/scripts/ai/battle/
```

Chưa làm:

```text
AIBattlePlanner.cs
AIBattleScorer.cs
AI action trong battle
```

---

## 2026-05-18 — Deck setup và opponent deck preview

### Đã làm deck_setup

Scene:

```text
Assets/scenes/deck_setup.unity
```

Script chính:

```text
Assets/scripts/ui/DeckSetupController.cs
Assets/scripts/ui/DeckCardButtonUI.cs
```

Prefab:

```text
Assets/prefabs/deck_card_button
```

Deck setup hiện làm được:

- Đọc `GameSession.Instance.playerCountry`.
- Sinh hero button từ `currentCountry.heroPool`.
- Sinh tactic button từ `currentCountry.tacticPool`.
- Player chọn/bỏ chọn hero.
- Player chọn/bỏ chọn tactic.
- Kiểm tra đúng:
  - 15 hero
  - 9 tactic
- Tactic bị lock nếu thiếu required tag.
- Lưu player deck vào `GameSession`.
- Gọi AI chọn enemy deck.
- Lưu enemy deck vào `GameSession`.
- Load scene:

```text
opponent_deck_preview
```

### Đã làm opponent_deck_preview

Scene:

```text
Assets/scenes/opponent_deck_preview.unity
```

Script:

```text
Assets/scripts/ui/OpponentDeckPreviewController.cs
Assets/scripts/ui/DeckPreviewCardUI.cs
```

Prefab:

```text
Assets/prefabs/deck_preview_card
```

Đã làm được:

- Hiển thị enemy heroes.
- Hiển thị enemy tactics.
- Timer 60 giây.
- Bấm Next sang `terrain_setup`.
- Hết giờ tự sang `terrain_setup`.

Lỗi từng gặp và đã xử lý:

- Card preview không hiện chữ.
- Text TMP bị màu trắng trên nền trắng.
- RectTransform của Text TMP quá nhỏ.
- Label Text chưa kéo đúng.
- Scroll View Content kéo nhầm object.

Ghi chú:

- `Enemy Hero List Parent` và `Enemy Tactic List Parent` phải kéo đúng object `Content` trong Scroll View.

---

## 2026-05-18 — Terrain setup

### Đã làm

Scene:

```text
Assets/scenes/terrain_setup.unity
```

Script:

```text
Assets/scripts/ui/TerrainSetupController.cs
Assets/scripts/ui/TerrainSlotButtonUI.cs
```

Prefab:

```text
Assets/prefabs/terrain_slot_button
```

Đã làm được:

- Hiển thị 7 terrain của player.
- Hiển thị 7 terrain của enemy.
- Player click 2 slot để swap.
- Click lại slot đang chọn để hủy chọn.
- Enemy tự swap terrain sau mỗi `enemySwapInterval`.
- Player nhìn thấy enemy terrain.
- Khi hết giờ hoặc bấm Next:
  - khóa terrain setup;
  - lưu `playerTerrainOrder`;
  - lưu `enemyTerrainOrder`;
  - load `battle`.

Thông số hiện tại:

```text
setupTime = 15
enemySwapInterval = 1.5
```

### Ghi chú AI terrain

AI terrain hiện không bắt buộc swap mỗi lần.

Nếu không có swap tốt, AI có thể không đổi.

Nếu terrain có bonus/favoredTags quá giống nhau hoặc enemy swap 2 terrain cùng loại, nhìn bên ngoài có thể thấy như không đổi.

---

## 2026-05-18 — Effect module

### Đã làm

Đã tạo effect module riêng:

```text
Assets/scripts/effects/
├── core/
├── runtime/
└── stat/
```

Mục tiêu:

- Không nhét effect vào `BattleManager`.
- Không sửa trực tiếp `HeroCardData`.
- Không sửa trực tiếp chỉ số gốc.
- Dễ thêm effect mới sau này.

### Effect core

Folder:

```text
Assets/scripts/effects/core/
```

Files:

```text
EffectData.cs
EffectDurationType.cs
EffectTargetType.cs
EffectInstance.cs
EffectStackingType.cs
```

`EffectData` là abstract ScriptableObject.

Field chính:

```csharp
public string effectName;
public string description;
public EffectTargetType targetType;
public EffectDurationType durationType;
public int durationTurns;
public EffectStackingType stackingType;
public int maxStacks;
public string stackKey;
```

### Stat effect

Folder:

```text
Assets/scripts/effects/stat/
```

Files:

```text
StatType.cs
StatModifierEffectData.cs
```

`StatModifierEffectData` dùng cho tăng/giảm chỉ số:

```csharp
public StatType statType = StatType.Attack;
public int value = 0;
```

### Effect runtime

Folder:

```text
Assets/scripts/effects/runtime/
```

Files:

```text
EffectResolver.cs
StatCalculator.cs
```

`EffectResolver` làm:

- Add effect.
- Xử lý stack.
- Tick cuối lượt.
- Xóa effect hết hạn.

`StatCalculator` làm:

- CalculateAttack.
- CalculateDefense.
- CalculateHealth.
- Cộng/trừ effect đang active.
- Có nhân `value * stackCount`.

### Chưa làm

Chưa tạo folder:

```text
Assets/game_data/effects/
```

Chưa tạo effect asset:

```text
attack_plus_2
defense_plus_2
attack_minus_2
defense_minus_2
rage_attack_plus_1
```

Chưa gắn effect asset vào tactic card.

Chưa test effect module bằng `EffectTestRunner`.

---

## 2026-05-18 — Battle hiện tại

### Đã có

Scene:

```text
Assets/scenes/battle.unity
```

Script UI hiện có:

```text
Assets/scripts/ui/BattleUIController.cs
```

Hiện script này mới làm rất đơn giản:

- `playerHp = 100`
- `enemyHp = 100`
- Hiển thị player HP.
- Hiển thị enemy HP.
- Hiển thị turn text.
- Có hàm quay lại menu.

### Chưa có battle runtime thật

Chưa làm:

```text
CardInstance
HeroInstance
PlayerBattleState
BoardSlot
BattleState
BattleInitializer
BattleManager thật
TurnManager
CombatResolver
TacticService
VisibilityService
GraveyardService
AIBattlePlanner
AIBattleScorer
```

### Battle chưa làm được

- Chưa đọc `GameSession`.
- Chưa tạo deck runtime.
- Chưa shuffle deck.
- Chưa draw 5 lá đầu.
- Chưa random người đi trước.
- Chưa hiển thị hand.
- Chưa hiển thị 7 slot hero player.
- Chưa hiển thị 7 slot hero enemy.
- Chưa xử lý đánh nhau.
- Chưa xử lý tactic.
- Chưa xử lý effect.
- Chưa xử lý terrain bonus.
- Chưa có AI đánh bài.

---

## 2026-05-18 — Các prefab hiện có

Folder:

```text
Assets/prefabs/
```

Prefab hiện có:

```text
deck_card_button
deck_preview_card
terrain_slot_button
```

### deck_card_button

Dùng trong:

```text
deck_setup
```

Script:

```text
DeckCardButtonUI.cs
```

Yêu cầu:

- Object gốc có `Button`.
- Có child `Text (TMP)`.
- Field `labelText` phải kéo Text TMP.

### deck_preview_card

Dùng trong:

```text
opponent_deck_preview
```

Script:

```text
DeckPreviewCardUI.cs
```

Yêu cầu:

- Object gốc có `Image`.
- Có child `Text (TMP)`.
- Text nên stretch full card.
- Text màu tối.
- Alignment Top Left.
- Wrapping bật.

### terrain_slot_button

Dùng trong:

```text
terrain_setup
```

Script:

```text
TerrainSlotButtonUI.cs
```

Yêu cầu:

- Object gốc có `Button`.
- Có child `Text (TMP)`.
- Field `labelText` phải kéo Text TMP.

---

## 2026-05-18 — Các lỗi / lưu ý quan trọng

### Không test thẳng scene giữa

Nên test từ:

```text
menu
```

Không nên test thẳng:

```text
deck_setup
opponent_deck_preview
terrain_setup
battle
```

Lý do:

- `GameSession` được tạo ở `menu`.
- Test thẳng scene giữa dễ bị `GameSession.Instance == null`.

### Scene phải nằm trong Build Settings

Nếu thiếu scene trong Scene List, `SceneManager.LoadScene()` sẽ lỗi.

Scene cần có:

```text
menu
enemy_setup
player_setup
deck_setup
opponent_deck_preview
terrain_setup
battle
```

### CountryData.battlefieldTerrains phải có đúng 7 phần tử

Nếu không đủ 7 terrain, `terrain_setup` sẽ không đúng luật game.

Hiện `viet_nam` đã có 7 terrain.

### Không dùng lại field bonus cũ của TacticCardData

Từ mốc 2026-05-22, AI deck scorer đã chuyển sang đọc `tacticEffects`.

Không nên thêm lại các field cũ chỉ để sửa lỗi compile:

```csharp
attackBonus
defenseBonus
healthBonus
```

Nếu còn script gọi các field này từ `TacticCardData`, cần sửa script đó sang đọc `tacticEffects`.

### Không sửa trực tiếp base stat

Không làm:

```csharp
hero.baseAttack += 2;
```

Cách đúng:

- base stat giữ nguyên.
- effect lưu trong `activeEffects`.
- dùng `StatCalculator` để tính chỉ số hiện tại.

---

## Việc cần làm tiếp

### Ưu tiên 1 — Hoàn thiện test effect module

Cần làm:

```text
1. Tạo folder Assets/game_data/effects.
2. Tạo asset Stat Modifier:
   - attack_plus_2
   - defense_plus_2
   - attack_minus_2
   - defense_minus_2
3. Set Inspector đúng cho từng asset.
4. Gắn effect vào 2 đến 4 tactic card.
5. Tạo EffectTestRunner hoặc test tạm.
6. Kiểm tra:
   - baseAttack = 5
   - add attack_plus_2
   - currentAttack = 7
```

### Ưu tiên 2 — Bắt đầu battle runtime

Cần tạo:

```text
Assets/scripts/battle/CardInstance.cs
Assets/scripts/battle/HeroInstance.cs
Assets/scripts/battle/PlayerBattleState.cs
Assets/scripts/battle/BoardSlot.cs
Assets/scripts/battle/BattleState.cs
Assets/scripts/battle/BattleInitializer.cs
```

Mục tiêu đầu tiên:

```text
BattleInitializer đọc GameSession và tạo trạng thái đầu trận.
```

Cần làm được:

```text
playerHealth = 100
enemyHealth = 100
tạo deck player từ selectedHeroes + selectedTactics
tạo deck enemy từ enemySelectedHeroes + enemySelectedTactics
shuffle deck hai bên
draw 5 lá đầu
random người đi trước
hiển thị HP, deck count, hand count, terrain hai bên
```

### Ưu tiên 3 — Battle UI tối thiểu

Cần hiển thị:

```text
player HP
enemy HP
turn text
player deck count
enemy deck count
player hand
7 player board slots
7 enemy board slots
player terrain row
enemy terrain row
```

### Ưu tiên 4 — Gameplay battle đơn giản

Sau khi state chạy ổn:

```text
1. Player đặt hero vào slot.
2. Hero tấn công hero đối diện.
3. Nếu ô đối diện trống thì đánh trực tiếp vào enemy HP.
4. End turn.
5. Enemy tạm thời hành động đơn giản trước.
```

### Ưu tiên 5 — Nối effect vào battle

Sau khi battle cơ bản chạy:

```text
1. Tactic card dùng tacticEffects.
2. Hero dùng passiveEffects.
3. Terrain bonus ảnh hưởng stat nếu hero phù hợp terrain.
4. Tick effect cuối lượt.
5. Xóa effect hết hạn.
```

### Ưu tiên 6 — AI battle

Sau khi battle chạy ổn:

```text
1. Tạo AIBattlePlanner.
2. Tạo AIBattleScorer.
3. AI chọn hành động theo profile:
   - Aggressive
   - Defensive
   - Balanced
4. Difficulty ảnh hưởng độ khôn và độ random.
```

---

## Mẫu ghi changelog cho lần sau

Khi làm xong một phần, thêm mục theo mẫu:

```markdown
## YYYY-MM-DD — Tên mốc

### Đã làm

- ...

### File đã thêm

- `Assets/scripts/...`

### File đã sửa

- `Assets/scripts/...`

### Test

- Đã test từ scene `menu`.
- Kết quả: ...

### Lỗi đã gặp

- ...

### Việc cần làm tiếp

- ...
```

---

## Commit gợi ý sau khi thêm file này

```bash
git add CHANGELOG_DEV.md PROJECT_CONTEXT.md
git commit -m "add project changelog and context"
git push
```
