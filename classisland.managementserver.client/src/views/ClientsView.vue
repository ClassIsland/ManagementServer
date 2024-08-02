<script setup lang="ts">
import type {paths} from "@/schema";
import createClient from "openapi-fetch";

const client = createClient<paths>({ baseUrl: "/" });
const { data } = await client.GET("/api/v1/clients_registry/list")
console.log(data);

</script>

<template>
  <div class="d-flex flex-1 ">
    <div class="split-part ma-4">
      <h2>客户端</h2>
      <v-list>
        <v-list-item v-for="i in data"
                     :key="i['cuid'].toString()"
                     :title="i['id'].toString()"
                     link :to="'/clients/' + i['cuid'].toString()"/>
      </v-list>
    </div>
    <RouterView class="flex-1 split-part"/>
  </div>
</template>

<style scoped>
.split-part {
  flex: 1;
  justify-content: stretch;
  align-items: stretch;
}
</style>