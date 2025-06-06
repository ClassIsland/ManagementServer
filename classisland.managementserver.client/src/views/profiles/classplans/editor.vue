<script setup lang="ts">
import { ref, h } from 'vue';
import { DataTableColumns } from "naive-ui";
import {ClassPlan, Subject, TimeLayout, TimeLayoutItem} from "@/api/globals";
import {IClassInfoEditingEntry} from "@/models/classPlanEditor/classInfoEditingEntry";
import Router from "@/router";
import {useRouter} from "vue-router";
import {onMounted} from 'vue';
import { NSelect } from 'naive-ui'
import { useDialog, useMessage } from 'naive-ui';


const classPlan = ref<ClassPlan | null>({} as ClassPlan);
const timeLayout = ref<TimeLayout | null>(null);
const classPlanEditingEntries = ref<IClassInfoEditingEntry[]>([]);
const mainTableColumns : DataTableColumns<IClassInfoEditingEntry> = [
  {
    title: "时间",
    key: "timePoint",
    render(x){
      return h(
        "span", {},
        [getTimeString(x.timePoint.startSecond) + " - " + getTimeString(x.timePoint.endSecond)]
      );
    }
  },
  {
    title: "科目",
    key: "subjectName",
    render(row, index) {
      return h(NSelect, {
        value: row.classInfo.subjectId,
        options: subjects.value,
        valueField: "id",
        labelField: "name",
        onUpdateValue(v) {
          classPlanEditingEntries.value[index].classInfo.subjectId = v
        }
      })
    }
  }
];
const router = useRouter();
const timeLayouts = ref<Array<TimeLayout>>([]);
const timeLayoutsPage = ref(1);
const timeLayoutsEnd = ref(false);    
const subjects = ref<Array<Subject>>([]);
const subjectsLayoutsPage = ref(1);
const subjectsLayoutsEnd = ref(false);
const isLoading = ref(false);
const isSaving = ref(false);
const message = useMessage();
const weeksDays = [
  { key: 0, name: '周日' },
  { key: 1, name: '周一' },
  { key: 2, name: '周二' },
  { key: 3, name: '周三' },
  { key: 4, name: '周四' },
  { key: 5, name: '周五' },
  { key: 6, name: '周六' }
];
const weekDivs = [
  { key: 0, name: '不限' },
  { key: 1, name: '第一周' },
  { key: 2, name: '第二周' },
  { key: 3, name: '第三周' },
  { key: 4, name: '第四周' }
];
const weekDivTotals = [
  { key: 2, name: '二周' },
  { key: 3, name: '三周' },
  { key: 4, name: '四周' }
];

function getTimeString(time){
   return new Date(time).toLocaleTimeString();
}

function updateView() {
  
}

async function loadData(){
  try {
    isLoading.value = true;
    const result = await Apis.classplans.get_api_v1_profiles_classplans_id({
      pathParams: {
        id: router.currentRoute.value.params.id
      }
    }) as any;
    const cp = result.classPlan as ClassPlan;
    classPlan.value = cp;
    const classNames = result.classNames as string[];
    timeLayout.value = await Apis.timelayouts.get_api_v1_profiles_timelayouts_id({
      pathParams: {
        id: cp.timeLayoutId ?? ""
      }
    }) as TimeLayout;
    const entries: IClassInfoEditingEntry[] = [];
    let ic = 0;
    cp.classes?.forEach((c, i) => {
      while (ic < timeLayout.value!.layouts!.length &&
         timeLayout.value!.layouts![ic].timeType != 0) {
        ic++;
      }
      entries.push({
        timePoint: timeLayout.value!.layouts![ic],
        subjectName: classNames[i],
        classInfo: c
      });
      if (ic + 1 < timeLayout.value!.layouts!.length) {
        ic++;
      }
    });
    classPlanEditingEntries.value = entries;
  } finally {
    isLoading.value = false;
  }
} 


async function loadSubjects(page: number) {
  let s = await Apis.subjects.get_api_v1_profiles_subjects({
    params: {
      pageSize: 50
    }
  });
  subjects.value.push(...s.items);
  if (s.items.count <= 0) {
    subjectsLayoutsEnd.value = true;
  }
}


async function saveClassPlan() {
  try {
    isSaving.value = true;
    const cp = classPlan.value;
    cp.timeLayout = {};
    const entries = classPlanEditingEntries.value;
    const classes = entries.map(e => e.classInfo);
    cp.classes = classes;
    await Apis.classplans.put_api_v1_profiles_classplans_id({
      pathParams: {
        id: router.currentRoute.value.params.id
      },
      data: cp
    });
    message.success("保存成功");
  } finally {
    isSaving.value = false;
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

onMounted(() => {
  loadData();
  loadSubjects(1);
});

</script>

<template>
  <n-spin :show="isLoading">
    <div class="flex gap-2 root">
      <n-card class="proCard" :bordered="false">
        <n-data-table :data="classPlanEditingEntries"
                      :columns="mainTableColumns"
                      :max-height="`calc(var(--content-height) - 90px)`">
          
        </n-data-table>
      </n-card>
      <n-card class="proCard" :bordered="false">
        <n-tabs type="line" animated>
          <n-tab-pane name="classPlanInfo" tab="课表信息">
            <n-form :model="classPlan">
              <n-form-item label="课表名称">
                <n-input v-model:value="classPlan.name"/>
              </n-form-item>
              <n-form-item label="时间表">
                <PagedSelect
                  v-model:value="classPlan.timeLayoutId"
                  labelField="name"
                  valueField="id"
                  :get-data="getTimeLayoutData"
                />
              </n-form-item>
              <n-form-item label="分组">
                <PagedSelect
                  v-model:value="classPlan.groupId"
                  labelField="name"
                  valueField="id"
                  :get-data="getGroupData"
                />
              </n-form-item>
            </n-form>
            
            <n-button type="primary" @click="saveClassPlan" :loading="isSaving">保存</n-button>
          </n-tab-pane>
          <n-tab-pane name="timeRule" tab="启用规则">
            <n-form :model="classPlan">
              <n-form-item :show-label="false">
                <n-checkbox v-model:checked="classPlan.isEnabled">
                  自动启用
                </n-checkbox>
              </n-form-item>
              <n-form-item label="当今天是" v-if="classPlan.isEnabled">
                <n-radio-group v-model:value="classPlan.timeRule.weekDay">
                  <n-radio-button
                    v-for="i in weeksDays"
                    :key="i.key"
                    :value="i.key"
                    :label="i.name"
                  />
                </n-radio-group>
              </n-form-item>
              <n-form-item label="当本周是" v-if="classPlan.isEnabled">
                <n-radio-group v-model:value="classPlan.timeRule.weekCountDiv">
                  <n-radio-button
                    v-for="i in weekDivs.slice(0, classPlan.timeRule.weekCountDivTotal + 1)"
                    :key="i.key"
                    :value="i.key"
                    :label="i.name"
                  />
                </n-radio-group>
              </n-form-item>
              <n-form-item label="每轮周数" v-if="classPlan.isEnabled">
                <n-radio-group v-model:value="classPlan.timeRule.weekCountDivTotal">
                  <n-radio-button
                    v-for="i in weekDivTotals"
                    :key="i.key"
                    :value="i.key"
                    :label="i.name"
                  />
                </n-radio-group>
              </n-form-item>
            </n-form>

            <n-button type="primary" @click="saveClassPlan" :loading="isSaving">保存</n-button>
          </n-tab-pane>
          <n-tab-pane name="assignee" tab="分配">
            <AssigneeTable :object-id="router.currentRoute.value.params.id"
                           :object-type="1"/>
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
