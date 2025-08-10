<script setup lang="ts">
import {useRouter} from 'vue-router';

const router = useRouter();

function goHome() {
  router.push("/")
}
</script>

<template>
  <div class="text-center">
    <img src="../../assets/stickers/米沙_导游.png" height="80" width="80"
         alt="Logo" class="mx-auto mb-4"/>
    <h2 class="mb-2">大功告成</h2>
    <p class="">已完成对集控服务器的基本配置，现在就开始使用集控服务器吧！</p>
    <n-button class="mx-auto mt-8 center" type="primary" @click="goHome">完成</n-button>
  </div>
    
</template>

<style scoped lang="less">
h2 {
  font-size: 24px;
  font-weight: bold;
}
</style>
