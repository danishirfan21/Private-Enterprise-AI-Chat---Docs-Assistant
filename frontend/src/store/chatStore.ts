import { create } from 'zustand';
import type { Message } from '../types';

interface ChatState {
  conversationId: string | null;
  messages: Message[];
  isStreaming: boolean;
  streamingMessageId: string | null;

  setConversationId: (id: string | null) => void;
  addMessage: (message: Message) => void;
  updateStreamingMessage: (content: string) => void;
  startStreaming: (messageId: string) => void;
  stopStreaming: () => void;
  clearChat: () => void;
  setMessages: (messages: Message[]) => void;
}

export const useChatStore = create<ChatState>((set) => ({
  conversationId: null,
  messages: [],
  isStreaming: false,
  streamingMessageId: null,

  setConversationId: (id) => set({ conversationId: id }),

  addMessage: (message) =>
    set((state) => ({
      messages: [...state.messages, message],
    })),

  updateStreamingMessage: (content) =>
    set((state) => ({
      messages: state.messages.map((msg) =>
        msg.id === state.streamingMessageId
          ? { ...msg, content }
          : msg
      ),
    })),

  startStreaming: (messageId) =>
    set({
      isStreaming: true,
      streamingMessageId: messageId,
    }),

  stopStreaming: () =>
    set({
      isStreaming: false,
      streamingMessageId: null,
    }),

  clearChat: () =>
    set({
      messages: [],
      isStreaming: false,
      streamingMessageId: null,
    }),

  setMessages: (messages) => set({ messages }),
}));
