<template>
  <div id="content">
    <h3>Liste des usagers connectés</h3>
    <div :class="{ hidden: this.userConnection }" style="margin-top: 25px">
      <p>Veuillez vous connecter pour voir les usagers en ligne!</p>
    </div>
    <div :class="{ hidden: !this.userConnection }">
      <ul style="list-style: none">
        <li
          v-for="(u, index) in this.userList"
          :key="index"
          :class="{
            'conversation-link': true,
            bold: this.interlocutorId === u.userId,
          }"
          @click="this.tryOpenConversation(this.user.userId, u.userId)"
        >
          {{ u.userId }}
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {
  useSignalR,
  HubCommandToken,
  HubEventToken,
  SignalRService,
} from "@quangdao/vue-signalr";
import { User, UserData, ConversationData } from "@/models/User";
import { mapMutations, mapState } from "vuex";

let signalr: SignalRService;

interface IdsSearch {
  userId: string;
  otherUserId: string;
}

const GetAllUsers: HubCommandToken<null, User[]> = "GetAllUsers";
const GetConversationId: HubCommandToken<IdsSearch, string> =
  "GetConversationId";
const OnConnect: HubEventToken<null> = "UserConnected";
const OnDisconnect: HubEventToken<null> = "UserDisconnected";

export default defineComponent({
  name: "UserListComponent",
  data() {
    return {
      userConnection: false,
      usernameText: "",
      userList: [] as User[],
    };
  },
  setup() {
    signalr = useSignalR();
  },
  mounted() {
    signalr.on(OnConnect, () => {
      this.loadUsers();
    });
    signalr.on(OnDisconnect, () => {
      this.loadUsers();
    });
  },
  methods: {
    ...mapMutations({
      setActiveConversationId: "setActiveConversationId",
      setInterlocutorId: "setInterlocutorId",
      setUserCount: "setUserCount",
    }),
    loadUsers() {
      if (!this.user) {
        return;
      } else {
        signalr.invoke(GetAllUsers).then((response: User[]) => {
          this.userList = response.filter((_) => _.userId != this.user.userId);
          this.setUserCount(this.userList.length);
        });
      }
    },
    tryOpenConversation(currentUserId: string, interlocutorId: string) {
      let ids: IdsSearch = {
        userId: currentUserId,
        otherUserId: interlocutorId,
      };
      signalr.invoke(GetConversationId, ids).then((conversationId) => {
        this.setActiveConversationId(conversationId);
        this.setInterlocutorId(interlocutorId);
      });
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
    user() {
      if (this.user) {
        this.userConnection = this.user !== null;
        this.loadUsers();
      }
    },
  },
});
</script>

<style scoped>
.hidden {
  display: none;
}
.conversation-link:hover {
  font-weight: bold;
  cursor: pointer;
}
.conversation-link {
  display: block;
}
</style>
