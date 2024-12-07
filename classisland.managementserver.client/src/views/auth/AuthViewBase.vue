<script setup lang="ts">

import {websiteConfig} from "@/config/website.config";
</script>

<template>
  <div class="view-account">
    <n-card class="view-account-container">
      <template #header>
        <div class="view-account-header">
          <img :src="websiteConfig.loginImage" alt="" width="36" height="36"
              class="align-middle"/>
          <span class="align-middle h-5">ClassIsland 集控控制台</span>
        </div>
      </template>
      <div class="view-account-main">
        <router-view/>
      </div>
    </n-card>
  </div>
</template>

<style scoped lang="less">
.view-account {
  display: flex;
  flex-direction: row;
  height: 100vh;
  overflow: auto;
  
  &-container {
    flex: 1;
    margin: 0 auto;
    flex-grow: 0;
    align-self: center;
    max-width: 384px;
    min-width: 384px;
    @media (max-width: 640px) {
      border: none !important;
      align-self: stretch;
      max-width: 100vw;
      min-width: 100vw;
      margin: 0;
    }
  }
  &-top {
    padding: 32px 0;
    text-align: center;

    &-desc {
      font-size: 14px;
      color: #808695;
    }
  }

  &-other {
    width: 100%;
  }

  &-header {
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: 500;
  }

  .default-color {
    color: #515a6e;

    .ant-checkbox-wrapper {
      color: #515a6e;
    }
  }
}


@media (min-width: 640px) {
  .view-account {
    background-image: url('../../assets/images/login.svg');
    background-repeat: no-repeat;
    background-position: 50%;
    background-size: 100%;
  }

} 
</style>
<style lang="less">
.auth-header {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 4px;
}

.auth-description {
  font-size: 13px;
}
</style>
