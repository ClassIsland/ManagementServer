<template>
  <n-grid cols="2 s:2 m:2 l:3 xl:3 2xl:3" responsive="screen">
    <n-grid-item>
      <n-h3>修改密码</n-h3>
      <n-form :label-width="80" :model="formValue" :rules="rules" ref="formRef">
        <n-form-item label="旧密码" path="oldPassword">
          <n-input v-model:value="formValue.oldPassword" placeholder="请输入旧密码" type="password" show-password-toggle/>
        </n-form-item>
    
        <n-form-item label="新密码" path="newPassword">
          <n-input placeholder="请输入新密码" v-model:value="formValue.newPassword" type="password" show-password-toggle/>
        </n-form-item>
    
        <n-form-item label="确认密码" path="confirmNewPassword">
          <n-input placeholder="请再输入一遍新密码" v-model:value="formValue.confirmNewPassword" type="password" show-password-toggle/>
        </n-form-item>
    
        <div>
          <n-space>
            <n-button type="primary" @click="formSubmit" :loading="isLoading">修改密码</n-button>
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
  oldPassword: {
    required: true,
    message: '请输入密码',
    trigger: 'blur',
  },
  newPassword: {
    required: true,
    message: '请输入密码',
    trigger: 'blur',
  },
  confirmNewPassword: {
    required: true,
    message: '请输入密码',
    trigger: 'blur',
  }
};
const formRef: any = ref(null);
const message = useMessage();
const isLoading = ref(false);
const users = useUser();

const formValue = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
});

function formSubmit() {
  
  formRef.value.validate(async (errors) => {
    if (!errors) {
      try {
        isLoading.value = true;
        if (formValue.value.newPassword !== formValue.value.confirmNewPassword) {
          message.error('两次输入的新密码不一致');
          return;
        }
        await Apis.users.post_api_v1_users_change_password({
          data: {
            oldPassword: formValue.value.oldPassword,
            newPassword: formValue.value.newPassword
          }
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
</script>
