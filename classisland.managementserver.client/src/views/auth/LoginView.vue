<script setup lang="ts">
import { ref } from 'vue';
import router from "@/router";


const username = ref('')
const password = ref('')

function login() {
  fetch('/api/v1/identity/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      email: username.value,
      password: password.value,
    })
  }).then(res => {
    if (res.ok) {
      router.push('/')
    } else {
      alert('登录失败')
    }
  })
}

</script>

<template>
  <div style="width: 400px" class="d-flex flex-column">
    <h2>登录</h2>
    <v-form>
      <v-text-field label="用户名" name="username" v-model="username"/>
      <v-text-field label="密码" name="password" type="password" v-model="password"/>
      
      <v-btn color="primary" @click="login">登录</v-btn>
    </v-form>
  </div>
  
</template>

<style scoped>

</style>