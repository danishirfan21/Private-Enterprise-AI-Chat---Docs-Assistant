import React, { useEffect, useRef } from 'react';
import { useChatStore } from '../../store/chatStore';
import { ChatMessage } from './ChatMessage';
import { Loader2 } from 'lucide-react';

export const MessageList: React.FC = () => {
  const { messages, isStreaming } = useChatStore();
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Auto-scroll to bottom when new messages arrive
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, isStreaming]);

  if (messages.length === 0) {
    return (
      <div className="flex-1 flex items-center justify-center text-gray-500 dark:text-gray-400">
        <div className="text-center">
          <p className="text-lg mb-2">Start a conversation</p>
          <p className="text-sm">
            Ask questions or upload documents to get started
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-y-auto">
      <div className="max-w-4xl mx-auto">
        {messages.map((message) => (
          <ChatMessage key={message.id} message={message} />
        ))}

        {/* Streaming indicator */}
        {isStreaming && (
          <div className="flex gap-4 p-4 bg-gray-50 dark:bg-gray-800/50">
            <div className="flex-shrink-0 w-8 h-8 rounded-full bg-purple-500 text-white flex items-center justify-center">
              <Loader2 size={18} className="animate-spin" />
            </div>
            <div className="flex-1">
              <span className="text-sm text-gray-600 dark:text-gray-400">
                Assistant is typing...
              </span>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>
    </div>
  );
};
