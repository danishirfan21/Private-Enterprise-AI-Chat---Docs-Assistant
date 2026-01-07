import apiClient from './client';
import type { ModelInfo } from '../types';

export const modelsApi = {
  list: async (): Promise<ModelInfo[]> => {
    const response = await apiClient.get<ModelInfo[]>('/models');
    return response.data;
  },
};
