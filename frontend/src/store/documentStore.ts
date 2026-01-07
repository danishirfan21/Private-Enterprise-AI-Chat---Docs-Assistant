import { create } from 'zustand';
import type { Document } from '../types';

interface DocumentState {
  documents: Document[];
  isUploading: boolean;
  uploadProgress: number;

  setDocuments: (documents: Document[]) => void;
  addDocument: (document: Document) => void;
  removeDocument: (id: string) => void;
  setUploading: (isUploading: boolean) => void;
  setUploadProgress: (progress: number) => void;
}

export const useDocumentStore = create<DocumentState>((set) => ({
  documents: [],
  isUploading: false,
  uploadProgress: 0,

  setDocuments: (documents) => set({ documents }),

  addDocument: (document) =>
    set((state) => ({
      documents: [...state.documents, document],
    })),

  removeDocument: (id) =>
    set((state) => ({
      documents: state.documents.filter((doc) => doc.id !== id),
    })),

  setUploading: (isUploading) => set({ isUploading }),

  setUploadProgress: (progress) => set({ uploadProgress: progress }),
}));
