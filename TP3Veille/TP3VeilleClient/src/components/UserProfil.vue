<template>
  <div id="content">
    <div :class="{ hidden: this.userConnection }" id="user-connection">
      <h3>Nom d'utilisateur:</h3>
      <input
        id="username-input"
        type="text"
        v-model="usernameText"
        maxlength="35"
        minlength="6"
        v-on:keyup.enter="tryConnect"
      />
    </div>
    <div :class="{ hidden: !this.userConnection }">USAGER CONNECTÉ!</div>
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
interface ConnectionResult {
  isSuccess: boolean;
  value: User;
}
interface DisconnectionResult {
  isSuccess: boolean;
}

const Connect: HubCommandToken<string, ConnectionResult> = "Connect";
const Disconnect: HubCommandToken<string, ConnectionResult> = "Disconnect";

export default defineComponent({
  name: "UserProfileComponent",
  data() {
    return {
      userConnection: false,
      usernameText: "",
    };
  },
  setup() {
    signalr = useSignalR();
  },
  methods: {
    ...mapMutations({ connect: "connect", disconnect: "disconnect" }),
    tryConnect() {
      signalr
        .invoke(Connect, this.usernameText)
        .then((response: ConnectionResult) => {
          this.connect(response.value);
          console.log(this.user);
          document.addEventListener("beforeunload", this.disconnectUser);
        });
    },
    disconnectUser: function (event: BeforeUnloadEvent) {
      this.disconnect(this.user.userId);
      console.log("CACA");
    },
  },
  computed: {
    ...mapState({ user: "user" }),
  },
});
</script>

<style scoped>
.hidden {
  display: none;
}
</style>
