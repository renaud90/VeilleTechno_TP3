import { createStore } from "vuex";
import User from "@/models/User";

export type State = {
  user: User | null;
};

export default createStore({
  state: {
    user: null,
  },
  getters: {},
  mutations: {
    connect(state: State, user: User) {
      state.user = user;
    },
  },
  actions: {},
  modules: {},
});
