<template>
  <n-card class="proCard" title="基本设置">
    <n-spin :show="isLoading">
      <n-grid cols="2 s:2 m:2 l:3 xl:3 2xl:3" responsive="screen">
        <n-grid-item>
          <n-form :label-width="80" :model="formValue" :rules="rules" ref="formRef">
            <n-form-item  path="allowUnregisteredClients" :show-label="false">
              <n-checkbox v-model:checked="formValue.allowUnregisteredClients">允许未预留的客户端连接集控</n-checkbox>
            </n-form-item>
            <n-form-item  path="allowPublicRegister" :show-label="false">
              <n-checkbox v-model:checked="formValue.allowPublicRegister">开放用户注册</n-checkbox>
            </n-form-item>
            <n-form-item path="customPublicApiUrl" label="自定义应用 API 基础 URL">
              <n-input v-model:value="formValue.customPublicApiUrl" placeholder="客户端访问本应用的 API 端点的 URL"/>
            </n-form-item>
            <n-form-item path="customPublicGrpcUrl" label="自定义应用 GRPC 基础 URL">
              <n-input v-model:value="formValue.customPublicGrpcUrl" placeholder="客户端访问本应用的 GRPC 端点的 URL"/>
            </n-form-item>
            <n-form-item path="customPublicRootUrl" label="自定义应用基础 URL">
              <n-input v-model:value="formValue.customPublicRootUrl" placeholder="用户访问本应用管理后台的 URL"/>
            </n-form-item>
            <div>
              <n-space>
                <n-button type="primary" @click="formSubmit" :loading="isSaving">保存设置</n-button>
              </n-space>
            </div>
          </n-form>
        </n-grid-item>
      </n-grid>
    </n-spin>
  </n-card>
</template>

<script lang="ts" setup>
  import { reactive, ref, onMounted } from 'vue';
  import { useDialog, useMessage } from 'naive-ui';
  // import "../../../../types/global"

  const rules = {
    organizationName: {
      required: true,
      message: '请输入组织名称',
      trigger: 'blur',
    },
  };
  
  const loginFormPlacementModes = [
    { label: '左对齐', value: 'left' },
    { label: '居中对齐', value: 'center' },
    { label: '右对齐', value: 'right' }
  ]

  const formRef: any = ref(null);
  const message = useMessage();
  const dialog = useDialog();
  const isSaving = ref(false);
  const isLoading = ref(false);

  const formValue = ref({
    "allowUnregisteredClients": true,
    "customPublicRootUrl": "",
    "customPublicApiUrl": "",
    "customPublicGrpcUrl": "",
    "allowPublicRegister": false});
  

  function formSubmit() {
    formRef.value.validate(async (errors) => {
      if (!errors) {
        try {
          isSaving.value = true;
          await Apis.organizationsettings.post_api_v1_settings_basic({
            data: formValue.value
          });
          message.success('保存成功，部分设置将在刷新后生效');
        } finally {
          isSaving.value = false;
        }
      } else {
        message.error('验证失败，请填写完整信息');
      }
    });
  }
  onMounted(async () => {
    isLoading.value = true;
    formValue.value = await Apis.organizationsettings.get_api_v1_settings_basic({});
    isLoading.value = false;
  })
</script>
