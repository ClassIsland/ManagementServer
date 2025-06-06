<template>
  <div>
    <n-tabs type="bar" placement="left"
            tab-style="width: 200px !important;">
      <n-tab-pane v-for="tab in typeTabList"
                  :key="tab.key"
                  :name="tab.key"
                  :tab="tab.name">
        <div class="overflow-y-auto content">
          <component :is="tab.element"/>
        </div>
      </n-tab-pane>
    </n-tabs>
  </div>
</template>
<script lang="ts" setup>
  import { reactive, h } from 'vue';
  import BrandSettings from './BrandSettings.vue';

  const typeTabList = [
    {
      name: '显示设置',
      desc: '系统常规设置',
      key: 1,
      element: h(BrandSettings)
    }
  ];

  const state = reactive({
    type: 1,
    typeTitle: '显示设置',
  });

  function switchType(e) {
    state.type = e.key;
    state.typeTitle = e.name;
  }
</script>
<style lang="less" scoped>
.content {
  height: calc(100vh - 116px);
  padding: 8px;
}
</style>
