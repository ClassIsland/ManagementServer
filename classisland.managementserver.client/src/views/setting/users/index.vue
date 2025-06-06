<template>
  <n-card :bordered="false" class="proCard">
    <BasicTable
      title="用户"
      titleTooltip="管理可以登陆到此系统的用户。"
      :columns="columns"
      :request="loadDataTable"
      :row-key="(row) => row.id"
      ref="actionRef"
      :actionColumn="actionColumn"
      :scroll-x="1360"
      @update:checked-row-keys="onCheckedRow">
      <template v-slot:toolbar>
        <n-button type="primary" @click="handleAdd">添加用户</n-button>
      </template>
    </BasicTable>
  </n-card>

  <n-drawer v-model:show="isEditingDrawerVisible" :width="500" placement="right">
    <n-drawer-content title="编辑用户信息">
      <n-form v-model="editingFormRef">
        <n-form-item label="用户名" path="userName">
          <n-input v-model:value="editingFormRef.userName" disabled/>
        </n-form-item>

        <n-form-item label="昵称" path="name">
          <n-input v-model:value="editingFormRef.name" placeholder="请输入昵称" />
        </n-form-item>

        <n-form-item label="邮箱" path="emailAddress">
          <n-input placeholder="请输入邮箱" v-model:value="editingFormRef.emailAddress" />
        </n-form-item>

        <n-form-item label="联系电话" path="phoneNumber">
          <n-input placeholder="请输入联系电话" v-model:value="editingFormRef.phoneNumber" />
        </n-form-item>
        <n-form-item :show-label="false">
          <n-button type="primary" attr-type="submit" @click="saveEntry" :loading="isSaving">
            保存
          </n-button>
        </n-form-item>
      </n-form>
    </n-drawer-content>
  </n-drawer>

  <basicModal @register="modalRegister" ref="modalRef" class="basicModal" @on-ok="okCreateUser">
    <template #default>
      <n-form v-model="newUserFormRef">
        <n-form-item label="用户名" path="user.userName">
          <n-input v-model:value="newUserFormRef.user.userName" placeholder="创建后不可更改"/>
        </n-form-item>
        <n-form-item label="昵称" path="user.name">
          <n-input v-model:value="newUserFormRef.user.name" placeholder="例：张三"/>
        </n-form-item>
        <n-form-item label="密码" path="password">
          <n-input type="password" show-password-on="click" v-model:value="newUserFormRef.password" />
        </n-form-item>
      </n-form>
    </template>
  </basicModal>

  <basicModal @register="modalSetPassword" class="basicModal" @on-ok="okSetPassword">
    <template #default>
      <n-form v-model="setPasswordFormRef">
        <n-form-item label="新密码" path="password">
          <n-input type="password" show-password-on="click" v-model:value="setPasswordFormRef.newPassword" />
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
import { DeleteOutlined, EditOutlined, KeyOutlined } from '@vicons/antd';
import {Subject} from "@/api/globals";
import { Guid } from 'guid-typescript';
import {BasicForm, FormSchema, useForm} from "@/components/Form";
import {useModal} from "@/components/Modal";

const message = useMessage();
const dialog = useDialog();
const actionRef = ref();
const editingFormRef = ref<Subject | null>(null);
const isEditingDrawerVisible = ref(false);
const isSaving = ref(false);
const isAdding = ref(false);
const modalRef: any = ref(null);
const newUserFormRef = ref({});
const setPasswordFormRef= ref({});

const [modalRegister, modalRegisterActions] = useModal({
  title: '添加用户',
});
const openRegisterModal = modalRegisterActions.openModal;
const closeRegisterModal = modalRegisterActions.closeModal;
const setRegisterSubLoading = modalRegisterActions.setSubLoading;

const [modalSetPassword, modalSetPasswordActions] = useModal({
  title: '修改密码',
});
const openSetPasswordModal = modalSetPasswordActions.openModal;
const closeSetPasswordModal = modalSetPasswordActions.closeModal;
const setSetPasswordSubLoading = modalSetPasswordActions.setSubLoading;


const params = reactive({
  pageSize: 5,
  name: 'NaiveAdmin',
});

const actionColumn = reactive({
  width: 300,
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
    await Apis.users.post_api_v1_users_id({
      pathParams: {
        id: editingFormRef.value?.id,
      },
      data: editingFormRef.value
    });
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
      auth: [],
    },
    {
      label: '编辑',
      icon: EditOutlined,
      onClick: handleEdit.bind(null, record),
      auth: [],
    },
    {
      label: '修改密码',
      icon: KeyOutlined,
      onClick: handleSetPassword.bind(null, record),
      auth: [],
    },
  ];
}

const loadDataTable = async (res) => {
  return Apis.users.get_api_v1_users_all({
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
    content: `您想删除用户 ${record.userName} 吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      await Apis.users.delete_api_v1_users_id({
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
  editingFormRef.value = record;
  isEditingDrawerVisible.value = true;
}

function handleSetPassword(record) {
  setPasswordFormRef.value = {
    newPassword: ""
  }
  editingFormRef.value = record;
  openSetPasswordModal();
}

function handleAdd() {
  newUserFormRef.value = {
    user: {
      userName: "",
      name: "",
    },
    password: "",
  };
  openRegisterModal();
}

async function okCreateUser() {
  try {
    await Apis.users.post_api_v1_users_create({
      data: newUserFormRef.value
    });
    message.success("用户创建成功");
    closeRegisterModal();
    actionRef.value.reload();
  } finally {
    setRegisterSubLoading(false);
  }
}

async function okSetPassword() {
  try {
    await Apis.users.post_api_v1_users_id_set_password({
      data: setPasswordFormRef.value,
      pathParams: {
        id: editingFormRef.value.id
      }
    });
    message.success("密码修改成功");
    closeSetPasswordModal();
    actionRef.value.reload();
  } finally {
    setSetPasswordSubLoading(false);
  }
} 
</script>

<style lang="less" scoped></style>
