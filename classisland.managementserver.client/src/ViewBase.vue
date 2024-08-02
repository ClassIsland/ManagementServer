<script setup lang="ts">

</script>

<template>
  <v-app-bar :elevation="2" density="comfortable">
    <template v-slot:prepend>
      <v-app-bar-nav-icon icon="mdi-menu"/>
    </template>
    <v-app-bar-title>ClassIsland 集控控制台</v-app-bar-title>
  </v-app-bar>
  <v-main>
    <RouterView />
  </v-main>
</template>

<style scoped>

</style>