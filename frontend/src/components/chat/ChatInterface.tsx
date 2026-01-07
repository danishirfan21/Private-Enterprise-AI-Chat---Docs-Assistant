import React from 'react';
import { MessageList } from './MessageList';
import { ChatInput } from './ChatInput';
import { useSSE } from '../../hooks/useSSE';

export const ChatInterface: React.FC = () => {
  const { sendMessage } = useSSE();

  return (
    <div className="flex flex-col h-full bg-white dark:bg-gray-900">
      <MessageList />
      <ChatInput onSendMessage={sendMessage} />
    </div>
  );
};
