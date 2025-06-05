<template>
  <n-grid cols="2 s:2 m:2 l:3 xl:3 2xl:3" responsive="screen">
    <n-grid-item>
      <n-form :label-width="80" :model="formValue" :rules="rules" ref="formRef">
        <n-form-item label="用户名" path="userName">
          <n-input v-model:value="formValue.userName" disabled/>
        </n-form-item>
        
        <n-form-item label="昵称" path="name">
          <n-input v-model:value="formValue.name" placeholder="请输入昵称" />
        </n-form-item>

        <n-form-item label="邮箱" path="email">
          <n-input placeholder="请输入邮箱" v-model:value="formValue.emailAddress" />
        </n-form-item>

        <n-form-item label="联系电话" path="mobile">
          <n-input placeholder="请输入联系电话" v-model:value="formValue.phoneNumber" />
        </n-form-item>

        <div>
          <n-space>
            <n-button type="primary" @click="formSubmit" :loading="isLoading">更新基本信息</n-button>
          </n-space>
        </div>
      </n-form>
    </n-grid-item>
  </n-grid>
</template>

<script lang="ts" setup>
  import { reactive, ref, onMounted } from 'vue';
  import { useMessage } from 'naive-ui';
  import {useUser} from "@/store/modules/user";

  const rules = {
    name: {
      required: true,
      message: '请输入昵称',
      trigger: 'blur',
    }
  };
  const formRef: any = ref(null);
  const message = useMessage();
  const isLoading = ref(false);
  const users = useUser();
  
  const formValue = ref({
    userName: '',
    name: '',
    phoneNumber: '',
    emailAddress: ''
  });

  function formSubmit() {
    formRef.value.validate(async (errors) => {
      if (!errors) {
        try {
          isLoading.value = true;
          await Apis.users.post_api_v1_users_current({
            data: formValue.value
          });
          await users.getInfo();
          message.success('更新成功');
        } finally {
          isLoading.value = false;
        }
      } else {
        message.error('验证失败，请填写完整信息');
      }
    });
  }
  
  onMounted(async () => {
    formValue.value = users.info
  });
</script>
