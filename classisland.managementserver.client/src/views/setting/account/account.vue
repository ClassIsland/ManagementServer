<template>
  <div>
    <n-tabs type="bar" placement="left"
            tab-style="width: 200px !important;">
      <n-tab-pane v-for="tab in typeTabList"
                  :key="tab.key"
                  :name="tab.key"
                  :tab="tab.name"
                  >
        <div class="overflow-y-auto content">
          <component :is="tab.element"/>
        </div>
      </n-tab-pane>
    </n-tabs>
  </div>
</template>
<script lang="ts" setup>
import { reactive, ref } from 'vue';
import BasicSetting from './BasicSetting.vue';
import SafetySetting from './SafetySetting.vue';

const typeTabList = [
  {
    name: '基本设置',
    desc: '个人账户信息设置',
    key: 1,
    element: BasicSetting,
  },
  {
    name: '安全设置',
    desc: '密码，邮箱等设置',
    key: 2,
    element: SafetySetting
  },
];

const state = reactive({
  type: 1,
  typeTitle: '基本设置',
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
