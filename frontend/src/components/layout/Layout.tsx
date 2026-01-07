import React from 'react';
import { Header } from './Header';
import { ChatHistorySidebar } from '../history/ChatHistorySidebar';
import { ChatInterface } from '../chat/ChatInterface';
import { DocumentPanel } from '../documents/DocumentPanel';
import { SettingsPanel } from '../settings/SettingsPanel';
import { useUIStore } from '../../store/uiStore';
import clsx from 'clsx';

export const Layout: React.FC = () => {
  const { sidebarOpen } = useUIStore();

  return (
    <div className="h-screen flex flex-col bg-gray-50 dark:bg-gray-950">
      {/* Header */}
      <Header />

      {/* Main content */}
      <div className="flex-1 flex overflow-hidden">
        {/* Chat history sidebar */}
        <aside
          className={clsx(
            'w-64 flex-shrink-0 transition-all duration-300',
            sidebarOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
            'lg:relative absolute inset-y-0 left-0 z-30'
          )}
        >
          <ChatHistorySidebar />
        </aside>

        {/* Chat interface */}
        <main className="flex-1 flex flex-col min-w-0">
          <ChatInterface />
        </main>

        {/* Document panel */}
        <aside className="w-80 flex-shrink-0 hidden xl:block">
          <DocumentPanel />
        </aside>
      </div>

      {/* Settings panel (overlay) */}
      <SettingsPanel />
    </div>
  );
};
