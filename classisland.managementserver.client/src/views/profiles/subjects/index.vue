<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="科目"
      titleTooltip="管理已连接到集控的客户端。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow"
    />
  </n-card>
</template>

<script lang="ts" setup>
import {columns} from './columns';
import { reactive, ref, h } from 'vue';
import { BasicTable, TableAction } from '@/components/Table';
import { getTableList } from '@/api/table/list';
import { useDialog, useMessage } from 'naive-ui';
import { DeleteOutlined, EditOutlined } from '@vicons/antd';

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();

const params = reactive({
  pageSize: 5,
  name: 'NaiveAdmin',
});

const actionColumn = reactive({
  width: 180,
  title: '操作',
  key: 'action',
  fixed: 'right',
  align: 'center',
  render(record) {
    return h(TableAction as any, {
      style: 'button',
      actions: createActions(record),
    });
  },
});

function createActions(record) {
  return [
    {
      label: '删除',
      // 配置 color 会覆盖 type
      icon: DeleteOutlined,
      onClick: handleDelete.bind(null, record),
      // 根据权限控制是否显示: 有权限，会显示，支持多个
      auth: [],
    },
    {
      label: '编辑',
      icon: EditOutlined,
      onClick: handleEdit.bind(null, record),
      auth: [],
    },
  ];
}

const loadDataTable = async (res) => {
  return Apis.subjects.get_api_v1_profiles_subjects({
    params: {
      pageSize: res.pageSize,
      pageIndex: res.pageIndex
    }
  });
};

function onCheckedRow(rowKeys) {
  console.log(rowKeys);
}

function handleDelete(record) {
  console.log(record);
  dialog.info({
    title: '提示',
    content: `您想删除${record.name}`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      const response = Apis.subjects.delete_api_v1_profiles_subjects_id({
        pathParams: {
          id: record.id
        }
      })
      message.success('删除成功');
    },
    onNegativeClick: () => {},
  });
}

function handleEdit(record) {
  console.log(record);
  message.success('您点击了编辑按钮');
}

function addClient() {
  window.open("/api/v1/clients_registry/client_configure")
}
</script>

<style lang="less" scoped></style>
