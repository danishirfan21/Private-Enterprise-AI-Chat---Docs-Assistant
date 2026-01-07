import React from 'react';
import { MessageSquare, Trash2, Loader2 } from 'lucide-react';
import type { Conversation } from '../../types';
import { format } from 'date-fns';
import clsx from 'clsx';

interface ConversationItemProps {
  conversation: Conversation;
  isActive: boolean;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export const ConversationItem: React.FC<ConversationItemProps> = ({
  conversation,
  isActive,
  onSelect,
  onDelete,
  isDeleting,
}) => {
  const handleDelete = (e: React.MouseEvent) => {
    e.stopPropagation();
    onDelete(conversation.id);
  };

  return (
    <div
      onClick={() => onSelect(conversation.id)}
      className={clsx(
        'group flex items-center gap-3 p-3 rounded-lg cursor-pointer transition-colors',
        isActive
          ? 'bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800'
          : 'hover:bg-gray-100 dark:hover:bg-gray-800 border border-transparent',
        isDeleting && 'opacity-50 cursor-not-allowed'
      )}
    >
      {/* Icon */}
      <div className="flex-shrink-0">
        <MessageSquare
          className={clsx(
            'w-4 h-4',
            isActive
              ? 'text-blue-600 dark:text-blue-400'
              : 'text-gray-400 dark:text-gray-500'
          )}
        />
      </div>

      {/* Content */}
      <div className="flex-1 min-w-0">
        <p
          className={clsx(
            'text-sm font-medium truncate',
            isActive
              ? 'text-blue-900 dark:text-blue-100'
              : 'text-gray-900 dark:text-gray-100'
          )}
        >
          {conversation.title || 'New Conversation'}
        </p>
        <p className="text-xs text-gray-500 dark:text-gray-400">
          {format(new Date(conversation.createdAt), 'MMM d, HH:mm')}
        </p>
      </div>

      {/* Delete button */}
      <button
        onClick={handleDelete}
        disabled={isDeleting}
        className="flex-shrink-0 p-1 opacity-0 group-hover:opacity-100 text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-all disabled:cursor-not-allowed"
        aria-label="Delete conversation"
      >
        {isDeleting ? (
          <Loader2 className="w-4 h-4 animate-spin" />
        ) : (
          <Trash2 className="w-4 h-4" />
        )}
      </button>
    </div>
  );
};
