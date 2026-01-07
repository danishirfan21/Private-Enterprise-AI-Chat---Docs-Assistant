export interface Message {
  id: string;
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: Date;
  sources?: SourceAttribution[];
}

export interface SourceAttribution {
  documentId: string;
  documentName: string;
  similarity: number;
}

export interface Conversation {
  id: string;
  title: string;
  createdAt: Date;
  updatedAt: Date;
  messageCount: number;
  messages?: Message[];
}

export interface Document {
  id: string;
  fileName: string;
  mimeType: string;
  fileSizeBytes: number;
  uploadedAt: Date;
  chunkCount: number;
  status: 'Pending' | 'Processing' | 'Processed' | 'Failed';
}

export interface ModelInfo {
  name: string;
  size: string;
  modifiedAt?: Date;
}

export interface ChatRequest {
  conversationId?: string;
  message: string;
  modelName: string;
  temperature: number;
  maxTokens: number;
  useRAG: boolean;
  systemPrompt?: string;
}

export type SSEEvent =
  | { type: 'start'; conversationId: string }
  | { type: 'token'; content: string }
  | { type: 'context'; sources: SourceAttribution[] }
  | { type: 'done' }
  | { type: 'error'; error: string };
