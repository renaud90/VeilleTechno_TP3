<template>
  <div class="body">
    <div class="entete">
      <slot name="entete"></slot>
    </div>
    <div :class="{ conversation: true }">
      <slot name="conversation"></slot>
    </div>
    <div class="profil">
      <slot name="userProfile"></slot>
    </div>
    <div :class="{ personnes: true, scrollable: user !== null }">
      <slot name="userList"></slot>
    </div>
  </div>
</template>

<script lang="ts">
import { useSignalR, HubCommandToken } from "@quangdao/vue-signalr";
import { mapState } from "vuex";
interface ConnectionResult {
  isSuccess: boolean;
}

const Connect: HubCommandToken<string, ConnectionResult> = "Connect";

export default {
  name: "ChatHub",
  props: {
    isConnected: Boolean,
  },
  data() {
    return {
      userConnection: false,
    };
  },
  setup() {
    const signalr = useSignalR();
  },
  computed: {
    ...mapState({ user: "user" }),
  },
};
</script>

<style lang="scss">
html,
body,
.body {
  height: 85vh;
  font-size: 20px;
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
  overflow: scroll;
}
.profil {
  grid-column-start: 4;
  grid-column-end: 5;
  grid-row-start: 1;
  grid-row-end: 3;
  border: 2px solid lightgray;
}
.personnes {
  grid-column-start: 4;
  grid-column-end: 5;
  grid-row-start: 3;
  grid-row-end: 9;
  border: 2px solid lightgray;
}
.scrollable {
  overflow: scroll;
}
.bold {
  font-weight: bold;
}
</style>
