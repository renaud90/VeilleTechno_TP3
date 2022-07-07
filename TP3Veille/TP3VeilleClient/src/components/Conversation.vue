<template>
  <div class="container column-reverse">
    <div id="div-conv-input">
      <input
        id="conv-input"
        type="text"
        v-model="textInput"
        v-on:keyup.enter="sendMessage"
      />
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

interface ChatMessage {
  userId: string;
  conversationId: string;
  message: string;
  date: Date;
}

interface GroupJoinOrLeave {
  userId: string;
  groupId: string;
}

const SendMessage: HubCommandToken<ChatMessage> = "SendMessage";
const JoinGroup: HubCommandToken<GroupJoinOrLeave> = "JoinGroup";
const ReceiveMessage: HubEventToken<ChatMessage> = "ReceiveMessage";
let signalr: SignalRService;

export default defineComponent({
  name: "ConversationComponent",
  props: {
    userId: { type: String, required: true },
    conversationId: { type: String, required: true },
  },
  data() {
    return {
      textInput: "",
    };
  },
  setup() {
    signalr = useSignalR();
    signalr.on(ReceiveMessage, (message) => console.log(message));
  },
  created() {
    let conversationJoin: GroupJoinOrLeave = {
      userId: this.userId,
      groupId: this.conversationId,
    };
    signalr
      .invoke(JoinGroup, conversationJoin)
      .then(() => console.log("Groupe joint!"));
  },
  methods: {
    sendMessage() {
      let chatMessage: ChatMessage = {
        userId: this.userId,
        conversationId: this.conversationId,
        message: this.textInput,
        date: new Date(),
      };
      signalr.invoke(SendMessage, chatMessage).then(() => {
        console.log(`Message envoyé!`);
        this.textInput = "";
      });
    },
  },
});
</script>

<style scoped>
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
