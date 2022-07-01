<template>
  <div class="home">
    <ChatHub isConnected="false" />
  </div>
</template>

<script lang="ts">
import ChatHub from "@/components/ChatHub.vue";

export default {
  name: "ChatHubView",
  components: {
    ChatHub,
  },
};
</script>
