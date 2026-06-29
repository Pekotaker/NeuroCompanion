# Changelog

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