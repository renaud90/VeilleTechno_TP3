import { createStore } from "vuex";
import User from "@/models/User";

export type State = {
  user: User | null;
  activeConversationId: string;
  userCount: number | null;
};

export default createStore({
  state: {
    user: null,
    activeConversationId: "",
    userCount: null,
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
    setUserCount(state: State, userCount: number) {
      state.userCount = userCount;
    },
  },
  actions: {},
  modules: {},
});
