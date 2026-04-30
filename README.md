# Neuro Companion

Neuro Companion is a tModLoader mod for Terraria that adds a summon companion intended to act as Neuro-sama’s in-game body.

The companion can work like a normal Terraria summon, but it can also connect to Randy from the VedalAI Neuro SDK for local action testing.

Neuro does not directly control the player. Instead, the mod exposes high-level actions such as:

- Recall the companion
- Follow the player
- Attack once
- Autoattack for a limited time
- Buff the player
- Debuff the player or a nearby enemy

## Setup

### 1. Install requirements

You need:

- Terraria on Steam
- tModLoader
- Node.js and npm
- VedalAI Neuro SDK

Links:

- tModLoader: https://www.tmodloader.net/
- Neuro SDK: https://github.com/VedalAI/neuro-sdk
- Randy README: https://github.com/VedalAI/neuro-sdk/blob/main/Randy/README.md

### 2. Enable the mod

1. Launch tModLoader.
2. Enable `NeuroCompanion`.
3. Load a world.
4. Craft or obtain the `Neuro Companion Staff`.
5. Use the staff to summon the companion.

Current test recipe:

```text
10 Wood at a Work Bench
```

### 3. Start Randy

Clone the Neuro SDK:

```powershell
git clone https://github.com/VedalAI/neuro-sdk.git
```

Go to Randy:

```powershell
cd "D:\Work\Neuro SDK\neuro-sdk\Randy"
```

Install dependencies:

```powershell
npm install
```

Start Randy:

```powershell
npm start
```

Randy should run at:

```text
WebSocket: ws://localhost:8000
HTTP:      http://localhost:1337
```

### 4. Connect Terraria to Randy

In Terraria chat:

```text
/neurows connect
```

Check connection:

```text
/neurows status
```

Disconnect:

```text
/neurows disconnect
```

## Commands

## Companion commands

### Show help

```text
/neuro help
```

### Show companion status

```text
/neuro status
```

### Follow

```text
/neuro follow
```

Stops autoattack and makes the companion follow the player.

### Attack once

```text
/neuro attack
```

Makes the companion fire one Razorblade Typhoon-style attack.

If an enemy is nearby, it attacks the enemy.  
If no enemy is nearby, it fires toward the cursor.

### Autoattack

```text
/neuro autoattack
```

Starts autoattack for the default duration.

```text
/neuro autoattack 30
```

Starts autoattack for 30 seconds.

The current maximum duration is 180 seconds.

### Buff player

```text
/neuro buff
```

Applies 3 random Red Potion-style positive buffs to the player.

### Debuff player

```text
/neuro debuff player
```

Applies Red Potion-style debuffs to the player.

### Debuff nearest enemy

```text
/neuro debuff enemy
```

Applies Red Potion-style debuffs to the nearest valid enemy.

## Websocket commands

### Show websocket help

```text
/neurows help
```

### Connect to Randy

```text
/neurows connect
```

### Disconnect from Randy

```text
/neurows disconnect
```

### Show connection status

```text
/neurows status
```

### Send current game context

```text
/neurows context
```

## Debug command

```text
/neurodebug
```

Shows useful debug information, such as:

- Whether Randy is connected
- Whether the companion is summoned
- Current companion mode
- Autoattack time remaining
- Action cooldowns
- Last received action
- Last action result

## Troubleshooting

### `/neurows status` says disconnected

Make sure Randy is running:

```powershell
cd "D:\Work\Neuro SDK\neuro-sdk\Randy"
npm start
```

Then reconnect in Terraria:

```text
/neurows connect
```

### The companion does not respond

Make sure the companion is summoned with the `Neuro Companion Staff`.

These actions require the companion to be summoned:

```text
/neuro recall
/neuro follow
/neuro attack
/neuro autoattack
```

### Randy sends an action but nothing happens

Use:

```text
/neurodebug
```

Check whether:

- Randy is connected
- The companion is summoned
- The action is on cooldown
- The last action result says it was skipped

### Actions are skipped because of cooldown

Wait a few seconds and try again.

Default cooldowns:

```text
recall: 2 seconds
follow: 1 second
attack once: 3 seconds
autoattack: 5 seconds
buff player: 60 seconds
debuff player: 60 seconds
debuff enemy: 30 seconds
```

### Randy keeps retrying an action

Usually this means Randy received a failed action result.

Normal skipped actions, such as cooldowns or missing companion, should be treated as skipped rather than failed. Check `/neurodebug` to see the last action result.

### The mod connects to the wrong Randy address

Open the mod config menu in tModLoader and check the Randy WebSocket URL.

Default:

```text
ws://localhost:8000
```
