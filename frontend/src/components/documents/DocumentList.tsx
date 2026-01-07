import React from 'react';
import { useDocuments } from '../../hooks/useDocuments';
import { DocumentItem } from './DocumentItem';
import { Loader2, Inbox } from 'lucide-react';

export const DocumentList: React.FC = () => {
  const { documents, isLoading, deleteDocument, isDeleting } = useDocuments();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 animate-spin text-gray-400" />
      </div>
    );
  }

  if (!documents || documents.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-gray-500 dark:text-gray-400">
        <Inbox className="w-12 h-12 mb-3 opacity-50" />
        <p className="text-sm">No documents uploaded yet</p>
      </div>
    );
  }

  return (
    <div className="space-y-2">
      {documents.map((document) => (
        <DocumentItem
          key={document.id}
          document={document}
          onDelete={deleteDocument}
          isDeleting={isDeleting}
        />
      ))}
    </div>
  );
};
