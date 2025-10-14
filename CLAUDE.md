# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6000.2.7f2 project implementing a turn-based tactical game with two distinct game modes:
1. **Turn-based strategy** (Revolucio scene) - Token-based board gameplay with influence mechanics
2. **Card-based combat** (Combat scene) - Character battles using deck-building mechanics

## Architecture

### Game Mode 1: Turn-Based Strategy (GenericTurnBased)

The turn-based system uses a manager pattern with the following core architecture:

- **GameManager** (singleton): Central coordinator that manages TurnManager and provides static access to game state
- **TurnManager**: Handles turn progression between PlayerSide instances, manages turn counter and active side
- **PlayerSide**: Represents a faction (player or AI), manages tokens by tag, can execute AI routines
- **InfluenceManager** (singleton): Tracks influence points that accumulate each player turn

Token system hierarchy:
- **Token** (base class): Manages actions, blocking state, and health/damage
- **BaseToken**, **PersonaToken**, **HunterToken**, **EnemyToken**: Specialized token types
- **AbstractAction**: Base for all actions with `OnActivate(Token active, Token target)` pattern
- Available actions: MoveAction, AttackAction, DetectAction, HideAction, InfluenceAction, RecruitAction

Key flow:
1. GameManager.Start() → TurnManager.Initialize()
2. TurnManager alternates between PlayerSide instances
3. Each side blocks/unblocks its tokens by tag
4. AI sides automatically execute IARoutine when active
5. InfluenceManager increments influence on player turns

### Game Mode 2: Card-Based Combat (Combat)

Combat uses a coroutine-based phase system:

- **CombatManager**: Orchestrates multi-phase combat rounds via coroutines
- **Character**: Data class with HP, speed, status effects, assigned card, and unique ability
- **Card** (ScriptableObject): Defines attack, defense, card type (Melee/Magic/Ranged), and status effects
- **Ability** (ScriptableObject): Character unique abilities usable once per combat

Combat phases (per round):
1. CardSelectionPhase: Player assigns cards, AI selects randomly
2. AbilityPhase: Optional ability activation
3. InitiativeResolution: Order characters by speed
4. CardCombatPhase: 1v1 resolution with damage calculation and status application
5. UpdateStatusEffects: Decrement status durations

Combat mechanics:
- Base HP scales inversely with team size (1=10HP, 2=8HP, 3=7HP, 4=6HP)
- Damage = max(0, attacker.totalAttack - defender.totalDefense)
- Combo system: 2 consecutive cards of same type grants +1 attack
- Status effects: Fire (-1 defense), Poison (-1 attack), Ice (blocks abilities)

### Data Management

ScriptableObjects stored in Assets/Data/:
- Cards: `Assets/Data/Cards/*.asset`
- Abilities: `Assets/Data/Abilities/*.asset`

Input system: Uses new Input System with actions defined in `Assets/InputSystem_Actions.inputactions`

### Unity-Specific Integration

This project includes Unity MCP (Model Context Protocol) bridge for external tool integration:
- Package: `com.coplaydev.unity-mcp` (local file reference)
- Located at: `C:/Users/eduar/Downloads/unity-mcp/UnityMcpBridge`

## Common Development Commands

### Opening the Project
Open the project in Unity Editor 6000.2.7f2 or later. The solution file is `revolution.sln`.

### Building
Build through Unity Editor: File → Build Settings → Build

### Testing Scenes
- Main gameplay: `Assets/Scenes/Revolucio.unity`
- Combat mode: `Assets/Scenes/Combat.unity`

### Creating New Content

**New Token Type:**
1. Create class inheriting from `Token` in `Assets/Scripts/TokenTypes/`
2. Add to prefab in `Assets/Prefabs/`
3. Assign appropriate tag in Unity Editor

**New Action:**
1. Create class inheriting from `AbstractAction` in `Assets/Scripts/TokenActions/`
2. Implement `OnActivate(Token active, Token target)` method
3. Attach to token prefabs' `buttonActions` array

**New Card:**
1. Right-click in `Assets/Data/Cards/`
2. Create → Card Game → Card
3. Configure stats and assign to CombatManager's deck

**New Ability:**
1. Right-click in `Assets/Data/Abilities/`
2. Create → Card Game → Ability
3. Assign to character's `uniqueAbility` field

## Key Implementation Patterns

### Singleton Pattern
GameManager and InfluenceManager use lazy singleton with Instance property. Always null-check Instance before use.

### Tag-Based Token Management
PlayerSide finds tokens via `GameObject.FindGameObjectsWithTag(tokenName)`. Tags must match PlayerSide.tokenName exactly.

### Action Economy
Tokens have `numActions`/`maxNumActions`. Calling `SpendAction()` decrements count and blocks token when reaching 0. `RestoreActions()` resets for new turn.

### Combat Coroutine Flow
CombatManager uses coroutines with `yield return` for phase transitions. Wait flags (`waitingForCardSelection`, `waitingForAbilityActivation`) pause execution until UI interaction completes.

## Important Notes

- The project uses URP (Universal Render Pipeline) 17.2.0
- Input uses the new Input System package (1.14.2)
- TextMesh Pro is configured in `Assets/TextMesh Pro/`
- Git repository uses standard Unity `.gitignore` (Library, Temp, Logs excluded)
