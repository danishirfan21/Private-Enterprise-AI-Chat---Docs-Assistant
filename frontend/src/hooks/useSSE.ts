import { useCallback } from 'react';
import { streamChat } from '../api/chat';
import { useChatStore } from '../store/chatStore';
import { useSettingsStore } from '../store/settingsStore';
import type { Message } from '../types';
import { v4 as uuidv4 } from 'uuid';

export const useSSE = () => {
  const { addMessage, updateStreamingMessage, startStreaming, stopStreaming, conversationId } = useChatStore();
  const { modelName, temperature, maxTokens, useRAG, systemPrompt } = useSettingsStore();

  const sendMessage = useCallback(
    async (content: string) => {
      // Add user message
      const userMessage: Message = {
        id: uuidv4(),
        role: 'user',
        content,
        timestamp: new Date(),
      };
      addMessage(userMessage);

      // Create assistant message placeholder
      const assistantMessageId = uuidv4();
      const assistantMessage: Message = {
        id: assistantMessageId,
        role: 'assistant',
        content: '',
        timestamp: new Date(),
      };
      addMessage(assistantMessage);

      // Start streaming
      startStreaming(assistantMessageId);

      try {
        let fullContent = '';
        let sources: any[] = [];

        // Stream chat response
        for await (const event of streamChat({
          conversationId: conversationId || undefined,
          message: content,
          modelName,
          temperature,
          maxTokens,
          useRAG,
          systemPrompt,
        })) {
          switch (event.type) {
            case 'start':
              // Update conversation ID if it's a new conversation
              if (event.conversationId && !conversationId) {
                useChatStore.getState().setConversationId(event.conversationId);
              }
              break;

            case 'token':
              fullContent += event.content || '';
              updateStreamingMessage(fullContent);
              break;

            case 'context':
              sources = event.sources || [];
              break;

            case 'done':
              // Update final message with sources
              useChatStore.getState().setMessages(
                useChatStore.getState().messages.map((msg) =>
                  msg.id === assistantMessageId
                    ? {
                        ...msg,
                        content: fullContent,
                        sources: sources.length > 0 ? sources : undefined,
                      }
                    : msg
                )
              );
              break;

            case 'error':
              console.error('SSE Error:', event.error);
              updateStreamingMessage(
                fullContent + '\n\n[Error: ' + event.error + ']'
              );
              break;
          }
        }
      } catch (error) {
        console.error('Streaming error:', error);
        updateStreamingMessage(
          useChatStore.getState().messages.find((m) => m.id === assistantMessageId)?.content +
            '\n\n[Error: Failed to complete response]'
        );
      } finally {
        stopStreaming();
      }
    },
    [
      conversationId,
      modelName,
      temperature,
      maxTokens,
      useRAG,
      systemPrompt,
      addMessage,
      updateStreamingMessage,
      startStreaming,
      stopStreaming,
    ]
  );

  return { sendMessage };
};
