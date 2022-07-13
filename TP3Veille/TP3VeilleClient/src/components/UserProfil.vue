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
    <div :class="{ hidden: !this.userConnection }">
      <h3 style="margin: 25px; auto;">
        Bienvenue {{ this.user ? this.user.userId : "" }}
      </h3>
      <h5 style="margin: 25px; auto;">Aucun autre usagé connecté.</h5>
      <h6 style="margin: 25px; auto;">
        Vous avez échangé 0 messages sur notre plateforme!
      </h6>
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
interface ConnectionResult {
  isSuccess: boolean;
  value: User;
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
          console.log(this.user);
          this.userConnection = true;
          console.log(this.userConnection);
          window.addEventListener("beforeunload", this.disconnectUser);
        });
    },
    disconnectUser: function (event: BeforeUnloadEvent) {
      signalr.invoke(Disconnect, this.user.userId).then(() => {
        this.userConnection = false;
      });
      this.disconnect(this.user.userId);
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
