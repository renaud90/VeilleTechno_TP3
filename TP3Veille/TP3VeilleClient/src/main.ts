import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import { VueSignalR } from "@quangdao/vue-signalr";

createApp(App)
  .use(VueSignalR, {
    url: "https://tp3veilleserveur.azurewebsites.net/chat" /*"https://localhost:7242/chat"*/ /*'http://localhost:5062'*/,
  })
  .use(store)
  .use(router)
  .mount("#app");
