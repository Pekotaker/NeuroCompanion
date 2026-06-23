# Neuro Companion

Neuro Companion is a tModLoader mod for Terraria that adds a summon companion with command-based behavior.

The companion works as a normal Terraria summon, but it can also be controlled through in-game commands. It can follow the player, attack enemies, use an equipped magic weapon, apply buffs/debuffs, and switch between follow and autoattack modes.

The mod can be used in two ways:

* **Standalone command mode**: control the companion directly with Terraria chat commands
* **Optional Neuro/Randy mode**: connect the mod to Randy from the VedalAI Neuro SDK so Neuro can choose actions through websocket

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
* [Weapon System](#weapon-system)
* [Optional Neuro/Randy Integration](#optional-neurorandy-integration)
* [Troubleshooting](#troubleshooting)
* [Repository](#repository)

## Features

* Adds the **Neuro Companion Staff**
* Summons a Neuro companion minion
* Companion follows the player
* Companion can attack once on command
* Companion can autoattack for a limited time
* Companion can use an equipped magic weapon
* Adds a Neuro weapon equipment slot in the inventory UI
* Player can manually assign a weapon to Neuro
* Neuro can take a valid weapon from the player inventory
* Supports direct-fire magic weapons
* Supports controllable magic weapons by treating them like normal attacks
* Supports some channeling magic weapons
* Rejects unsupported support/stationary/persistent-field magic weapons
* Can apply Red Potion-style buffs to the player
* Can apply Red Potion-style debuffs to the player or a nearby enemy
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

Shows companion status, current mode, autoattack timer, cooldowns, and weapon status.


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

```text
/neuro buff
```

Applies random positive Red Potion-style buffs to the player.

```text
/neuro debuff player
```

Applies random Red Potion-style debuffs to the player.

```text
/neuro debuff enemy
```

Applies random Red Potion-style debuffs to the nearest valid enemy.

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

## Weapon System

Neuro has her own magic weapon slot.

The slot appears in the inventory UI during normal inventory use. It is hidden when shops, chests, reforging, or guide crafting are open.

You can equip a weapon by dragging it into the slot or by using commands.

Neuro currently accepts:

* Direct-fire magic weapons (e.g., Wand of Sparking, Water Bolt, Demon Scythe, Razorblade Typhoon)
* Controllable magic weapons, treated like normal attacks (e.g., Magic Missile, Flamelash, Rainbow Rod)

Neuro rejects:

* Non-magic weapons
* Support magic weapons
* Channeling magic weapons that require the player to hold the attack button

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

Last Prism, Laser Machinegun and other handheld weapons are not supported yet because they need custom beam behavior attached to the companion body instead of the player.

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
* Recent action results

Randy/Neuro can then choose high-level actions such as:

* Follow player
* Attack once
* Start autoattack
* Buff player
* Debuff player
* Debuff enemy
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

## Repository

Terraria mod:

```text
https://github.com/Pekotaker/NeuroCompanion
```

NeuroSDK:

```text
https://github.com/VedalAI/neuro-sdk
```
