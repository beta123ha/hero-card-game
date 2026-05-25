# CHANGELOG_DEV.md — Hero Card Game

> Nhật ký phát triển nội bộ của project.  
> File này dùng để ghi lại các mốc đã làm, thay đổi quan trọng, lỗi đã gặp và việc cần làm tiếp.  
> Khi code xong một phần lớn, hãy cập nhật file này rồi commit cùng code.

---

## 2026-05-25 — Chốt dữ liệu tạm thời cho hero và tactic trước khi làm battle

### Bối cảnh

- Trước khi bắt đầu làm `battle` screen và battle runtime thật, cần hoàn thiện bộ lá bài tạm thời để test tổng thể.
- Mục tiêu hiện tại không phải tạo full card pool cuối cùng, mà là có một bộ dữ liệu đủ sạch, đủ cân bằng và dùng đúng kiến trúc `EffectData`.
- Người dùng kiểm tra trực tiếp trong Unity Inspector và xác nhận:
  - `AIPlayStyleProfile.playStyle` đang hiển thị bằng tên enum như `Aggressive`, không phải người dùng phải nhập `0/1/2`;
  - `HeroCardData` hiện đang dùng field mới `passiveDescription` và `passiveEffects`, không còn 4 field mô tả cũ trong Inspector.

### Đã chốt trong chat

- Tạm thời hoàn thiện card pool hiện có của `Viet Nam` trước:
  - 16 hero asset;
  - 10 tactic asset.
- Khi vào trận test đầu tiên vẫn theo luật đã chốt:
  - chọn 15 hero;
  - chọn 9 tactic.
- Các chỉ số hero tạm thời nằm trong khoảng dễ test:
  - `baseAttack`: 3–7;
  - `baseDefense`: 2–6;
  - `baseHealth`: 9–13.
- Mỗi hero tạm thời dùng 1 passive đơn giản bằng `StatModifierEffectData`.
- Tactic giai đoạn đầu chỉ dùng các hiệu ứng tăng chỉ số đơn giản:
  - Attack;
  - Defense;
  - Health.
- Chưa làm thật các tactic/effect phức tạp như:
  - hồi máu thật;
  - rút bài;
  - di chuyển hero bằng tactic;
  - hồi sinh;
  - che thông tin;
  - sát thương đặc biệt;
  - giảm sát thương trực tiếp lên người chơi.

### Quy ước nhập hero tạm thời

Tất cả hero tạm thời dùng:

```text
Country Name: Viet Nam
```

Quy ước passive hero:

```text
Target Type: SelfHero
Duration Type: WhileConditionTrue
Duration Turns: 1
Stacking Type: NotStackableKeepStrongest
Max Stacks: 1
```

Nếu passive phụ thuộc địa hình cụ thể:

```text
Condition Type: HeroOnSpecificTerrain
Required Terrain: Plain / Water / High-land
```

Nếu passive phụ thuộc địa hình phù hợp với tag của hero:

```text
Condition Type: HeroOnFavoredTerrain
Required Terrain: None
```

### Bảng hero tạm thời đã chốt

```text
1. Dinh Bo Linh
- Asset: dinh_bo_linh
- Stats: ATK 5 / DEF 4 / HP 12
- Tags: brute_force, land_warfare
- Passive: Twelve Warlords Unifier
- Effect asset: dinh_bo_linh_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Attack +2

2. Ngo Quyen
- Asset: ngo_quyen
- Stats: ATK 5 / DEF 4 / HP 10
- Tags: naval_warfare, positional_warfare
- Passive: Bach Dang Ambush
- Effect asset: ngo_quyen_water_attack_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Water
- Stat: Attack +2

3. Tran Hung Dao
- Asset: tran_hung_dao
- Stats: ATK 4 / DEF 5 / HP 12
- Tags: naval_warfare, strategic_mind
- Passive: River Defense
- Effect asset: tran_hung_dao_water_defense_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Water
- Stat: Defense +2

4. Ly Thuong Kiet
- Asset: ly_thuong_kiet
- Stats: ATK 4 / DEF 5 / HP 11
- Tags: positional_warfare, strategic_mind
- Passive: Preemptive Defense
- Effect asset: ly_thuong_kiet_high_land_defense_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = High-land
- Stat: Defense +2

5. Le Hoan
- Asset: le_hoan
- Stats: ATK 5 / DEF 3 / HP 10
- Tags: mobile_warfare, strategic_mind
- Passive: Rapid Counterattack
- Effect asset: le_hoan_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Plain
- Stat: Attack +2

6. Le Loi
- Asset: le_loi
- Stats: ATK 5 / DEF 4 / HP 11
- Tags: land_warfare, mobile_warfare
- Passive: Lam Son Uprising
- Effect asset: le_loi_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Attack +2

7. Nguyen Trai
- Asset: nguyen_trai
- Stats: ATK 3 / DEF 5 / HP 11
- Tags: strategic_mind, guardian
- Passive: Strategic Counsel
- Effect asset: nguyen_trai_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Defense +2

8. Pham Ngu Lao
- Asset: pham_ngu_lao
- Stats: ATK 6 / DEF 3 / HP 10
- Tags: land_warfare, shock_assault
- Passive: Frontline Charge
- Effect asset: pham_ngu_lao_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Plain
- Stat: Attack +2

9. Quang Trung
- Asset: quang_trung
- Stats: ATK 7 / DEF 2 / HP 9
- Tags: mobile_warfare, shock_assault
- Passive: Lightning March
- Effect asset: quang_trung_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Plain
- Stat: Attack +2

10. Tran Quang Khai
- Asset: tran_quang_khai
- Stats: ATK 4 / DEF 4 / HP 11
- Tags: land_warfare, strategic_mind
- Passive: Stabilize Front
- Effect asset: tran_quang_khai_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Defense +2

11. Tran Quoc Toan
- Asset: tran_quoc_toan
- Stats: ATK 6 / DEF 2 / HP 9
- Tags: shock_assault, land_warfare
- Passive: Youthful Resolve
- Effect asset: tran_quoc_toan_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Attack +2

12. Trieu Thi Trinh
- Asset: trieu_thi_trinh
- Stats: ATK 6 / DEF 3 / HP 10
- Tags: brute_force, shock_assault
- Passive: Fearless Assault
- Effect asset: trieu_thi_trinh_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = Plain
- Stat: Attack +2

13. Trung Trac
- Asset: trung_trac
- Stats: ATK 5 / DEF 4 / HP 11
- Tags: guardian, shock_assault
- Passive: Rising Banner
- Effect asset: trung_trac_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Defense +2

14. Trung Nhi
- Asset: trung_nhi
- Stats: ATK 4 / DEF 5 / HP 11
- Tags: guardian, mobile_warfare
- Passive: Sister's Guard
- Effect asset: trung_nhi_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Defense +2

15. Vo Nguyen Giap
- Asset: vo_nguyen_giap
- Stats: ATK 4 / DEF 5 / HP 12
- Tags: land_warfare, strategic_mind
- Passive: People's War
- Effect asset: vo_nguyen_giap_high_land_defense_plus_2
- Condition: HeroOnSpecificTerrain, Required Terrain = High-land
- Stat: Defense +2

16. Ho Chi Minh
- Asset: ho_chi_minh
- Stats: ATK 3 / DEF 5 / HP 12
- Tags: strategic_mind, guardian
- Passive: National Resolve
- Effect asset: ho_chi_minh_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat: Defense +2
```

### Bộ 15 hero nên chọn cho trận test đầu tiên

```text
1. Dinh Bo Linh
2. Ngo Quyen
3. Tran Hung Dao
4. Ly Thuong Kiet
5. Le Hoan
6. Le Loi
7. Nguyen Trai
8. Pham Ngu Lao
9. Quang Trung
10. Tran Quang Khai
11. Tran Quoc Toan
12. Trieu Thi Trinh
13. Trung Trac
14. Trung Nhi
15. Vo Nguyen Giap
```

Tạm thời chưa đưa `Ho Chi Minh` vào deck test đầu tiên vì hero này hợp hơn với effect hỗ trợ đồng minh/toàn bàn, nên nên để dành cho giai đoạn sau khi battle runtime đã ổn hơn.

### Quy ước nhập tactic tạm thời

Tất cả tactic tạm thời dùng:

```text
Country Name: Viet Nam
```

Với tactic buff đồng minh:

```text
Target Type: SelectedAllyHero
Duration Type: UntilEndOfTurn
Duration Turns: 1
Stacking Type: NotStackableKeepStrongest
Max Stacks: 1
```

Với tactic dùng chung:

```text
Condition Type: Always
Required Tags: empty
```

Với tactic yêu cầu tag:

```text
Condition Type: HeroHasTag
Required Tag: tag tương ứng
```

### Bảng tactic tạm thời đã chốt

```text
1. Call To Arms
- Asset: call_to_arms
- Is Shared: true
- Required Tags: empty
- Effect Description: Choose 1 friendly hero. That hero gains +1 Attack and +1 Defense until end of turn.
- Effect assets:
  - call_to_arms_attack_plus_1: Attack +1
  - call_to_arms_defense_plus_1: Defense +1

2. Field Medicine
- Asset: field_medicine
- Is Shared: true
- Required Tags: empty
- Effect Description: Choose 1 friendly hero. That hero gains +2 Health until end of turn.
- Effect asset:
  - field_medicine_health_plus_2: Health +2
- Note: đây chưa phải hồi máu thật, chỉ là tăng Health tạm thời để test stat effect.

3. River Stake Ambush
- Asset: river_stake_ambush
- Is Shared: false
- Required Tags: naval_warfare
- Effect Description: Choose 1 friendly hero with Naval Warfare. That hero gains +2 Attack until end of turn.
- Effect asset:
  - river_stake_ambush_attack_plus_2: Attack +2

4. Entrenched Hold
- Asset: entrenched_hold
- Is Shared: false
- Required Tags: positional_warfare
- Effect Description: Choose 1 friendly hero with Positional Warfare. That hero gains +3 Defense until end of turn.
- Effect asset:
  - entrenched_hold_defense_plus_3: Defense +3

5. Terrain Offensive
- Asset: terrain_offensive
- Is Shared: false
- Required Tags: land_warfare
- Effect Description: Choose 1 friendly hero with Land Warfare. That hero gains +2 Attack and +1 Defense until end of turn.
- Effect assets:
  - terrain_offensive_attack_plus_2: Attack +2
  - terrain_offensive_defense_plus_1: Defense +1

6. Thunder Charge
- Asset: thunder_charge
- Is Shared: false
- Required Tags: shock_assault
- Effect Description: Choose 1 friendly hero with Shock Assault. That hero gains +3 Attack until end of turn.
- Effect asset:
  - thunder_charge_attack_plus_3: Attack +3

7. Iron Wall Formation
- Asset: iron_wall_formation
- Is Shared: false
- Required Tags: guardian
- Effect Description: Choose 1 friendly hero with Guardian. That hero gains +2 Defense and +2 Health until end of turn.
- Effect assets:
  - iron_wall_formation_defense_plus_2: Defense +2
  - iron_wall_formation_health_plus_2: Health +2

8. Master Campaign Plan
- Asset: master_campaign_plan
- Is Shared: false
- Required Tags: strategic_mind
- Effect Description: Choose 1 friendly hero with Strategic Mind. That hero gains +1 Attack and +1 Defense until end of turn.
- Effect assets:
  - master_campaign_plan_attack_plus_1: Attack +1
  - master_campaign_plan_defense_plus_1: Defense +1

9. Rapid Redeployment
- Asset: rapid_redeployment
- Is Shared: false
- Required Tags: mobile_warfare
- Effect Description: Choose 1 friendly hero with Mobile Warfare. That hero gains +1 Attack and +1 Defense until end of turn.
- Effect assets:
  - rapid_redeployment_attack_plus_1: Attack +1
  - rapid_redeployment_defense_plus_1: Defense +1

10. Overwhelming Force
- Asset: overwhelming_force
- Is Shared: false
- Required Tags: brute_force
- Effect Description: Choose 1 friendly hero with Brute Force. That hero gains +3 Attack until end of turn.
- Effect asset:
  - overwhelming_force_attack_plus_3: Attack +3
```

### Bộ 9 tactic nên chọn cho trận test đầu tiên

```text
1. Call To Arms
2. River Stake Ambush
3. Entrenched Hold
4. Terrain Offensive
5. Thunder Charge
6. Iron Wall Formation
7. Master Campaign Plan
8. Rapid Redeployment
9. Overwhelming Force
```

Tạm thời chưa chọn `Field Medicine` trong trận test đầu tiên để tránh nhầm giữa `Health +2` tạm thời và hồi máu thật.

### Việc cần làm tiếp

```text
1. Nhập/sửa đầy đủ hero stats, tags, passiveDescription trong Unity Inspector.
2. Tạo đầy đủ effect asset trong Assets/game_data/effects.
3. Gán effect asset vào passiveEffects của từng hero.
4. Gán effect asset vào tacticEffects của từng tactic.
5. Kiểm tra lại requiredTags của tactic.
6. Chạy từ scene menu để test:
   menu -> enemy_setup -> player_setup -> deck_setup -> opponent_deck_preview -> terrain_setup -> battle
7. Kiểm tra deck_setup:
   - chọn được đúng 15 hero + 9 tactic;
   - tactic bị lock/unlock đúng theo requiredTags.
8. Kiểm tra opponent_deck_preview:
   - hero hiển thị đúng chỉ số;
   - tactic hiển thị đúng effect từ tacticEffects.
9. Sau khi data card ổn mới bắt đầu BattleInitializer.
```

### Lưu ý quan trọng

- Không cần tạo class C# riêng cho từng effect như `AttackPlus2Effect.cs`.
- Mọi effect tăng/giảm chỉ số giai đoạn này vẫn dùng chung `StatModifierEffectData`.
- Mỗi effect cụ thể là một asset trong `Assets/game_data/effects`.
- Không sửa trực tiếp stat gốc trong runtime.
- Không quay lại dùng `attackBonus`, `defenseBonus`, `healthBonus` trong `TacticCardData`.
- Chưa ghi trong tài liệu rằng toàn bộ asset đã nhập xong nếu người dùng mới chỉ chốt thông tin; cần kiểm tra lại trực tiếp trong Unity trước khi commit data asset.

---

## 2026-05-24 — Thêm điều kiện cho EffectData và bắt đầu tạo effect asset

### Bối cảnh

- Khi bắt đầu gán effect vào `passiveEffects` của hero, phát hiện nếu để nội tại dạng `Permanent` thì không đúng thiết kế lâu dài.
- Nội tại hero không nên mặc định luôn áp dụng vĩnh viễn.
- Thiết kế đúng là nội tại có thể phụ thuộc vào điều kiện, ví dụ:
  - hero đứng trên địa hình phù hợp thì được cộng chỉ số;
  - hero rời khỏi địa hình đó thì buff ngừng tác dụng;
  - hero đối đầu enemy có tag cụ thể thì nhận lợi thế hoặc bất lợi.

### Đã làm / đã chốt trong chat

- Bổ sung hướng thiết kế điều kiện cho effect bằng enum mới:

```text
Assets/scripts/effects/core/EffectConditionType.cs
```

- Các loại condition hiện chốt cho giai đoạn đầu:

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

- Cập nhật `EffectData.cs` để mỗi effect có thể khai báo điều kiện:

```csharp
[Header("Condition")]
public EffectConditionType conditionType = EffectConditionType.Always;
public TerrainData requiredTerrain;
public TagData requiredTag;
```

- Quy ước mới:
  - effect không có điều kiện đặc biệt dùng `conditionType = Always`;
  - nội tại phụ thuộc địa hình nên dùng `conditionType = HeroOnFavoredTerrain` hoặc `HeroOnSpecificTerrain`;
  - nội tại chỉ tồn tại khi điều kiện đúng nên dùng `durationType = WhileConditionTrue`, không dùng `Permanent` bừa bãi.

### Đã tạo / đang tạo data effect

Đã bắt đầu tạo folder asset effect:

```text
Assets/game_data/effects/
```

Git đang thấy các file/folder mới liên quan:

```text
Assets/game_data/effects.meta
Assets/game_data/effects/
Assets/scripts/effects/core/EffectConditionType.cs
Assets/scripts/effects/core/EffectConditionType.cs.meta
```

Đã có chỉnh sửa asset tactic để thử gắn effect:

```text
Assets/game_data/tactics/entrenched_hold.asset
Assets/game_data/tactics/river_stake_ambush.asset
```

### Lưu ý thiết kế quan trọng

- `Permanent` chỉ dùng cho effect thật sự tồn tại lâu dài trong trận sau khi được áp dụng.
- Passive hero kiểu “đứng trên địa hình phù hợp thì nhận buff” phải dùng:

```text
Condition Type: HeroOnFavoredTerrain
Duration Type: WhileConditionTrue
```

- Không sửa trực tiếp `baseAttack`, `baseDefense`, `baseHealth` của `HeroCardData`.
- Chỉ số hiện tại sau này phải được tính từ:

```text
base stat + effect đang có hiệu lực + terrain/passive/tactic hợp lệ
```

### Trạng thái Git tại thời điểm cập nhật

Các file đã sửa nhưng chưa commit:

```text
modified:   Assets/game_data/tactics/entrenched_hold.asset
modified:   Assets/game_data/tactics/river_stake_ambush.asset
modified:   Assets/scripts/effects/core/EffectData.cs
```

Các file mới cần `git add`:

```text
Assets/game_data/effects.meta
Assets/game_data/effects/
Assets/scripts/effects/core/EffectConditionType.cs
Assets/scripts/effects/core/EffectConditionType.cs.meta
```

### Lệnh Git nên dùng

Nếu muốn commit toàn bộ phần effect condition và asset test hiện tại:

```bash
git add Assets/scripts/effects/core/EffectData.cs
git add Assets/scripts/effects/core/EffectConditionType.cs Assets/scripts/effects/core/EffectConditionType.cs.meta
git add Assets/game_data/effects.meta Assets/game_data/effects
git add Assets/game_data/tactics/entrenched_hold.asset Assets/game_data/tactics/river_stake_ambush.asset
git commit -m "feat(effects): add conditional effect support"
```

### Việc cần làm tiếp

```text
1. Mở Unity kiểm tra compile sau khi thêm EffectConditionType.cs.
2. Kiểm tra Inspector của các effect asset trong Assets/game_data/effects.
3. Với tactic effect ngắn hạn, dùng UntilEndOfTurn hoặc duration phù hợp.
4. Với hero passive phụ thuộc địa hình, dùng WhileConditionTrue + conditionType phù hợp.
5. Gắn thử effect vào passiveEffects của 1 hero và tacticEffects của 1-2 tactic.
6. Test lại deck preview xem effect name/target/stat/duration có hiển thị đúng không.
7. Sau khi data effect ổn mới bắt đầu BattleInitializer.
```

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

### Trạng thái sau cập nhật 2026-05-24

Đã bắt đầu tạo folder asset effect:

```text
Assets/game_data/effects/
```

Đã bổ sung điều kiện cho effect:

```text
Assets/scripts/effects/core/EffectConditionType.cs
```

Các effect asset cụ thể trong `Assets/game_data/effects/` cần tiếp tục kiểm tra trong Unity Inspector, đặc biệt các asset test kiểu:

```text
attack_plus_2
defense_plus_2
attack_minus_2
defense_minus_2
```

Còn cần làm:

```text
1. Gắn effect asset đầy đủ vào tacticEffects của các tactic card.
2. Gắn effect asset hợp lý vào passiveEffects của hero.
3. Test effect module bằng EffectTestRunner hoặc test trong battle runtime sau này.
```

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
1. Kiểm tra Assets/game_data/effects đã được tạo đúng và có file .meta.
2. Kiểm tra/tạo các asset Stat Modifier:
   - attack_plus_2
   - defense_plus_2
   - attack_minus_2
   - defense_minus_2
3. Set Inspector đúng cho từng asset:
   - targetType
   - conditionType
   - durationType
   - stackingType
   - statType
   - value
4. Với passive phụ thuộc địa hình, dùng WhileConditionTrue + HeroOnFavoredTerrain/HeroOnSpecificTerrain.
5. Với tactic ngắn hạn, dùng UntilEndOfTurn hoặc duration phù hợp.
6. Gắn effect vào 2 đến 4 tactic card.
7. Tạo EffectTestRunner hoặc test tạm.
8. Kiểm tra:
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
