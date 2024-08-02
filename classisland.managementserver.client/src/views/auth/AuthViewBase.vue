<script setup lang="ts">

</script>

<template>
  <v-main>
    <div class="d-flex authView_workspace_container">
      <v-sheet class="authView_workspace" elevation="4">
        <RouterView />
      </v-sheet>
    </div>
  </v-main>
</template>

<style scoped>
.authView_workspace {
  align-self: center;
  padding: 12px;
}

.authView_workspace_container {
  align-items: center;
  justify-content: center;
  height: 100%;
}
</style>