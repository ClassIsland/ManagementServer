<script setup lang="ts">
import { ref, defineProps, onMounted, toRefs, watch, defineEmits} from 'vue';
import {IPagedSelectState} from "@/components/PagedSelect/IPagedSelectState";

const props = defineProps<{
  labelField: string,
  valueField: string,
  value?: string | number | object | null | undefined,
  getData: Function,
  seek?: Function,
  sharedStates?: IPagedSelectState | null,
  onUpdateSharedStates?: Function
}>();

const { value, sharedStates } = toRefs(props)

const myValueRef = ref(value.value);
const seekedItem = ref(null);
const mySharedStates = ref<IPagedSelectState>({
  pageEnd: false,
  pageIndex: 1,
  items: [],
  isLoading: false
});
const emits = defineEmits(['update:value', 'update:sharedStates']);

watch(
  () => value.value,
  (newVal) => {
    myValueRef.value = newVal;
  }
);
watch(
  () => myValueRef.value,
  (newVal) => {
    emits('update:value', newVal)
  }
)

watch(
  () => sharedStates.value,
  (newVal) => {
    mySharedStates.value = newVal;
  }
);
watch(
  () => mySharedStates.value,
  (newVal) => {
    emits('update:sharedStates', newVal);
    if (props.onUpdateSharedStates) {
      props.onUpdateSharedStates(newVal);
    }
  }
)

async function handleScroll(e: Event) {
  const currentTarget = e.currentTarget as HTMLElement
  if (
    currentTarget.scrollTop + currentTarget.offsetHeight
    >= currentTarget.scrollHeight
  ) {
    console.log("loading external data");
    if (!mySharedStates.value.pageEnd) {
      mySharedStates.value.pageIndex++;
      await loadData(mySharedStates.value.pageIndex, 50);
    }
  }
}

async function loadData(page: number) {
  try {
    console.log(`loading data of page ${page}`)
    mySharedStates.value.isLoading = true;
    let s = await props.getData(page, 50);
    if (s.items.length < 50) {
      mySharedStates.value.pageEnd = true;
    }
    // s.items.remove(seekedItem);
    // console.log(s)
    mySharedStates.value.items.push(...s.items);
  } finally {
    mySharedStates.value.isLoading = false;
  }
}

onMounted(async () => {
  if (props.sharedStates && !(props.sharedStates?.__v_isRef === true && !props.sharedStates?.value )) {
    mySharedStates.value = props.sharedStates;
  } else {
    emits('update:sharedStates', mySharedStates.value);
    if (props.onUpdateSharedStates) {
      props.onUpdateSharedStates(mySharedStates.value);
    }
  }
  
  if (props.seek) {
    try {
      mySharedStates.value.isLoading = true;
      const v = await props.seek(props.value);
      if (v && mySharedStates.value.items.indexOf(v) !== -1) {
        mySharedStates.value.items.push(v);
      }
    } finally {
      mySharedStates.value.isLoading = false;
    }
  }
  
  if (!mySharedStates.value.isLoading && mySharedStates.value.pageIndex <= 1 && !mySharedStates.value.pageEnd) {
    mySharedStates.value.isLoading = true;
    await loadData(1);
  }
});

</script>

<template>
  <n-select
    v-model:value="myValueRef"
    :options="mySharedStates.items"
    :reset-menu-on-options-change="false"
    @scroll="handleScroll"
    :label-field="props.labelField"
    :value-field="props.valueField"
    :loading="mySharedStates.isLoading"
  />
</template>

<style scoped lang="less">

</style>
