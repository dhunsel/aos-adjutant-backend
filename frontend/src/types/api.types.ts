import type { components } from "./schema.d.ts";

type Schemas = components["schemas"];

export type Faction = Schemas["FactionResponseDto"];
export type CreateFactionRequest = Schemas["CreateFactionDto"];
export type ChangeFactionRequest = Schemas["ChangeFactionDto"];

export type BattleFormation = Schemas["BattleFormationResponseDto"];
export type CreateBattleFormationRequest = Schemas["CreateBattleFormationDto"];
export type ChangeBattleFormationRequest = Schemas["ChangeBattleFormationDto"];

export type Unit = Schemas["UnitResponseDto"];
export type CreateUnitRequest = Schemas["CreateUnitDto"];
export type ChangeUnitRequest = Schemas["ChangeUnitDto"];

export type AttackProfile = Schemas["AttackProfileResponseDto"];
export type CreateAttackProfileRequest = Schemas["CreateAttackProfileDto"];
export type ChangeAttackProfileRequest = Schemas["ChangeAttackProfileDto"];

export type Ability = Schemas["AbilityResponseDto"];
export type CreateAbilityRequest = Schemas["CreateAbilityDto"];
export type ChangeAbilityRequest = Schemas["ChangeAbilityDto"];

export type WeaponEffect = Schemas["WeaponEffectResponseDto"];

// Enums
export type TurnPhase = Schemas["TurnPhase"];
export type ActivationRestriction = Schemas["ActivationRestriction"];
export type PlayerTurn = Schemas["PlayerTurn"];
