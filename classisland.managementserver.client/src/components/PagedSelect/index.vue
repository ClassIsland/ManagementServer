<script setup lang="ts">
import { ref, defineProps, onMounted, toRefs, watch, defineEmits} from 'vue';

const props = defineProps({
  labelField: String,
  valueField: String,
  value: [String, Number, Object],
  getData: Function<number, number>,
  seek: Function<number>
});

const { value } = toRefs(props)

const myValueRef = ref(value.value);
const pageEnd = ref(false);
const pageIndex = ref(1);
const items = ref([]);
const seekedItem = ref(null);
const emits = defineEmits(['update:value']);

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

async function handleScroll(e: Event) {
  const currentTarget = e.currentTarget as HTMLElement
  if (
    currentTarget.scrollTop + currentTarget.offsetHeight
    >= currentTarget.scrollHeight
  ) {
    console.log("loading external data");
    if (!pageEnd.value) {
      pageIndex.value++;
      await loadData(pageIndex.value, 50);
    }
  }
}

async function loadData(page: number) {
  let s = await props.getData(page, 50);
  if (s.items.count <= 0) {
    pageEnd.value = true;
  }
  // s.items.remove(seekedItem);
  console.log(s)
  items.value.push(...s.items);
}

onMounted(async () => {
  if (props.seek !== null && props.seek !== undefined) {
    const v = await props.seek(props.value);
    if (v !== null && v !== undefined) {
      items.value.push(v);
    }
  }
  await loadData(1);
});

</script>

<template>
  <n-select
    v-model:value="myValueRef"
    :options="items"
    :reset-menu-on-options-change="false"
    @scroll="handleScroll"
    :label-field="props.labelField"
    :value-field="props.valueField"
  />
</template>

<style scoped lang="less">

</style>
