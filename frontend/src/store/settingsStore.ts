import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface SettingsState {
  modelName: string;
  temperature: number;
  maxTokens: number;
  useRAG: boolean;
  systemPrompt: string;

  setModelName: (model: string) => void;
  setTemperature: (temp: number) => void;
  setMaxTokens: (tokens: number) => void;
  setUseRAG: (useRAG: boolean) => void;
  setSystemPrompt: (prompt: string) => void;
  resetToDefaults: () => void;
}

const defaultSettings = {
  modelName: 'llama3:latest',
  temperature: 0.7,
  maxTokens: 2000,
  useRAG: true,
  systemPrompt: 'You are a helpful AI assistant with access to document knowledge.',
};

export const useSettingsStore = create<SettingsState>()(
  persist(
    (set) => ({
      ...defaultSettings,

      setModelName: (modelName) => set({ modelName }),
      setTemperature: (temperature) => set({ temperature }),
      setMaxTokens: (maxTokens) => set({ maxTokens }),
      setUseRAG: (useRAG) => set({ useRAG }),
      setSystemPrompt: (systemPrompt) => set({ systemPrompt }),
      resetToDefaults: () => set(defaultSettings),
    }),
    {
      name: 'enterprise-ai-settings',
    }
  )
);
