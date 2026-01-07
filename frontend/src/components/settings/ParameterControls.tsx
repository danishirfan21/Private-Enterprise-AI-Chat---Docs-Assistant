import React from 'react';
import { useSettingsStore } from '../../store/settingsStore';

export const ParameterControls: React.FC = () => {
  const {
    temperature,
    setTemperature,
    maxTokens,
    setMaxTokens,
    useRAG,
    setUseRAG,
  } = useSettingsStore();

  return (
    <div className="space-y-5">
      {/* Temperature */}
      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <label className="text-sm font-medium text-gray-900 dark:text-gray-100">
            Temperature
          </label>
          <span className="text-sm text-gray-600 dark:text-gray-400">
            {temperature.toFixed(2)}
          </span>
        </div>
        <input
          type="range"
          min="0"
          max="2"
          step="0.1"
          value={temperature}
          onChange={(e) => setTemperature(parseFloat(e.target.value))}
          className="w-full h-2 bg-gray-200 dark:bg-gray-700 rounded-lg appearance-none cursor-pointer accent-blue-600"
        />
        <p className="text-xs text-gray-500 dark:text-gray-400">
          Higher values make output more random, lower values more deterministic
        </p>
      </div>

      {/* Max Tokens */}
      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <label className="text-sm font-medium text-gray-900 dark:text-gray-100">
            Max Tokens
          </label>
          <span className="text-sm text-gray-600 dark:text-gray-400">
            {maxTokens}
          </span>
        </div>
        <input
          type="range"
          min="100"
          max="4000"
          step="100"
          value={maxTokens}
          onChange={(e) => setMaxTokens(parseInt(e.target.value))}
          className="w-full h-2 bg-gray-200 dark:bg-gray-700 rounded-lg appearance-none cursor-pointer accent-blue-600"
        />
        <p className="text-xs text-gray-500 dark:text-gray-400">
          Maximum length of the generated response
        </p>
      </div>

      {/* RAG Toggle */}
      <div className="space-y-2">
        <label className="flex items-center justify-between cursor-pointer">
          <div>
            <div className="text-sm font-medium text-gray-900 dark:text-gray-100">
              Use Document Context (RAG)
            </div>
            <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
              Include relevant document chunks in responses
            </p>
          </div>
          <div className="relative">
            <input
              type="checkbox"
              checked={useRAG}
              onChange={(e) => setUseRAG(e.target.checked)}
              className="sr-only peer"
            />
            <div className="w-11 h-6 bg-gray-200 dark:bg-gray-700 peer-focus:ring-2 peer-focus:ring-blue-500 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
          </div>
        </label>
      </div>
    </div>
  );
};
