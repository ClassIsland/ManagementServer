<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
import { NCard, NSelect, NDataTable, NButton, NSpace, NTag, NPopconfirm, NUpload, NEmpty, NSpin, useMessage } from 'naive-ui'
import type { UploadFileInfo } from 'naive-ui'
import { Alova } from '@/utils/http/alova'

const message = useMessage()
const selectedClient = ref<string>('')
const clientOptions = ref<{ label: string; value: string }[]>([])
const plugins = ref<any[]>([])
const isLoading = ref(false)
const isUploading = ref(false)

const statusMap: Record<string, { label: string; type: string }> = {
  Loaded: { label: '已加载', type: 'success' },
  NotLoaded: { label: '未加载', type: 'warning' },
  Disabled: { label: '已禁用', type: 'default' },
  Error: { label: '错误', type: 'error' },
}

const columns = [
  { title: '插件ID', key: 'pluginId', ellipsis: { tooltip: true }, width: 200 },
  { title: '名称', key: 'pluginName', width: 150 },
  { title: '版本', key: 'version', width: 80 },
  { title: '作者', key: 'author', width: 100 },
  {
    title: '状态', key: 'loadStatus', width: 80,
    render(row: any) {
      const s = statusMap[row.loadStatus] || { label: row.loadStatus, type: 'default' }
      return h(NTag, { type: s.type as any, size: 'small' }, { default: () => s.label })
    }
  },
  {
    title: '启用', key: 'isEnabled', width: 70,
    render(row: any) {
      return h(NTag, { type: row.isEnabled ? 'success' : 'default', size: 'small' }, { default: () => row.isEnabled ? '是' : '否' })
    }
  },
  {
    title: '操作', key: 'actions', width: 220,
    render(row: any) {
      return h(NSpace, { size: 'small' }, {
        default: () => [
          h(NButton, { size: 'tiny', onClick: () => togglePlugin(row.pluginId, !row.isEnabled) }, { default: () => row.isEnabled ? '禁用' : '启用' }),
          h(NPopconfirm, { onPositiveClick: () => uninstallPlugin(row.pluginId) }, {
            trigger: () => h(NButton, { size: 'tiny', type: 'error' }, { default: () => '卸载' }),
            default: () => `确定卸载插件 ${row.pluginName || row.pluginId}？`
          })
        ]
      })
    }
  }
]

async function loadClients() {
  try {
    const res: any = await Alova.Get('/api/v1/clients_registry/all', { params: { pageIndex: 1, pageSize: 100 } })
    clientOptions.value = (res.items || []).map((c: any) => ({
      label: `${c.id || c.cuid}`,
      value: c.cuid
    }))
  } catch (e) {
    console.error('Failed to load clients', e)
  }
}

async function loadPlugins() {
  if (!selectedClient.value) return
  isLoading.value = true
  try {
    const res: any = await Alova.Get(`/api/v1/clients/${selectedClient.value}/plugins`)
    plugins.value = res || []
  } catch (e: any) {
    plugins.value = []
    message.error('加载插件列表失败: ' + (e.message || e))
  } finally {
    isLoading.value = false
  }
}

async function requestUpload() {
  if (!selectedClient.value) return
  try {
    await Alova.Post(`/api/v1/clients/${selectedClient.value}/plugins/request-upload`)
    message.success('已请求客户端上报插件列表')
  } catch (e: any) {
    message.error('请求失败: ' + (e.message || e))
  }
}

async function togglePlugin(pluginId: string, enable: boolean) {
  if (!selectedClient.value) return
  try {
    await Alova.Post(`/api/v1/clients/${selectedClient.value}/plugins/${pluginId}/toggle?enable=${enable}`)
    message.success(enable ? '已启用' : '已禁用')
    await loadPlugins()
  } catch (e: any) {
    message.error('操作失败: ' + (e.message || e))
  }
}

async function uninstallPlugin(pluginId: string) {
  if (!selectedClient.value) return
  try {
    await Alova.Post(`/api/v1/clients/${selectedClient.value}/plugins/${pluginId}/uninstall`)
    message.success('已发送卸载指令')
    await loadPlugins()
  } catch (e: any) {
    message.error('卸载失败: ' + (e.message || e))
  }
}

async function handleUpload(options: { file: UploadFileInfo }) {
  if (!selectedClient.value) return
  isUploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', options.file.file as File)
    await Alova.Post(`/api/v1/clients/${selectedClient.value}/plugins/install`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    message.success('插件包已上传')
    await loadPlugins()
  } catch (e: any) {
    message.error('上传失败: ' + (e.message || e))
  } finally {
    isUploading.value = false
  }
}

onMounted(loadClients)
</script>

<template>
  <div class="p-4">
    <n-card title="插件管理">
      <n-space vertical :size="16">
        <n-space align="center">
          <span>选择客户端：</span>
          <n-select v-model:value="selectedClient" :options="clientOptions" filterable
            placeholder="选择客户端实例" style="width: 300px" @update:value="loadPlugins" />
          <n-button @click="loadPlugins" :disabled="!selectedClient">刷新</n-button>
          <n-button @click="requestUpload" :disabled="!selectedClient" type="info">请求客户端上报</n-button>
        </n-space>

        <n-data-table v-if="selectedClient" :columns="columns" :data="plugins" :loading="isLoading" :bordered="false" />

        <n-upload v-if="selectedClient" :max="1" :custom-request="handleUpload" accept=".cipx" :show-file-list="false">
          <n-button :loading="isUploading" type="primary">上传 .cipx 插件包</n-button>
        </n-upload>

        <n-empty v-if="!selectedClient" description="请先选择一个客户端实例" />
      </n-space>
    </n-card>
  </div>
</template>
