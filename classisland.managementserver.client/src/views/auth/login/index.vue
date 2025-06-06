<template>
  <div class="view-account-form">
    <n-alert type="warning" class="mb-2 mobile-warning">
      您正在使用移动设备使用本应用。为获得更好的体验，建议使用桌面设备使用本应用。
    </n-alert>
    <div class="auth-header">登录</div>
    <div class="view-account-top-desc">
      登录以继续到<b>{{ brand.organizationName }}</b>。
    </div>
    <n-form
      ref="formRef"
      label-placement="left"
      size="medium"
      :model="formInline"
      :rules="rules"
    >
      <!-- TODO: 适配 SSO -->
      <n-form-item path="username">
        <n-input v-model:value="formInline.username" id="username" placeholder="用户名" >
          <template #prefix>
            <n-icon size="18" color="#808695">
              <PersonOutline />
            </n-icon>
          </template>
        </n-input>
      </n-form-item>
      <n-form-item path="password">
        <n-input
          v-model:value="formInline.password"
          type="password"
          showPasswordOn="click"
          id="password"
          :onkeydown="onKeyDown"
          placeholder="密码"
        >
          <template #prefix>
            <n-icon size="18" color="#808695">
              <LockClosedOutline />
            </n-icon>
          </template>
        </n-input>
      </n-form-item>
      <n-form-item :show-feedback="false">
        <div class="flex flex-col view-account-actions">
          <n-button type="primary" attr-type="submit" @click="handleSubmit" size="large" :loading="loading" block>
            登录
          </n-button>
          <div class="flex view-account-other">
          </div>
        </div>
      </n-form-item>
    </n-form>
  </div>
</template>

<script lang="ts" setup>
  import { reactive, ref, onMounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { useUserStore } from '@/store/modules/user';
  import { useMessage } from 'naive-ui';
  import { PersonOutline, LockClosedOutline, LogoGithub, LogoFacebook } from '@vicons/ionicons5';
  import { PageEnum } from '@/enums/pageEnum';
  import { websiteConfig } from '@/config/website.config';
  import createClient from "openapi-fetch";
  import Apis from "@/api/index";
  import {ACCESS_TOKEN} from "@/store/mutation-types";
  import {store} from "@/store";
  import {useBrand} from "@/store/modules/brand";

  interface FormState {
    username: string;
    password: string;
  }

  const formRef = ref();
  const message = useMessage();
  const loading = ref(false);
  const autoLogin = ref(true);
  const LOGIN_NAME = PageEnum.BASE_LOGIN_NAME;
  const brand = useBrand();

  const formInline = reactive({
    username: '',
    password: '',
    isCaptcha: true,
  });

  const rules = {
    username: { required: true, message: '请输入电子邮箱', trigger: 'blur' },
    password: { required: true, message: '请输入密码', trigger: 'blur' },
  };

  const userStore = useUserStore();

  const router = useRouter();
  const route = useRoute();

  const handleSubmit = (e) => {
    e.preventDefault();
    formRef.value.validate(async (errors) => {
      if (!errors) {
        const { username, password } = formInline;
        message.loading('登录中...');
        loading.value = true;

        const params: FormState = {
          username,
          password,
        };

        try {
          await userStore.login(params);
          message.destroyAll();
          message.success('登录成功，即将进入系统');
          await redirect();
        }
        finally {
          loading.value = false;
        }
      } else {
        message.error('请填写完整信息，并且进行验证码校验');
      }
    });
  };
  
  function onKeyDown(e) {
    if (e.code == "Enter"){
      handleSubmit(e);
    }
  }
  
  async function redirect() {
    if (userStore.info.redirectToOobe) {
      await router.push('/get_started/wizard');
      return;
    }
    const toPath = decodeURIComponent((route.query?.redirect || '/') as string);
    if (route.name === LOGIN_NAME) {
      await router.push('/');
    } else await router.push(toPath);
  }
  
  onMounted(() => {
    if (userStore.getIsLoggedIn) {
      redirect();
    }
  });
</script>

<style lang="less" scoped>

  .mobile-warning {
    display: none;  

    @media (max-width: 640px) {
      display: flex;
    }
  }
  .view-account {
    &-top-desc{
      margin-bottom: 12px;
    }
    
    &-other {
      width: 100%;
    }
    
    &-actions{
      width: 100%;
      row-gap: 8px;
    }
  }

</style>
