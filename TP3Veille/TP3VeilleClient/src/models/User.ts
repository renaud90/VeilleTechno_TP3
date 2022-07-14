export interface UserData {
  userId: string | null;
  conversationData: ConversationData[] | null;
}

export interface ConversationData {
  conversationId: string | null;
  interlocutorId: string | null;
}

export interface User {
  userId: string | null;
  lastTimeConnected: Date | null;
}

export interface Message {
  content: string;
  userId: string;
  conversationId: string;
  moment: Date;
}
