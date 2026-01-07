import React from 'react';
import { FileText, Trash2, Loader2 } from 'lucide-react';
import type { Document } from '../../types';
import { format } from 'date-fns';
import clsx from 'clsx';

interface DocumentItemProps {
  document: Document;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export const DocumentItem: React.FC<DocumentItemProps> = ({
  document,
  onDelete,
  isDeleting,
}) => {
  const formatFileSize = (bytes: number): string => {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
  };

  return (
    <div
      className={clsx(
        'flex items-center gap-3 p-3 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg hover:shadow-md transition-shadow',
        isDeleting && 'opacity-50'
      )}
    >
      {/* File icon */}
      <div className="flex-shrink-0 w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded flex items-center justify-center">
        <FileText className="w-5 h-5 text-blue-600 dark:text-blue-400" />
      </div>

      {/* File info */}
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-gray-900 dark:text-gray-100 truncate">
          {document.fileName}
        </p>
        <div className="flex items-center gap-2 text-xs text-gray-500 dark:text-gray-400 mt-1">
          <span>{formatFileSize(document.fileSizeBytes)}</span>
          <span>•</span>
          <span>{document.chunkCount} chunks</span>
          <span>•</span>
          <span>{format(new Date(document.uploadedAt), 'MMM d, yyyy')}</span>
        </div>
      </div>

      {/* Delete button */}
      <button
        onClick={() => onDelete(document.id)}
        disabled={isDeleting}
        className="flex-shrink-0 p-2 text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors disabled:cursor-not-allowed"
        aria-label="Delete document"
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
