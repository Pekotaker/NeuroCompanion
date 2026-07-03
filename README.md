# Neuro Companion

Neuro Companion is a tModLoader mod for Terraria that adds a summon companion you can control with chat commands.

The companion works like a normal summon, but you can also give her simple commands. She can follow you, attack enemies, use a magic weapon, apply buffs or debuffs, and use some special weapon profiles.

The mod works without Randy or the Neuro SDK. You can play it as a normal standalone Terraria mod.

Randy/Neuro SDK support is optional. It is only needed if you want to test Neuro/Randy control through websocket.

## Table of Contents

* [Features](#features)
* [Standalone Setup](#standalone-setup)
* [Commands](#commands)

  * [Companion Commands](#companion-commands)
  * [Weapon Commands](#weapon-commands)
  * [Websocket Commands](#websocket-commands)
  * [Debug Command](#debug-command)
* [Staff Upgrades](#staff-upgrades)
* [Weapon System](#weapon-system)
* [Evil Neuro](#evil-neuro)
* [Randy / Neuro SDK Integration](#randy--neuro-sdk-integration)

  * [Requirements](#requirements)
  * [Start Randy](#start-randy)
  * [Connect Terraria to Randy](#connect-terraria-to-randy)
  * [Manual Randy Action Tests](#manual-randy-action-tests)
  * [Randy Safety Notes](#randy-safety-notes)
* [Mod Config](#mod-config)
* [Troubleshooting](#troubleshooting)
* [Repository](#repository)

## Features

* Adds the **Neuro Companion Staff**.
* Summons a Neuro companion minion.
* Lets Neuro follow the player by default.
* Lets Neuro attack once on command.
* Lets Neuro attack nearby enemies for a limited time.
* Adds Evil Neuro behavior for harmful player-facing actions.
* Gives Neuro her own magic weapon slot.
* Lets you assign a weapon to Neuro through commands or the inventory UI.
* Lets Neuro take a valid magic weapon from your inventory.
* Shows Neuro's weapon damage, crit chance, and fire interval on the staff tooltip.
* Adds upgraded staff tiers: Mk2, Mk3, and Mk4.
* Supports several types of magic weapons.
* Lets Neuro apply allowed buffs and debuffs.
* Can connect to Randy through websocket for optional Neuro SDK testing.

## Standalone Setup

You only need:

* Terraria
* tModLoader

In tModLoader:

1. Enable Neuro Companion.
2. Enter a world.
3. Obtain or craft the Neuro Companion Staff.
4. Use the staff to summon Neuro.
5. Open your inventory and give Neuro a valid magic weapon, or use `/neuro weapon set`.
6. Use `/neuro` commands in chat.

Randy is not required.

## Commands

### Companion Commands

```text
/neuro help
```

Shows the Neuro command list.

```text
/neuro status
```

Shows Neuro's current status, mode, autoattack timer, cooldowns, and weapon info.

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
/neuro attack player
```

Makes Evil Neuro attack the player.

If Neuro has a valid equipped magic weapon, Evil Neuro uses it.

If Neuro has no valid equipped weapon, Evil Neuro uses a fallback evil bolt.

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

Shows Neuro's equipped weapon.

```text
/neuro weapon inspect
```

Checks your selected item and tells you if Neuro can use it.

```text
/neuro weapon set
```

Moves your selected hotbar item into Neuro's weapon slot if it is valid.

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

Returns Neuro's weapon to your inventory.

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

Neuro's displayed damage is based on:

* Neuro's equipped magic weapon
* the weapon's modifier
* your magic damage bonuses
* the staff's universal prefix

The staff can be reforged, but only with selected universal combat prefixes.

Staff tiers:

| Staff                     | Staff fire limit | Extra behavior                                                                                     |
| ------------------------- | ---------------: | -------------------------------------------------------------------------------------------------- |
| Neuro Companion Staff     |         50 ticks | Base tier                                                                                          |
| Neuro Companion Staff Mk2 |         30 ticks | Faster firing                                                                                      |
| Neuro Companion Staff Mk3 |           1 tick | Fires as fast as the equipped weapon allows                                                        |
| Neuro Companion Staff Mk4 |           1 tick | Lets Neuro detect enemies through blocks. Projectiles fired by Neuro can also pass through blocks. |

Important rule for most weapons:

```text
Actual Neuro fire interval = max(staff fire limit, equipped weapon useTime)
```

Some custom weapon profiles can define special behavior.

For example, Laser Machinegun uses a custom Neuro firing profile instead of normal player-held channeling behavior.

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

* Moving a weapon into Neuro's slot moves the item. It does not copy it.
* If Neuro already has a weapon, the old weapon is swapped back safely.
* `/neuro weapon take` ignores your selected hotbar item.
* Neuro does not take the item held by your mouse cursor.
* Neuro does not use the item currently in your hand. She uses her own equipped weapon.

Neuro currently accepts:

* Magic weapons that fire normal projectiles.
* Magic weapons with controllable projectiles, such as Magic Missile.
* Magic weapons that place attacks at a target position, such as Resonance Scepter.
* Supported custom weapon profiles, such as Laser Machinegun.

Neuro currently rejects:

* Non-magic weapons.
* Items with no damage.
* Items with no mana cost.
* Items that do not shoot projectiles.
* Weapons that create stationary clouds, fields, walls, or similar effects.
* Generic channeling weapons that depend on the player holding the attack button.
* Beam weapons that need custom behavior attached to Neuro's body, such as Last Prism.

Examples:

```text
Wand of Sparking     -> accepted
Water Bolt           -> accepted
Demon Scythe         -> accepted
Magic Missile        -> accepted
Flamelash            -> accepted
Rainbow Rod          -> accepted
Resonance Scepter    -> accepted
Laser Machinegun     -> accepted with custom Neuro profile
Nimbus Rod           -> rejected
Clinger Staff        -> rejected
Magnet Sphere        -> rejected
Last Prism           -> rejected
```

Some weapons do not work like normal projectiles.

For example, Resonance Scepter places its attack at the target position, so Neuro handles it as a targeted-area weapon.

Laser Machinegun normally depends on channeling behavior attached to the player. Neuro handles it with a custom weapon profile instead.

Last Prism and similar beam weapons still need custom support.

## Evil Neuro

Some actions make Neuro use her Evil Neuro behavior.

Current command:

```text
/neuro attack player
```

Evil Neuro attacks the player with her equipped magic weapon.

If Neuro has no valid equipped weapon, she uses a fallback evil bolt.

The fallback evil bolt scales with staff tier and world difficulty.

## Randy / Neuro SDK Integration

Randy integration is optional. You do not need it for normal command-based use.

This section is for testing Neuro/Randy control through websocket.

The mod sends Terraria context to Randy/Neuro, including:

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
* Attack player with Evil Neuro
* Buff player
* Choose a specific allowed buff
* Debuff player
* Debuff enemy
* Choose a specific allowed debuff
* Equip weapon from inventory
* Return weapon to player
* Check weapon status

### Requirements

You need:

* Git
* Node.js / npm
* tModLoader
* Neuro Companion enabled
* The VedalAI Neuro SDK repository

### Start Randy

Open a terminal somewhere you keep source code.

Clone the Neuro SDK:

```text
git clone https://github.com/VedalAI/neuro-sdk.git
cd neuro-sdk
```

Go into the Randy folder:

```text
cd Randy
```

Install dependencies:

```text
npm install
```

Start Randy:

```text
npm start
```

Leave this terminal open.

The exact terminal text may vary depending on the SDK version, but the important result is:

* Randy stays running.
* The terminal does not close with an error.
* You see server/websocket startup messages.
* The mod can connect to Randy at the configured websocket URL.

By default, Neuro Companion expects Randy at:

```text
ws://localhost:8000
```

You can change this in the tModLoader mod config.

### Connect Terraria to Randy

In tModLoader:

1. Enable Neuro Companion.
2. Enter a world.
3. Summon Neuro with the Neuro Companion Staff.
4. Give Neuro a valid magic weapon.

Then type this in Terraria chat:

```text
/neurows connect
```

Check the connection:

```text
/neurows status
```

You can also manually send the current game context:

```text
/neurows context
```

Expected result:

* Terraria should show that the websocket is connected.
* Randy's terminal should show activity.
* `/neurows status` should report the connection state.

### Manual Randy Action Tests

Open a second terminal.

Do not close the first terminal running Randy.

PowerShell autoattack test:

```powershell
$body = @{
    command = "action"
    data = @{
        id = "autoattack-test-1"
        name = "autoattack"
        data = '{"duration_seconds":10}'
    }
} | ConvertTo-Json -Depth 5

Invoke-RestMethod `
    -Method Post `
    -Uri "http://localhost:1337/" `
    -ContentType "application/json" `
    -Body $body
```

Expected result:

* Randy receives the action request.
* Neuro Companion receives the action through websocket.
* Neuro starts autoattacking in Terraria for about 10 seconds.
* The action result should appear in the mod's debug/status output.

PowerShell Evil Neuro test:

```powershell
$body = @{
    command = "action"
    data = @{
        id = "evil-neuro-test-1"
        name = "attack_player"
    }
} | ConvertTo-Json -Depth 5

Invoke-RestMethod `
    -Method Post `
    -Uri "http://localhost:1337/" `
    -ContentType "application/json" `
    -Body $body
```

Expected result:

* Evil Neuro attacks the player in-game.
* If Neuro has a valid magic weapon equipped, she uses that weapon.
* If Neuro has no valid weapon, she uses a fallback evil bolt.

More Randy test commands:

```text
[PASTE GOOGLE DRIVE LINK HERE]
```

### Randy Safety Notes

Randy/Neuro can only choose high-level actions exposed by the mod.

Randy does not directly control the player.

Randy does not press movement keys, attack keys, mouse buttons, or other low-level inputs.

The mod checks each action before running it.

You can test the same gameplay features yourself with `/neuro` commands.

## Mod Config

The mod has options in the tModLoader config menu.

Current options include:

* Randy websocket URL
* Auto-connect on world load
* Context sending interval
* Event context messages
* Low health context threshold
* Action cooldowns
* Evil Neuro player attack cooldown
* Maximum autoattack duration

The maximum autoattack duration can be set up to:

```text
600 seconds
10 minutes
```

This limit applies to both `/neuro autoattack <seconds>` and Randy/Neuro autoattack actions.

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

If you are testing Randy integration, check that:

* Randy is running.
* The Randy terminal did not show a startup error.
* The mod config websocket URL matches Randy's websocket URL.
* The default expected URL is `ws://localhost:8000`.
* You used `/neurows connect` in Terraria.

Useful commands:

```text
/neurows connect
/neurows status
/neurows context
/neurodebug
```

### Randy action test reaches Randy, but nothing happens in Terraria

Check that:

* Terraria is running.
* You are inside a world.
* Neuro Companion is enabled.
* Neuro is summoned if the action requires her.
* Neuro has a valid weapon if the action requires a weapon.
* The websocket is connected.
* The action name is spelled correctly.
* Action arguments are inside `data.data` as a JSON string when required.

For example:

```powershell
data = '{"duration_seconds":10}'
```

not:

```powershell
data = @{
    duration_seconds = 10
}
```

## Repository

```text
https://github.com/Pekotaker/NeuroCompanion
```
