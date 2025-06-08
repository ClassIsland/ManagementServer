<script setup lang="ts">
import { ref, h } from 'vue';
import { DataTableColumns, NTimePicker, NSelect, NButton, MenuOption, NDropdown } from "naive-ui";
import {ClassPlan, Subject, TimeLayout, TimeLayoutItem} from "@/api/globals";
import {IClassInfoEditingEntry} from "@/models/classPlanEditor/classInfoEditingEntry";
import Router from "@/router";
import {useRouter} from "vue-router";
import {onMounted} from 'vue';
import { useDialog, useMessage } from 'naive-ui';
import {useModal} from "@/components/Modal";

const timeKinds = [
  {
    value: 0,
    key: 0,
    label: '上课'
  },
  {
    value: 1,
    key: 1,
    label: '课间休息'
  },
  {
    value: 2,
    key: 2,
    label: '分割线'
  },
  {
    value: 3,
    key: 3,
    label: '行动'
  }
];

const menuOptions: MenuOption[] = [
  {
    label: '添加时间点',
    key: 'addTimePoint',
    children: timeKinds
  }
];

const timeLayout = ref<TimeLayout | null>({
  layouts: []
});
const classPlanEditingEntries = ref<IClassInfoEditingEntry[]>([]);
const mainTableColumns : DataTableColumns<IClassInfoEditingEntry> = [
  {
    title: "类型",
    key: "timeType",
    render(x){
      return h(
        NSelect, {
          value: x.timeType,
          options: timeKinds,
          onUpdateValue(v) {
            x.timeType = v;
          }
        }
      );
    }
  },
  {
    title: "开始时间",
    key: "startSecond",
    render(x){
      return h(
        NTimePicker, {
          formattedValue: x.startSecondSafe,
          onUpdateFormattedValue(v) {
            x.startSecondSafe = v;
          }
        }
      );
    }
  },
  {
    title: "结束时间",
    key: "endSecond",
    render(x){
      return h(
        NTimePicker, {
          formattedValue: x.endSecondSafe,
          onUpdateFormattedValue(v) {
            x.endSecondSafe = v;
          }
        }
      );
    }
  },
  {
    title: "操作",
    key: "endSecond",
    render(x){
      return h(
        "div", {
          "class": "d-flex gap-2"
        }, [
          h(NButton, {
            size: "small",
            onClick(e) {
              const i = timeLayout.value?.layouts.indexOf(x);
              delete timeLayout.value?.layouts[i];
            },
          }, {
            default: () => "删除"
          }),
          h(NDropdown, {
            options: timeKinds,
            trigger: "click",
            onSelect(v) {
              const i = timeLayout.value?.layouts.indexOf(x);
              insertTimePointAfter(v, i );
            }
          }, {
            default: () => h(NButton, {
              size: "small"
            }, {
              default: () => "插入"
            })
          }),
        ]
      );
    }
  },
];
const router = useRouter();
const subjects = ref<Array<Subject>>([]);
const isLoading = ref(false);
const isSaving = ref(false);
const message = useMessage();

function padNumber(num: number, length: number): string {
  return num.toString().padStart(length, '0');
}

function formatLocalIsoDate(date: Date) {
  return `${date.getFullYear()}-${padNumber(date.getMonth() + 1, 2)}-${padNumber(date.getDate(), 2)}T${padNumber(date.getHours(), 2)}:${padNumber(date.getMinutes(), 2)}:${padNumber(date.getSeconds(), 2)}`
}

function formatSafeTime(date: Date) {
  return `${padNumber(date.getHours(), 2)}:${padNumber(date.getMinutes(), 2)}:${padNumber(date.getSeconds(), 2)}`
}

function contactDateTime(date: Date, time: string) {
  return `${date.getFullYear()}-${date.getMonth()}-${date.getDate()}T${time}`
}

function insertTimePointAfter(type, index) {
  const layouts = timeLayout.value?.layouts ?? [];
  const realIndex = index === -1 ? layouts.length - 1 : index;
  console.log(realIndex)
  const base = layouts[realIndex];
  let lasting = 0;
  const today = new Date();
  const defaultStartTime = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 7, 30);
  
  if (type == 0) lasting = 40 * 60 * 1000;
  else lasting = 10 * 60 * 1000;
  
  const startSecond = new Date(base?.endSecond ?? defaultStartTime);
  const endSecond = new Date(startSecond.getTime() + lasting);
  console.log(startSecond);
  
  timeLayout.value?.layouts.splice(realIndex + 1, 0, {
    timeType: type,
    startSecond: formatLocalIsoDate(startSecond),
    endSecond: formatLocalIsoDate(endSecond),
    startSecondSafe: formatSafeTime(startSecond),
    endSecondSafe: formatSafeTime(endSecond),
    isHideDefault: false,
    defaultClassId: ""
  })
}

async function loadData(){
  try {
    isLoading.value = true;
    const tl = await Apis.timelayouts.get_api_v1_profiles_timelayouts_id({
      pathParams: {
        id: router.currentRoute.value.params.id
      }
    });
    for (const tlKey in tl.layouts) {
      const point = tl.layouts[tlKey];
      point.startSecondSafe = point.startSecond.split("T")[1];
      point.endSecondSafe = point.endSecond.split("T")[1];
    }
    
    timeLayout.value = tl;
    
  } finally {
    isLoading.value = false;
  }
}

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

async function saveTimeLayout() {
  try{
    isSaving.value = true;
    const today = new Date();
    for (const i in timeLayout.layouts) {
      const point = timeLayout.layouts[i];
      point.startSecond = contactDateTime(today, point.startSecondSafe);
      point.endSecond = contactDateTime(today, point.endSecondSafe);
    }
    await Apis.timelayouts.put_api_v1_profiles_timelayouts_id({
      data: timeLayout.value,
      pathParams: {
        id: router.currentRoute.value.params.id
      }
    })
    message.success("保存成功");
  } finally {
    isSaving.value = false;
  }
} 

onMounted(() => {
  loadData();
});

</script>

<template>
  <n-spin :show="isLoading">
    <div class="d-flex gap-2 ">
      <n-card class="proCard " :bordered="false">
        <n-data-table :data="timeLayout.layouts"
                      :columns="mainTableColumns"
                      :max-height="`calc(var(--content-height) - 90px)`">
          <template #empty>
            <n-empty>
              <template #extra>
                <n-dropdown :options="timeKinds" trigger="click"
                            :on-select="v => insertTimePointAfter(v, -1)">
                  <n-button>创建时间点</n-button>
                </n-dropdown>
              </template>
            </n-empty>
          </template>
        </n-data-table>
      </n-card>
      <n-card class="proCard " :bordered="false">
        <n-tabs type="line" animated>
          <n-tab-pane name="info" tab="时间表信息">
            <n-form :model="timeLayout">
              <n-form-item label="时间表名称">
                <n-input v-model:value="timeLayout.name"/>
              </n-form-item>
              <n-form-item label="分组">
                <PagedSelect
                  v-model:value="timeLayout.groupId"
                  labelField="name"
                  valueField="id"
                  :get-data="getGroupData"
                />
              </n-form-item>
            </n-form>

            <n-button type="primary" @click="saveTimeLayout" :loading="isSaving">保存</n-button>
          </n-tab-pane>
          <n-tab-pane name="assignee" tab="分配">
            <AssigneeTable :object-id="router.currentRoute.value.params.id"
                           :object-type="2"/>
          </n-tab-pane>
        </n-tabs>
      </n-card>
    </div>
  </n-spin>

</template>

<style scoped lang="less">
.schedule-container {
}

.root {
}
</style>
