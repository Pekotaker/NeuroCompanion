# Neuro Companion

Neuro Companion is a tModLoader mod for Terraria that adds a summon companion with command-based behavior.

The companion works as a normal Terraria summon, but it can also be controlled through in-game commands. It can follow the player, recall to the player, attack enemies, use an equipped magic weapon, apply buffs/debuffs, and switch between follow and autoattack modes.

The mod can be used in two ways:

* **Standalone command mode**: control the companion directly with Terraria chat commands
* **Optional Neuro/Randy mode**: connect the mod to Randy from the VedalAI Neuro SDK so Neuro can choose high-level actions through websocket

Neuro/Randy is optional. The mod works without it.

## Table of Contents

* [Features](#features)
* [Setup](#setup)

  * [Standalone Setup](#standalone-setup)
  * [Optional Randy Setup](#optional-randy-setup)
* [Commands](#commands)

  * [Companion Commands](#companion-commands)
  * [Weapon Commands](#weapon-commands)
  * [Websocket Commands](#websocket-commands)
  * [Debug Command](#debug-command)
* [Staff Upgrades](#staff-upgrades)
* [Weapon System](#weapon-system)
* [Mod Config](#mod-config)
* [Optional Neuro/Randy Integration](#optional-neurorandy-integration)
* [Troubleshooting](#troubleshooting)
* [Latest Update](#latest-update)
* [Repository](#repository)

## Features

* Adds the **Neuro Companion Staff**
* Summons a Neuro companion minion
* Companion follows the player by default
* Companion can recall near the player
* Companion can attack once on command
* Companion can autoattack for a limited time
* Maximum autoattack duration is configurable, up to 10 minutes
* Companion can use an equipped magic weapon
* Adds a Neuro weapon equipment slot in the inventory UI
* Player can manually assign a weapon to Neuro
* Neuro can take a valid weapon from the player inventory
* Neuro Companion Staff tooltip shows Neuro’s current weapon damage, crit chance, and fire interval
* Neuro Companion Staff prefixes apply to Neuro’s weapon attacks
* Adds upgraded staff tiers: Mk2, Mk3, and Mk4
* Supports direct-fire magic weapons
* Supports controllable magic weapons by treating them like normal attacks
* Supports some simple channeling magic weapons
* Rejects unsupported support/stationary/persistent-field magic weapons
* Rejects held beam and handheld channeling weapons that need custom player-held behavior
* Can apply positive buffs to the player
* Can apply specific allowed buffs by name or buff ID
* Can apply debuffs to the player or a nearby enemy
* Can apply specific allowed debuffs by name or buff ID
* Can optionally connect to Randy through websocket
* Can optionally send Terraria context to Randy/Neuro

## Setup

### Standalone Setup

You only need:

* Terraria
* tModLoader

In tModLoader:

1. Open **Workshop**
2. Open **Develop Mods**
3. Build the mod
4. Enable **Neuro Companion**
5. Reload mods
6. Enter a world
7. Obtain or craft the Neuro Companion Staff

After that, you can control the companion with `/neuro` commands.

Randy is not required for standalone use.

### Optional Randy Setup

You only need this if you want to test Neuro/Randy control.

Start Randy locally.

By default, the mod expects Randy to be running at:

```text
ws://localhost:8000
```

This can be changed in the mod config.

Then, in Terraria chat, run:

```text
/neurows connect
```

To check the connection:

```text
/neurows status
```

## Commands

### Companion Commands

```text
/neuro help
```

Shows the Neuro command list.

```text
/neuro status
```

Shows companion status, current mode, autoattack timer, and active action cooldowns.

```text
/neuro recall
```

Teleports the companion back near the player.

```text
/neuro follow
```

Stops autoattack mode and makes the companion follow the player.

```text
/neuro attack
```

Makes the companion fire one attack with Neuro’s equipped magic weapon.

If a valid enemy is nearby, the attack targets that enemy.

If no valid enemy is nearby, the attack fires toward the player’s cursor.

```text
/neuro autoattack
```

Starts timed autoattack mode for the default duration.

```text
/neuro autoattack 30
```

Starts timed autoattack mode for 30 seconds.

The requested duration is clamped by the maximum autoattack duration in the mod config.

```text
/neuro buff
```

Applies positive Red Potion-style buffs to the player, prioritizing buffs the player does not already have.

```text
/neuro buff ironskin
/neuro buff obsidian skin
/neuro buff 5
```

Applies a specific allowed positive buff by name or buff ID.

```text
/neuro debuff player
```

Applies Red Potion-style debuffs to the player.

```text
/neuro debuff player poisoned
/neuro debuff player 20
```

Applies a specific allowed debuff to the player by name or buff ID.

```text
/neuro debuff enemy
```

Applies Red Potion-style debuffs to the nearest valid enemy.

```text
/neuro debuff enemy poisoned
/neuro debuff enemy 20
```

Applies a specific allowed debuff to the nearest valid enemy by name or buff ID.

### Weapon Commands

```text
/neuro weapon status
```

Shows Neuro’s currently equipped weapon.

```text
/neuro weapon inspect
```

Inspects the player’s currently selected item and shows whether Neuro can use it.

```text
/neuro weapon set
```

Moves the player’s currently selected hotbar item into Neuro’s weapon slot if it is valid.

If Neuro already has a weapon, the old weapon is automatically swapped back.

```text
/neuro weapon take
```

Lets Neuro take the strongest valid magic weapon from the player’s inventory.

The selected hotbar item is ignored so Neuro does not take what the player is currently holding.

If Neuro already has a weapon, the old weapon is automatically swapped back.

```text
/neuro weapon return
```

Returns Neuro’s equipped weapon to the player inventory.

### Websocket Commands

These commands are only needed for Randy/Neuro testing.

```text
/neurows connect
```

Connects the mod to Randy.

```text
/neurows status
```

Shows websocket connection status.

```text
/neurows context
```

Manually sends current Terraria context to Randy.

```text
/neurows disconnect
```

Disconnects from Randy.

```text
/neurows help
```

Shows websocket command help.

### Debug Command

```text
/neurodebug
```

Shows detailed debug information, including connection status, summoned status, companion mode, cooldowns, last received action, queued command, executed command, last action result, and last context sent.

## Staff Upgrades

The Neuro Companion Staff controls Neuro’s firing interval.

The staff tooltip shows:

* Neuro weapon damage
* Neuro weapon crit chance
* Neuro fire interval

Neuro’s displayed damage is based on:

* Neuro’s equipped magic weapon
* the weapon’s own modifier/reforge
* player magic damage bonuses from armor, accessories, and buffs
* the Neuro Companion Staff’s universal prefix

The staff can be reforged, but it is limited to universal combat prefixes instead of class-specific prefixes.

Staff tiers:

```text
Neuro Companion Staff      -> normal fire interval
Neuro Companion Staff Mk2  -> faster fire interval
Neuro Companion Staff Mk3  -> faster fire interval
Neuro Companion Staff Mk4  -> fastest fire interval
```

Recipes:

```text
Neuro Companion Staff:
10 Wood at a Work Bench

Neuro Companion Staff Mk2:
Neuro Companion Staff + 10 Hellstone Bars at an Anvil

Neuro Companion Staff Mk3:
Neuro Companion Staff Mk2 + 10 Hallowed Bars at a Mythril/Orichalcum Anvil

Neuro Companion Staff Mk4:
Neuro Companion Staff Mk3 + 10 Luminite Bars at an Ancient Manipulator
```

After changing or reforging the staff, resummon Neuro so the companion uses the new staff tier and prefix.

## Weapon System

Neuro has her own magic weapon slot.

The slot appears in the inventory UI during normal inventory use. It is hidden when shops, chests, reforging, or guide crafting are open.

You can equip a weapon by dragging it into the slot or by using commands.

Weapons can be assigned by:

* Dragging a valid weapon into the Neuro weapon slot
* Using `/neuro weapon set`
* Using `/neuro weapon take`
* Letting Randy/Neuro choose the equivalent weapon action when connected

Important weapon rules:

* Moving a weapon into Neuro’s slot moves the item, not copies it
* If Neuro already has a weapon, the old weapon is swapped back safely
* `/neuro weapon take` ignores the player’s currently selected hotbar item
* Neuro does not steal the item currently held by the mouse cursor

Neuro currently accepts:

* Direct-fire magic weapons
* Controllable magic weapons, treated like normal attacks
* Some simple channeling magic weapons

Neuro rejects:

* Non-magic weapons
* Items with no damage
* Items with no mana cost
* Items that do not shoot projectiles
* Support/stationary/persistent-field magic weapons
* Player-held channeling weapons that require special player-held behavior
* Held beam weapons

Examples:

```text
Wand of Sparking     -> accepted
Water Bolt           -> accepted
Demon Scythe         -> accepted
Magic Missile        -> accepted
Flamelash            -> accepted
Rainbow Rod          -> accepted
Laser Machinegun     -> rejected
Nimbus Rod           -> rejected
Clinger Staff        -> rejected
Magnet Sphere        -> rejected
Last Prism           -> rejected
```

Last Prism, Laser Machinegun, and other player-held beam/channeling weapons are not supported yet because they need custom behavior attached to Neuro’s companion body instead of the player.

## Mod Config

The mod has configuration options in the tModLoader mod config menu.

Current configurable options include:

* Randy websocket URL
* Auto-connect on world load
* Context sending interval
* Event context messages
* Low health context threshold
* Action cooldowns
* Maximum autoattack duration

The maximum autoattack duration can be set up to:

```text
600 seconds
10 minutes
```

This limit applies to both manual `/neuro autoattack <seconds>` commands and Randy/Neuro autoattack actions.

## Optional Neuro/Randy Integration

The mod can optionally connect to Randy from the VedalAI Neuro SDK.

```text
https://github.com/VedalAI/neuro-sdk
```

When connected, the mod can send Terraria context to Randy/Neuro, including:

* Player health
* Companion status
* Companion mode
* Equipped weapon
* Nearby enemies
* Boss status
* Cooldowns
* Maximum autoattack duration
* Allowed buff choices
* Allowed debuff choices
* Recent action results

Randy/Neuro can then choose high-level actions such as:

* Recall companion
* Follow player
* Attack once
* Start autoattack
* Buff player
* Choose a specific allowed buff
* Debuff player
* Debuff enemy
* Choose a specific allowed debuff
* Equip weapon from inventory
* Return weapon to player
* Check weapon status

Neuro does not directly control the player or press low-level inputs. The mod validates each action and executes it safely through Terraria logic.

This integration is optional. The same companion actions can be tested manually with `/neuro` commands.

## Troubleshooting

### The companion does not attack

Check that:

* The companion is summoned
* Neuro has a valid magic weapon equipped
* There is a valid enemy nearby
* The action is not on cooldown
* Autoattack mode is active, or `/neuro attack` was used

Use:

```text
/neuro status
/neuro weapon status
```

### Neuro cannot equip a weapon

Use:

```text
/neuro weapon inspect
```

This will show why the selected item is accepted or rejected.

### The weapon slot is missing

Open the normal player inventory.

The slot is intentionally hidden during:

* Chest UI
* Shop UI
* Reforge UI
* Guide crafting UI

### The staff tooltip shows 0 damage

Neuro has no weapon equipped.

Use:

```text
/neuro weapon set
```

or:

```text
/neuro weapon take
```

Then check the staff tooltip again.

### The staff was upgraded or reforged, but Neuro still fires at the old rate

Resummon Neuro with the upgraded/reforged staff.

The companion remembers the staff tier and prefix used when she was summoned.

### Randy does not connect

Randy is optional. You do not need it for normal command-based use.

If you are testing Randy integration, make sure Randy is running, then use:

```text
/neurows connect
/neurows status
```

Also check that the websocket URL in the config matches Randy’s address.

Default:

```text
ws://localhost:8000
```

### Randy keeps trying actions when the companion is missing

The mod treats missing-companion actions as skipped instead of hard failures, so Randy should not repeatedly retry them.

Summon the companion with the Neuro Companion Staff before testing combat actions.

## Latest Update

Version 0.2.0

* Added specific buff/debuff commands
* Added Randy support for specific buff/debuff choices
* Added allowed buff/debuff lists to Randy context
* Added Neuro Companion Staff damage, crit chance, and fire interval tooltip display
* Added universal staff prefix support for Neuro attacks
* Added upgraded staff tiers: Mk2, Mk3, and Mk4
* Added configurable maximum autoattack duration, up to 10 minutes
* Changed `/neuro buff` to prioritize buffs the player does not already have
* Limited Neuro Companion Staff reforges to universal combat prefixes

## Repository

Terraria mod:

```text
https://github.com/Pekotaker/NeuroCompanion
```

Neuro SDK:

```text
https://github.com/VedalAI/neuro-sdk
```
