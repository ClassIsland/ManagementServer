<script setup lang="ts">
import { ref } from 'vue'

const isSiderBarOpen = ref(true)

</script>

<template>
  <v-app-bar :elevation="2" density="compact">
    <template v-slot:prepend>
      <v-app-bar-nav-icon icon="mdi-menu" @click="() => {
        isSiderBarOpen = !isSiderBarOpen;
      }"/>
    </template>
    <v-app-bar-title>ClassIsland 集控控制台</v-app-bar-title>
    <template v-slot:append>
      <v-avatar>
        
      </v-avatar>
    </template>
  </v-app-bar>
  <v-NavigationDrawer v-model="isSiderBarOpen" class="d-flex flex-column">
    <div>
      <v-list-item link title="首页" prepend-icon="mdi-home" to="/home"/>
      <v-divider/>
      <v-list-item link title="客户端" prepend-icon="mdi-monitor" to="/clients"/>
      <v-list-item link title="分组" prepend-icon="mdi-select-group" to="/groups"/>
      <v-divider/>
      <v-list-item link title="课表" prepend-icon="mdi-file-chart-outline"/>
      <v-list-item link title="时间表" prepend-icon="mdi-timetable"/>
      <v-list-item link title="科目" prepend-icon="mdi-home"/>
      <v-divider/>
      <v-list-item link title="策略" prepend-icon="mdi-script-text-outline"/>
      <v-list-item link title="默认配置" prepend-icon="mdi-book-outline"/>
      <v-divider/>
      <v-list-item link title="设置" prepend-icon="mdi-cog-outline"/>
    </div>
  </v-NavigationDrawer>
  <v-main>
    <RouterView class="expanded"/>
    
  </v-main>
</template>

<style scoped>
.expanded {
  height: 100%;
}
</style>