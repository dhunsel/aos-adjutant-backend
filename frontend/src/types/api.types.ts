import type { components, paths } from "./schema.d";

type Schemas = components["schemas"];

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PagedQuery<TSortBy = never> {
  page?: number;
  pageSize?: number;
  sortBy?: TSortBy;
  sortDirection?: SortDirection;
}

type GetQueryType<TSortBy, P extends keyof paths> = PagedQuery<TSortBy> &
  Omit<
    NonNullable<paths[P]["get"]["parameters"]["query"]>,
    "Page" | "PageSize" | "SortBy" | "SortDirection"
  >;

export type SortDirection = Schemas["SortDirection"];

export type User = Schemas["CurrentUserResponseDto"];

export type Faction = Schemas["FactionResponseDto"];
export type CreateFactionRequest = Schemas["CreateFactionDto"];
export type ChangeFactionRequest = Schemas["ChangeFactionDto"];
export type FactionSortBy = Schemas["FactionSortBy"];
export type FactionQuery = GetQueryType<FactionSortBy, "/factions">;

export type BattleFormation = Schemas["BattleFormationResponseDto"];
export type CreateBattleFormationRequest = Schemas["CreateBattleFormationDto"];
export type ChangeBattleFormationRequest = Schemas["ChangeBattleFormationDto"];
export type BattleFormationSortBy = Schemas["BattleFormationSortBy"];
export type BattleFormationQuery = GetQueryType<
  BattleFormationSortBy,
  "/factions/{factionId}/battle-formations"
>;

export type Unit = Schemas["UnitResponseDto"];
export type CreateUnitRequest = Schemas["CreateUnitDto"];
export type ChangeUnitRequest = Schemas["ChangeUnitDto"];
export type UnitSortBy = Schemas["UnitSortBy"];
export type UnitQuery = GetQueryType<UnitSortBy, "/factions/{factionId}/units">;

export type AttackProfile = Schemas["AttackProfileResponseDto"];
export type CreateAttackProfileRequest = Schemas["CreateAttackProfileDto"];
export type ChangeAttackProfileRequest = Schemas["ChangeAttackProfileDto"];
export type AttackProfileSortBy = Schemas["AttackProfileSortBy"];
export type AttackProfileQuery = GetQueryType<
  AttackProfileSortBy,
  "/units/{unitId}/attack-profiles"
>;

export type Ability = Schemas["AbilityResponseDto"];
export type CreateAbilityRequest = Schemas["CreateAbilityDto"];
export type ChangeAbilityRequest = Schemas["ChangeAbilityDto"];
export type AbilitySortBy = Schemas["AbilitySortBy"];
export type AbilityQuery = GetQueryType<AbilitySortBy, "/factions/{factionId}/abilities">;

export type WeaponEffect = Schemas["WeaponEffectResponseDto"];
export type WeaponEffectSortBy = Schemas["WeaponEffectSortBy"];
export type WeaponEffectQuery = GetQueryType<WeaponEffectSortBy, "/weapon-effects">;

export type Phase = Schemas["Phase"];
export type Restriction = Schemas["Restriction"];
export type Turn = Schemas["Turn"];
export type GrandAlliance = Schemas["GrandAlliance"];
