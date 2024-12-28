<script setup lang="ts">
import { ref, h } from 'vue';
import { DataTableColumns } from "naive-ui";
import {ClassPlan, TimeLayout, TimeLayoutItem} from "@/api/globals";
import {IClassInfoEditingEntry} from "@/models/classPlanEditor/classInfoEditingEntry";
import Router from "@/router";
import {useRouter} from "vue-router";

const classPlanData = ref<any>(null);
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
    key: "subjectName"
  }
];
const router = useRouter();

function getTimeString(time){
   return new Date(time).toLocaleTimeString();
}

function updateView() {
  
}

async function loadData(){
  const result = await Apis.classplans.get_api_v1_profiles_classplans_id({
    pathParams: {
      id: router.currentRoute.value.params.id
    }
  }) as any;
  const classPlan = result.classPlan as ClassPlan;
  const classNames = result.classNames as string[];
  timeLayout.value = await Apis.timelayouts.get_api_v1_profiles_timelayouts_id({
    pathParams: {
      id: classPlan.timeLayoutId ?? ""
    }
  }) as TimeLayout;
  const entries: IClassInfoEditingEntry[] = [];
  let ic = 0;
  classPlan.classes?.forEach((c, i) => {
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
} 

loadData();

</script>

<template>
  <n-card class="proCard">
    <n-data-table :data="classPlanEditingEntries"
                  :columns="mainTableColumns"
                  :max-height="`calc(100vh)`">
      
    </n-data-table>
  </n-card>
</template>

<style scoped lang="less">

</style>
