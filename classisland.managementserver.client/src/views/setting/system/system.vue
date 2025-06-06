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
  import BasicSettings from './BasicSettings.vue';

  const typeTabList = [
    {
      name: '基本',
      desc: '系统常规设置',
      key: 1,
      element: h(BasicSettings)
    },
    {
      name: '显示',
      desc: '系统外观等',
      key: 2,
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
  height: var(--content-height);
  padding: 8px;
}
</style>
