import React from 'react';
import { Sun, Moon, Settings, Menu, X } from 'lucide-react';
import { useUIStore } from '../../store/uiStore';

export const Header: React.FC = () => {
  const { theme, toggleTheme, sidebarOpen, toggleSidebar, toggleSettings } =
    useUIStore();

  return (
    <header className="flex items-center justify-between px-4 py-3 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700">
      {/* Left side */}
      <div className="flex items-center gap-3">
        <button
          onClick={toggleSidebar}
          className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors lg:hidden"
          aria-label="Toggle sidebar"
        >
          {sidebarOpen ? <X size={20} /> : <Menu size={20} />}
        </button>
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg" />
          <div>
            <h1 className="font-bold text-lg text-gray-900 dark:text-gray-100">
              Enterprise AI Assistant
            </h1>
            <p className="text-xs text-gray-500 dark:text-gray-400">
              Private & Local
            </p>
          </div>
        </div>
      </div>

      {/* Right side */}
      <div className="flex items-center gap-2">
        {/* Theme toggle */}
        <button
          onClick={toggleTheme}
          className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
          aria-label="Toggle theme"
        >
          {theme === 'light' ? (
            <Moon className="w-5 h-5 text-gray-700 dark:text-gray-300" />
          ) : (
            <Sun className="w-5 h-5 text-gray-700 dark:text-gray-300" />
          )}
        </button>

        {/* Settings button */}
        <button
          onClick={toggleSettings}
          className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
          aria-label="Open settings"
        >
          <Settings className="w-5 h-5 text-gray-700 dark:text-gray-300" />
        </button>
      </div>
    </header>
  );
};
