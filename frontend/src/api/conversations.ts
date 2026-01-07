import apiClient from './client';
import type { Conversation } from '../types';

export const conversationsApi = {
  list: async (): Promise<Conversation[]> => {
    const response = await apiClient.get<Conversation[]>('/conversations');
    return response.data;
  },

  get: async (id: string): Promise<Conversation> => {
    const response = await apiClient.get<Conversation>(`/conversations/${id}`);
    return response.data;
  },

  create: async (title?: string): Promise<Conversation> => {
    const response = await apiClient.post<Conversation>('/conversations', { title });
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/conversations/${id}`);
  },
};
