<script setup lang="ts">
import { ArchiveOutline as ArchiveIcon } from '@vicons/ionicons5'
import { defineComponent, ref } from 'vue'
import type { UploadFileInfo, UploadInst, UploadCustomRequestOptions } from 'naive-ui'
import { useMessage } from 'naive-ui'
import {Alova} from "@/utils/http/alova";
import {on} from "@/utils/domUtils";

const message = useMessage();
const override = ref(false);
const upload = ref<UploadInst | null>(null);
const isLoading = ref(false);
const groupId = ref("00000000-0000-0000-0000-000000000001");

const customRequest = async ({
                         file,
                         data,
                         headers,
                         withCredentials,
                         action,
                         onFinish,
                         onError,
                         onProgress
                       }: UploadCustomRequestOptions) => {
  isLoading.value = true;
  onProgress({ percent: 10 });
  try{
    await Alova.Post('/api/v1/profiles/upload', await file.file?.text(), {
      headers: {
        'Content-Type': 'application/json',
      },
      params: {
        replace: override.value,
        groupId: groupId.value
      }
    });
  } catch (e) {
    onError();
    return;
  }
  onProgress({ percent: 100 });
  onFinish()
  message.success('上传成功');
  isLoading.value = false;
  
};

function beforeUpload(data: {
  file: UploadFileInfo
  fileList: UploadFileInfo[]
}) {
  if (data.file.file?.type !== 'application/json') {
    message.error('只能上传 Json 档案文件')
    return false
  }
  
  return true
}

async function handleSubmit(){
  upload.value?.submit();
}

function handleUpdate(fileList: UploadFileInfo[]){
  console.log(fileList);
}

function getGroupData(pageIndex: number, pageSize: number) {
  return Apis.profilegroups.get_api_v1_profiles_groups({
    params: { pageIndex, pageSize }
  })
}

</script>

<template>
  <n-card :bordered="false" title="上传档案">
    将本地的 ClassIsland 档案导入到集控服务器中。
  </n-card>
  <n-card :bordered="false" class="proCard mt-4" content-class="d-flex flex-col ga-3 items-start">
    <n-upload
      multiple
      ref="upload"
      directory-dnd
      :default-upload="false"
      :max="1"
      :custom-request="customRequest"
      accept=".json"
      action="/api/v1/profiles/upload"
      @onsubmit="handleSubmit"
      @on-update="handleUpdate"
      @beforeUpload="beforeUpload"
    >
      <n-upload-dragger>
        <div style="margin-bottom: 12px">
          <n-icon size="48" :depth="3">
            <ArchiveIcon />
          </n-icon>
        </div>
        <n-text style="font-size: 16px">
          点击或者拖动文件到该区域来上传
        </n-text>
        <n-p depth="3" style="margin: 8px 0 0 0">
          支持上传 ClassIsland 的档案文件
        </n-p>
      </n-upload-dragger>
    </n-upload>
    <n-form-item label="分组" feedback="此文件中的对象将被分配到的对象组。">
      <PagedSelect
        style="min-width: 250px"
        v-model:value="groupId"
        labelField="name"
        valueField="id"
        :get-data="getGroupData"
      />
    </n-form-item>
    <n-checkbox v-model:checked="override">
      覆盖存在的项目
    </n-checkbox>
    <div>
      <n-button @click="handleSubmit" :loading="isLoading" type="primary" class="">
        上传
      </n-button>
    </div>
  </n-card>
</template>

<style scoped lang="less">

</style>
