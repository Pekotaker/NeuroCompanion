# Hướng Dẫn: Thêm Hỗ Trợ Ranged Weapons cho Neuro Companion

## Tổng Quan

Hiện tại, mod NeuroCompanion chỉ hỗ trợ **Magic weapons**. Để thêm hỗ trợ cho **Ranged weapons** (cung, súng, v.v.), bạn cần sửa đổi:

1. **NeuroWeaponClassifier.cs** - Thêm logic phân loại cho ranged weapons
2. **Các weapon profiles** - Tạo profiles cho các ranged weapons cụ thể
3. **Registry** - Đăng ký ranged profiles vào hệ thống

---

## Bước 1: Sửa NeuroWeaponClassifier.cs

### Vị trí: `Neuro/Weapons/NeuroWeaponClassifier.cs`

Hiện tại, code kiểm tra:
```csharp
if (item.DamageType != DamageClass.Magic)
{
	return Invalid($"{item.Name} is not a magic weapon.");
}
```

**Sửa thành:**

```csharp
// Chấp nhận cả Magic và Ranged
if (item.DamageType != DamageClass.Magic && 
	item.DamageType != DamageClass.Ranged)
{
	return Invalid($"{item.Name} is not a magic or ranged weapon.");
}

// Loại bỏ kiểm tra mana (chỉ magic weapons cần mana)
if (item.DamageType == DamageClass.Magic && item.mana <= 0)
{
	return Invalid($"{item.Name} does not use mana.");
}
```

---

## Bước 2: Tạo Ranged Weapon Profiles

Ranged weapons không yêu cầu profiles đặc biệt như magic weapons, nhưng bạn có thể tạo nếu muốn điều chỉnh hành vi.

### Tạo một Ranged Profile cơ bản

**File: `Neuro/Weapons/Firing/WeaponProfiles/BowProfile.cs`**

```csharp
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
	public static class BowProfile
	{
		public static bool IsWeapon(Item weapon)
		{
			// Áp dụng cho các cung/bow
			return weapon != null &&
				   !weapon.IsAir &&
				   weapon.type == ItemID.WoodenBow;  // Hoặc các bow khác
		}

		public static NeuroWeaponShot[] CreateShots(
			Item weapon,
			Vector2 basePosition,
			Vector2 baseVelocity
		)
		{
			// Ranged weapons thường chỉ phát 1 phát mũi tên
			return new NeuroWeaponShot[]
			{
				new NeuroWeaponShot(
					weapon.shoot,
					basePosition,
					baseVelocity
				)
			};
		}
	}
}
```

### Tạo Gun Profile (Ranged súng)

**File: `Neuro/Weapons/Firing/WeaponProfiles/GunProfile.cs`**

```csharp
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
	public static class GunProfile
	{
		public static bool IsWeapon(Item weapon)
		{
			// Áp dụng cho các súng
			return weapon != null &&
				   !weapon.IsAir &&
				   (weapon.type == ItemID.Musket ||
					weapon.type == ItemID.Minishark ||
					weapon.type == ItemID.Revolver);
		}

		public static NeuroWeaponShot[] CreateShots(
			Item weapon,
			Vector2 basePosition,
			Vector2 baseVelocity
		)
		{
			// Súng thường phát 1 viên đạn
			return new NeuroWeaponShot[]
			{
				new NeuroWeaponShot(
					weapon.shoot,
					basePosition,
					baseVelocity
				)
			};
		}
	}
}
```

---

## Bước 3: Thay đổi NeuroWeaponFiringProfileRegistry.cs

### Thêm hỗ trợ cho Ranged weapons mặc định

**File: `Neuro/Weapons/Firing/WeaponProfiles/NeuroWeaponFiringProfileRegistry.cs`**

Trong method `CreateShots()`, thêm logic xử lý ranged:

```csharp
public static NeuroWeaponShot[] CreateShots(
	Player owner,
	Item weapon,
	Vector2 basePosition,
	Vector2 baseVelocity,
	Vector2 targetPosition
)
{
	// ... các magic weapon profiles ...

	// Thêm ranged weapon profiles
	if (BowProfile.IsWeapon(weapon))
	{
		return BowProfile.CreateShots(
			weapon,
			basePosition,
			baseVelocity
		);
	}

	if (GunProfile.IsWeapon(weapon))
	{
		return GunProfile.CreateShots(
			weapon,
			basePosition,
			baseVelocity
		);
	}

	// Default ranged weapon handling
	if (weapon.DamageType == DamageClass.Ranged)
	{
		return CreateDefaultRangedShots(weapon, basePosition, baseVelocity);
	}

	// ... trở lại magic weapon defaults ...
}

// Thêm method cho default ranged handling
private static NeuroWeaponShot[] CreateDefaultRangedShots(
	Item weapon,
	Vector2 basePosition,
	Vector2 baseVelocity
)
{
	return new NeuroWeaponShot[]
	{
		new NeuroWeaponShot(
			weapon.shoot,
			basePosition,
			baseVelocity
		)
	};
}
```

---

## Bước 4: Sửa NeuroDamageService.cs

Hiện tại chỉ tính damage cho Magic weapons, cần thêm Ranged:

**File: `Neuro/Weapons/NeuroDamageService.cs`**

```csharp
public static int GetNeuroWeaponDamage(
	Player player,
	Item weapon,
	int staffDamageBonus
)
{
	// Thay đổi từ:
	// float bonusMultiplier = player.GetTotalDamage(DamageClass.Magic).Additive;

	// Thành:
	DamageClass damageClass = weapon.DamageType;
	float bonusMultiplier = player.GetTotalDamage(damageClass).Additive;

	// ... phần còn lại giữ nguyên ...
}

public static int GetNeuroWeaponCritChance(
	Player player,
	Item weapon
)
{
	// Tương tự, sửa:
	DamageClass damageClass = weapon.DamageType;
	int critChance = player.GetTotalCritChance(damageClass);

	// ... phần còn lại giữ nguyên ...
}
```

---

## Bước 5: Kiểm Tra Ammo (Tùy chọn)

Nếu muốn kiểm tra ammo cho bow/gun:

```csharp
public static bool HasValidAmmo(Player player, Item weapon)
{
	if (weapon.DamageType != DamageClass.Ranged)
		return true;  // Magic weapons không cần ammo check

	// Kiểm tra xem player có ammo không
	int ammoType = weapon.useAmmo;

	foreach (Item item in player.inventory)
	{
		if (item.ammo == ammoType && !item.IsAir)
			return true;
	}

	return false;
}
```

---

## Tóm Tắt Các Thay Đổi Cần Làm

| File | Thay Đổi |
|------|---------|
| `NeuroWeaponClassifier.cs` | Xóa check `DamageType == Magic`, thêm hỗ trợ Ranged |
| `NeuroWeaponFiringProfileRegistry.cs` | Thêm ranged profiles vào `CreateShots()` |
| `NeuroDamageService.cs` | Thay `DamageClass.Magic` thành `weapon.DamageType` |
| (Tạo mới) `BowProfile.cs` | Default bow handling |
| (Tạo mới) `GunProfile.cs` | Default gun handling |

---

## Các Weapon Types Có Thể Thêm

### Ranged Weapons:
- **Bows**: Wooden Bow, Cursed Bow, Demon Bow, Molten Fury, v.v.
- **Guns**: Musket, Revolver, Minishark, Megashark, v.v.
- **Launchers**: Grenade Launcher, Rocket Launcher, v.v.
- **Flamethrowers**: Flamethrower, v.v.

---

## Lưu Ý Quan Trọng

1. **Ammo không tự động nhặt**: Bạn cần quản lý ammo trong companion attack logic
2. **Speed**: Ranged weapons thường nhanh hơn magic, điều chỉnh cooldown nếu cần
3. **Knockback**: Có thể khác với magic weapons, kiểm tra trong `NeuroDamageService.cs`

---

## Ví Dụ Đầy Đủ: Thêm Hỗ Trợ Cho Bow

**Bước 1**: Sửa `NeuroWeaponClassifier.cs` (dòng ~25)
```csharp
// Cũ:
if (item.DamageType != DamageClass.Magic)
	return Invalid($"{item.Name} is not a magic weapon.");

if (item.mana <= 0)
	return Invalid($"{item.Name} does not use mana.");

// Mới:
bool isMagic = item.DamageType == DamageClass.Magic;
bool isRanged = item.DamageType == DamageClass.Ranged;

if (!isMagic && !isRanged)
	return Invalid($"{item.Name} is not a magic or ranged weapon.");

if (isMagic && item.mana <= 0)
	return Invalid($"{item.Name} does not use mana.");
```

**Bước 2**: Kiểm tra Ranged projectile
```csharp
if (item.shootSpeed <= MinimumUsefulShootSpeed && isMagic)
{
	return Invalid("...");
}
```

---

Sau khi thực hiện các thay đổi này, Neuro sẽ có thể sử dụng các ranged weapons như bows, guns, v.v. 🏹🔫
