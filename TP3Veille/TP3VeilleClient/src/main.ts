import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import { VueSignalR } from "@quangdao/vue-signalr";

createApp(App)
  .use(VueSignalR, {
    url: "https://localhost:7242" /*'http://localhost:5062'*/,
  })
  .use(store)
  .use(router)
  .mount("#app");
