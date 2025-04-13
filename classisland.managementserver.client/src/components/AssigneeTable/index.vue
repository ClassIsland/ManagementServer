<script setup lang="ts">
import { defineProps, ref, reactive, h } from 'vue';
import {BasicTable, TableAction} from "@/components/Table";
import {columns} from "./columns";
import { DeleteOutlined, EditOutlined } from '@vicons/antd';
import { useDialog, useMessage } from 'naive-ui';


const isEditDrawerActive = ref(false);
const assigneeFormData = ref({
  objectId: '',
  objectType: 0,
  targetClientId: null,
  targetClientCuid: null,
  targetGroupId: null,
  assigneeType: 2
});
const isSaving = ref(false);
const actionRef = ref();
const message = useMessage();


const assigneeTargets = [
  {
    label: '按 CUID',
    value: 1
  },
  {
    label: '按客户端 ID',
    value: 2
  },
  {
    label: '按客户端组',
    value: 3
  }
]

const props = defineProps({
  objectId: String,
  objectType: Number,
});

async function saveEntry(e: MouseEvent) {
  
}

function createActions(record) {
  return [
    {
      label: '删除',
      // 配置 color 会覆盖 type
      icon: DeleteOutlined,
      onClick: handleDelete.bind(null, record),
      // 根据权限控制是否显示: 有权限，会显示，支持多个
      auth: [],
    }
  ];
}

const loadDataTable = async (res) => {
  return Apis.assignees.get_api_v1_assignees_by_object_objecttype_id({
    pathParams: {
      id: props.objectId,
      objectType: props.objectType
    },
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
  
}

function handleEdit(record) {
  
}

function handleAdd() {
  assigneeFormData.value = {
    objectId: props.objectId ?? "",
    objectType: props.objectType ?? 0,
    targetClientId: null,
    targetClientCuid: null,
    targetGroupId: null,
    assigneeType: 2
  };
  isEditDrawerActive.value = true;
}

async function saveAssignee(){
  isSaving.value = true;
  await Apis.assignees.post_api_v1_assignees_all(
    {
      data: assigneeFormData.value
    }
  );
  isSaving.value = false;
  isEditDrawerActive.value = false;
  actionRef.value.reload();
  message.success("保存成功");
}

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
</script>

<template>
  <BasicTable
    title="分配"
    titleTooltip="管理当前对象分配给的客户端。"
    :columns="columns"
    :request="loadDataTable"
    :row-key="(row) => row.id"
    ref="actionRef"
    :actionColumn="actionColumn"
    @update:checked-row-keys="onCheckedRow">
    <template v-slot:toolbar>
      <n-button type="primary" @click="handleAdd">添加分配</n-button>
    </template>
  </BasicTable>
  <n-drawer v-model:show="isEditDrawerActive" :width="400" placement="right">
    <n-drawer-content title="编辑分配">
      <n-form label-placement="top" :model="assigneeFormData">
        <n-form-item label="目标类型" path="assigneeType">
          <n-select :items="assigneeTargets" v-model:value="assigneeFormData.assigneeType"/>
        </n-form-item>
        <n-form-item label="客户端 ID" :v-show="assigneeFormData.assigneeType == 2">
          <n-input v-model:value="assigneeFormData.targetClientId"/>
        </n-form-item>
        <n-form-item :show-label="false">
          <n-button type="primary" :loading="isSaving"
                    @click="saveAssignee">保存</n-button>
        </n-form-item>
      </n-form>
    </n-drawer-content>
  </n-drawer>
</template>

<style scoped lang="less">

</style>
