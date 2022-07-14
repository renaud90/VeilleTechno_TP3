<template>
  <div class="home">
    <ChatHub isConnected="false">
      <template v-slot:entete>
        <EnteteComponent />
      </template>
      <template v-slot:conversation>
        <ConversationComponent />
      </template>
      <template v-slot:userProfile>
        <UserProfileComponent />
      </template>
      <template v-slot:userList>
        <UserListComponent />
      </template>
    </ChatHub>
  </div>
</template>

<script lang="ts">
import ChatHub from "@/components/ChatHub.vue";
import EnteteComponent from "@/components/Entete.vue";
import ConversationComponent from "@/components/Conversation.vue";
import UserProfileComponent from "@/components/UserProfil.vue";
import UserListComponent from "@/components/UserList.vue";

export default {
  name: "ChatHubView",
  components: {
    ChatHub,
    EnteteComponent,
    ConversationComponent,
    UserProfileComponent,
    UserListComponent,
  },
};
</script>
