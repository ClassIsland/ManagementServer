<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="课表"
      titleTooltip="管理课表。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd"
                  v-if="hasPermission(['ObjectsWrite'])">添加课表</n-button>
      </template>
    </BasicTable>
  </n-card>

  <basicModal @register="modalCreate" class="basicModal" @on-ok="okCreateClassPlan">
    <template #default>
      <n-form :model="createClassPlanForm" :rules="createFormRules" ref="createFormRef">
        <n-form-item label="名称" path="name">
          <n-input v-model:value="createClassPlanForm.name" />
        </n-form-item>
        <n-form-item label="时间表" path="timeLayoutId">
          <PagedSelect
            v-model:value="createClassPlanForm.timeLayoutId"
            labelField="name"
            valueField="id"
            :get-data="getTimeLayoutData"
          />
        </n-form-item>
        <n-form-item label="分组" path="groupId">
          <PagedSelect
            v-model:value="createClassPlanForm.groupId"
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
import {ClassPlan, Subject} from "@/api/globals";
import { Guid } from 'guid-typescript';
import {useAsyncRoute} from "@/store/modules/asyncRoute";
import Router from "@/router";
import { useRouter } from 'vue-router';
import {usePermission} from "@/hooks/web/usePermission";
import {useModal} from "@/components/Modal";

const { hasPermission } = usePermission();
const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<ClassPlan | null>(null);
const isEditingDrawerVisible = ref(false);
const isSaving = ref(false);
const isAdding = ref(false);
const router = useRouter();
const createFormRef = ref();
const createClassPlanForm = ref();

const [modalCreate, modalCreateActions] = useModal({
  title: '添加课表',
});
const openCreateModal = modalCreateActions.openModal;
const closeCreateModal = modalCreateActions.closeModal;
const setCreateSubLoading = modalCreateActions.setSubLoading;

const createFormRules = {
  timeLayoutId: {
    required: true,
    message: '请指定时间表'
  },
  groupId: {
    required: true,
    message: '请指定对象组'
  }
}

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

function getTimeLayoutData(pageIndex: number, pageSize: number) {
  return Apis.timelayouts.get_api_v1_profiles_timelayouts({
    params: { pageIndex, pageSize }
  })
}

function getGroupData(pageIndex: number, pageSize: number) {
  return Apis.profilegroups.get_api_v1_profiles_groups({
    params: { pageIndex, pageSize }
  })
}

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
  return Apis.classplans.get_api_v1_profiles_classplans({
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
    content: `您想删除课表 ${record.name} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.classplans.delete_api_v1_profiles_classplans_id({
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
  router.push(`/profiles/classplans/${record.id}`);
}

function handleAdd() {
  createClassPlanForm.value = {
    name: "新课表",
    groupId: "00000000-0000-0000-0000-000000000001",
    timeLayoutId: ""
  }
  openCreateModal();
}

async function okCreateClassPlan() {
  createFormRef.value.validate(async errors => {
    if (!errors) {
      try {
        await Apis.classplans.put_api_v1_profiles_classplans({
          data: createClassPlanForm.value
        });
        message.success("创建成功");
        closeCreateModal();
        actionRef.value?.reload();
      } finally {
        setCreateSubLoading(false);
      }
    } else {
      setCreateSubLoading(false);
      message.error("请完整填写信息");
    }
  })
}
</script>

<style lang="less" scoped></style>
