<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="策略"
      titleTooltip="管理分发到客户端的策略。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd" v-if="hasPermission(['ObjectsWrite'])">添加策略</n-button>
      </template>
    </BasicTable>
  </n-card>

  <n-drawer v-model:show="isEditingDrawerVisible" :width="500" placement="right">
    <n-drawer-content title="编辑策略">
      <template #footer>
        <n-button type="primary" attr-type="submit" @click="saveEntry" :loading="isSaving">
          保存
        </n-button>
      </template>
      <n-tabs animated>
        <n-tab-pane name="basic" label="基本">
          <n-form v-model="editingFormRef">
            <n-form-item label="策略名称" path="name">
              <n-input v-model:value="editingFormRef.name"/>
            </n-form-item>
            <n-form-item :show-label="false" path="isOutDoor">
              <n-checkbox label="策略已启用" v-model:checked="editingFormRef.isEnabled"/>
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
          </n-form>
        </n-tab-pane>
        <n-tab-pane name="policy" label="策略">
          <n-form v-model="editingFormRef">
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableProfileEditing">
                <n-thing title="禁止编辑档案" 
                         description="启用此项后，用户将不能编辑档案内所有内容，同时【从 Excel 表格导入功能】也将被禁用。临时换课和启用临时课表功能不受影响。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableProfileClassPlanEditing">
                <n-thing title="禁止编辑课表"
                         description="启用此项后，用户将不能创建、删除和编辑课表，同时【从 Excel 表格导入功能】也将被禁用。临时换课和启用临时课表功能不受影响。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableProfileTimeLayoutEditing">
                <n-thing title="禁止编辑时间表"
                         description="启用此项后，用户将不能创建、删除和编辑时间表，同时【从 Excel 表格导入功能】也将被禁用。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableProfileSubjectsEditing">
                <n-thing title="禁止编辑科目"
                         description="启用此项后，用户将不能创建、删除和编辑科目。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableSettingsEditing">
                <n-thing title="禁止编辑应用设置"
                         description="启用此项后，用户将不能调整应用的设置。但先前调整过的设置在启用此项后不受影响。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableSplashCustomize">
                <n-thing title="禁止自定义启动加载界面"
                         description="启用此项后，用户将不能自定义启动界面。如果先前调整过启动界面自定义设置，这些设置会被清除。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.disableDebugMenu">
                <n-thing title="禁用调试菜单"
                         description="启用此项后，用户将不能进入调试页面。"/>
              </n-checkbox>
            </n-form-item>
            <n-form-item :show-label="false">
              <n-checkbox v-model:checked="editingFormRef.content.allowExitManagement">
                <n-thing title="允许退出集控"
                         description="控制用户是否能主动退出集控。禁用后，用户将无法自行退出集控。"/>
              </n-checkbox>
            </n-form-item>
          </n-form>
        </n-tab-pane>
        <n-tab-pane name="assignees" label="分配" :disabled="isAdding">
          <AssigneeTable :object-id="editingFormRef.id"
                         :object-type="5"/>
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
import {Policy, Subject} from "@/api/globals";
import {usePermission} from "@/hooks/web/usePermission";
import { Guid } from 'guid-typescript';
import {IPagedSelectState} from "@/components/PagedSelect/IPagedSelectState";

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<Policy | null>(null);
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
      await Apis.policies.post_api_v1_policies({
        data: editingFormRef.value
      });
    } else {
      await Apis.policies.post_api_v1_policies_id({
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
  return Apis.policies.get_api_v1_policies({
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
    content: `您想删除策略 ${record.name} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.policies.delete_api_v1_policies_id({
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
    "name": "新策略",
    "isEnabled": true,
    "content": {
      "disableProfileClassPlanEditing": false,
      "disableProfileTimeLayoutEditing": false,
      "disableProfileSubjectsEditing": false,
      "disableProfileEditing": false,
      "disableSettingsEditing": false,
      "disableSplashCustomize": false,
      "disableDebugMenu": false,
      "allowExitManagement": true
    },
    groupId: "00000000-0000-0000-0000-000000000001"
  } as Policy;
  isEditingDrawerVisible.value = true;
  isAdding.value = true;

}
</script>

<style lang="less" scoped></style>
