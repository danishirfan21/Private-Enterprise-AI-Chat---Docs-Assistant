import React from 'react';
import { useSettingsStore } from '../../store/settingsStore';

export const SystemPromptEditor: React.FC = () => {
  const { systemPrompt, setSystemPrompt } = useSettingsStore();

  return (
    <div className="space-y-2">
      <label className="block text-sm font-medium text-gray-900 dark:text-gray-100">
        System Prompt
      </label>
      <textarea
        value={systemPrompt}
        onChange={(e) => setSystemPrompt(e.target.value)}
        rows={4}
        className="w-full px-3 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-900 dark:text-gray-100 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
        placeholder="Enter system prompt..."
      />
      <p className="text-xs text-gray-500 dark:text-gray-400">
        Instructions that guide the AI's behavior and personality
      </p>
    </div>
  );
};
