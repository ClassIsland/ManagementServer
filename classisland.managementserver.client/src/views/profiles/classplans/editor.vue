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

async function loadTimeLayouts(page: number) {
  let tl = await Apis.timelayouts.get_api_v1_profiles_timelayouts({
    params: {
      pageSize: 50,
      pageIndex: page
    }
  });
  timeLayouts.value.push(...tl.items);
  if (tl.items.count <= 0) {
    timeLayoutsEnd.value = true;
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

async function handleTimeLayoutMenuScroll(e: Event) {
  const currentTarget = e.currentTarget as HTMLElement
  if (
    currentTarget.scrollTop + currentTarget.offsetHeight
    >= currentTarget.scrollHeight
  ) {
    console.log("loading external data");
    if (!timeLayoutsEnd.value) {
      timeLayoutsPage.value++;
      await loadTimeLayouts(timeLayoutsPage.value);
    }
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

onMounted(() => {
  loadTimeLayouts(1);
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
                      :max-height="`calc(100vh - 210px)`">
          
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
