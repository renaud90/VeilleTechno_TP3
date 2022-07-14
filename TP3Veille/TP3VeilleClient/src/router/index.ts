import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import ChatHubView from "../views/ChatHub.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    component: ChatHubView,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
