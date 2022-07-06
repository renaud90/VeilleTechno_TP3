<template>
  <div class="body">
    <div class="entete">
      <slot name="entete"></slot>
    </div>
    <div class="conversation">
      <slot name="conversation"></slot>
    </div>
    <div class="profil">PROFIL</div>
    <div class="personnes">PERSONNES CONNECTÉES</div>
    <div class="groupes">GROUPES</div>
  </div>
</template>

<script lang="ts">
import { useSignalR, HubCommandToken } from "@quangdao/vue-signalr";

interface ConnectionPayload {
  isSuccess: boolean;
}

const Connect: HubCommandToken<string, ConnectionPayload> = "Connect";

export default {
  name: "ChatHub",
  props: {
    isConnected: Boolean,
  },
  setup() {
    const signalr = useSignalR();
    signalr
      .invoke(Connect, "test1234")
      .then(({ isSuccess }) =>
        console.log(`Résultat: ${isSuccess} Je suis connecté!`)
      );
  },
  created() {
    console.log("ALLO");
  },
};
</script>

<style lang="scss">
html,
body,
.body {
  min-height: 85vh;
}
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
.container {
  display: flex;
  height: 100%;
}
.column-reverse {
  flex-direction: column-reverse;
}
.body {
  display: grid;
  grid-template-rows: 12% 12% 12% 12% 12% 12% 12% 12%;
  gap: 20px;
}
.entete {
  grid-column-start: 1;
  grid-column-end: 4;
  grid-row-start: 1;
  grid-row-end: 2;
}
.conversation {
  grid-column-start: 1;
  grid-column-end: 4;
  grid-row-start: 2;
  grid-row-end: 9;
  border: 2px solid lightgray;
  border-radius: 5px;
}
.profil {
  grid-column-start: 4;
  grid-column-end: 5;
  grid-row-start: 1;
  grid-row-end: 3;
  border: 2px solid yellow;
}
.personnes {
  grid-column-start: 4;
  grid-column-end: 5;
  grid-row-start: 3;
  grid-row-end: 6;
  border: 2px solid purple;
}
.groupes {
  grid-column-start: 4;
  grid-column-end: 5;
  grid-row-start: 6;
  grid-row-end: 9;
  border: 2px solid orange;
}
</style>
