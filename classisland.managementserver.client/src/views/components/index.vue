<script setup lang="ts">
import { ref, reactive, onMounted, computed, h, watch } from 'vue';
import { NCard, NButton, NSelect, useMessage, NSpace, NTag, NModal, NForm, NFormItem, NInput, NCollapse, NCollapseItem, NSwitch, NPopconfirm, NDivider, NInputNumber, NColorPicker, NSlider } from 'naive-ui';
import { getClientComponents, updateClientComponents, requestClientUpload } from '@/api/components/index';
import { Alova } from '@/utils/http/alova/index';
import RulesetEditor from '@/views/automation/RulesetEditor.vue';

const message = useMessage();
const selectedClient = ref('');
const clientOptions = ref<{ label: string; value: string }[]>([]);
const isLoading = ref(false);
const showJsonEditor = ref(false);
const rawJson = ref('');
const editingComponent = ref<{ lineIndex: number; compIndex: number; childIndex?: number } | null>(null);
const editingComp = computed(() => {
  if (!editingComponent.value) return null;
  const { lineIndex, compIndex, childIndex } = editingComponent.value;
  const comp = layout.value[lineIndex]?.Children[compIndex];
  if (!comp) return null;
  if (childIndex !== undefined && comp.Children) return comp.Children[childIndex];
  return comp;
});
const addCompKeys = ref<Record<number, number>>({});
const addChildKeys = ref<Record<string, number>>({});

const componentTypes = [
  { id: 'ee8f66bd-c423-4e7c-ab46-aa9976b00e08', name: '文本', icon: '📝', category: '基础', hasSettings: true },
  { id: 'ab0f26d5-9df6-4575-b844-73b04d0907c1', name: '分割线', icon: '➖', category: '基础', hasSettings: false },
  { id: '1db2017d-e374-4bc6-9d57-0b4adf03a6b8', name: '课程表', icon: '📋', category: '基础', hasSettings: true },
  { id: 'df3f8295-21f6-482e-bada-fa0e5f14bb66', name: '日期', icon: '📅', category: '基础', hasSettings: false },
  { id: '9e1af71d-8f77-4b21-a342-448787104dd9', name: '时钟', icon: '🕐', category: '基础', hasSettings: true },
  { id: 'ca495086-e297-4beb-9603-c5c1c1a8551e', name: '天气', icon: '🌤️', category: '基础', hasSettings: true },
  { id: '7c645d35-8151-48ba-b4ac-15017460d994', name: '倒计时', icon: '⏱️', category: '基础', hasSettings: true },
  { id: '7e19a113-d281-4f33-970a-834a0b78b5ad', name: '轮播容器', icon: '🔄', category: '容器', hasSettings: true },
  { id: '70fcd5ea-3fae-4e06-aca2-4f4df47f9acd', name: '滚动容器', icon: '📜', category: '容器', hasSettings: true },
  { id: 'c911d762-107f-40c6-84cc-0146ab3c86b1', name: '分组容器', icon: '📦', category: '容器', hasSettings: false },
  { id: '2d849ece-9f21-4c78-9434-415cfc283294', name: '堆叠容器', icon: '📚', category: '容器', hasSettings: false },
];

const containerIds = new Set([
  '7e19a113-d281-4f33-970a-834a0b78b5ad',
  '70fcd5ea-3fae-4e06-aca2-4f4df47f9acd',
  'c911d762-107f-40c6-84cc-0146ab3c86b1',
  '2d849ece-9f21-4c78-9434-415cfc283294',
]);

interface ComponentItem {
  Id: string;
  NameCache?: string;
  Settings?: any;
  Children?: ComponentItem[];
  IsMinWidthEnabled?: boolean;
  MinWidth?: number;
  IsMaxWidthEnabled?: boolean;
  MaxWidth?: number;
  IsFixedWidthEnabled?: boolean;
  FixedWidth?: number;
  HorizontalAlignment?: number;
  IsCustomMarginEnabled?: boolean;
  MarginLeft?: number;
  MarginTop?: number;
  MarginRight?: number;
  MarginBottom?: number;
  HideOnRule?: boolean;
  HidingRules?: any;
}

interface LineItem {
  IsMainLine: boolean;
  IsNotificationEnabled: boolean;
  Children: ComponentItem[];
  IslandSeparationMode?: number;
}

const layout = ref<LineItem[]>([]);

const componentMap = computed(() => {
  const map: Record<string, typeof componentTypes[0]> = {};
  componentTypes.forEach(c => { map[c.id] = c; });
  return map;
});

function getComponentInfo(id: string) {
  return componentMap.value[id.toLowerCase()] || componentMap.value[id] || { id, name: '未知组件', icon: '❓', category: '未知', hasSettings: false };
}

function isContainer(id: string) {
  return containerIds.has(id.toLowerCase()) || containerIds.has(id);
}

function getSettingsDefaults(id: string): any {
  const defaults: Record<string, any> = {
    'ee8f66bd-c423-4e7c-ab46-aa9976b00e08': { TextContent: '', FontSize: 16, FontColor: '#FFFFFF', UseCustomFontColor: true },
    '9e1af71d-8f77-4b21-a342-448787104dd9': { ShowSeconds: false, ShowRealTime: false, FlashTimeSeparator: true },
    '1db2017d-e374-4bc6-9d57-0b4adf03a6b8': { ShowExtraInfoOnTimePoint: true, IsCountdownEnabled: true, CountdownSeconds: 60, ScheduleSpacing: 1 },
    'ca495086-e297-4beb-9603-c5c1c1a8551e': { ShowAlerts: true, ShowRainTime: true, ShowMainWeatherInfo: true, MainWeatherInfoKind: 0 },
    '7c645d35-8151-48ba-b4ac-15017460d994': { CountDownName: '倒计时', FontSize: 16, IsCompactModeEnabled: false, ShowProgress: false },
    '7e19a113-d281-4f33-970a-834a0b78b5ad': { SlideSeconds: 15, SlideMode: 0, IsAnimationEnabled: false },
    '70fcd5ea-3fae-4e06-aca2-4f4df47f9acd': { SpeedPixelPerSecond: 40, IsPauseEnabled: true, PauseSeconds: 10 },
  };
  return defaults[id.toLowerCase()] || defaults[id] || {};
}

async function loadClients() {
  try {
    const res = await Alova.Get('/api/v1/clients_registry/all', { params: { pageIndex: 1, pageSize: 100 } });
    clientOptions.value = (res.items || []).map((c: any) => ({
      label: `${c.id} (${c.cuid?.substring(0, 8)}...)`,
      value: c.cuid,
    }));
  } catch (e) {
    console.error('加载客户端失败', e);
  }
}

async function loadConfig() {
  if (!selectedClient.value) return;
  isLoading.value = true;
  try {
    const config = await getClientComponents(selectedClient.value);
    const json = config?.layoutJson || '[]';
    try {
      const parsed = JSON.parse(json);
      if (parsed.Lines && Array.isArray(parsed.Lines)) {
        layout.value = parsed.Lines;
      } else if (Array.isArray(parsed)) {
        layout.value = parsed;
      } else {
        layout.value = [];
      }
    } catch {
      layout.value = [];
    }
  } catch (e) {
    console.error('加载配置失败', e);
  } finally {
    isLoading.value = false;
  }
}

async function saveConfig() {
  if (!selectedClient.value) return;
  isLoading.value = true;
  try {
    const json = JSON.stringify({ Lines: layout.value }, null, 2);
    await updateClientComponents(selectedClient.value, { layoutJson: json });
    message.success('配置已保存并推送到客户端');
  } catch (e: any) {
    message.error('保存失败: ' + (e.message || e));
  } finally {
    isLoading.value = false;
  }
}

async function requestUpload() {
  if (!selectedClient.value) return;
  try {
    await requestClientUpload(selectedClient.value);
    message.success('已请求客户端上报配置，请稍后刷新');
  } catch (e: any) {
    message.error('请求失败: ' + (e.message || e));
  }
}

function addLine() {
  layout.value.push({ IsMainLine: false, IsNotificationEnabled: true, Children: [] });
}

function removeLine(index: number) {
  layout.value.splice(index, 1);
}

function addComponent(lineIndex: number, componentId: string) {
  const info = getComponentInfo(componentId);
  const newComponent: ComponentItem = {
    Id: componentId,
    NameCache: info.name,
    Settings: getSettingsDefaults(componentId),
  };
  if (isContainer(componentId)) {
    newComponent.Children = [];
  }
  layout.value[lineIndex].Children.push(newComponent);
  addCompKeys.value[lineIndex] = (addCompKeys.value[lineIndex] || 0) + 1;
}

function removeComponent(lineIndex: number, compIndex: number) {
  layout.value[lineIndex].Children.splice(compIndex, 1);
}

function moveComponent(lineIndex: number, compIndex: number, direction: 'up' | 'down') {
  const children = layout.value[lineIndex].Children;
  const newIndex = direction === 'up' ? compIndex - 1 : compIndex + 1;
  if (newIndex < 0 || newIndex >= children.length) return;
  const temp = children[compIndex];
  children[compIndex] = children[newIndex];
  children[newIndex] = temp;
}

function addChildToContainer(lineIndex: number, compIndex: number, componentId: string) {
  const comp = layout.value[lineIndex].Children[compIndex];
  if (!comp.Children) comp.Children = [];
  const info = getComponentInfo(componentId);
  comp.Children.push({ Id: componentId, NameCache: info.name, Settings: getSettingsDefaults(componentId) });
  const key = `${lineIndex}-${compIndex}`;
  addChildKeys.value[key] = (addChildKeys.value[key] || 0) + 1;
}

function removeChildFromContainer(lineIndex: number, compIndex: number, childIndex: number) {
  const comp = layout.value[lineIndex].Children[compIndex];
  if (comp.Children) comp.Children.splice(childIndex, 1);
}

function openSettings(lineIndex: number, compIndex: number, childIndex?: number) {
  editingComponent.value = { lineIndex, compIndex, childIndex };
}

function getEditingComp(): ComponentItem | null {
  if (!editingComponent.value) return null;
  const { lineIndex, compIndex, childIndex } = editingComponent.value;
  const comp = layout.value[lineIndex]?.Children[compIndex];
  if (!comp) return null;
  if (childIndex !== undefined && comp.Children) return comp.Children[childIndex];
  return comp;
}

function openJsonEditor() {
  rawJson.value = JSON.stringify({ Lines: layout.value }, null, 2);
  showJsonEditor.value = true;
}

function applyJsonEdit() {
  try {
    const parsed = JSON.parse(rawJson.value);
    if (parsed.Lines && Array.isArray(parsed.Lines)) {
      layout.value = parsed.Lines;
    } else if (Array.isArray(parsed)) {
      layout.value = parsed;
    }
    showJsonEditor.value = false;
    message.success('JSON 已应用');
  } catch (e: any) {
    message.error('JSON 格式错误: ' + e.message);
  }
}

onMounted(loadClients);
</script>

<template>
  <div class="d-flex flex-col gap-y-4">
    <n-card title="组件配置管理">
      <n-form label-placement="left" label-width="auto" style="max-width: 700px">
        <n-form-item label="选择客户端">
          <n-select v-model:value="selectedClient" :options="clientOptions" filterable placeholder="选择客户端实例" @update:value="loadConfig" />
        </n-form-item>
      </n-form>

      <div v-if="selectedClient">
        <n-space class="mb-4">
          <n-button @click="requestUpload" size="small">请求客户端上报</n-button>
          <n-button @click="loadConfig" size="small">刷新</n-button>
          <n-button @click="addLine" size="small" type="primary">添加行</n-button>
          <n-button @click="openJsonEditor" size="small">编辑 JSON</n-button>
          <n-button @click="saveConfig" size="small" type="success" :loading="isLoading">保存并推送</n-button>
        </n-space>
      </div>
    </n-card>

    <n-card v-if="selectedClient && layout.length > 0" title="主界面布局">
      <div class="layout-editor">
        <div v-for="(line, lineIndex) in layout" :key="lineIndex" class="line-container">
          <div class="line-header">
            <n-space align="center">
              <n-tag :type="line.IsMainLine ? 'success' : 'default'" size="small">
                {{ line.IsMainLine ? '主行' : '行 ' + (lineIndex + 1) }}
              </n-tag>
              <n-tag v-if="line.IsNotificationEnabled" type="info" size="small">通知</n-tag>
              <n-button size="tiny" @click="line.IsMainLine = !line.IsMainLine">
                {{ line.IsMainLine ? '取消主行' : '设为主行' }}
              </n-button>
              <n-button size="tiny" @click="line.IsNotificationEnabled = !line.IsNotificationEnabled">
                {{ line.IsNotificationEnabled ? '关闭通知' : '开启通知' }}
              </n-button>
              <n-button size="tiny" @click="lineIndex > 0 && moveComponent(lineIndex, 0, 'up')" :disabled="lineIndex === 0">↑</n-button>
              <n-button size="tiny" @click="lineIndex < layout.length - 1 && (() => { const t = layout[lineIndex]; layout[lineIndex] = layout[lineIndex+1]; layout[lineIndex+1] = t })()" :disabled="lineIndex === layout.length - 1">↓</n-button>
              <n-popconfirm @positive-click="removeLine(lineIndex)">
                <template #trigger><n-button size="tiny" type="error">删除行</n-button></template>
                确定删除此行？
              </n-popconfirm>
            </n-space>
          </div>

          <div class="components-row">
            <div v-for="(comp, compIndex) in line.Children" :key="compIndex" class="component-card">
              <div class="component-info">
                <span class="component-icon">{{ getComponentInfo(comp.Id).icon }}</span>
                <span class="component-name">{{ getComponentInfo(comp.Id).name }}</span>
              </div>
              <div class="component-actions">
                <n-button size="tiny" @click="openSettings(lineIndex, compIndex)" v-if="getComponentInfo(comp.Id).hasSettings">⚙</n-button>
                <n-button size="tiny" @click="moveComponent(lineIndex, compIndex, 'up')" :disabled="compIndex === 0">↑</n-button>
                <n-button size="tiny" @click="moveComponent(lineIndex, compIndex, 'down')" :disabled="compIndex === line.Children.length - 1">↓</n-button>
                <n-popconfirm @positive-click="removeComponent(lineIndex, compIndex)">
                  <template #trigger><n-button size="tiny" type="error">×</n-button></template>
                  确定删除此组件？
                </n-popconfirm>
              </div>

              <div v-if="isContainer(comp.Id) && comp.Children" class="container-children">
                <div class="container-header">子组件 ({{ comp.Children.length }})</div>
                <div class="children-list">
                  <div v-for="(child, childIndex) in comp.Children" :key="childIndex" class="child-card">
                    <span>{{ getComponentInfo(child.Id).icon }} {{ getComponentInfo(child.Id).name }}</span>
                    <n-button size="tiny" @click="openSettings(lineIndex, compIndex, childIndex)" v-if="getComponentInfo(child.Id).hasSettings">⚙</n-button>
                    <n-button size="tiny" type="error" @click="removeChildFromContainer(lineIndex, compIndex, childIndex)">×</n-button>
                  </div>
                  <n-select size="tiny" placeholder="添加子组件"
                    :key="addChildKeys[`${lineIndex}-${compIndex}`] || 0"
                    :options="componentTypes.map(c => ({ label: `${c.icon} ${c.name}`, value: c.id }))"
                    @update:value="(val: string) => addChildToContainer(lineIndex, compIndex, val)" style="width: 150px" />
                </div>
              </div>
            </div>

            <n-select size="small" placeholder="+ 添加组件"
              :key="addCompKeys[lineIndex] || 0"
              :options="componentTypes.map(c => ({ label: `${c.icon} ${c.name}`, value: c.id }))"
              @update:value="(val: string) => addComponent(lineIndex, val)" style="width: 150px" />
          </div>
        </div>
      </div>
    </n-card>

    <!-- 组件属性编辑弹窗 -->
    <n-modal v-model:show="editingComponent" title="组件属性" preset="dialog" style="width: 600px">
      <template v-if="editingComp">
        <n-form label-placement="left" label-width="120px">
          <n-form-item label="组件名称">
            <n-input v-model:value="editingComp.NameCache" />
          </n-form-item>

          <!-- 文本组件 -->
          <template v-if="editingComp.Id.toLowerCase() === 'ee8f66bd-c423-4e7c-ab46-aa9976b00e08'">
            <n-form-item label="文本内容">
              <n-input v-model:value="editingComp.Settings.TextContent" type="textarea" />
            </n-form-item>
            <n-form-item label="字号">
              <n-input-number v-model:value="editingComp.Settings.FontSize" :min="8" :max="72" />
            </n-form-item>
            <n-form-item label="自定义颜色">
              <n-switch v-model:value="editingComp.Settings.UseCustomFontColor" />
            </n-form-item>
            <n-form-item label="字体颜色" v-if="editingComp.Settings.UseCustomFontColor">
              <n-color-picker v-model:value="editingComp.Settings.FontColor" />
            </n-form-item>
          </template>

          <!-- 时钟组件 -->
          <template v-if="editingComp.Id.toLowerCase() === '9e1af71d-8f77-4b21-a342-448787104dd9'">
            <n-form-item label="显示秒数">
              <n-switch v-model:value="editingComp.Settings.ShowSeconds" />
            </n-form-item>
            <n-form-item label="显示实时时间">
              <n-switch v-model:value="editingComp.Settings.ShowRealTime" />
            </n-form-item>
            <n-form-item label="闪烁分隔符">
              <n-switch v-model:value="editingComp.Settings.FlashTimeSeparator" />
            </n-form-item>
          </template>

          <!-- 课程表组件 -->
          <template v-if="editingComp.Id.toLowerCase() === '1db2017d-e374-4bc6-9d57-0b4adf03a6b8'">
            <n-form-item label="显示额外信息">
              <n-switch v-model:value="editingComp.Settings.ShowExtraInfoOnTimePoint" />
            </n-form-item>
            <n-form-item label="启用倒计时">
              <n-switch v-model:value="editingComp.Settings.IsCountdownEnabled" />
            </n-form-item>
            <n-form-item label="倒计时秒数">
              <n-input-number v-model:value="editingComp.Settings.CountdownSeconds" :min="0" :max="300" />
            </n-form-item>
            <n-form-item label="课程间距">
              <n-input-number v-model:value="editingComp.Settings.ScheduleSpacing" :min="0" :max="10" />
            </n-form-item>
            <n-form-item label="仅显示当前课程">
              <n-switch v-model:value="editingComp.Settings.ShowCurrentLessonOnlyOnClass" />
            </n-form-item>
            <n-form-item label="隐藏已完成课程">
              <n-switch v-model:value="editingComp.Settings.HideFinishedClass" />
            </n-form-item>
          </template>

          <!-- 天气组件 -->
          <template v-if="editingComp.Id.toLowerCase() === 'ca495086-e297-4beb-9603-c5c1c1a8551e'">
            <n-form-item label="显示预警">
              <n-switch v-model:value="editingComp.Settings.ShowAlerts" />
            </n-form-item>
            <n-form-item label="显示降雨时间">
              <n-switch v-model:value="editingComp.Settings.ShowRainTime" />
            </n-form-item>
            <n-form-item label="显示天气信息">
              <n-switch v-model:value="editingComp.Settings.ShowMainWeatherInfo" />
            </n-form-item>
            <n-form-item label="天气信息类型">
              <n-select v-model:value="editingComp.Settings.MainWeatherInfoKind" :options="[
                { label: '当前天气+温度', value: 0 },
                { label: '湿度', value: 1 },
                { label: '风力', value: 2 },
                { label: '空气质量', value: 3 },
                { label: '气压', value: 4 },
                { label: '体感温度', value: 5 },
              ]" />
            </n-form-item>
          </template>

          <!-- 倒计时组件 -->
          <template v-if="editingComp.Id.toLowerCase() === '7c645d35-8151-48ba-b4ac-15017460d994'">
            <n-form-item label="倒计时名称">
              <n-input v-model:value="editingComp.Settings.CountDownName" />
            </n-form-item>
            <n-form-item label="字号">
              <n-input-number v-model:value="editingComp.Settings.FontSize" :min="8" :max="72" />
            </n-form-item>
            <n-form-item label="紧凑模式">
              <n-switch v-model:value="editingComp.Settings.IsCompactModeEnabled" />
            </n-form-item>
            <n-form-item label="显示进度">
              <n-switch v-model:value="editingComp.Settings.ShowProgress" />
            </n-form-item>
            <n-form-item label="自定义格式">
              <n-input v-model:value="editingComp.Settings.CustomStringFormat" />
            </n-form-item>
          </template>

          <!-- 轮播容器 -->
          <template v-if="editingComp.Id.toLowerCase() === '7e19a113-d281-4f33-970a-834a0b78b5ad'">
            <n-form-item label="轮播秒数">
              <n-input-number v-model:value="editingComp.Settings.SlideSeconds" :min="1" :max="120" />
            </n-form-item>
            <n-form-item label="轮播模式">
              <n-select v-model:value="editingComp.Settings.SlideMode" :options="[
                { label: '循环', value: 0 },
                { label: '随机', value: 1 },
                { label: '乒乓', value: 2 },
              ]" />
            </n-form-item>
            <n-form-item label="启用动画">
              <n-switch v-model:value="editingComp.Settings.IsAnimationEnabled" />
            </n-form-item>
          </template>

          <!-- 滚动容器 -->
          <template v-if="editingComp.Id.toLowerCase() === '70fcd5ea-3fae-4e06-aca2-4f4df47f9acd'">
            <n-form-item label="速度(px/s)">
              <n-input-number v-model:value="editingComp.Settings.SpeedPixelPerSecond" :min="1" :max="200" />
            </n-form-item>
            <n-form-item label="启用暂停">
              <n-switch v-model:value="editingComp.Settings.IsPauseEnabled" />
            </n-form-item>
            <n-form-item label="暂停秒数">
              <n-input-number v-model:value="editingComp.Settings.PauseSeconds" :min="0" :max="60" />
            </n-form-item>
          </template>

          <!-- 通用布局设置 -->
          <n-divider>布局设置</n-divider>
          <n-form-item label="对齐方式">
            <n-select v-model:value="editingComp.HorizontalAlignment" :options="[
              { label: '左对齐', value: 0 },
              { label: '居中', value: 1 },
              { label: '右对齐', value: 2 },
              { label: '拉伸', value: 3 },
            ]" />
          </n-form-item>
          <n-form-item label="最小宽度">
            <n-switch v-model:value="editingComp.IsMinWidthEnabled" />
            <n-input-number v-if="editingComp.IsMinWidthEnabled" v-model:value="editingComp.MinWidth" :min="0" :max="2000" style="margin-left: 8px" />
          </n-form-item>
          <n-form-item label="最大宽度">
            <n-switch v-model:value="editingComp.IsMaxWidthEnabled" />
            <n-input-number v-if="editingComp.IsMaxWidthEnabled" v-model:value="editingComp.MaxWidth" :min="0" :max="2000" style="margin-left: 8px" />
          </n-form-item>
          <n-form-item label="固定宽度">
            <n-switch v-model:value="editingComp.IsFixedWidthEnabled" />
            <n-input-number v-if="editingComp.IsFixedWidthEnabled" v-model:value="editingComp.FixedWidth" :min="0" :max="2000" style="margin-left: 8px" />
          </n-form-item>

          <!-- 隐藏规则 -->
          <n-divider>隐藏规则</n-divider>
          <n-form-item label="按规则隐藏">
            <n-switch v-model:value="editingComp.HideOnRule" />
          </n-form-item>
          <div v-if="editingComp.HideOnRule" style="margin-bottom: 12px">
            <RulesetEditor
              :value="editingComp.HidingRules ?? (editingComp.HidingRules = { Mode: 0, IsReversed: false, Groups: [] })"
              @update-value="(v: any) => editingComp.HidingRules = v"
            />
          </div>
        </n-form>
      </template>
      <template #action>
        <n-button @click="editingComponent = null" type="primary">确定</n-button>
      </template>
    </n-modal>

    <n-modal v-model:show="showJsonEditor" title="编辑 JSON" preset="dialog" style="width: 800px">
      <n-input v-model:value="rawJson" type="textarea" :rows="20" />
      <template #action>
        <n-button @click="applyJsonEdit" type="primary">应用</n-button>
      </template>
    </n-modal>
  </div>
</template>

<style scoped>
.layout-editor { display: flex; flex-direction: column; gap: 16px; }
.line-container { border: 1px solid #e0e0e0; border-radius: 8px; padding: 12px; background: #fafafa; }
.line-header { margin-bottom: 8px; padding-bottom: 8px; border-bottom: 1px solid #e0e0e0; }
.components-row { display: flex; flex-wrap: wrap; gap: 8px; align-items: flex-start; }
.component-card { border: 1px solid #d0d0d0; border-radius: 6px; padding: 8px; background: white; min-width: 120px; }
.component-info { display: flex; align-items: center; gap: 6px; margin-bottom: 4px; }
.component-icon { font-size: 18px; }
.component-name { font-size: 13px; font-weight: 500; }
.component-actions { display: flex; gap: 4px; }
.container-children { margin-top: 8px; padding-top: 8px; border-top: 1px dashed #c0c0c0; }
.container-header { font-size: 11px; color: #888; margin-bottom: 4px; }
.children-list { display: flex; flex-wrap: wrap; gap: 4px; align-items: center; }
.child-card { display: flex; align-items: center; gap: 4px; border: 1px solid #e0e0e0; border-radius: 4px; padding: 2px 6px; background: #f5f5f5; font-size: 12px; }
</style>
