<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { NCard, NButton, NSelect, useMessage, NSpace, NTag, NModal, NForm, NFormItem, NInput, NSwitch, NPopconfirm, NInputNumber, NDivider, NCollapse, NCollapseItem } from 'naive-ui';
import { getClientAutomation, updateClientAutomation, requestAutomationUpload } from '@/api/automation/index';
import { Alova } from '@/utils/http/alova/index';
import RulesetEditor from './RulesetEditor.vue';

const message = useMessage();
const selectedClient = ref('');
const clientOptions = ref<{ label: string; value: string }[]>([]);
const isLoading = ref(false);
const showJsonEditor = ref(false);
const rawJson = ref('');
const editingTrigger = ref<{ wfIndex: number; trigIndex: number } | null>(null);
const editingAction = ref<{ wfIndex: number; actIndex: number } | null>(null);
const editingTriggerItem = computed(() => {
  if (!editingTrigger.value) return null;
  return workflows.value[editingTrigger.value.wfIndex]?.Triggers[editingTrigger.value.trigIndex] ?? null;
});
const editingActionItem = computed(() => {
  if (!editingAction.value) return null;
  return workflows.value[editingAction.value.wfIndex]?.ActionSet.Actions[editingAction.value.actIndex] ?? null;
});

const triggerTypes = [
  { id: 'classisland.cron', name: 'cron 定时', icon: '⏰', hasSettings: true, settingsFields: [{ key: 'CronExpression', label: 'Cron 表达式', type: 'string', default: '* * * * *' }] },
  { id: 'classisland.lessons.preTimePoint', name: '特定时间点前', icon: '⏳', hasSettings: true, settingsFields: [
    { key: 'TargetState', label: '目标状态', type: 'select', options: [
      { label: '上课', value: 1 }, { label: '准备上课', value: 2 }, { label: '课间休息', value: 3 }, { label: '放学', value: 4 }
    ], default: 1 },
    { key: 'TimeSeconds', label: '提前秒数', type: 'number', default: 60 },
  ]},
  { id: 'classisland.signal', name: '收到信号时', icon: '📡', hasSettings: true, settingsFields: [
    { key: 'SignalName', label: '信号名称', type: 'string', default: '' },
    { key: 'IsRevert', label: '触发恢复', type: 'bool', default: false },
  ]},
  { id: 'classisland.uri', name: '调用 Uri 时', icon: '🔗', hasSettings: true, settingsFields: [
    { key: 'UriSuffix', label: 'Uri 后缀', type: 'string', default: '' },
  ]},
  { id: 'classisland.trayMenu', name: '从托盘菜单运行', icon: '📋', hasSettings: true, settingsFields: [
    { key: 'Header', label: '菜单标题', type: 'string', default: '' },
    { key: 'IsRevert', label: '触发恢复', type: 'bool', default: false },
  ]},
  { id: 'classisland.lifetime.startup', name: '应用启动时', icon: '🚀', hasSettings: false, settingsFields: [] },
  { id: 'classisland.lifetime.stopping', name: '应用退出时', icon: '🛑', hasSettings: false, settingsFields: [] },
  { id: 'classisland.lessons.onClass', name: '上课时', icon: '📖', hasSettings: false, settingsFields: [] },
  { id: 'classisland.lessons.onBreakingTime', name: '课间休息时', icon: '☕', hasSettings: false, settingsFields: [] },
  { id: 'classisland.lessons.onAfterSchool', name: '放学时', icon: '🏠', hasSettings: false, settingsFields: [] },
  { id: 'classisland.lessons.currentTimeStateChanged', name: '时间状态变化时', icon: '🔄', hasSettings: false, settingsFields: [] },
  { id: 'classisland.ruleSet.rulesetChanged', name: '规则集更新时', icon: '📐', hasSettings: false, settingsFields: [] },
];

const actionTypes = [
  { id: 'classisland.broadcastSignal', name: '广播信号', icon: '📡', hasSettings: true, revertable: true, settingsFields: [
    { key: 'SignalName', label: '信号名称', type: 'string', default: '' },
    { key: 'IsRevert', label: '触发恢复', type: 'bool', default: false },
  ]},
  { id: 'classisland.os.run', name: '运行', icon: '▶️', hasSettings: true, revertable: false, settingsFields: [
    { key: 'RunType', label: '运行类型', type: 'select', options: [
      { label: '应用程序', value: 0 }, { label: '命令', value: 1 }, { label: '文件', value: 2 }, { label: '文件夹', value: 3 }, { label: 'URL', value: 4 }
    ], default: 0 },
    { key: 'Value', label: '值', type: 'string', default: '' },
    { key: 'Args', label: '参数', type: 'string', default: '' },
  ]},
  { id: 'classisland.showNotification', name: '显示提醒', icon: '🔔', hasSettings: true, revertable: false, settingsFields: [
    { key: 'Content', label: '内容', type: 'string', default: '' },
    { key: 'Mask', label: '遮罩', type: 'string', default: '' },
    { key: 'IsContentSpeechEnabled', label: '内容语音', type: 'bool', default: true },
    { key: 'IsSoundEffectEnabled', label: '音效', type: 'bool', default: true },
    { key: 'IsTopmostEnabled', label: '置顶', type: 'bool', default: true },
    { key: 'MaskDurationSeconds', label: '遮罩秒数', type: 'number', default: 5 },
    { key: 'ContentDurationSeconds', label: '内容秒数', type: 'number', default: 10 },
  ]},
  { id: 'classisland.action.sleep', name: '等待时长', icon: '💤', hasSettings: true, revertable: false, settingsFields: [
    { key: 'Value', label: '等待秒数', type: 'number', default: 5 },
  ]},
  { id: 'classisland.settings', name: '应用设置', icon: '⚙️', hasSettings: true, revertable: true, settingsFields: [
    { key: 'Name', label: '设置名', type: 'string', default: '' },
    { key: 'Value', label: '值', type: 'string', default: '' },
    { key: 'Mode', label: '模式', type: 'number', default: 0 },
  ]},
  { id: 'classisland.notification.weather', name: '显示天气提醒', icon: '🌤️', hasSettings: true, revertable: false, settingsFields: [
    { key: 'NotificationKind', label: '提醒类型', type: 'select', options: [
      { label: '天气预报', value: 0 }, { label: '气象预警', value: 1 }, { label: '逐时预报', value: 2 }
    ], default: 0 },
  ]},
  { id: 'classisland.app.quit', name: '退出 ClassIsland', icon: '🚪', hasSettings: false, revertable: false, settingsFields: [] },
  { id: 'classisland.app.restart', name: '重启 ClassIsland', icon: '🔄', hasSettings: true, revertable: false, settingsFields: [
    { key: 'Value', label: '强制重启', type: 'bool', default: false },
  ]},
];

const triggerMap = computed(() => {
  const m: Record<string, typeof triggerTypes[0]> = {};
  triggerTypes.forEach(t => { m[t.id] = t; });
  return m;
});

const actionMap = computed(() => {
  const m: Record<string, typeof actionTypes[0]> = {};
  actionTypes.forEach(a => { m[a.id] = a; });
  return m;
});

function getTriggerInfo(id: string) {
  return triggerMap.value[id] || { id, name: id, icon: '❓', hasSettings: false, settingsFields: [] };
}

function getActionInfo(id: string) {
  return actionMap.value[id] || { id, name: id, icon: '❓', hasSettings: false, revertable: false, settingsFields: [] };
}

interface Workflow {
  Name: string;
  Triggers: { Id: string; Settings: any }[];
  IsConditionEnabled: boolean;
  Ruleset?: any;
  ActionSet: {
    Name: string;
    Actions: { Id: string; Settings: any }[];
    IsEnabled: boolean;
    IsRevertEnabled: boolean;
  };
}

const workflows = ref<Workflow[]>([]);

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
    const config = await getClientAutomation(selectedClient.value);
    const json = config?.workflowsJson || '[]';
    try {
      const parsed = JSON.parse(json);
      workflows.value = Array.isArray(parsed) ? parsed : [];
    } catch {
      workflows.value = [];
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
    const json = JSON.stringify(workflows.value, null, 2);
    await updateClientAutomation(selectedClient.value, { workflowsJson: json });
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
    await requestAutomationUpload(selectedClient.value);
    message.success('已请求客户端上报配置，请稍后刷新');
  } catch (e: any) {
    message.error('请求失败: ' + (e.message || e));
  }
}

function addWorkflow() {
  workflows.value.push({
    Name: '新工作流',
    Triggers: [],
    IsConditionEnabled: false,
    ActionSet: { Name: '新行动组', Actions: [], IsEnabled: true, IsRevertEnabled: false },
  });
}

function removeWorkflow(index: number) {
  workflows.value.splice(index, 1);
}

function addTrigger(wfIndex: number, triggerId: string) {
  const info = getTriggerInfo(triggerId);
  const settings: any = {};
  if (info.settingsFields) {
    info.settingsFields.forEach(f => { settings[f.key] = f.default; });
  }
  workflows.value[wfIndex].Triggers.push({ Id: triggerId, Settings: settings });
}

function removeTrigger(wfIndex: number, trigIndex: number) {
  workflows.value[wfIndex].Triggers.splice(trigIndex, 1);
}

function addAction(wfIndex: number, actionId: string) {
  const info = getActionInfo(actionId);
  const settings: any = {};
  if (info.settingsFields) {
    info.settingsFields.forEach(f => { settings[f.key] = f.default; });
  }
  workflows.value[wfIndex].ActionSet.Actions.push({ Id: actionId, Settings: settings });
}

function removeAction(wfIndex: number, actIndex: number) {
  workflows.value[wfIndex].ActionSet.Actions.splice(actIndex, 1);
}

function openTriggerSettings(wfIndex: number, trigIndex: number) {
  editingTrigger.value = { wfIndex, trigIndex };
}

function openActionSettings(wfIndex: number, actIndex: number) {
  editingAction.value = { wfIndex, actIndex };
}

function openJsonEditor() {
  rawJson.value = JSON.stringify(workflows.value, null, 2);
  showJsonEditor.value = true;
}

function applyJsonEdit() {
  try {
    const parsed = JSON.parse(rawJson.value);
    workflows.value = Array.isArray(parsed) ? parsed : [];
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
    <n-card title="自动化管理">
      <n-form label-placement="left" label-width="auto" style="max-width: 700px">
        <n-form-item label="选择客户端">
          <n-select v-model:value="selectedClient" :options="clientOptions" filterable placeholder="选择客户端实例" @update:value="loadConfig" />
        </n-form-item>
      </n-form>
      <div v-if="selectedClient">
        <n-space class="mb-4">
          <n-button @click="requestUpload" size="small">请求客户端上报</n-button>
          <n-button @click="loadConfig" size="small">刷新</n-button>
          <n-button @click="addWorkflow" size="small" type="primary">添加工作流</n-button>
          <n-button @click="openJsonEditor" size="small">编辑 JSON</n-button>
          <n-button @click="saveConfig" size="small" type="success" :loading="isLoading">保存并推送</n-button>
        </n-space>
      </div>
    </n-card>

    <n-card v-if="selectedClient && workflows.length > 0" title="工作流配置">
      <n-collapse>
        <n-collapse-item v-for="(workflow, wfIndex) in workflows" :key="wfIndex" :title="`工作流 ${wfIndex + 1}: ${workflow.Name}`" :name="wfIndex">
          <template #header-extra>
            <n-space size="small">
              <n-tag size="small">{{ workflow.Triggers.length }} 触发器</n-tag>
              <n-tag size="small">{{ workflow.ActionSet.Actions.length }} 动作</n-tag>
              <n-popconfirm @positive-click="removeWorkflow(wfIndex)">
                <template #trigger><n-button size="tiny" type="error" @click.stop>删除</n-button></template>
                确定删除此工作流？
              </n-popconfirm>
            </n-space>
          </template>

          <n-form label-placement="left" label-width="100px">
            <n-form-item label="工作流名称">
              <n-input v-model:value="workflow.Name" style="max-width: 300px" />
            </n-form-item>
          </n-form>

          <!-- 触发器 -->
          <div class="section">
            <div class="section-title">触发器</div>
            <div class="items-list">
              <div v-for="(trigger, trigIndex) in workflow.Triggers" :key="trigIndex" class="item-card">
                <span class="item-icon">{{ getTriggerInfo(trigger.Id).icon }}</span>
                <span class="item-name">{{ getTriggerInfo(trigger.Id).name }}</span>
                <n-button v-if="getTriggerInfo(trigger.Id).hasSettings" size="tiny" @click="openTriggerSettings(wfIndex, trigIndex)">⚙</n-button>
                <n-button size="tiny" type="error" @click="removeTrigger(wfIndex, trigIndex)">×</n-button>
              </div>
              <n-select size="tiny" placeholder="+ 添加触发器"
                :options="triggerTypes.map(t => ({ label: `${t.icon} ${t.name}`, value: t.id }))"
                @update:value="(val: string) => addTrigger(wfIndex, val)" style="width: 200px" />
            </div>
          </div>

          <!-- 条件 -->
          <div class="section">
            <div class="section-title">
              条件（规则集）
              <n-switch v-model:value="workflow.IsConditionEnabled" size="small" style="margin-left: 8px" />
            </div>
            <div v-if="workflow.IsConditionEnabled" style="margin-top: 8px">
              <RulesetEditor
                :value="workflow.Ruleset ?? (workflow.Ruleset = { Mode: 0, IsReversed: false, Groups: [] })"
                @update:value="(v: any) => workflow.Ruleset = v"
              />
            </div>
          </div>

          <!-- 动作 -->
          <div class="section">
            <div class="section-title">
              动作组: {{ workflow.ActionSet.Name }}
              <n-input v-model:value="workflow.ActionSet.Name" size="tiny" style="width: 150px; margin-left: 8px" />
            </div>
            <n-space size="small" class="mb-2">
              <n-switch v-model:value="workflow.ActionSet.IsEnabled" size="small">
                <template #checked>已启用</template>
                <template #unchecked>已禁用</template>
              </n-switch>
              <n-switch v-model:value="workflow.ActionSet.IsRevertEnabled" size="small">
                <template #checked>恢复已启用</template>
                <template #unchecked>恢复已禁用</template>
              </n-switch>
            </n-space>
            <div class="items-list">
              <div v-for="(action, actIndex) in workflow.ActionSet.Actions" :key="actIndex" class="item-card">
                <span class="item-icon">{{ getActionInfo(action.Id).icon }}</span>
                <span class="item-name">{{ getActionInfo(action.Id).name }}</span>
                <n-button v-if="getActionInfo(action.Id).hasSettings" size="tiny" @click="openActionSettings(wfIndex, actIndex)">⚙</n-button>
                <n-button size="tiny" type="error" @click="removeAction(wfIndex, actIndex)">×</n-button>
              </div>
              <n-select size="tiny" placeholder="+ 添加动作"
                :options="actionTypes.map(a => ({ label: `${a.icon} ${a.name}`, value: a.id }))"
                @update:value="(val: string) => addAction(wfIndex, val)" style="width: 200px" />
            </div>
          </div>
        </n-collapse-item>
      </n-collapse>
    </n-card>

    <!-- 触发器设置弹窗 -->
    <n-modal v-model:show="editingTrigger" title="触发器设置" preset="dialog" style="width: 500px">
      <template v-if="editingTriggerItem">
        <n-form label-placement="left" label-width="100px">
          <n-form-item v-for="field in getTriggerInfo(editingTriggerItem.Id).settingsFields" :key="field.key" :label="field.label">
            <n-input v-if="field.type === 'string'" v-model:value="editingTriggerItem.Settings[field.key]" />
            <n-input-number v-else-if="field.type === 'number'" v-model:value="editingTriggerItem.Settings[field.key]" />
            <n-switch v-else-if="field.type === 'bool'" v-model:value="editingTriggerItem.Settings[field.key]" />
            <n-select v-else-if="field.type === 'select'" v-model:value="editingTriggerItem.Settings[field.key]" :options="field.options || []" />
          </n-form-item>
        </n-form>
      </template>
      <template #action>
        <n-button @click="editingTrigger = null" type="primary">确定</n-button>
      </template>
    </n-modal>

    <!-- 动作设置弹窗 -->
    <n-modal v-model:show="editingAction" title="动作设置" preset="dialog" style="width: 500px">
      <template v-if="editingActionItem">
        <n-form label-placement="left" label-width="100px">
          <n-form-item v-for="field in getActionInfo(editingActionItem.Id).settingsFields" :key="field.key" :label="field.label">
            <n-input v-if="field.type === 'string'" v-model:value="editingActionItem.Settings[field.key]" />
            <n-input-number v-else-if="field.type === 'number'" v-model:value="editingActionItem.Settings[field.key]" />
            <n-switch v-else-if="field.type === 'bool'" v-model:value="editingActionItem.Settings[field.key]" />
            <n-select v-else-if="field.type === 'select'" v-model:value="editingActionItem.Settings[field.key]" :options="field.options || []" />
          </n-form-item>
        </n-form>
      </template>
      <template #action>
        <n-button @click="editingAction = null" type="primary">确定</n-button>
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
.section { margin-bottom: 16px; padding: 12px; border: 1px solid #e0e0e0; border-radius: 6px; }
.section-title { font-weight: 500; margin-bottom: 8px; display: flex; align-items: center; }
.items-list { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
.item-card { display: flex; align-items: center; gap: 6px; border: 1px solid #d0d0d0; border-radius: 6px; padding: 6px 10px; background: white; }
.item-icon { font-size: 16px; }
.item-name { font-size: 13px; }
</style>
