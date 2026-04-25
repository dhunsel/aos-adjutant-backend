import { api } from "@/lib/api-client";
import type { Faction, ChangeFactionRequest, CreateFactionRequest } from "@/types/api.types";
import { queryOptions, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

const factionKeys = {
  all: ["factions"] as const,
  lists: () => [...factionKeys.all, "list"] as const,
  details: () => [...factionKeys.all, "detail"] as const,
  detail: (factionId: number) => [...factionKeys.details(), factionId] as const,
};

export const factionsQueryOptions = () =>
  queryOptions({ queryKey: factionKeys.lists(), queryFn: () => api.get<Faction[]>("/factions") });

export const useFactions = () => useQuery(factionsQueryOptions());

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
