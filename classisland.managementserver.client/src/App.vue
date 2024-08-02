<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <v-app>
    <v-layout full-height class="">
      <RouterView/>
    </v-layout>
  </v-app>
</template>

<style scoped>

</style>
