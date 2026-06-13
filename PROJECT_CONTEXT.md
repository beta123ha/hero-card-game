# PROJECT_CONTEXT.md — Hero Card Game

> Cập nhật gần nhất: 2026-06-14  
> Mục đích file này: giúp người hỗ trợ đọc nhanh tình trạng thật của project, tránh hướng dẫn sai tên scene, tên folder, tên script, tên biến hoặc viết lệch kiến trúc hiện tại.

---

## 0. Cập nhật mới nhất — 2026-06-14

Mốc mới nhất của project là **thêm player hand scroll, deploy hero từ hand xuống board và đặt úp tactic trong scene `battle`**.

### Lý do cập nhật

- Battle runtime foundation đã chạy được: đã có `BattleState`, `BattlePlayerState`, deck runtime, draw 5 lá, random turn và render board slots.
- Mốc mới này bắt đầu cho người chơi thao tác thật trong trận:
  - nhìn thấy hand;
  - kéo ngang hand nếu có nhiều card;
  - chọn hero trên tay và deploy xuống board;
  - chọn tactic trên tay và đặt úp vào tactic slot;
  - dùng nút end turn tạm để test lượt.

### Runtime/UI battle đã có thêm

Đã thêm / cập nhật các thành phần:

```text
BattleHandCardUI
BattleTacticSlotUI
BattleUIController
BattleBoardSlotUI
BattlePlayerState
battle_hand_card.prefab
battle.unity
```

### Player hand hiện tại

Player hand trong `battle` hiện dùng Scroll View ngang:

```text
PlayerHandScrollView
└── Viewport
    └── PlayerHandContent
```

Mục đích:

```text
Nếu số card trên tay tăng nhiều, card không tràn khỏi khung mà người chơi có thể kéo trái/phải để xem.
```

`BattleUIController` render hand từ:

```text
BattleState.playerState.hand
```

Mỗi card trên tay được hiển thị bằng:

```text
Assets/scripts/ui/BattleHandCardUI.cs
Assets/prefabs/battle_hand_card.prefab
```

### Deploy hero hiện tại

Player có thể:

```text
1. Click 1 hero card trên tay.
2. Click 1 ô player board trống.
3. BattleUIController tạo BattleHeroInstance.
4. Đặt hero vào BattleBoardSlot.
5. Xóa card khỏi hand.
6. Refresh UI.
```

Luật đang áp dụng:

```text
chỉ deploy khi tới lượt player
chỉ deploy card loại Hero
không deploy vào enemy board
không deploy vào ô đã có hero
mỗi lượt chỉ dùng 1 hero action
```

Biến theo dõi trong `BattlePlayerState`:

```csharp
public bool hasUsedHeroActionThisTurn;
```

### Tactic placement hiện tại

Player có thể:

```text
1. Click 1 tactic card trên tay.
2. Click 1 trong 3 tactic slot của player.
3. Tactic được đặt vào slot ở trạng thái face down.
4. Card bị xóa khỏi hand.
5. UI refresh.
```

UI tactic slot dùng:

```text
Assets/scripts/ui/BattleTacticSlotUI.cs
```

Luật đang áp dụng:

```text
chỉ đặt tactic khi tới lượt player
chỉ đặt card loại Tactic
không đặt vào enemy tactic slot
không đặt vào tactic slot đã có card
mỗi lượt chỉ dùng 1 tactic action
```

Biến theo dõi trong `BattlePlayerState`:

```csharp
public bool hasUsedTacticActionThisTurn;
```

### End turn hiện tại

`BattleUIController.OnEndTurnClicked()` hiện làm:

```text
đánh dấu tactic của người chơi cũ không còn là vừa đặt trong lượt này
SwitchTurn()
reset hero action và tactic action cho current player mới
current player rút 1 lá
bỏ chọn card đang chọn
refresh UI
```

### Back trong battle

Nút back trong battle hiện quay về:

```text
enemy_setup
```

Hàm dùng:

```csharp
public void BackToEnemySetup()
{
    SceneManager.LoadScene("enemy_setup");
}
```

### Kết quả test hiện tại

Người dùng đã xác nhận làm được:

```text
player hand hiển thị card
hand có thể kéo ngang
deploy hero từ hand xuống board
đặt úp tactic vào 3 tactic slot
back trong battle quay về enemy_setup
```

### Chưa làm trong battle sau mốc này

```text
1. Chưa kích hoạt tactic đã đặt úp.
2. Chưa chọn mục tiêu cho tactic.
3. Chưa apply tacticEffects vào hero/player.
4. Chưa nối hero passiveEffects vào runtime theo condition.
5. Chưa nối terrain bonus/effect vào tính chỉ số thật trong battle.
6. Chưa có phase manager đầy đủ.
7. Chưa có combat resolver.
8. Chưa có enemy AI battle action.
9. Chưa có win/lose/draw resolver.
```

### Việc làm tiếp ngay sau mốc này

Mốc nhỏ tiếp theo nên làm là **kích hoạt tactic đã đặt úp và apply effect lên hero**:

```text
1. Click tactic slot đã có tactic face down.
2. Kiểm tra tactic không phải vừa được đặt trong lượt hiện tại.
3. Nếu tactic target là SelectedAllyHero, cho player chọn 1 hero đồng minh.
4. Dùng EffectResolver để add tacticEffects vào hero.activeEffects.
5. Refresh board UI để thấy chỉ số thay đổi.
6. Sau khi tactic chạy được, làm combat cơ bản.
```

---

## 0.1. Mốc trước — 2026-05-25

Mốc này là **chốt dữ liệu tạm thời cho hero và tactic card trước khi bắt đầu làm battle screen/battle runtime**.

### Lý do cập nhật

- Battle runtime chưa nên làm sâu khi card data vẫn còn placeholder hoặc chưa nối đầy đủ với `EffectData`.
- Cần có một bộ lá bài tạm thời nhưng tương đối hoàn chỉnh để test tổng thể:
  - deck setup;
  - AI chọn deck;
  - opponent deck preview;
  - terrain setup;
  - battle initializer sau này.
- Người dùng đã kiểm tra trực tiếp trong Unity Inspector và xác nhận trạng thái hiện tại:
  - `AIPlayStyleProfile.playStyle` đang hiển thị bằng tên enum như `Aggressive`, `Defensive`, `Balanced`, không phải người dùng phải nhập `0/1/2`;
  - `HeroCardData` hiện đang hiển thị field mới `passiveDescription` và `passiveEffects`, không còn 4 field description cũ trong Inspector.

### Hướng chốt cho card data tạm thời

Tạm thời hoàn thiện card pool hiện có của `Viet Nam`:

```text
16 hero
10 tactic
```

Khi test trận đầu vẫn chọn theo luật hiện tại:

```text
15 hero
9 tactic
```

Mục tiêu của giai đoạn này:

```text
1. Dữ liệu hero không còn để 1/1/1.
2. Mỗi hero có tags rõ ràng.
3. Mỗi hero có passiveDescription.
4. Mỗi hero có 1 passiveEffects đơn giản.
5. Mỗi tactic có tacticEffects rõ ràng.
6. AI deck scorer và deck preview có dữ liệu effect để đọc.
7. BattleInitializer sau này có dữ liệu thật để hiển thị/test.
```

Chưa làm trong mốc này:

```text
1. Chưa code battle runtime thật.
2. Chưa code HealEffectData.
3. Chưa code DrawCardEffectData.
4. Chưa code MoveHeroEffectData.
5. Chưa code ReviveEffectData.
6. Chưa code HiddenInformationEffectData.
```

### Quy ước hero tạm thời

Tất cả hero dùng:

```text
Country Name: Viet Nam
```

Chỉ số tạm thời:

```text
Base Attack: 3–7
Base Defense: 2–6
Base Health: 9–13
```

Passive giai đoạn đầu:

```text
1 hero = 1 passive effect
Effect class = StatModifierEffectData
Target Type = SelfHero
Duration Type = WhileConditionTrue
Duration Turns = 1
Stacking Type = NotStackableKeepStrongest
Max Stacks = 1
```

Passive phụ thuộc địa hình cụ thể:

```text
Condition Type = HeroOnSpecificTerrain
Required Terrain = Plain / Water / High-land
```

Passive phụ thuộc địa hình phù hợp với tag:

```text
Condition Type = HeroOnFavoredTerrain
Required Terrain = None
```

### Hero card data đã chốt để nhập

```text
Dinh Bo Linh
- Asset: dinh_bo_linh
- Country Name: Viet Nam
- Base Attack: 5
- Base Defense: 4
- Base Health: 12
- Tags: brute_force, land_warfare
- Passive: Twelve Warlords Unifier
- Effect: dinh_bo_linh_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Attack +2

Ngo Quyen
- Asset: ngo_quyen
- Country Name: Viet Nam
- Base Attack: 5
- Base Defense: 4
- Base Health: 10
- Tags: naval_warfare, positional_warfare
- Passive: Bach Dang Ambush
- Effect: ngo_quyen_water_attack_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Water
- Stat Modifier: Attack +2

Tran Hung Dao
- Asset: tran_hung_dao
- Country Name: Viet Nam
- Base Attack: 4
- Base Defense: 5
- Base Health: 12
- Tags: naval_warfare, strategic_mind
- Passive: River Defense
- Effect: tran_hung_dao_water_defense_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Water
- Stat Modifier: Defense +2

Ly Thuong Kiet
- Asset: ly_thuong_kiet
- Country Name: Viet Nam
- Base Attack: 4
- Base Defense: 5
- Base Health: 11
- Tags: positional_warfare, strategic_mind
- Passive: Preemptive Defense
- Effect: ly_thuong_kiet_high_land_defense_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: High-land
- Stat Modifier: Defense +2

Le Hoan
- Asset: le_hoan
- Country Name: Viet Nam
- Base Attack: 5
- Base Defense: 3
- Base Health: 10
- Tags: mobile_warfare, strategic_mind
- Passive: Rapid Counterattack
- Effect: le_hoan_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Plain
- Stat Modifier: Attack +2

Le Loi
- Asset: le_loi
- Country Name: Viet Nam
- Base Attack: 5
- Base Defense: 4
- Base Health: 11
- Tags: land_warfare, mobile_warfare
- Passive: Lam Son Uprising
- Effect: le_loi_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Attack +2

Nguyen Trai
- Asset: nguyen_trai
- Country Name: Viet Nam
- Base Attack: 3
- Base Defense: 5
- Base Health: 11
- Tags: strategic_mind, guardian
- Passive: Strategic Counsel
- Effect: nguyen_trai_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Defense +2

Pham Ngu Lao
- Asset: pham_ngu_lao
- Country Name: Viet Nam
- Base Attack: 6
- Base Defense: 3
- Base Health: 10
- Tags: land_warfare, shock_assault
- Passive: Frontline Charge
- Effect: pham_ngu_lao_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Plain
- Stat Modifier: Attack +2

Quang Trung
- Asset: quang_trung
- Country Name: Viet Nam
- Base Attack: 7
- Base Defense: 2
- Base Health: 9
- Tags: mobile_warfare, shock_assault
- Passive: Lightning March
- Effect: quang_trung_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Plain
- Stat Modifier: Attack +2

Tran Quang Khai
- Asset: tran_quang_khai
- Country Name: Viet Nam
- Base Attack: 4
- Base Defense: 4
- Base Health: 11
- Tags: land_warfare, strategic_mind
- Passive: Stabilize Front
- Effect: tran_quang_khai_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Defense +2

Tran Quoc Toan
- Asset: tran_quoc_toan
- Country Name: Viet Nam
- Base Attack: 6
- Base Defense: 2
- Base Health: 9
- Tags: shock_assault, land_warfare
- Passive: Youthful Resolve
- Effect: tran_quoc_toan_favored_terrain_attack_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Attack +2

Trieu Thi Trinh
- Asset: trieu_thi_trinh
- Country Name: Viet Nam
- Base Attack: 6
- Base Defense: 3
- Base Health: 10
- Tags: brute_force, shock_assault
- Passive: Fearless Assault
- Effect: trieu_thi_trinh_plain_attack_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: Plain
- Stat Modifier: Attack +2

Trung Trac
- Asset: trung_trac
- Country Name: Viet Nam
- Base Attack: 5
- Base Defense: 4
- Base Health: 11
- Tags: guardian, shock_assault
- Passive: Rising Banner
- Effect: trung_trac_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Defense +2

Trung Nhi
- Asset: trung_nhi
- Country Name: Viet Nam
- Base Attack: 4
- Base Defense: 5
- Base Health: 11
- Tags: guardian, mobile_warfare
- Passive: Sister's Guard
- Effect: trung_nhi_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Defense +2

Vo Nguyen Giap
- Asset: vo_nguyen_giap
- Country Name: Viet Nam
- Base Attack: 4
- Base Defense: 5
- Base Health: 12
- Tags: land_warfare, strategic_mind
- Passive: People's War
- Effect: vo_nguyen_giap_high_land_defense_plus_2
- Condition: HeroOnSpecificTerrain
- Required Terrain: High-land
- Stat Modifier: Defense +2

Ho Chi Minh
- Asset: ho_chi_minh
- Country Name: Viet Nam
- Base Attack: 3
- Base Defense: 5
- Base Health: 12
- Tags: strategic_mind, guardian
- Passive: National Resolve
- Effect: ho_chi_minh_favored_terrain_defense_plus_2
- Condition: HeroOnFavoredTerrain
- Stat Modifier: Defense +2
```

### Hero deck test đầu tiên

Bộ 15 hero nên dùng cho trận test đầu tiên:

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

Tạm thời chưa chọn:

```text
Ho Chi Minh
```

Lý do:

```text
Ho Chi Minh hợp hơn với effect hỗ trợ đồng minh/toàn bàn, nên nên để sau khi battle runtime và effect system ổn hơn.
```

### Quy ước tactic tạm thời

Tactic giai đoạn đầu chỉ dùng `StatModifierEffectData`.

Với tactic dùng chung:

```text
Is Shared: true
Required Tags: empty
Condition Type: Always
```

Với tactic yêu cầu tag:

```text
Is Shared: false
Required Tags: tag tương ứng
Condition Type: HeroHasTag
Required Tag: tag tương ứng
```

Với các effect buff đồng minh:

```text
Target Type: SelectedAllyHero
Duration Type: UntilEndOfTurn
Duration Turns: 1
Stacking Type: NotStackableKeepStrongest
Max Stacks: 1
```

### Tactic card data đã chốt để nhập

```text
Call To Arms
- Asset: call_to_arms
- Country Name: Viet Nam
- Is Shared: true
- Required Tags: empty
- Effect Description: Choose 1 friendly hero. That hero gains +1 Attack and +1 Defense until end of turn.
- Effects:
  - call_to_arms_attack_plus_1: Attack +1
  - call_to_arms_defense_plus_1: Defense +1

Field Medicine
- Asset: field_medicine
- Country Name: Viet Nam
- Is Shared: true
- Required Tags: empty
- Effect Description: Choose 1 friendly hero. That hero gains +2 Health until end of turn.
- Effects:
  - field_medicine_health_plus_2: Health +2
- Note: đây chưa phải hồi máu thật.

River Stake Ambush
- Asset: river_stake_ambush
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: naval_warfare
- Effect Description: Choose 1 friendly hero with Naval Warfare. That hero gains +2 Attack until end of turn.
- Effects:
  - river_stake_ambush_attack_plus_2: Attack +2

Entrenched Hold
- Asset: entrenched_hold
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: positional_warfare
- Effect Description: Choose 1 friendly hero with Positional Warfare. That hero gains +3 Defense until end of turn.
- Effects:
  - entrenched_hold_defense_plus_3: Defense +3

Terrain Offensive
- Asset: terrain_offensive
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: land_warfare
- Effect Description: Choose 1 friendly hero with Land Warfare. That hero gains +2 Attack and +1 Defense until end of turn.
- Effects:
  - terrain_offensive_attack_plus_2: Attack +2
  - terrain_offensive_defense_plus_1: Defense +1

Thunder Charge
- Asset: thunder_charge
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: shock_assault
- Effect Description: Choose 1 friendly hero with Shock Assault. That hero gains +3 Attack until end of turn.
- Effects:
  - thunder_charge_attack_plus_3: Attack +3

Iron Wall Formation
- Asset: iron_wall_formation
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: guardian
- Effect Description: Choose 1 friendly hero with Guardian. That hero gains +2 Defense and +2 Health until end of turn.
- Effects:
  - iron_wall_formation_defense_plus_2: Defense +2
  - iron_wall_formation_health_plus_2: Health +2

Master Campaign Plan
- Asset: master_campaign_plan
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: strategic_mind
- Effect Description: Choose 1 friendly hero with Strategic Mind. That hero gains +1 Attack and +1 Defense until end of turn.
- Effects:
  - master_campaign_plan_attack_plus_1: Attack +1
  - master_campaign_plan_defense_plus_1: Defense +1

Rapid Redeployment
- Asset: rapid_redeployment
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: mobile_warfare
- Effect Description: Choose 1 friendly hero with Mobile Warfare. That hero gains +1 Attack and +1 Defense until end of turn.
- Effects:
  - rapid_redeployment_attack_plus_1: Attack +1
  - rapid_redeployment_defense_plus_1: Defense +1

Overwhelming Force
- Asset: overwhelming_force
- Country Name: Viet Nam
- Is Shared: false
- Required Tags: brute_force
- Effect Description: Choose 1 friendly hero with Brute Force. That hero gains +3 Attack until end of turn.
- Effects:
  - overwhelming_force_attack_plus_3: Attack +3
```

### Tactic deck test đầu tiên

Bộ 9 tactic nên dùng cho trận test đầu tiên:

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

Tạm thời chưa chọn:

```text
Field Medicine
```

Lý do:

```text
Field Medicine hiện mới là Health +2 tạm thời, chưa phải hồi máu thật. Để tránh nhầm khi test battle đầu tiên, nên thêm lại sau.
```

### Việc làm tiếp ngay sau mốc này

```text
1. Mở Unity và nhập/sửa hero data theo bảng trên.
2. Tạo đủ effect asset trong Assets/game_data/effects.
3. Gán passiveEffects cho từng hero.
4. Gán tacticEffects cho từng tactic.
5. Kiểm tra lại requiredTags của tactic.
6. Test từ scene menu.
7. Kiểm tra deck_setup chọn đúng 15 hero + 9 tactic.
8. Kiểm tra tactic lock/unlock đúng theo selected hero tags.
9. Kiểm tra opponent_deck_preview hiển thị effect đúng.
10. Sau khi data ổn mới làm BattleInitializer.
```

### Lưu ý không được quên

```text
1. Không tạo mỗi effect thành một file C# riêng.
2. Không sửa trực tiếp baseAttack/baseDefense/baseHealth trong runtime.
3. Không quay lại dùng attackBonus/defenseBonus/healthBonus trong TacticCardData.
4. Không ghi rằng toàn bộ data asset đã nhập xong nếu thực tế mới chỉ chốt thông tin trong chat.
5. Nếu đã nhập xong trong Unity, cần commit cả Assets/game_data/heroes, Assets/game_data/tactics và Assets/game_data/effects.
```

---

## 0.2. Mốc trước — 2026-05-24

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

## 0.3. Mốc trước — 2026-05-22

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

### 9.7. BattleUIController, BattleBoardSlotUI, BattleHandCardUI và BattleTacticSlotUI

Files:

```text
Assets/scripts/ui/BattleUIController.cs
Assets/scripts/ui/BattleBoardSlotUI.cs
Assets/scripts/ui/BattleHandCardUI.cs
Assets/scripts/ui/BattleTacticSlotUI.cs
```

Trạng thái cập nhật 2026-06-14:

- `BattleUIController` hiện nhận `BattleState` qua hàm:

```csharp
public void ShowBattleState(BattleState battleState)
```

- UI battle hiện hiển thị được:

```text
player HP
enemy HP
turn number + current side
player deck count
player hand count
player graveyard count
enemy deck count
enemy hand count
enemy graveyard count
player board slots
enemy board slots
player hand cards
player tactic slots
```

- `BattleBoardSlotUI` dùng để hiển thị từng ô board và nhận click deploy hero.
- Mỗi board slot UI cần kéo đủ 5 TMP Text:

```text
slotIndexText
terrainText
heroNameText
heroStatsText
heroHealthText
```

- `BattleHandCardUI` dùng để hiển thị card trên tay player.
- Mỗi hand card UI cần kéo:

```text
cardNameText
cardTypeText
cardStatsText
cardDescriptionText
```

- `BattleTacticSlotUI` dùng để hiển thị 3 ô tactic hàng sau của player.
- Mỗi tactic slot UI cần kéo:

```text
slotIndexText
tacticNameText
tacticStateText
```

- Scene `battle` cần có các array trong `BattleUIController`:

```text
playerBoardSlotUis: size 7
enemyBoardSlotUis: size 7
playerTacticSlotUis: size 3
enemyTacticSlotUis: có thể để 0 tạm thời
```

- `BattleUIController` hiện có logic:

```text
render player hand
click chọn card trên tay
deploy hero xuống player board slot trống
đặt úp tactic vào player tactic slot trống
end turn tạm thời
back về enemy_setup
```

Lưu ý:

```text
Player Hand Content phải kéo đúng object Content trong Scroll View, không kéo Viewport hoặc Scroll View.
Board slot và tactic slot cần có Button/Image để nhận click.
Nếu terrain hiện tên asset thay vì tên đẹp, kiểm tra BattleBoardSlotUI đang dùng terrainData.terrainName.
```


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
battle_hand_card
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

### battle_hand_card

Dùng trong `battle` để hiển thị card trên tay player.

Script:

```text
BattleHandCardUI.cs
```

Yêu cầu:

```text
Object gốc có Button hoặc có Button trong child.
Có các TMP Text:
- CardNameText
- CardTypeText
- CardStatsText
- CardDescriptionText
Nên có Layout Element để set Preferred Width/Height.
Prefab này được instantiate vào PlayerHandContent trong Scroll View ngang.
```

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
    - EffectConditionType
    - EffectInstance
    - EffectResolver
    - StatCalculator
    - StatModifierEffectData
14. Chuyển hướng AI deck scorer sang đọc EffectData.
15. Bổ sung condition cho effect để passive không mặc định permanent vô điều kiện.
16. Người dùng đã xác nhận đã fix xong các card và effect tạm thời.
17. Bắt đầu battle runtime foundation.
18. Đã có BattleCardType, BattleCardInstance, BattleHeroInstance, BattleBoardSlot, BattleTacticSlot, BattlePlayerState, BattleState.
19. Đã có BattleInitializer đọc GameSession, tạo state đầu trận, shuffle deck, draw 5 lá và random người đi trước.
20. Đã mở rộng BattleUIController để hiển thị HP, turn, deck count, hand count, graveyard count và board slots.
21. Đã có BattleBoardSlotUI để hiển thị terrain/hero placeholder cho từng slot.
22. Scene battle đã có 7 player board slots và 7 enemy board slots.
23. Đã có BattleHandCardUI để hiển thị card trên tay player.
24. Player hand trong battle đã chuyển sang Scroll View ngang để không tràn card khỏi khung.
25. Đã có prefab battle_hand_card.
26. Đã click chọn card trên tay.
27. Đã deploy hero từ hand xuống player board slot trống.
28. Sau khi deploy hero, card bị remove khỏi hand và UI refresh.
29. Đã giới hạn mỗi lượt chỉ dùng 1 hero action.
30. Đã có BattleTacticSlotUI.
31. Đã có 3 player tactic slots trong battle.
32. Đã click tactic card trên tay và đặt úp vào tactic slot trống.
33. Sau khi đặt tactic, card bị remove khỏi hand và UI refresh.
34. Đã giới hạn mỗi lượt chỉ dùng 1 tactic action.
35. Đã có End Turn tạm để switch turn, reset action flags và draw card.
36. Nút back trong battle đã đổi sang quay về enemy_setup.
```

---

## 12. Việc đang dở

Các phần battle còn đang dở sau mốc 2026-06-14:

```text
1. Chưa kích hoạt tactic đã đặt úp.
2. Chưa kiểm tra luật tactic vừa đặt không được kích hoạt ngay trong lượt đó.
3. Chưa chọn mục tiêu cho tactic.
4. Chưa apply tacticEffects vào BattleHeroInstance.activeEffects.
5. Chưa tick/remove effect theo duration ở cuối lượt.
6. Chưa nối hero passiveEffects vào runtime theo condition.
7. Chưa nối terrain bonus/effect vào tính chỉ số thật trong battle.
8. Chưa có phase system đầy đủ:
   - Draw
   - Deploy/Move
   - Tactic
   - Combat
   - End Turn
9. Chưa có TurnManager riêng.
10. Chưa có CombatResolver.
11. Chưa có logic hero tấn công ô đối diện.
12. Chưa có logic damage trực tiếp vào player HP.
13. Chưa chuyển hero chết vào graveyard.
14. Chưa có enemy AI đánh bài trong battle.
15. Chưa có win/lose/draw resolver khi HP về 0.
```

Các phần đã không còn coi là chưa làm:

```text
BattleCardInstance: đã có bản foundation
BattleHeroInstance: đã có bản foundation
BattlePlayerState: đã có bản foundation
BattleBoardSlot: đã có bản foundation
BattleTacticSlot: đã có bản foundation
BattleState: đã có bản foundation
BattleInitializer: đã có bản foundation
BattleUIController: đã render battle state, board, hand và tactic slots
BattleHandCardUI: đã có
BattleTacticSlotUI: đã có
Deploy hero từ hand xuống board: đã có bản đầu
Đặt úp tactic từ hand vào tactic slot: đã có bản đầu
Player hand scroll ngang: đã có
```

---

## 13. Mốc làm tiếp được khuyến nghị

Mốc tiếp theo nên làm theo thứ tự:

```text
Bước 1: Kích hoạt tactic đã đặt úp
- click tactic slot đã có card
- chỉ cho kích hoạt nếu tactic không phải vừa đặt trong lượt hiện tại
- lật tactic từ face down sang active/ready
- tạm thời chỉ xử lý tactic của player

Bước 2: Chọn target cho tactic
- nếu targetType = SelectedAllyHero, player click 1 hero đồng minh
- nếu targetType = SelectedEnemyHero, player click 1 hero địch
- nếu targetType = SelfHero thì áp dụng lên hero sở hữu nếu có ngữ cảnh phù hợp
- nếu targetType là AllAllyHeroes/AllEnemyHeroes thì apply cho toàn bộ nhóm

Bước 3: Apply tacticEffects
- dùng EffectResolver.AddEffect(...)
- thêm EffectInstance vào BattleHeroInstance.activeEffects
- refresh BattleBoardSlotUI để thấy ATK/DEF/HP hiện tại thay đổi

Bước 4: Tick effect cuối lượt
- UntilEndOfTurn hết hạn khi end turn
- duration nhiều lượt giảm remainingTurns
- remove effect hết hạn
- refresh UI sau khi tick

Bước 5: Làm combat cơ bản
- mỗi hero sống được đánh 1 lần
- hero mới deploy/move trong lượt này chưa được đánh
- hero chỉ đánh ô đối diện
- nếu có enemy hero thì tính damage lên hero
- nếu ô đối diện trống thì damage trực tiếp vào enemy HP
- hero chết thì chuyển vào graveyard

Bước 6: Sau khi combat chạy ổn
- nối hero passiveEffects theo condition
- nối terrain bonus/effect
- làm AI battle planner/scorer
- làm win/lose/draw resolver
```

Mốc nhỏ nhất nên làm ngay sau file này:

```text
Kích hoạt tactic đã đặt úp và apply StatModifierEffectData lên hero được chọn.
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
Tôi đang làm Hero Card Game bằng Unity 6000.3.11f1. Project hiện có flow scene: menu -> enemy_setup -> player_setup -> deck_setup -> opponent_deck_preview -> terrain_setup -> battle. Đã có GameSession singleton giữ dữ liệu xuyên scene. Đã có data ScriptableObject cho CountryData, HeroCardData, TacticCardData, TerrainData, TagData. Country Viet Nam có 16 hero, 10 tactic, 7 terrain. Đã tách AI thành module trong Assets/scripts/ai gồm core, profiles, deck, terrain, battle. AI hiện đã random play style, chọn enemy deck 15 hero + 9 tactic, và tự swap terrain theo hướng counter trong terrain_setup. Đã có scene opponent_deck_preview hiển thị deck địch 60 giây. Đã có effect module trong Assets/scripts/effects gồm EffectData, EffectConditionType, EffectInstance, EffectResolver, StatCalculator, StatModifierEffectData và enum liên quan. Hero passive dùng passiveEffects, tactic card dùng tacticEffects, AIDeckScorer đọc EffectData thay vì attackBonus/defenseBonus/healthBonus cũ. Người dùng đã xác nhận đã fix xong card/effect tạm thời. Battle runtime foundation đã có BattleCardType, BattleCardInstance, BattleHeroInstance, BattleBoardSlot, BattleTacticSlot, BattlePlayerState, BattleState, BattleInitializer; BattleInitializer đọc GameSession, tạo deck runtime cho player/enemy, shuffle, draw 5 lá, random người đi trước và gửi BattleState cho BattleUIController. BattleUIController đã hiển thị HP, turn, deck count, hand count, graveyard count và 7 board slots mỗi bên. Đã thêm BattleBoardSlotUI để hiển thị terrain, hero name/Empty, ATK/DEF, HP. Mốc mới nhất là đã có BattleHandCardUI, prefab battle_hand_card, player hand scroll ngang, click chọn card trên tay, deploy hero từ hand xuống player board slot trống, BattleTacticSlotUI, 3 player tactic slots, click tactic card rồi đặt úp vào tactic slot trống, giới hạn mỗi lượt 1 hero action và 1 tactic action, End Turn tạm để switch turn/draw card/reset action flags, và nút back trong battle quay về enemy_setup. Việc tiếp theo nên làm là kích hoạt tactic đã đặt úp, chọn target cho tactic, dùng EffectResolver apply tacticEffects lên BattleHeroInstance.activeEffects, rồi sau đó làm combat cơ bản.
```
