<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="时间表"
      titleTooltip="管理档案时间表。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd" v-if="hasPermission(['ObjectsWrite'])">添加时间表</n-button>
      </template>
    </BasicTable>
  </n-card>


  <basicModal @register="modalCreate" class="basicModal" @on-ok="okCreateTimeLayout">
    <template #default>
      <n-form v-model="createTimeLayoutForm">
        <n-form-item label="名称" path="name">
          <n-input v-model:value="createTimeLayoutForm.name" />
        </n-form-item>
        <n-form-item label="分组" path="groupId">
          <PagedSelect
            v-model:value="createTimeLayoutForm.groupId"
            labelField="name"
            valueField="id"
            :get-data="getGroupData"
          />
        </n-form-item>
      </n-form>
    </template>
  </basicModal>
</template>

<script lang="ts" setup>
import {columns} from './columns';
import { reactive, ref, h } from 'vue';
import { BasicTable, TableAction } from '@/components/Table';
import { getTableList } from '@/api/table/list';
import { useDialog, useMessage } from 'naive-ui';
import { DeleteOutlined, EditOutlined } from '@vicons/antd';
import {Subject} from "@/api/globals";
import { Guid } from 'guid-typescript';
import {useRouter} from 'vue-router';
import {usePermission} from "@/hooks/web/usePermission";
import {useModal} from "@/components/Modal";

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const isEditingDrawerVisible = ref(false);
const isSaving = ref(false);
const isAdding = ref(false);
const router = useRouter();
const createTimeLayoutForm = ref();

const [modalCreate, modalCreateActions] = useModal({
  title: '添加时间表',
});
const openCreateModal = modalCreateActions.openModal;
const closeCreateModal = modalCreateActions.closeModal;
const setCreateSubLoading = modalCreateActions.setSubLoading;

const { hasPermission } = usePermission();

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
      auth: ["ObjectsDelete"],
    },
    {
      label: '编辑',
      icon: EditOutlined,
      onClick: handleEdit.bind(null, record),
      auth: ["ObjectsWrite"],
    },
  ];
}

const loadDataTable = async (res) => {
  return Apis.timelayouts.get_api_v1_profiles_timelayouts({
    params: {
      pageSize: res.pageSize,
      pageIndex: res.pageIndex
    }
  });
};

function onCheckedRow(rowKeys) {
  console.log(rowKeys);
}

function getGroupData(pageIndex: number, pageSize: number) {
  return Apis.profilegroups.get_api_v1_profiles_groups({
    params: { pageIndex, pageSize }
  })
}

function handleDelete(record) {
  console.log(record);
  dialog.info({
    title: '提示',
    content: `您想删除时间表 ${record.name} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.timelayouts.delete_api_v1_profiles_timelayouts_id({
        pathParams: {
          id: record.id
        }
      });
      message.success('删除成功');
      actionRef.value.reload();
    },
    onNegativeClick: () => {},
  });
}

function handleEdit(record) {
  router.push(`/profiles/time_layouts/${record.id}`)
}

function handleAdd() {
  createTimeLayoutForm.value = {
    name: "新时间表",
    groupId: "00000000-0000-0000-0000-000000000001"
  };
  openCreateModal();
}

async function okCreateTimeLayout() {
  try {
    await Apis.timelayouts.put_api_v1_profiles_timelayouts({
      data: createTimeLayoutForm.value
    });
    closeCreateModal();
    message.success("创建成功");
    actionRef.value?.reload();
  } finally {
    setCreateSubLoading(false);
  }
}
</script>

<style lang="less" scoped></style>
