# Changelog

## 0.2.4.1

### Added

* Added custom Neuro firing profiles for more magic weapons:

  * Last Prism
  * Charged Blaster Cannon
  * Betsy's Wrath
  * Spirit Flame
  * Medusa Head
  * Life Drain


## 0.2.4

### Added

* Added custom Neuro firing profiles for more magic weapons:
  * Bubble Gun
  * Bee Gun
  * Wasp Gun
  * Bat Scepter
  * Poison Staff
  * Venom Staff
  * Meteor Staff
  * Blizzard Staff
  * Lunar Flare
  * Sky Fracture
  * Nightglow
  * Shadowflame Hex Doll
  * Blood Thorn
  * Stellar Tune

### Changed

* Improved how Neuro handles magic weapons with unusual projectile behavior.
* Sky-based weapons now spawn their attacks from above instead of firing like simple straight projectiles.
* Blood Thorn now creates visible thorns from nearby blocks or platforms, with a fallback shot from Neuro when no valid tile is available.
* Stellar Tune now fires from Neuro and follows a single wide S-shaped path toward the target.
* Nightglow now uses upward bloom-style shots with related volley colors.
* Shadowflame Hex Doll now fires bending tentacles instead of straight shots.
* Custom weapon profiles now better respect the equipped weapon's own use timing and Neuro's staff tier.

### Fixed

* Fixed several magic weapons only producing particles, invisible projectiles, or misplaced attacks when used by Neuro.
* Fixed Sky Fracture using generic spread behavior instead of its own custom sword pattern.
* Fixed Blood Thorn failing to show visible thorns when spawned by Neuro.
* Fixed Stellar Tune projectiles starting from the player instead of Neuro.

## 0.2.3.1

### Changed

* Added a unique Neuro companion sprite for each staff tier.
* Added matching companion buff icons and names for each staff tier:

  * Neuro Companion
  * Hellfire Neuro
  * Hallowed Neuro
  * AI Lord
* Updated the mod icon.


## 0.2.3

### Added

* Added custom Neuro support for Laser Machinegun.
* Laser Machinegun now uses a custom firing profile instead of relying on normal player-held channeling behavior.

### Changed

* Generic channeling magic weapons are now rejected unless they have explicit custom Neuro support.
* Buff and debuff commands now require Neuro to be summoned.
* Buff and debuff Randy/Neuro actions now also require Neuro to be summoned.
* Updated documentation to better explain supported weapon profiles, rejected channeling weapons, and summon requirements.

### Fixed

* Fixed buff and debuff actions being usable when the companion was not summoned.


## 0.2.2

### Added

* Added Evil Neuro.
* Added `/neuro attack player`.
* Added an Evil Neuro sprite that appears when Neuro does something harmful to the player.
* Added player attack behavior for Evil Neuro.
* Evil Neuro can attack the player with Neuro's equipped magic weapon.
* If Neuro has no valid magic weapon equipped, Evil Neuro fires a fallback evil bolt.
* Added fallback evil bolt damage scaling based on staff tier and world difficulty:
  * Mk1: 100 base damage
  * Mk2: 150 base damage
  * Mk3: 200 base damage
  * Mk4: 400 base damage
  * Classic: 1.00x
  * Expert: 1.33x
  * Master: 1.66x
  * Legendary: 1.99x
* Added a separate config option for Evil Neuro's player attack cooldown.

## 0.2.1

### Added

* Added unique sprites for Neuro Companion Staff Mk2, Mk3, and Mk4.
* Added support for targeted-area magic weapons such as Resonance Scepter.
* Added endgame Mk4 behavior: Neuro can detect enemies through solid blocks, and Neuro-fired projectiles can pass through solid blocks.

### Changed

* Rebalanced Neuro Companion Staff upgrade tiers:

  * Mk1: 50-tick staff fire limit.
  * Mk2: 30-tick staff fire limit.
  * Mk3: 1-tick staff fire limit, still limited by the equipped weapon's own use time.
  * Mk4: 1-tick staff fire limit, still limited by the equipped weapon's own use time, plus through-block utility.
* Neuro can no longer fire weapons faster than the equipped weapon’s own use time allows.
* Updated staff behavior so higher tiers improve consistency and utility instead of simply doubling fire speed every tier.

### Fixed

* Fixed Resonance Scepter firing as a traveling projectile instead of placing its attack at the target position.
* Fixed targeted-area weapon inspection/status text displaying the weapon kind as invalid.

## 0.2.0

### Added

* Added specific positive buff commands:

  * `/neuro buff`
  * `/neuro buff <buff name or buff ID>`
* Added specific debuff commands:

  * `/neuro debuff player`
  * `/neuro debuff player <debuff name or debuff ID>`
  * `/neuro debuff enemy`
  * `/neuro debuff enemy <debuff name or debuff ID>`
* Added Randy/Neuro support for choosing specific buffs and debuffs.
* Added allowed buff/debuff lists to Randy context.
* Added Neuro weapon damage display to the Neuro Companion Staff tooltip.
* Added Neuro weapon crit chance display to the Neuro Companion Staff tooltip.
* Added Neuro fire interval display to the Neuro Companion Staff tooltip.
* Added staff prefix bonuses to Neuro’s weapon damage, knockback, crit chance, and firing interval where applicable.
* Added upgraded staff tiers:

  * Neuro Companion Staff Mk2
  * Neuro Companion Staff Mk3
  * Neuro Companion Staff Mk4
* Added configurable maximum autoattack duration in the mod config menu, up to 10 minutes.

### Changed

* `/neuro buff` now prioritizes positive buffs the player does not already have.
* Autoattack duration is now clamped by the configured maximum duration instead of a fixed hardcoded limit.
* Neuro Companion Staff now acts more like Neuro’s controller item, showing the damage of Neuro’s equipped weapon instead of a misleading summon damage value.
* Neuro Companion Staff reforges are limited to universal combat prefixes instead of class-specific prefixes.

### Fixed

* Fixed repeated/random buff behavior where Neuro could waste buff rolls on effects the player already had.
* Fixed Randy action descriptions/context to better explain available buff and debuff choices.