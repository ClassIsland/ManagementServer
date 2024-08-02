<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import createClient from "openapi-fetch";
import type {paths} from "@/schema";

const route = useRoute()

const client = createClient<paths>({ baseUrl: "/" });
const { data } = await client.GET("/api/v1/clients_registry/query/{cuid}", {
  params: {
    path: { cuid: route.params.cuid!.toString() },
  },
});
const detailsTab = ref(0)

</script>

<template>
  <div class="ma-4">
    <v-sheet elevation="4" class="pa-4 gap-y-3">

      <span class="text-h6">{{ data?.['id'] }}</span>
      <p>
        <span class="font-weight-bold">CUID: </span>
        <span>{{ data?.['cuid'] }}</span>
      </p>
      <div class="d-flex ga-2 flex-row">
        <v-btn variant="text" color="primary" prepend-icon="mdi-refresh">刷新客户端</v-btn>
        <v-btn variant="text" color="primary" prepend-icon="mdi-link-off">解除绑定</v-btn>
        <v-btn variant="text" color="primary" prepend-icon="mdi-message-alert-outline">发送提醒</v-btn>
      </div>
    </v-sheet>

    <v-tabs class="mt-4" color="primary" v-model="detailsTab">
      <v-tab :value="0">详细信息</v-tab>
      <v-tab :value="1">已分配</v-tab>
      <v-tab :value="2">结果</v-tab>
      <v-tab :value="3">实例</v-tab>
    </v-tabs>
    <v-tabs-window v-model="detailsTab">
      <v-tabs-window-item
          :value="0"
      >
        <p>
          <span class="font-weight-bold">CUID: </span>
          <span>{{ data?.['cuid'] }}</span>
        </p>
        <p>
          <span class="font-weight-bold">首次注册: </span>
          <span>{{ data?.['registerTime'] }}</span>
        </p>
      </v-tabs-window-item>
    </v-tabs-window>
  </div>
</template>

<style scoped>

</style>