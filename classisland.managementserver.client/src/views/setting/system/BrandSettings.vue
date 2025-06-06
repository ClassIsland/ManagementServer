<template>
  <n-card class="proCard" title="显示设置">
    <n-spin :show="isLoading">
      <n-grid cols="2 s:2 m:2 l:3 xl:3 2xl:3" responsive="screen">
        <n-grid-item>
          <n-form :label-width="80" :model="formValue" :rules="rules" ref="formRef">
            <n-form-item  path="organizationName" label="组织名称">
              <n-input v-model:value="formValue.organizationName" placeholder="例：XX学校"/>
            </n-form-item>
            <n-form-item path="logoUrl" label="组织 Logo URL">
              <n-input v-model:value="formValue.logoUrl" placeholder="需要静态地址，留空使用默认值"/>
            </n-form-item>
            <n-form-item path="customLoginBanner" label="登录背景图片 URL">
              <n-input v-model:value="formValue.customLoginBanner" placeholder="需要静态地址，留空使用默认值"/>
            </n-form-item>
            <n-form-item path="loginFormPlacement" label="登录界面对齐方式">
              <n-radio-group v-model:value="formValue.loginFormPlacement" name="radiogroup">
                <n-space>
                  <n-radio v-for="i in loginFormPlacementModes" :key="i.value" :value="i.value">
                    {{ i.label }}
                  </n-radio>
                </n-space>
              </n-radio-group>
            </n-form-item>
            <div>
              <n-space>
                <n-button type="primary" @click="formSubmit" :loading="isSaving">更新基本信息</n-button>
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
    organizationName: "",
    logoUrl: "",
    customLoginBanner: "",
    loginFormPlacement: "left"
  });
  

  function formSubmit() {
    formRef.value.validate(async (errors) => {
      if (!errors) {
        try {
          isSaving.value = true;
          await Apis.organizationsettings.post_api_v1_settings_brand({
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
    formValue.value = await Apis.organizationsettings.get_api_v1_settings_brand({});
    isLoading.value = false;
  })
</script>
