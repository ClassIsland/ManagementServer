<script setup lang="ts">
import {ref} from 'vue';
import {ObjectsAssignee} from "@/api/globals";
import {useMessage} from "naive-ui";

const formRef = ref(null);
const targets = ref<Array<ObjectsAssignee>>([]);
const payload = ref({
  "messageMask": "",
  "messageContent": "",
  "overlayIconLeft": 0,
  "overlayIconRight": 0,
  "isEmergency": false,
  "isSpeechEnabled": true,
  "isEffectEnabled": true,
  "isSoundEnabled": true,
  "isTopmost": true,
  "durationSeconds": 15,
  "repeatCounts": 1
})
const isSaving = ref(false);
const message = useMessage();

const rules = {
  messageMask: {
    required: true,
    message: "请输入提醒遮罩内容",
    trigger: ["blur"]
  },
  messageContent: {
    required: true,
    message: "请输入提醒正文",
    trigger: ["blur"]
  }
}

async function submit() {
  formRef.value?.validate(async errors => {
    if (!errors) {
      isSaving.value = true;
      try {
        await Apis.clientcommanddeliver.post_api_v1_client_commands_show_notification({
          data: {
            targets: targets.value,
            payload: payload.value
          }
        });
        message.success("发送成功");
      } finally {
        isSaving.value = false;
      }
    } else {
      message.error("请完整填写必填字段");
    }
  })
}

</script>

<template>
  <n-card title="广播提醒">
    <n-alert type="warning" class="mb-4">
      警告！此功能不保证完全可靠，请不要仅依靠此功能传播及其重要的信息。ClassIsland 开发者不对因使用此功能造成的任何损失负责。
    </n-alert>
    <n-form style="max-width: 700px" class="mx-auto" :model="payload"
            label-placement="left" label-width="auto" ref="formRef" :rules="rules">
      <n-form-item label="目标">
        <AssigneeList v-model:value="targets"/>
      </n-form-item>
      <n-form-item label="遮罩内容" path="messageMask">
        <n-input v-model:value="payload.messageMask"/>
      </n-form-item>
      <n-form-item label="正文内容" path="messageContent">
        <n-input type="textarea" v-model:value="payload.messageContent"/>
      </n-form-item>
      <n-form-item label="持续时间">
        <n-input-number v-model:value="payload.durationSeconds">
          <template #suffix>
            秒
          </template>
        </n-input-number> 
      </n-form-item>
      <n-form-item label="重复次数">
        <n-input-number v-model:value="payload.repeatCounts" :min="1"/>
      </n-form-item>

      <n-form-item :show-label="false">
        <n-checkbox v-model:checked="payload.isSpeechEnabled">启用语音</n-checkbox>
      </n-form-item>
      <n-form-item :show-label="false">
        <n-checkbox v-model:checked="payload.isSoundEnabled">启用提醒音效</n-checkbox>
      </n-form-item>
      <n-form-item :show-label="false">
        <n-checkbox v-model:checked="payload.isEffectEnabled">启用提醒特效</n-checkbox>
      </n-form-item>
      <n-form-item :show-label="false">
        <n-checkbox v-model:checked="payload.isTopmost">提醒时置顶主界面</n-checkbox>
      </n-form-item>
      <n-form-item :show-label="false">
        <n-button type="primary" round @click="submit" :loading="isSaving">发送</n-button> 
      </n-form-item>
    </n-form>
  </n-card>
</template>

<style scoped lang="less">

</style>
