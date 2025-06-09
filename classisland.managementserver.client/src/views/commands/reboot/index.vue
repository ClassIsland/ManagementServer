<script setup lang="ts">
import {ref} from "vue";
import {ObjectsAssignee} from "@/api/globals";
import {useMessage} from "naive-ui";

const targets = ref<Array<ObjectsAssignee>>([]);
const isSaving = ref(false);
const message = useMessage();
const formRef = ref();

async function submit() {
  isSaving.value = true;
  try {
    await Apis.clientcommanddeliver.post_api_v1_client_commands_restart_app({
      data: {
        targets: targets.value
      }
    });
    message.success("发送成功");
  } finally {
    isSaving.value = false;
  }
}
</script>

<template>
  <n-card title="重启实例">
    <p class="mb-4">此操作将重启目标 ClassIsland 实例。</p>
    <n-form style="max-width: 700px" class="mx-auto"
            label-placement="left" label-width="auto" ref="formRef">
      <n-form-item label="目标">
        <AssigneeList v-model:value="targets"/>
      </n-form-item>
      
      <n-form-item :show-label="false">
        <n-button type="primary" round @click="submit" :loading="isSaving">发送</n-button>
      </n-form-item>
    </n-form>
  </n-card>
</template>

<style lang="less">

</style>
