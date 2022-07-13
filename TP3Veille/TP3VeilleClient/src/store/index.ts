import { createStore } from "vuex";
import User from "@/models/User";

export type State = {
  user: User | null;
  activeConversationId: string;
};

export default createStore({
  state: {
    user: null,
    activeConversationId: "",
  },
  getters: {},
  mutations: {
    connect(state: State, user: User) {
      state.user = user;
    },
    disconnect(state: State) {
      state.user = null;
    },
    openConversation(state: State, activeConversationId: string) {
      state.activeConversationId = activeConversationId;
    },
  },
  actions: {},
  modules: {},
});
