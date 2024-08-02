import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import ViewBase from "@/ViewBase.vue";
import AuthViewBase from "@/views/auth/AuthViewBase.vue";
import NotFoundView from "@/views/NotFoundView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'base',
      component: ViewBase,
      children: [
        {
          path: 'home',
          name: 'home',
          component: HomeView
        },
        {
          path: 'clients',
          name: 'clients',
          component: () => import('../views/ClientsView.vue')
        },
        {
          path: 'about',
          name: 'about',
          // route level code-splitting
          // this generates a separate chunk (About.[hash].js) for this route
          // which is lazy-loaded when the route is visited.
          component: () => import('../views/AboutView.vue')
        }
      ]
    },
    {
      path: '/auth',
      name: 'authBase',
      component: AuthViewBase,
      children:[
        {
          path: 'login',
          name: 'login',
          component: () => import('../views/auth/LoginView.vue')
        }
      ]
    },
    // Not Found
    { path: '/:pathMatch(.*)', component: NotFoundView },
      
  ]
})

export default router
