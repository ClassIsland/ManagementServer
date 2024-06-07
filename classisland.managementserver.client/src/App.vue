<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <v-app>
    <v-layout full-height class="">
      <v-app-bar :elevation="2" density="comfortable">
        <template v-slot:prepend>
          <v-app-bar-nav-icon icon="mdi-menu"/>
        </template>
        <v-app-bar-title>ClassIsland 集控控制台</v-app-bar-title>
      </v-app-bar>
      <v-navigation-drawer>
        <v-list>
          <v-list-item title="Navigation drawer"></v-list-item>
        </v-list>
      </v-navigation-drawer>
      <v-main>
        <RouterView />
      </v-main>
    </v-layout>
  </v-app>
</template>

<style scoped>

</style>
