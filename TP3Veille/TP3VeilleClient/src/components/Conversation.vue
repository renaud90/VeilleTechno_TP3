<template>
  <div class="container column-reverse">
    <div id="div-conv-input">
      <input
        id="conv-input"
        type="text"
        v-model="textInput"
        v-on:keyup.enter="sendMessage"
        :disabled="!this.user || !this.activeConversationId"
      />
    </div>
    <div
      :class="m.class"
      class="message"
      v-for="(m, index) in this.messages"
      :key="index"
    >
      <img
        class="user-image"
        v-if="m.class === 'message-received'"
        :src="m.image ?? require('@/assets/person-icon.png')"
        alt=""
      />{{ m.message.message }}
    </div>
  </div>
</template>

<script lang="ts">
import {
  useSignalR,
  HubCommandToken,
  HubEventToken,
  SignalRService,
} from "@quangdao/vue-signalr";
import { defineComponent } from "vue";
import { mapState } from "vuex";

interface ChatMessage {
  userId: string;
  conversationId: string;
  message: string;
  date: Date;
}

interface ChatMessageView {
  message: ChatMessage;
  class: string;
  image: string | null;
}

interface ConversationInfo {
  userId: string;
  conversationId: string;
}

const SendMessage: HubCommandToken<ChatMessage> = "SendMessage";
const JoinGroup: HubCommandToken<ConversationInfo> = "JoinConversation";
const ReceiveMessage: HubEventToken<ChatMessage> = "ReceiveMessage";
let signalr: SignalRService;

export default defineComponent({
  name: "ConversationComponent",
  data() {
    return {
      textInput: "",
      messages: [] as ChatMessageView[],
    };
  },
  setup() {
    signalr = useSignalR();
  },
  created() {
    signalr.on(ReceiveMessage, (message) => {
      let el: ChatMessageView = {
        message: message,
        class: "message-received",
        image: null,
      };
      this.messages.splice(0, 0, el);
    });
  },
  methods: {
    sendMessage() {
      if (!this.textInput) {
        return;
      }
      let chatMessage: ChatMessage = {
        userId: this.user.userId,
        conversationId: this.activeConversationId,
        message: this.textInput,
        date: new Date(),
      };
      signalr.send(SendMessage, chatMessage);
      let el: ChatMessageView = {
        message: chatMessage,
        class: "message-sent",
        image: null,
      };
      this.messages.splice(0, 0, el);
      this.textInput = "";
    },
  },
  computed: {
    ...mapState({ user: "user", activeConversationId: "activeConversationId" }),
  },
  watch: {
    activeConversationId() {
      let conversationInfo: ConversationInfo = {
        userId: this.user.userId,
        conversationId: this.activeConversationId,
      };
      signalr.invoke(JoinGroup, conversationInfo);
    },
  },
});
</script>

<style scoped>
.message {
  border-radius: 10px;
  padding: 6px;
  margin: 5px;
}
.message-sent {
  background-color: #5c7bdb;
  align-self: flex-end;
  margin-right: 15px;
}
.message-received {
  background-color: lightgray;
  align-self: flex-start;
  margin-left: 15px;
}
.user-image {
  display: inline-block;
  width: 20px;
  height: 20px;
  padding-right: 5px;
}
#div-conv-input {
  width: 100%-20px;
  padding: 5px;
  border: 2px solid lightgrey;
  border-radius: 20px;
  margin: 5px;
  height: 50px;
}
#conv-input {
  margin: auto;
  width: 80%;
  border: none;
  height: 95%;
}
</style>
