import React from 'react';
import { useConversations } from '../../hooks/useConversations';
import { useChatStore } from '../../store/chatStore';
import { ConversationItem } from './ConversationItem';
import { Loader2, Inbox } from 'lucide-react';

export const ConversationList: React.FC = () => {
  const { conversations, isLoading, deleteConversation, isDeleting } =
    useConversations();
  const { conversationId, setConversationId, clearChat } = useChatStore();

  const handleSelect = (id: string) => {
    if (id !== conversationId) {
      setConversationId(id);
      clearChat();
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 animate-spin text-gray-400" />
      </div>
    );
  }

  if (!conversations || conversations.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-gray-500 dark:text-gray-400">
        <Inbox className="w-12 h-12 mb-3 opacity-50" />
        <p className="text-sm">No conversations yet</p>
      </div>
    );
  }

  return (
    <div className="space-y-1">
      {conversations.map((conversation) => (
        <ConversationItem
          key={conversation.id}
          conversation={conversation}
          isActive={conversation.id === conversationId}
          onSelect={handleSelect}
          onDelete={deleteConversation}
          isDeleting={isDeleting}
        />
      ))}
    </div>
  );
};
