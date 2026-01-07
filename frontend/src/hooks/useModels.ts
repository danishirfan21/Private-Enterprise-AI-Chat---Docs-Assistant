import { useQuery } from '@tanstack/react-query';
import { modelsApi } from '../api/models';

export const useModels = () => {
  const { data: models, isLoading, error } = useQuery({
    queryKey: ['models'],
    queryFn: () => modelsApi.list(),
    staleTime: 5 * 60 * 1000, // Cache for 5 minutes
  });

  return {
    models: models || [],
    isLoading,
    error,
  };
};
