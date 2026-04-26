import { api } from "@/lib/api-client";
import type {
  PaginatedResponse,
  Faction,
  ChangeFactionRequest,
  CreateFactionRequest,
  FactionQuery,
} from "@/types/api.types";
import { queryOptions, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

const factionKeys = {
  all: ["factions"] as const,
  lists: () => [...factionKeys.all, "list"] as const,
  list: (filters: FactionQuery) => [...factionKeys.lists(), filters] as const,
  details: () => [...factionKeys.all, "detail"] as const,
  detail: (factionId: number) => [...factionKeys.details(), factionId] as const,
};

export const factionsQueryOptions = (params?: FactionQuery) =>
  queryOptions({
    queryKey: factionKeys.list(params ?? {}),
    queryFn: () => api.get<PaginatedResponse<Faction>>("/factions", params),
  });

export const useFactions = (params?: FactionQuery) => useQuery(factionsQueryOptions(params));

export const factionQueryOptions = (factionId: number) =>
  queryOptions({
    queryKey: factionKeys.detail(factionId),
    queryFn: () => api.get<Faction>(`/factions/${factionId.toString()}`),
  });

export const useFaction = (factionId: number) => useQuery(factionQueryOptions(factionId));

export const useCreateFaction = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateFactionRequest) => api.post<Faction>("/factions", data),
    onSuccess: (createdFaction) => {
      queryClient.setQueryData(factionKeys.detail(createdFaction.factionId), createdFaction);
      return queryClient.invalidateQueries({ queryKey: factionKeys.lists() });
    },
  });
};

export const useUpdateFaction = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ factionId, data }: { factionId: number; data: ChangeFactionRequest }) =>
      api.put<Faction>(`/factions/${factionId.toString()}`, data),
    onSuccess: (updatedFaction) => {
      queryClient.setQueryData(factionKeys.detail(updatedFaction.factionId), updatedFaction);
      return queryClient.invalidateQueries({ queryKey: factionKeys.lists() });
    },
  });
};

export const useDeleteFaction = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (factionId: number) => api.delete(`/factions/${factionId.toString()}`),
    onSuccess: (_, factionId) => {
      queryClient.removeQueries({ queryKey: factionKeys.detail(factionId) });
      return queryClient.invalidateQueries({ queryKey: factionKeys.lists() });
    },
  });
};
