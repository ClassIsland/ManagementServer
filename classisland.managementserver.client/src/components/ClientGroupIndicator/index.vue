<script setup lang="ts">
import {defineProps, ref} from 'vue';

const props = defineProps({
  group: Object
})

</script>

<template>
  <NTag size="small"
        :color="{textColor: props?.group?.colorHex, borderColor: props?.group?.colorHex}"
        round>
    {{ props?.group?.name }}
  </NTag>
</template>

<style scoped lang="less">

</style>
