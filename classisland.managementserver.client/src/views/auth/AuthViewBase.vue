<script setup lang="ts">

import {websiteConfig} from "@/config/website.config";
import {useBrand} from "@/store/modules/brand";
import {computed, ref} from "vue";
import {useRouter} from 'vue-router';

const brand = useBrand();

const router = useRouter();
const bannerUrl = computed(() => {
  return "url(\"" + (brand.customLoginBanner ?? "") + "\")";
});
const bannerUrlRef = ref(bannerUrl);
const isDefaultBackground = ref(brand.customLoginBanner == null || brand.customLoginBanner === "");
const loginFormPlacement = ref<"left" | "center" | "right">(brand.loginFormPlacement ?? "left");
const overrideDefaultAuthContainer = ref(router.currentRoute.value?.meta?.overrideDefaultAuthContainer === true);
const contentWidth = ref(`${router.currentRoute.value?.meta?.width ?? 384}px`);

</script>

<template>
  <div class="view-account" :class="{ 
    'default-background': isDefaultBackground,
     }">
    <n-card class="view-account-container" :class="{
      left: loginFormPlacement === 'left',
      center: loginFormPlacement === 'center',
      right: loginFormPlacement === 'right',
    }" >
      <template #header v-if="!overrideDefaultAuthContainer">
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
    margin-top: 0;
    flex-grow: 0;
    align-self: center;
    max-width: v-bind(contentWidth);
    min-width: v-bind(contentWidth);
    @media (max-width: 640px) {
      border: none !important;
      align-self: stretch;
      max-width: 100vw;
      min-width: 100vw;
      margin: 0 !important;
      border-radius: 0;
    }

    &.left {
      margin-right: auto;
      margin-left: 100px;
    }

    &.center {
      margin-right: auto;
      margin-left: auto;
    }

    &.right {
      margin-right: 100px;
      margin-left: auto;
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
    background-image: linear-gradient(#00000040), v-bind(bannerUrlRef);
    background-repeat: no-repeat;
    background-position: 50%;
    background-size: cover;
    background-clip: border-box;
    
    &.default-background {
      background-image: linear-gradient(#00000040), url('../../assets/Banner-Web-24.png'), linear-gradient(135deg, #041515, #111, #111) !important;
    }
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
