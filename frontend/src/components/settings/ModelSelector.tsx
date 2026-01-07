import React from 'react';
import { useSettingsStore } from '../../store/settingsStore';
import { useModels } from '../../hooks/useModels';
import { Loader2 } from 'lucide-react';

export const ModelSelector: React.FC = () => {
  const { modelName, setModelName } = useSettingsStore();
  const { models, isLoading } = useModels();

  return (
    <div className="space-y-2">
      <label className="block text-sm font-medium text-gray-900 dark:text-gray-100">
        Model
      </label>
      <div className="relative">
        <select
          value={modelName}
          onChange={(e) => setModelName(e.target.value)}
          disabled={isLoading}
          className="w-full px-3 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
        >
          {isLoading ? (
            <option>Loading models...</option>
          ) : models.length > 0 ? (
            models.map((model) => (
              <option key={model.name} value={model.name}>
                {model.name}
              </option>
            ))
          ) : (
            <option value={modelName}>{modelName}</option>
          )}
        </select>
        {isLoading && (
          <div className="absolute right-3 top-1/2 -translate-y-1/2">
            <Loader2 className="w-4 h-4 animate-spin text-gray-400" />
          </div>
        )}
      </div>
      <p className="text-xs text-gray-500 dark:text-gray-400">
        Select the language model to use for chat
      </p>
    </div>
  );
};
