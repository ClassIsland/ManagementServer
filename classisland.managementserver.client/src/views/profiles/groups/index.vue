<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="分组"
      titleTooltip="管理档案分组。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd">添加分组</n-button>
      </template>
    </BasicTable>
  </n-card>

  <n-drawer v-model:show="isEditingDrawerVisible" :width="500" placement="right">
    <n-drawer-content title="编辑分组">
      <n-tabs>
        <n-tab-pane name="basic" label="基本">
          <n-form v-model="editingFormRef">
            <n-form-item label="名称" path="name">
              <n-input v-model:value="editingFormRef.name"/>
            </n-form-item>
            <n-form-item label="颜色" path="color" :rule="colorRule">
              <n-color-picker v-model:value="editingFormRef.colorHex" :show-alpha="false" />
            </n-form-item>
            <n-form-item :show-label="false">
              <n-button type="primary" attr-type="submit" @click="saveEntry" :loading="isSaving">
                保存
              </n-button>
            </n-form-item>
          </n-form>
        </n-tab-pane>
        <n-tab-pane name="assignees" label="分配" :disabled="isAdding">
          <AssigneeTable :object-type="6" :object-id="editingFormRef.id"/>
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
import {ProfileGroup, Subject} from "@/api/globals";
import { Guid } from 'guid-typescript';

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<ProfileGroup | null>(null);
const isEditingDrawerVisible = ref(false);
const isSaving = ref(false);
const isAdding = ref(false);

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
  e.preventDefault();
  console.log("saving");
  if (editingFormRef?.value == null){
    return;
  }
  isSaving.value = true;
  if (isAdding.value) {
    await Apis.profilegroups.put_api_v1_profiles_groups({
      data: editingFormRef.value
    });
  } else {
    await Apis.profilegroups.put_api_v1_profiles_groups_id({
      pathParams: {
        id: editingFormRef.value?.id,
      },
      data: editingFormRef.value
    });
  }
  isEditingDrawerVisible.value = false;
  isSaving.value = false;
  actionRef.value.reload();
  message.success("保存成功");
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
  return Apis.profilegroups.get_api_v1_profiles_groups({
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
    content: `您想删除分组 ${record.name} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.profilegroups.delete_api_v1_profiles_groups_id({
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
  editingFormRef.value = { ... record } as ProfileGroup;
  isEditingDrawerVisible.value = true;
}

function handleAdd() {
  editingFormRef.value = {
    name: "新分组",
    colorHex: "#66CCFF"
  } as ProfileGroup;
  isEditingDrawerVisible.value = true;
  isAdding.value = true;

}
</script>

<style lang="less" scoped></style>
