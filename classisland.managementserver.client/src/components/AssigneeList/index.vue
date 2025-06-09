<script setup lang="ts">
import {AssigneeTypes, ObjectsAssignee} from "@/api/globals";
import {toRef, ref, defineEmits, watch} from 'vue';

const assigneeTypes = [
  {
    key: 1,
    label: "实例"
  },
  {
    key: 2,
    label: "抽象实例"
  },
  {
    key: 3,
    label: "实例组"
  }
]

const props = defineProps<{
  value: Array<ObjectsAssignee>,
  
}>();

const { value } = toRef(props);

const myAssignees = ref<Array<ObjectsAssignee>>([]);
const clientsSelectSharedState = ref(null);
const abstractClientsSelectSharedState = ref(null);
const clientGroupsSelectSharedState = ref(null);

const emits = defineEmits(['update:value']);
watch(
  () => value.value,
  (newVal) => {
    myAssignees.value = newVal;
  }
);
watch(
  () => myAssignees.value,
  (newVal) => {
    emits('update:value', newVal)
  }
)

function onCreate() {
  return {
    "targetClientId": "",
    "targetClientCuid": "00000000-0000-0000-0000-000000000000",
    "targetGroupId": 1,
    "assigneeType": 2,
  }
}

async function getClients(pageIndex: number, pageSize: number) {
  return Apis.clientregistry.get_api_v1_clients_registry_all({
    params: {
      pageIndex, pageSize
    }
  })
}

async function getAbstractClients(pageIndex: number, pageSize: number) {
  return Apis.clientregistry.get_api_v1_clients_registry_abstract({
    params: {
      pageIndex, pageSize
    }
  })
}

async function getClientGroups(pageIndex: number, pageSize: number) {
  return Apis.clientgroup.get_api_v1_client_groups({
    params: {
      pageIndex, pageSize
    }
  })
}

</script>

<template>
  <n-dynamic-input v-model:value="myAssignees" :on-create="onCreate">
    <template #create-button-default>
      添加目标
    </template>
    <template #default="{ value }">
      <div style="display: flex; align-items: center; width: 100%" class="ga-2">
        <n-select :options="assigneeTypes"
                  v-model:value="value.assigneeType" 
                  label-field="label"
                  value-field="key"/>
        <PagedSelect v-if="value.assigneeType === 1"
                     v-model:value="value.targetClientCuid"
                     labelField="cuid"
                     valueField="cuid"
                     :get-data="getClients"
                     v-model:shared-states="clientsSelectSharedState"/>
        <PagedSelect v-if="value.assigneeType === 2"
                     v-model:value="value.targetClientId"
                     labelField="id"
                     valueField="id"
                     :get-data="getAbstractClients"
                     v-model:shared-states="abstractClientsSelectSharedState"/>
        <PagedSelect v-if="value.assigneeType === 3"
                     v-model:value="value.targetGroupId"
                     labelField="name"
                     valueField="id"
                     :get-data="getClientGroups"
                     v-model:shared-states="clientGroupsSelectSharedState"/>
      </div>
    </template>
  </n-dynamic-input>
</template>

<style scoped lang="less">

</style>
