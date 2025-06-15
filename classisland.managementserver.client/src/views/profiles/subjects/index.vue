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
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd" v-if="hasPermission(['ObjectsWrite'])">添加科目</n-button>
      </template>
    </BasicTable>
  </n-card>

  <n-drawer v-model:show="isEditingDrawerVisible" :width="500" placement="right">
    <n-drawer-content title="编辑科目">
      <n-tabs>
        <n-tab-pane name="basic" label="基本">
          <n-form v-model="editingFormRef">
            <n-form-item label="科目名称" path="name">
              <n-input v-model:value="editingFormRef.name"/>
            </n-form-item>
            <n-form-item label="科目简称" path="initials">
              <n-input v-model:value="editingFormRef.initials"/>
            </n-form-item>
            <n-form-item :show-label="false" path="isOutDoor">
              <n-checkbox label="该科目是户外课程" v-model:checked="editingFormRef.isOutDoor"/>
            </n-form-item>
            <n-form-item label="分组">
              <PagedSelect
                v-model:value="editingFormRef.groupId"
                labelField="name"
                valueField="id"
                :get-data="getGroupData"
                v-model:shared-states="groupSelectSharedState"
              />
            </n-form-item>
            <n-form-item :show-label="false">
              <n-button type="primary" attr-type="submit" @click="saveEntry" :loading="isSaving">
                保存
              </n-button>
            </n-form-item>
          </n-form>
        </n-tab-pane>

        <n-tab-pane name="assignees" label="分配" :disabled="isAdding">
          <AssigneeTable :object-id="editingFormRef.id"
                         :object-type="3"/>
        </n-tab-pane>
      </n-tabs>
    </n-drawer-content>
  </n-drawer>
</template>

<script lang="ts" setup>
import {columns} from './columns';
import { reactive, ref, h } from 'vue';
import { BasicTable, TableAction } from '@/components/Table';
import { getTableList } from '@/api/table/list';
import { useDialog, useMessage } from 'naive-ui';
import { DeleteOutlined, EditOutlined } from '@vicons/antd';
import {Subject} from "@/api/globals";
import {usePermission} from "@/hooks/web/usePermission";
import { Guid } from 'guid-typescript';
import {IPagedSelectState} from "@/components/PagedSelect/IPagedSelectState";

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<Subject | null>(null);
const isEditingDrawerVisible = ref(false);
const isSaving = ref(false);
const isAdding = ref(false);
const groupSelectSharedState = ref<IPagedSelectState | null>(null);

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

async function saveEntry(e: MouseEvent) {
  try {
    e.preventDefault();
    console.log("saving");
    if (editingFormRef?.value == null){
      return;
    }
    isSaving.value = true;
    if (isAdding.value) {
      await Apis.subjects.put_api_v1_profiles_subjects({
        data: editingFormRef.value
      });
    } else {
      await Apis.subjects.put_api_v1_profiles_subjects_id({
        pathParams: {
          id: editingFormRef.value?.id,
        },
        data: editingFormRef.value
      });
    }
    isEditingDrawerVisible.value = false;
    actionRef.value.reload();
    message.success("保存成功");
  } finally {
    isSaving.value = false;
  }
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

function getGroupData(pageIndex: number, pageSize: number) {
  return Apis.profilegroups.get_api_v1_profiles_groups({
    params: { pageIndex, pageSize }
  })
}

function handleDelete(record) {
  console.log(record);
  dialog.info({
    title: '提示',
    content: `您想删除科目 ${record.name} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.subjects.delete_api_v1_profiles_subjects_id({
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
  console.log(record);
  isAdding.value = false;
  editingFormRef.value = { ... record } as Subject;
  isEditingDrawerVisible.value = true;
}

function handleAdd() {
  editingFormRef.value = {
    id: Guid.create().toString(),
    groupId: "00000000-0000-0000-0000-000000000001",
    name: '',
    initials: '',
    isOutDoor: false,
    attachedObjects: {}
  } as Subject;
  isEditingDrawerVisible.value = true;
  isAdding.value = true;
  
}
</script>

<style lang="less" scoped></style>
