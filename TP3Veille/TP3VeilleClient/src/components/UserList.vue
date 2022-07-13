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
          :class="{ 'conversation-link': true }"
          @click="this.openConversation('test1234')"
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
  SignalRService,
} from "@quangdao/vue-signalr";
import User from "@/models/User";
import { mapMutations, mapState } from "vuex";

let signalr: SignalRService;

const GetAllUsers: HubCommandToken<null, User[]> = "GetAllUsers";

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
    setInterval(this.loadUsers, 7000);
  },
  methods: {
    ...mapMutations({
      openConversation: "openConversation",
      setUserCount: "setUserCount",
    }),
    loadUsers() {
      if (!this.user) {
        return;
      } else {
        console.log("chargement des usagers...");
        signalr.invoke(GetAllUsers).then((response: User[]) => {
          this.userList = response.filter((_) => _.userId != this.user.userId);
          this.setUserCount(this.userList.length);
        });
      }
    },
  },
  computed: {
    ...mapState({ user: "user", activeConversationId: "activeConversationId" }),
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
