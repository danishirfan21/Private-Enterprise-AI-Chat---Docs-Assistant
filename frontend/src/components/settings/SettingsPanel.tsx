import React from 'react';
import { Settings, X } from 'lucide-react';
import { useSettingsStore } from '../../store/settingsStore';
import { useUIStore } from '../../store/uiStore';
import { ModelSelector } from './ModelSelector';
import { ParameterControls } from './ParameterControls';
import { SystemPromptEditor } from './SystemPromptEditor';

export const SettingsPanel: React.FC = () => {
  const { settingsOpen, toggleSettings } = useUIStore();
  const { resetToDefaults } = useSettingsStore();

  if (!settingsOpen) return null;

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black/50 z-40"
        onClick={toggleSettings}
      />

      {/* Panel */}
      <div className="fixed right-0 top-0 bottom-0 w-full sm:w-96 bg-white dark:bg-gray-900 shadow-xl z-50 flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
          <div className="flex items-center gap-2">
            <Settings className="w-5 h-5 text-gray-700 dark:text-gray-300" />
            <h2 className="font-semibold text-gray-900 dark:text-gray-100">
              Settings
            </h2>
          </div>
          <button
            onClick={toggleSettings}
            className="p-1 rounded hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
            aria-label="Close settings"
          >
            <X className="w-5 h-5 text-gray-500 dark:text-gray-400" />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-4 space-y-6">
          <ModelSelector />
          <ParameterControls />
          <SystemPromptEditor />
        </div>

        {/* Footer */}
        <div className="p-4 border-t border-gray-200 dark:border-gray-700">
          <button
            onClick={resetToDefaults}
            className="w-full px-4 py-2 bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors"
          >
            Reset to Defaults
          </button>
        </div>
      </div>
    </>
  );
};
