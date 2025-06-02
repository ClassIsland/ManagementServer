<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="客户端"
      titleTooltip="管理可以连接到集控的客户端配置。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <div class="d-flex gap-x-2">
          <n-button :onclick="createAbstractClient">添加客户端</n-button>
          <n-button type="primary" :onclick="addClient">下载集控配置</n-button>
        </div>
      </template>
    </BasicTable>
  </n-card>
  <n-drawer v-model:show="isEditingDrawerActive" :width="400" placement="right">
    <n-drawer-content title="客户端编辑">
      <n-form v-model="editingFormRef">
        <n-form-item :show-label="false" v-show="isCreating">
          <n-alert type="warning">
            注意！客户端 ID 在创建后将无法修改，请谨慎填写。
          </n-alert>
        </n-form-item>
        <n-form-item label="ID" path="id">
          <n-input v-model:value="editingFormRef.id" :disabled="!isCreating"/>
        </n-form-item>
        <n-form-item label="分组">
          <n-select
            v-model:value="editingFormRef.groupId"
            :options="clientGroups"
            :reset-menu-on-options-change="false"
            @scroll="handleGroupsScroll"
            label-field="name"
            value-field="id"
          />
        </n-form-item>
        <n-form-item :show-label="false">
          <n-button type="primary" attr-type="submit" @click="saveEntry" :loading="isSaving">
            保存
          </n-button>
        </n-form-item>
      </n-form>
    </n-drawer-content>
  </n-drawer>
</template>

<script lang="ts" setup>
import {columns} from './columns';
import { reactive, ref, h, onMounted } from 'vue';
import { BasicTable, TableAction } from '@/components/Table';
import { getTableList } from '@/api/table/list';
import { useDialog, useMessage } from 'naive-ui';
import { DeleteOutlined, EditOutlined } from '@vicons/antd';
import {Client, ClientGroup, TimeLayout} from "@/api/globals";
import Api from "@/api";

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<null | Client>();
const isEditingDrawerActive = ref(false);
const clientGroups = ref<Array<ClientGroup>>([]);
const clientGroupEnd = ref<null | ClientGroup>(null);
const clientGroupPage = ref(1);
const isCreating = ref(false);
const isSaving = ref(false);

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
      // auth: ['basic_list'],
    },
    {
      label: '编辑',
      icon: EditOutlined,
      onClick: handleEdit.bind(null, record),
      // auth: ['basic_list'],
    },
  ];
}

const loadDataTable = async (res) => {
  return Apis.clientregistry.get_api_v1_clients_registry_abstract({
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
    content: `您想删除客户端 ${record.id} 吗？与此客户端相关的分配等信息都将被移除。此操作不可撤销。`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Api.clientregistry.delete_api_v1_clients_registry_abstract_id({
        pathParams: {
          id: record.internalId
        }
      });
      message.success('删除成功');
      actionRef.value?.reload();
    },
    onNegativeClick: () => {},
  });
}

async function saveEntry() {
  isSaving.value = true;
  if (isCreating.value) {
    await Api.clientregistry.put_api_v1_clients_registry_abstract({
      data: editingFormRef.value
    });
  } else {
    await Api.clientregistry.put_api_v1_clients_registry_abstract_id({
      pathParams: {
        id: editingFormRef.value.internalId
      },
      data: editingFormRef.value
    });
  }
  isSaving.value = false;
  isEditingDrawerActive.value = false;
  actionRef?.value.reload();
  message.success("保存成功");
}

function handleEdit(record) {
  isCreating.value = false;
  editingFormRef.value = record;
  isEditingDrawerActive.value = true;
}

function addClient() {
  window.open("/api/v1/clients_registry/client_configure")
}

function createAbstractClient() {
  isCreating.value = true;
  editingFormRef.value = {
    groupId: 0,
    id: ""
  } as Client;
  isEditingDrawerActive.value = true;
}

async function handleGroupsScroll(e: Event) {
  const currentTarget = e.currentTarget as HTMLElement
  if (
    currentTarget.scrollTop + currentTarget.offsetHeight
    >= currentTarget.scrollHeight
  ) {
    console.log("loading external data");
    if (!clientGroupEnd.value) {
      clientGroupPage.value++;
      await loadClientGroups(clientGroupPage.value);
    }
  }
}

async function loadClientGroups(page: number) {
  let tl = await Apis.clientgroup.get_api_v1_client_groups({
    params: {
      pageSize: 50,
      pageIndex: page
    }
  });
  clientGroups.value.push(...tl.items);
  if (tl.items.count <= 0) {
    clientGroupEnd.value = true;
  }
}

onMounted(() => {
  loadClientGroups(1);
});
</script>

<style lang="less" scoped></style>
