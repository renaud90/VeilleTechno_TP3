<template>
  <div class="home">
    <ChatHub isConnected="false">
      <template v-slot:entete>
        <EnteteComponent />
      </template>
      <template v-slot:conversation>
        <ConversationComponent userId="test1234" conversationId="test1234" />
      </template>
      <template v-slot:userProfile>
        <UserProfileComponent />
      </template>
    </ChatHub>
  </div>
</template>

<script lang="ts">
import ChatHub from "@/components/ChatHub.vue";
import EnteteComponent from "@/components/Entete.vue";
import ConversationComponent from "@/components/Conversation.vue";
import UserProfileComponent from "@/components/UserProfil.vue";

export default {
  name: "ChatHubView",
  components: {
    ChatHub,
    EnteteComponent,
    ConversationComponent,
    UserProfileComponent,
  },
};
</script>
