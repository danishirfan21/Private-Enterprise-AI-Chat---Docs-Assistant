import React from 'react';
import { Plus } from 'lucide-react';
import { useConversations } from '../../hooks/useConversations';
import { useChatStore } from '../../store/chatStore';
import { ConversationList } from './ConversationList';

export const ChatHistorySidebar: React.FC = () => {
  const { isCreating } = useConversations();
  const { setConversationId, clearChat } = useChatStore();

  const handleNewChat = () => {
    setConversationId(null);
    clearChat();
  };

  return (
    <div className="flex flex-col h-full bg-white dark:bg-gray-900 border-r border-gray-200 dark:border-gray-700">
      {/* Header */}
      <div className="p-4 border-b border-gray-200 dark:border-gray-700">
        <button
          onClick={handleNewChat}
          disabled={isCreating}
          className="w-full flex items-center justify-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Plus className="w-4 h-4" />
          <span className="font-medium">New Chat</span>
        </button>
      </div>

      {/* Conversations List */}
      <div className="flex-1 overflow-y-auto p-3">
        <ConversationList />
      </div>
    </div>
  );
};
