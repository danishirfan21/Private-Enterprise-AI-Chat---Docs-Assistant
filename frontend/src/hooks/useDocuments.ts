import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { documentsApi } from '../api/documents';
import { useDocumentStore } from '../store/documentStore';

export const useDocuments = () => {
  const queryClient = useQueryClient();
  const { setDocuments, addDocument, removeDocument, setUploading } = useDocumentStore();

  // Query to list documents
  const { data: documents, isLoading } = useQuery({
    queryKey: ['documents'],
    queryFn: async () => {
      const docs = await documentsApi.list();
      setDocuments(docs);
      return docs;
    },
  });

  // Mutation to upload document
  const uploadMutation = useMutation({
    mutationFn: async (file: File) => {
      setUploading(true);
      return await documentsApi.upload(file);
    },
    onSuccess: (document) => {
      addDocument(document);
      queryClient.invalidateQueries({ queryKey: ['documents'] });
      setUploading(false);
    },
    onError: (error) => {
      console.error('Upload error:', error);
      setUploading(false);
    },
  });

  // Mutation to delete document
  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await documentsApi.delete(id);
      return id;
    },
    onSuccess: (id) => {
      removeDocument(id);
      queryClient.invalidateQueries({ queryKey: ['documents'] });
    },
    onError: (error) => {
      console.error('Delete error:', error);
    },
  });

  return {
    documents,
    isLoading,
    upload: uploadMutation.mutate,
    deleteDocument: deleteMutation.mutate,
    isUploading: uploadMutation.isPending,
    isDeleting: deleteMutation.isPending,
  };
};
