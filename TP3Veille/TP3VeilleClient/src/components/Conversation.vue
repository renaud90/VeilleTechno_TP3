<template>
  <div style="height: 100%">
    <div style="height: 25px; text-align: left; margin: 5px 0px 0px 5px">
      <img v-if="interlocutorId" class="user-image" src="../assets/person-icon.png" alt="" />
      {{ interlocutorId }}
    </div>
    <div class="container column-reverse" id="conv-window">
      <div id="div-conv-input">
        <input id="conv-input" type="text" v-model="textInput" v-on:keyup.enter="sendMessage"
          :disabled="!user || !activeConversationId" />
      </div>
      <div :class="m.class" class="message" v-for="(m, index) in messages" :key="index">
        <img class="user-image" v-if="m.class === 'message-received'" :src="m.image ?? '../assets/person-icon.png'"
          alt="" />{{ m.message.content }}
      </div>
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
import { Message } from "../models/User";

interface ChatMessage {
  userId: string;
  conversationId: string;
  content: string;
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

interface MessagesResult {
  isSuccess: boolean;
  value: Message[];
}

const SendMessage: HubCommandToken<ChatMessage> = "SendMessage";
const JoinGroup: HubCommandToken<ConversationInfo, MessagesResult> =
  "JoinConversation";
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
        content: this.textInput,
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
    ...mapState({
      user: "user",
      activeConversationId: "activeConversationId",
      interlocutorId: "interlocutorId",
    }),
  },
  watch: {
    activeConversationId() {
      let conversationInfo: ConversationInfo = {
        userId: this.user.userId,
        conversationId: this.activeConversationId,
      };
      signalr.invoke(JoinGroup, conversationInfo).then((messages) => {
        this.messages = [];
        if (messages.value) {
          messages.value.reverse();

          messages.value.forEach((m) => {
            let chatMessage: ChatMessage = {
              userId: m.userId,
              conversationId: m.conversationId,
              content: m.content,
              date: m.moment,
            };
            let messageView: ChatMessageView = {
              message: chatMessage,
              class:
                this.user.userId === m.userId
                  ? "message-sent"
                  : "message-received",
              image: null,
            };
            this.messages.push(messageView);
          });
        }
      });
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

#conv-window {
  height: calc(100% - 30px);
}

#conv-input {
  margin: auto;
  width: 80%;
  border: none;
  height: 95%;
}
</style>
