import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { conversationsApi } from '../api/conversations';
import { useChatStore } from '../store/chatStore';

export const useConversations = () => {
  const queryClient = useQueryClient();
  const { setConversationId, setMessages, clearChat } = useChatStore();

  // Query to list conversations
  const { data: conversations, isLoading } = useQuery({
    queryKey: ['conversations'],
    queryFn: () => conversationsApi.list(),
  });

  // Query to get conversation messages
  const useConversationMessages = (conversationId: string | null) => {
    return useQuery({
      queryKey: ['conversation', conversationId],
      queryFn: async () => {
        if (!conversationId) return null;
        const conversation = await conversationsApi.get(conversationId);
        setMessages(conversation.messages || []);
        return conversation;
      },
      enabled: !!conversationId,
    });
  };

  // Mutation to create conversation
  const createMutation = useMutation({
    mutationFn: async (title?: string) => {
      return await conversationsApi.create(title);
    },
    onSuccess: (conversation) => {
      setConversationId(conversation.id);
      clearChat();
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
    },
    onError: (error) => {
      console.error('Create conversation error:', error);
    },
  });

  // Mutation to delete conversation
  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await conversationsApi.delete(id);
      return id;
    },
    onSuccess: (id) => {
      // If we deleted the current conversation, clear it
      if (useChatStore.getState().conversationId === id) {
        setConversationId(null);
        clearChat();
      }
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
    },
    onError: (error) => {
      console.error('Delete conversation error:', error);
    },
  });

  return {
    conversations,
    isLoading,
    useConversationMessages,
    createConversation: createMutation.mutate,
    deleteConversation: deleteMutation.mutate,
    isCreating: createMutation.isPending,
    isDeleting: deleteMutation.isPending,
  };
};
