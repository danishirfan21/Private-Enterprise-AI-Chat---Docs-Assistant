import React from 'react';
import { DocumentUploader } from './DocumentUploader';
import { DocumentList } from './DocumentList';
import { FileText } from 'lucide-react';

export const DocumentPanel: React.FC = () => {
  return (
    <div className="flex flex-col h-full bg-white dark:bg-gray-900 border-l border-gray-200 dark:border-gray-700">
      {/* Header */}
      <div className="flex items-center gap-2 p-4 border-b border-gray-200 dark:border-gray-700">
        <FileText className="w-5 h-5 text-gray-700 dark:text-gray-300" />
        <h2 className="font-semibold text-gray-900 dark:text-gray-100">
          Documents
        </h2>
      </div>

      {/* Content */}
      <div className="flex-1 overflow-y-auto p-4 space-y-6">
        <DocumentUploader />
        <DocumentList />
      </div>
    </div>
  );
};
