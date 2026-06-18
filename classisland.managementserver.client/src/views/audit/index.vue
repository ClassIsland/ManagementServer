<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { NCard, NDataTable, NTag } from 'naive-ui';
import { listAuditLogs, type AuditLog } from '@/api/audit/logs';

const logs = ref<AuditLog[]>([]);
const loading = ref(false);

const eventNameMap: Record<number, string> = {
  0: '默认事件',
  1: '授权成功',
  2: '授权失败',
  4: '设置更新',
  5: '换课完成',
  6: '课表更新',
  7: '时间表更新',
  8: '科目更新',
  9: '应用崩溃',
  10: '应用启动',
  11: '应用退出',
  12: '插件安装',
  13: '插件卸载',
};

const eventTypeMap: Record<number, 'default' | 'success' | 'error' | 'warning' | 'info'> = {
  0: 'default',
  1: 'success',
  2: 'error',
  4: 'info',
  5: 'info',
  6: 'info',
  7: 'info',
  8: 'info',
  9: 'error',
  10: 'success',
  11: 'warning',
  12: 'info',
  13: 'warning',
};

const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '客户端', key: 'clientCuid', width: 260, ellipsis: { tooltip: true } },
  {
    title: '事件类型',
    key: 'event',
    width: 120,
    render(row: any) {
      const name = eventNameMap[row.event] || `未知(${row.event})`;
      const type = eventTypeMap[row.event] || 'default';
      return h(NTag, { type, size: 'small' }, { default: () => name });
    },
  },
  {
    title: '事件时间',
    key: 'eventTimeUtc',
    width: 180,
    render(row: any) { return new Date(row.eventTimeUtc).toLocaleString('zh-CN'); },
  },
  {
    title: '接收时间',
    key: 'createdTime',
    width: 180,
    render(row: any) { return new Date(row.createdTime).toLocaleString('zh-CN'); },
  },
];

async function loadLogs() {
  loading.value = true;
  try {
    const res = await listAuditLogs({ pageIndex: 1, pageSize: 100 });
    logs.value = res.items || [];
  } catch (e) {
    console.error('加载审计日志失败', e);
  } finally {
    loading.value = false;
  }
}

onMounted(loadLogs);
</script>

<template>
  <n-card title="审计日志">
    <n-data-table :columns="columns" :data="logs" :loading="loading" :bordered="false" size="small" />
  </n-card>
</template>

<script lang="ts">
import { h } from 'vue';
export default { name: 'AuditLogs' };
</script>
