<template>
  <div id="content">
    <div :class="{ hidden: userConnection }" id="user-connection">
      <h3 style="margin-bottom: 25px">Connexion</h3>
      <label style="fint-size: 1.5em">
        Nom d'utilisateur:
        <input id="username-input" type="text" v-model="usernameText" maxlength="35" minlength="6"
          @keyup.enter="tryConnect" />
      </label>
      <div style="margin-top: 1em;">
        <button @click="tryConnect">Connexion</button>
      </div>
    </div>
    <div :class="{ hidden: !userConnection }">
      <h3 style="margin: 35px; auto;">
        Bienvenue {{ user ? user.userId : "" }}
      </h3>
      <h5 style="margin: 35px; auto;">
        {{ userCount }} autre(s) usager(s) connecté(s).
      </h5>
      <button @click="disconnectUser">Déconnexion</button>
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
import { UserData } from "../models/User";
import { mapMutations, mapState } from "vuex";

let signalr: SignalRService;
interface ConnectionResult {
  isSuccess: boolean;
  value: UserData;
}
interface DisconnectionResult {
  isSuccess: boolean;
}

const Connect: HubCommandToken<string, ConnectionResult> = "Connect";
const Disconnect: HubCommandToken<string, DisconnectionResult> = "Disconnect";

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
          window.addEventListener("beforeunload", this.disconnectUser);
        });
    },
    disconnectUser: function (event: BeforeUnloadEvent) {
      if (this.userConnection) {
        signalr.invoke(Disconnect, this.user.userId);
        this.disconnect(this.user.userId);
      }
    },
  },
  computed: {
    ...mapState({ user: "user", userCount: "userCount" }),
  },
  watch: {
    user() {
      this.userConnection = this.user !== null;
    },
  },
});
</script>

<style scoped>
.hidden {
  display: none;
}

button {
  width: 10em;
  margin-left: 1em;
  height: 1.7em;
}
</style>
