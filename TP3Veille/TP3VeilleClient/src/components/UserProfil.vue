<template>
  <div id="content">
    <div
      :style="{
        display: this.userConnected ? 'display:none' : 'display:visible',
      }"
      id="user-connection"
    >
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
    <div
      :style="{
        display: this.userConnected ? 'display:visible' : 'display:none',
      }"
    >
      USAGER CONNECTÉ!
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
import { mapMutations } from "vuex";

let signalr: SignalRService;
interface ConnectionResult {
  isSuccess: boolean;
  user: User;
}

const Connect: HubCommandToken<string, ConnectionResult> = "Connect";

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
    ...mapMutations({ connect: "connect" }),
    tryConnect() {
      signalr
        .invoke(Connect, this.usernameText)
        .then((reponse: ConnectionResult) => {
          console.log(`Résultat: ${reponse.isSuccess} Je suis connecté!`);
          console.log(reponse.user.username);
        });
    },
  },
});
</script>

<style scoped></style>
