<script setup lang="ts">
import {ref, onMounted} from 'vue';
import {useRouter} from 'vue-router';
import {useMessage} from 'naive-ui';

const formValue = ref({
  organizationName: "",
  apiBaseUrl: "",
  grpcBaseUrl: ""
});
const formRef = ref(null);
const isLoading = ref(false);
const isSaving = ref(false);
const rawBrandSettings = ref({});
const rawBasicSettings = ref({});
const message = useMessage();
const router = useRouter();

const rules = {
  organizationName: {
    required: true,
      trigger: ['blur'],
      message: '请输入组织名称'
  }
}

async function handleOk() {
  formRef.value.validate(async (errors) => {
    if (!errors) {
      try {
        isSaving.value = true;
        rawBrandSettings.value.organizationName = formValue.value.organizationName;
        rawBasicSettings.value.customPublicApiUrl = formValue.value.apiBaseUrl;
        rawBasicSettings.value.customPublicGrpcUrl = formValue.value.grpcBaseUrl;
        await Apis.organizationsettings.post_api_v1_settings_brand({
          data: rawBrandSettings.value
        });
        await Apis.organizationsettings.post_api_v1_settings_basic({
          data: rawBasicSettings.value
        });
        await Apis.organizationsettings.post_api_v1_settings_complete_oobe();
        message.success('保存成功');
        await router.push('./completed');
      } finally {
        isSaving.value = false;
      }
    } else {
      message.error('验证失败');
    }
  });
}

onMounted(async () => {
  try {
    isLoading.value = true;
    const brand = await Apis.organizationsettings.get_api_v1_settings_brand();
    const basic = await Apis.organizationsettings.get_api_v1_settings_basic();
    const newValue = {
      organizationName: brand.organizationName,
      apiBaseUrl: basic.customPublicApiUrl,
      grpcBaseUrl: basic.customPublicGrpcUrl
    };
    rawBrandSettings.value = brand;
    rawBasicSettings.value = basic;
    formValue.value = newValue;
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <div>
    
    <div class="text-center">
      <img src="../../assets/icons/logo.svg" height="64" width="64"
           alt="Logo" class="mx-auto mb-4"/>
      <h2 class="mb-2">欢迎使用 ClassIsland 集控服务器</h2>
      <p class="">感谢您选用 ClassIsland 集控服务器！让我们完成对集控服务器的初始设置。</p>
    </div>
    <n-spin :show="isLoading">
      <n-form label-placement="left" class="content mt-6"
              label-width="auto"
              :model="formValue"
              require-mark-placement="right-hanging"
              ref="formRef" :rules="rules">
        <n-form-item label="组织名称" feedback="会在在登陆界面和加入集控后的应用中显示。" path="organizationName">
          <n-input placeholder="例：XX 学校" v-model:value="formValue.organizationName"/>
        </n-form-item>
        <n-divider/>
        <n-form-item label="应用 API 基础 URL" feedback="客户端连接到此集控服务器的 API 接口使用的 URL。">
          <n-input   v-model:value="formValue.apiBaseUrl"/>
        </n-form-item>
        <n-form-item label="应用 GRPC 基础 URL" feedback="客户端连接到此集控服务器的 GRPC 接口使用的 URL。">
          <n-input  v-model:value="formValue.grpcBaseUrl"/>
        </n-form-item>
        <n-form-item :show-label="false">
          <n-button class="mx-auto mt-8 center" type="primary" :loading="isSaving" @click="handleOk">下一步</n-button>
        </n-form-item>
      </n-form>
    </n-spin>
  </div>
</template>

<style scoped lang="less">
h2 {
  font-size: 24px;
  font-weight: bold;
}

.content {
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}
</style>
