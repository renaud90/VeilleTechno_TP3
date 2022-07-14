import { createStore } from "vuex";
import { User, UserData } from "../models/User"

export type State = {
  user: UserData | null;
  activeConversationId: string | null;
  interlocutorId: string | null;
  userCount: number | null;
};

export default createStore({
  state: {
    user: null,
    activeConversationId: "",
    interlocutorId: "",
    userCount: null,
  },
  getters: {},
  mutations: {
    connect(state: State, user: UserData) {
      state.user = user;
    },
    disconnect(state: State) {
      state.user = null;
    },
    setActiveConversationId(state: State, activeConversationId: string) {
      state.activeConversationId = activeConversationId;
    },
    setInterlocutorId(state: State, interlocutorId: string) {
      state.interlocutorId = interlocutorId;
    },
    setUserCount(state: State, userCount: number) {
      state.userCount = userCount;
    },
  },
  actions: {},
  modules: {},
});
