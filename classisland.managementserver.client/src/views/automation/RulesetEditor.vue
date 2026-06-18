<script setup lang="ts">
import { computed } from 'vue';

interface Rule {
  Id: string;
  Settings?: any;
  IsReversed?: boolean;
  State?: number;
}

interface RuleGroup {
  Mode: number; // 0=Or, 1=And
  IsReversed?: boolean;
  IsEnabled?: boolean;
  Rules: Rule[];
  State?: number;
}

interface Ruleset {
  Mode: number; // 0=Or, 1=And
  IsReversed?: boolean;
  Groups: RuleGroup[];
  State?: number;
}

const props = defineProps<{
  value: Ruleset;
}>();

const emit = defineEmits<{
  (e: 'update:value', val: Ruleset): void;
}>();

const ruleset = computed({
  get: () => props.value,
  set: (val) => emit('update:value', val),
});

const ruleTypes = [
  { id: 'classisland.test.true', name: '总是为真', icon: '✅' },
  { id: 'classisland.test.false', name: '总是为假', icon: '❌' },
  { id: 'classisland.windows.className', name: '前台窗口类名', icon: '🪟' },
  { id: 'classisland.windows.text', name: '前台窗口标题', icon: '📝' },
  { id: 'classisland.windows.status', name: '前台窗口状态', icon: '📊' },
  { id: 'classisland.windows.processName', name: '前台窗口进程', icon: '⚙️' },
  { id: 'classisland.lessons.currentSubject', name: '当前科目是', icon: '📚' },
  { id: 'classisland.lessons.nextSubject', name: '下节课科目是', icon: '📖' },
  { id: 'classisland.lessons.previousSubject', name: '上节课科目是', icon: '📕' },
  { id: 'classisland.lessons.timeState', name: '当前时间状态', icon: '⏰' },
  { id: 'classisland.weather.currentWeather', name: '当前天气', icon: '🌤️' },
  { id: 'classisland.weather.tomorrowWeather', name: '明天天气', icon: '🌤️' },
  { id: 'classisland.weather.hasWeatherAlert', name: '存在气象预警', icon: '⚠️' },
  { id: 'classisland.weather.rainTime', name: '距离降水时间', icon: '🌧️' },
  { id: 'classisland.weather.sunRiseSet', name: '日出/日落', icon: '🌅' },
];

function addGroup() {
  const rs = { ...ruleset.value };
  rs.Groups = [...rs.Groups, { Mode: 1, IsEnabled: true, Rules: [{ Id: 'classisland.test.true' }] }];
  ruleset.value = rs;
}

function removeGroup(gIdx: number) {
  const rs = { ...ruleset.value };
  rs.Groups = rs.Groups.filter((_, i) => i !== gIdx);
  ruleset.value = rs;
}

function addRule(gIdx: number) {
  const rs = { ...ruleset.value };
  rs.Groups = rs.Groups.map((g, i) => {
    if (i !== gIdx) return g;
    return { ...g, Rules: [...g.Rules, { Id: 'classisland.test.true' }] };
  });
  ruleset.value = rs;
}

function removeRule(gIdx: number, rIdx: number) {
  const rs = { ...ruleset.value };
  rs.Groups = rs.Groups.map((g, i) => {
    if (i !== gIdx) return g;
    return { ...g, Rules: g.Rules.filter((_, j) => j !== rIdx) };
  });
  ruleset.value = rs;
}

function updateGroupProp(gIdx: number, key: string, val: any) {
  const rs = { ...ruleset.value };
  rs.Groups = rs.Groups.map((g, i) => {
    if (i !== gIdx) return g;
    return { ...g, [key]: val };
  });
  ruleset.value = rs;
}

function updateRuleProp(gIdx: number, rIdx: number, key: string, val: any) {
  const rs = { ...ruleset.value };
  rs.Groups = rs.Groups.map((g, i) => {
    if (i !== gIdx) return g;
    return {
      ...g,
      Rules: g.Rules.map((r, j) => {
        if (j !== rIdx) return r;
        return { ...r, [key]: val };
      }),
    };
  });
  ruleset.value = rs;
}

function getRuleName(id: string) {
  return ruleTypes.find((r) => r.id === id)?.name ?? id;
}
function getRuleIcon(id: string) {
  return ruleTypes.find((r) => r.id === id)?.icon ?? '❓';
}
</script>

<template>
  <div class="ruleset-editor">
    <div class="ruleset-header">
      <n-space align="center" size="small">
        <n-tag :type="ruleset.IsReversed ? 'warning' : 'default'" size="small"
               @click="ruleset = { ...ruleset, IsReversed: !ruleset.IsReversed }"
               style="cursor: pointer">
          {{ ruleset.IsReversed ? '非' : '正' }}
        </n-tag>
        <n-radio-group :value="ruleset.Mode" @update-value="(v: number) => ruleset = { ...ruleset, Mode: v }" size="small">
          <n-radio-button :value="0">任一满足</n-radio-button>
          <n-radio-button :value="1">全部满足</n-radio-button>
        </n-radio-group>
        <n-button size="tiny" @click="addGroup">+ 添加规则组</n-button>
      </n-space>
    </div>

    <div v-if="!ruleset.Groups || ruleset.Groups.length === 0" class="empty-hint">
      暂无规则组，点击上方添加
    </div>

    <div v-for="(group, gIdx) in ruleset.Groups" :key="gIdx" class="rule-group">
      <div class="group-header">
        <n-space align="center" size="small">
          <n-tag size="tiny" type="info">组 {{ gIdx + 1 }}</n-tag>
          <n-tag :type="group.IsReversed ? 'warning' : 'default'" size="tiny"
                 @click="updateGroupProp(gIdx, 'IsReversed', !group.IsReversed)"
                 style="cursor: pointer">
            {{ group.IsReversed ? '非' : '正' }}
          </n-tag>
          <n-radio-group :value="group.Mode"
                          @update-value="(v: number) => updateGroupProp(gIdx, 'Mode', v)" size="tiny">
            <n-radio-button :value="0">任一</n-radio-button>
            <n-radio-button :value="1">全部</n-radio-button>
          </n-radio-group>
          <n-switch :value="group.IsEnabled ?? true"
                    @update-value="(v: boolean) => updateGroupProp(gIdx, 'IsEnabled', v)" size="small" />
          <n-button size="tiny" @click="addRule(gIdx)">+ 规则</n-button>
          <n-popconfirm @positive-click="removeGroup(gIdx)">
            <template #trigger>
              <n-button size="tiny" type="error" quaternary>删除组</n-button>
            </template>
            确定删除此规则组？
          </n-popconfirm>
        </n-space>
      </div>

      <div v-for="(rule, rIdx) in group.Rules" :key="rIdx" class="rule-item">
        <n-space align="center" size="small">
          <span class="rule-icon">{{ getRuleIcon(rule.Id) }}</span>
          <n-tag :type="rule.IsReversed ? 'warning' : 'default'" size="tiny"
                 @click="updateRuleProp(gIdx, rIdx, 'IsReversed', !rule.IsReversed)"
                 style="cursor: pointer">
            {{ rule.IsReversed ? '非' : '正' }}
          </n-tag>
          <n-select :value="rule.Id"
                    @update-value="(v: string) => updateRuleProp(gIdx, rIdx, 'Id', v)"
                    :options="ruleTypes.map(r => ({ label: `${r.icon} ${r.name}`, value: r.id }))"
                    size="tiny" style="width: 220px" filterable />
          <n-button size="tiny" type="error" quaternary @click="removeRule(gIdx, rIdx)">×</n-button>
        </n-space>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ruleset-editor {
  border: 1px solid var(--border-color);
  border-radius: 6px;
  padding: 12px;
}
.ruleset-header {
  margin-bottom: 8px;
}
.rule-group {
  background: var(--card-color);
  border: 1px solid var(--divider-color);
  border-radius: 4px;
  padding: 8px;
  margin-bottom: 6px;
}
.group-header {
  margin-bottom: 6px;
  padding-bottom: 6px;
  border-bottom: 1px solid var(--divider-color);
}
.rule-item {
  padding: 4px 0;
}
.rule-icon {
  font-size: 14px;
}
.empty-hint {
  color: #999;
  text-align: center;
  padding: 16px;
  font-size: 13px;
}
</style>
