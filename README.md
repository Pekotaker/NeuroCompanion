# Neuro Companion

Neuro Companion is a tModLoader mod for Terraria that adds a summon companion you can control with chat commands.

The companion works like a normal summon, but you can also give her simple commands. She can follow you, attack enemies, use a magic weapon, and apply buffs or debuffs.

The mod works without Randy or the Neuro SDK. You can play it as a normal Terraria mod.

If you want to test Neuro/Randy control, the mod can also connect to Randy through websocket. Randy can choose high-level actions, but it does not directly control the player.

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
* [Repository](#repository)

## Features

* Adds the **Neuro Companion Staff**.
* Summons a Neuro companion minion.
* Lets Neuro follow the player by default.
* Lets Neuro attack once on command.
* Lets Neuro attack nearby enemies for a limited time.
* Gives Neuro her own magic weapon slot.
* Lets you assign a weapon to Neuro through commands or the inventory UI.
* Lets Neuro take a valid magic weapon from your inventory.
* Shows Neuro’s weapon damage, crit chance, and fire interval on the staff tooltip.
* Adds upgraded staff tiers: Mk2, Mk3, and Mk4.
* Supports several types of magic weapons.
* Lets Neuro apply allowed buffs and debuffs.
* Can connect to Randy through websocket for optional Neuro SDK testing.

## Setup

### Standalone Setup

You only need:

* Terraria
* tModLoader

In tModLoader:

1. Enter a world.
2. Obtain or craft the Neuro Companion Staff.

After that, use `/neuro` commands in chat.

Randy is not required.

### Optional Randy Setup

This is for proper Neuro Integration.

Clone this repository and the Neuro SDK repository:
```text
https://github.com/VedalAI/neuro-sdk
```

Start Randy locally:

```text
cd Randy
npm start
```

By default, the mod expects Randy at:

```text
ws://localhost:8000
```

You can change this in the mod config.

Then run this in Terraria chat:

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

Shows Neuro’s current status, mode, autoattack timer, cooldowns, and weapon info.

```text
/neuro follow
```

Stops autoattack mode and makes Neuro follow the player.

```text
/neuro attack
```

Makes Neuro attack once with her equipped magic weapon.

If there is a valid enemy nearby, Neuro targets that enemy.

If there is no valid enemy nearby, Neuro attacks toward your cursor.

```text
/neuro autoattack
```

Starts autoattack mode for the default duration.

```text
/neuro autoattack 30
```

Starts autoattack mode for 30 seconds.

The duration is limited by the maximum autoattack duration in the mod config.

```text
/neuro buff
```

Applies an allowed positive buff to the player.

Neuro tries to pick a buff you do not already have.

```text
/neuro buff ironskin
/neuro buff obsidian skin
/neuro buff 5
```

Applies a specific allowed buff by name or ID.

```text
/neuro debuff player
```

Applies an allowed debuff to the player.

```text
/neuro debuff player poisoned
/neuro debuff player 20
```

Applies a specific allowed debuff to the player by name or ID.

```text
/neuro debuff enemy
```

Applies an allowed debuff to the nearest valid enemy.

```text
/neuro debuff enemy poisoned
/neuro debuff enemy 20
```

Applies a specific allowed debuff to the nearest valid enemy by name or ID.

### Weapon Commands

```text
/neuro weapon status
```

Shows Neuro’s equipped weapon.

```text
/neuro weapon inspect
```

Checks your selected item and tells you if Neuro can use it.

```text
/neuro weapon set
```

Moves your selected hotbar item into Neuro’s weapon slot if it is valid.

If Neuro already has a weapon, the old weapon is swapped back.

```text
/neuro weapon take
```

Lets Neuro take the strongest valid magic weapon from your inventory.

This ignores your selected hotbar item, so Neuro does not take the weapon you are currently holding.

If Neuro already has a weapon, the old weapon is swapped back.

```text
/neuro weapon return
```

Returns Neuro’s weapon to your inventory.

### Websocket Commands

These commands are only needed for Randy/Neuro testing.

```text
/neurows connect
```

Connects the mod to Randy.

```text
/neurows status
```

Shows the websocket connection status.

```text
/neurows context
```

Sends the current Terraria context to Randy.

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

Shows detailed debug info, including connection status, summon status, companion mode, cooldowns, queued actions, last action result, and last context sent.

## Staff Upgrades

The Neuro Companion Staff controls how often Neuro can fire her equipped weapon.

The staff tooltip shows:

* Neuro weapon damage
* Neuro weapon crit chance
* Neuro staff fire limit
* Neuro effective fire interval

Neuro’s displayed damage is based on:

* Neuro’s equipped magic weapon
* the weapon’s modifier
* your magic damage bonuses
* the staff’s universal prefix

The staff can be reforged, but only with selected universal combat prefixes.

Staff tiers:

| Staff | Staff fire limit | Extra behavior |
| --- | ---: | --- |
| Neuro Companion Staff | 50 ticks | Base tier |
| Neuro Companion Staff Mk2 | 30 ticks | Faster firing |
| Neuro Companion Staff Mk3 | 1 tick | Fires as fast as the equipped weapon allows |
| Neuro Companion Staff Mk4 | 1 tick | Lets Neuro detect enemies through blocks. Projectiles fired by Neuro can also pass through blocks. |

Important rule:

```text
Actual Neuro fire interval = max(staff fire limit, equipped weapon useTime)
```

So Mk3 and Mk4 are not true 1-tick spam unless the equipped weapon itself has a 1-tick use time.

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

After changing or reforging the staff, summon Neuro again. She uses the staff tier and prefix from the staff that summoned her.

## Weapon System

Neuro has her own magic weapon slot.

The slot appears in the normal inventory UI. It is hidden when shops, chests, reforging, or guide crafting are open.

You can equip a weapon by dragging it into the slot or by using commands.

Weapons can be assigned by:

* Dragging a valid weapon into the Neuro weapon slot.
* Using `/neuro weapon set`.
* Using `/neuro weapon take`.
* Letting Randy/Neuro choose the matching weapon action when connected.

Important weapon rules:

* Moving a weapon into Neuro’s slot moves the item. It does not copy it.
* If Neuro already has a weapon, the old weapon is swapped back safely.
* `/neuro weapon take` ignores your selected hotbar item.
* Neuro does not take the item held by your mouse cursor.

Neuro currently accepts:

* Magic weapons that fire normal projectiles.
* Magic weapons with controllable projectiles, such as Magic Missile.
* Some simple channeling magic weapons.
* Magic weapons that place an attack at a target position, such as Resonance Scepter.

Neuro rejects:

* Non-magic weapons.
* Items with no damage.
* Items with no mana cost.
* Items that do not shoot projectiles.
* Weapons that create stationary clouds, fields, walls, or similar effects.
* Beam weapons that need special behavior attached to the player.

Examples:

```text
Wand of Sparking     -> accepted
Water Bolt           -> accepted
Demon Scythe         -> accepted
Magic Missile        -> accepted
Flamelash            -> accepted
Rainbow Rod          -> accepted
Resonance Scepter    -> accepted
Laser Machinegun     -> rejected
Nimbus Rod           -> rejected
Clinger Staff        -> rejected
Magnet Sphere        -> rejected
Last Prism           -> rejected
```

Some weapons do not work like normal projectiles. For example, Resonance Scepter places its attack at the target position. Neuro handles that as a special case.

Last Prism, Laser Machinegun, and similar beam weapons are not supported yet. They need custom behavior attached to Neuro’s body instead of the player.

## Mod Config

The mod has options in the tModLoader config menu.

Current options include:

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

This limit applies to both `/neuro autoattack <seconds>` and Randy/Neuro autoattack actions.

## Optional Neuro/Randy Integration

The mod can connect to Randy from the VedalAI Neuro SDK.

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

Randy/Neuro can choose high-level actions such as:

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

Neuro does not control the player directly. She does not press movement keys, attack keys, or other low-level inputs.

The mod checks each action before running it.

This integration is optional. You can test the same actions yourself with `/neuro` commands.

## Troubleshooting

### The companion does not attack

Check that:

* The companion is summoned.
* Neuro has a valid magic weapon equipped.
* There is a valid enemy nearby.
* The action is not on cooldown.
* Autoattack mode is active, or `/neuro attack` was used.

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

This shows why the selected item is accepted or rejected.

### The weapon slot is missing

Open the normal player inventory.

The slot is hidden during:

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

Summon Neuro again with the upgraded or reforged staff.

Neuro remembers the staff tier and prefix used when she was summoned.

### Randy does not connect

Randy is optional. You do not need it for normal command-based use.

If you are testing Randy integration, make sure Randy is running. Then use:

```text
/neurows connect
/neurows status
```

Also check that the websocket URL in the config matches Randy’s address.

Default:

```text
ws://localhost:8000
```

### Randy keeps trying actions when the companion is not summoned

The mod treats these actions as skipped instead of hard failures, so Randy should not keep retrying them forever.

Summon the companion with the Neuro Companion Staff before testing combat actions.

## Repository

Terraria mod:

```text
https://github.com/Pekotaker/NeuroCompanion
```

Neuro SDK:

```text
https://github.com/VedalAI/neuro-sdk
```